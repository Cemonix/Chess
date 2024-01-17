using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Chess
{
    class GameController
    {
        public ChessBoard ChessBoard { get; private set; }
        public Piece[,] Board { get; private set; }
        private PieceColor _currentPlayerColor;
        private bool _isGameEnd;

        public GameController()
        {
            Board = new Piece[8, 8];
            ChessBoard = new ChessBoard(Board);
            _currentPlayerColor = PieceColor.White;
        }

        public void Game()
        {
            do
            {   
                Console.WriteLine("Welcome to the new game of chess.");
                CreateChessPiecesAndFillBoard();

                while (!_isGameEnd)
                {
                    ChessBoard.ClearPossibleMovesFromBoard();
                    ChessBoard.DrawBoard();
                    ResetPawnTwoSquareMove();

                    Console.WriteLine($"It is {_currentPlayerColor}'s turn.");
                    Turn();

                    _currentPlayerColor = GetOppositeColor(_currentPlayerColor);
                }
                
                _currentPlayerColor = PieceColor.White;
                _isGameEnd = false;
                ChessBoard.ClearBoard();

            } while (GetConfirmationFromUserInput("Would you like to play another game? (y/n)"));
        }

        public void Turn()
        {
            var allCurrentPlayersMoves = GetAllPlayerPossibleMoves(_currentPlayerColor);
            var allOpponentPlayersMoves = GetAllPlayerPossibleMoves(GetOppositeColor(_currentPlayerColor));

            King king = ChessBoard.GetKing(_currentPlayerColor);
            bool isKingUnderAttack = ChessBoard.IsSquareUnderAttack(
                king.Position, allOpponentPlayersMoves
            );

            var castlingMoves = king.GetCastlingMoves(Board, allOpponentPlayersMoves);
            if (castlingMoves.Count > 0)
            {
                allCurrentPlayersMoves[(king.Position.x, king.Position.y)]
                    .AddRange(castlingMoves);
            }

            allCurrentPlayersMoves = FilterPlayerPossibleMoves(allCurrentPlayersMoves);
            
            if (allCurrentPlayersMoves.Count == 0)
            {
                if (isKingUnderAttack)
                {
                    // Checkmate condition
                    Console.WriteLine("Checkmate!");
                    _isGameEnd = true;
                }
                else
                {
                    // Stalemate condition
                    Console.WriteLine("Stalemate!");
                    _isGameEnd = true;
                }
            }
            else
            {
                if(isKingUnderAttack)
                {
                    Console.WriteLine("Your king is in check.");
                }

                (int x, int y) chosenPieceCoor;
                Piece chosenPiece;
                List<(int x, int y)>? piecePossibleMoves;
                do{
                    (chosenPieceCoor.x, chosenPieceCoor.y) = ChoosePiece();
                    chosenPiece = Board[chosenPieceCoor.x, chosenPieceCoor.y];
                    
                    if (
                        !allCurrentPlayersMoves.TryGetValue(
                            (chosenPieceCoor.x, chosenPieceCoor.y),
                            out piecePossibleMoves
                        )
                    )
                    {
                        Console.WriteLine(
                            "Selected piece has no legal moves. Please choose another piece."
                        );
                    }

                } while (piecePossibleMoves == null || piecePossibleMoves.Count == 0);

                ChessBoard.AddPossibleMovesToBoard(piecePossibleMoves);
                ChessBoard.DrawBoard();

                // Move chosen chess piece
                int moveX, moveY;
                do
                {
                    (moveX, moveY) = GetCoordinatesFromUserInput(
                        "Write the coordinates where you would like to move the chosen piece."
                    );

                    // Castling
                    if (
                        chosenPiece is King chosenKing &&
                        !chosenKing.HasMoved &&
                        castlingMoves.Contains((moveX, moveY))
                    )
                    {
                        Castling(moveX, moveY);

                    }

                    // En Passant
                    if (chosenPiece is Pawn enPassantPawn)
                    {
                        var enPassantMove = enPassantPawn.GetEnPassantPossibleMove(Board);
                        if (
                            enPassantMove.HasValue &&
                            moveX == enPassantMove.Value.x &&
                            moveY == enPassantMove.Value.y
                        )
                        {
                            EnPassant(moveX, moveY);
                        }
                    }
                } while (!ChessBoard.IsMoveCoorValid(moveX, moveY, piecePossibleMoves));
                           
                if (Board[chosenPieceCoor.x, chosenPieceCoor.y] is Pawn pawn)
                {
                    if (pawn.IsTwoSquareMove(moveX))
                    {
                        pawn.HasMovedTwoSquares = true;
                    }
                }

                Board[chosenPieceCoor.x, chosenPieceCoor.y] = null!;
                Board[moveX, moveY] = chosenPiece;
                chosenPiece.Move((moveX, moveY));
            }
        }

        private void Castling(int kingMoveX, int kingMoveY)
        {
            // Determine which side the castling is on based on the king's final position
            bool isKingside = kingMoveY == 6;

            // True king's side, false queen's side
            int rookInitialY = isKingside ? 7 : 0;
            int rookFinalY = isKingside ? 5 : 3;

            // Move the Rook
            Rook rook = (Rook)Board[kingMoveX, rookInitialY];
            Board[kingMoveX, rookInitialY] = null!;
            Board[kingMoveX, rookFinalY] = rook;
            rook.Move((kingMoveX, rookFinalY));
        }

        private void EnPassant(int movedX, int movedY)
        {
            // Determine the position of the captured pawn
            int capturedPawnX = _currentPlayerColor == PieceColor.White ?
                movedX - 1 : movedX + 1;

            // Remove the captured pawn from the board
            Board[capturedPawnX, movedY] = null!;
        }

        private void ResetPawnTwoSquareMove()
        {
            foreach (Piece piece in Board)
            {
                if (piece is Pawn pawn && pawn.Color == _currentPlayerColor)
                {
                    pawn.HasMovedTwoSquares = false;
                }
            }
        }

        private (int x, int y) ChoosePiece()
        {
            int chosenPieceX, chosenPieceY;
            bool isPieceCoorValid;
            do
            {
                (chosenPieceX, chosenPieceY) = GetCoordinatesFromUserInput(
                    "Write the coordinates of a piece you would like to play."
                );

                isPieceCoorValid = ChessBoard.IsPieceCoorValid(chosenPieceX, chosenPieceY, _currentPlayerColor);
                if (!isPieceCoorValid)
                {
                    Console.WriteLine("Invalid selection. Please choose another piece.");
                }
            } while (!isPieceCoorValid);

            return (chosenPieceX, chosenPieceY);
        }
        
        private Dictionary<(int X, int Y), List<(int X, int Y)>> GetAllPlayerPossibleMoves(
            PieceColor playerColor
        )
        {
            var allPlayerPossibleMoves = new Dictionary<(int X, int Y), List<(int X, int Y)>>();

            foreach (Piece piece in Board)
            {
                if (piece != null && piece.Color == playerColor)
                {
                    var moves = piece.GetPossibleMoves(Board);
                    if (moves.Count > 0)
                    {
                        allPlayerPossibleMoves.Add(piece.Position, moves);
                    }
                }
            }

            return allPlayerPossibleMoves;
        }

        private Dictionary<(int x, int y), List<(int x, int y)>> FilterPlayerPossibleMoves(
            Dictionary<(int x, int y), List<(int x, int y)>> playerPossibleMoves
        )
        {
            var filteredPlayerMoves = new Dictionary<(int x, int y), List<(int x, int y)>>();
            foreach (var possibleMovesPair in playerPossibleMoves)
            {
                (int x, int y) = possibleMovesPair.Key;
                var filteredPossibleMoves = possibleMovesPair.Value
                    .Where(move => !MoveResultsInCheck(Board[x, y], move))
                    .ToList();

                if (filteredPossibleMoves.Count > 0)
                {
                    filteredPlayerMoves.Add(possibleMovesPair.Key, filteredPossibleMoves);
                }
            }
            return filteredPlayerMoves;
        }

        private bool MoveResultsInCheck(
            Piece piece, (int x, int y) move
        )
        {
            // Save the original state
            var (x, y) = piece.Position;
            Piece? targetPositionPiece = Board[move.x, move.y];

            // Simulate the move
            Board[x, y] = null!;
            piece.Position = move;
            Board[move.x, move.y] = piece;

            var allOpponentMoves = GetAllPlayerPossibleMoves(GetOppositeColor(_currentPlayerColor));
            King king = ChessBoard.GetKing(_currentPlayerColor);
            bool isInCheck = ChessBoard.IsSquareUnderAttack(king.Position, allOpponentMoves);

            // Revert to original state
            piece.Position = (x, y);
            Board[x, y] = piece;
            Board[move.x, move.y] = targetPositionPiece;

            return isInCheck;
        }

        private static (int x, int y) GetCoordinatesFromUserInput(string message)
        {
            (int x, int y) coordinates;
            Console.WriteLine(message);
            while(true)
            {
                string? chosenPieceStrCoor = Console.ReadLine();
                if(!string.IsNullOrEmpty(chosenPieceStrCoor))
                {
                    coordinates = ChessBoard.CoordinatesMapper(chosenPieceStrCoor);
                    if(coordinates.x != -1)
                        break;
                }
                Console.WriteLine($"Provided coordinates are not valid! Try again.");
            }
            return coordinates;
        }

        private static bool GetConfirmationFromUserInput(string message)
        {
            Console.WriteLine(message);
            Regex yesRegex = new ("^(y(es)?|yep|sure|ok|okay|y)$", RegexOptions.IgnoreCase);
            Regex noRegex = new ("^(n(o)?|nope|nah|n)$", RegexOptions.IgnoreCase);

            while (true)
            {
                string? confirmStr = Console.ReadLine();
                if (!string.IsNullOrEmpty(confirmStr))
                {
                    if (yesRegex.IsMatch(confirmStr))
                        return true;
                    if (noRegex.IsMatch(confirmStr))
                        return false;
                }
                Console.WriteLine("Provided response is not valid! Try again.");
            }
        }

        private void HandlePawnPromotion(Pawn pawn, (int X, int Y) position)
        {
            Board[position.X, position.Y] = new Queen(
                $" {pawn.Color.ToString()[0]}_Qu ", pawn.Color, position
            );
        }

        public void CreateChessPiecesAndFillBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                Pawn wPawn = new (" W_Pa ", PieceColor.White, (1, i));
                Pawn bPawn = new (" B_Pa ", PieceColor.Black, (6, i));

                wPawn.OnPromotion += HandlePawnPromotion;
                bPawn.OnPromotion += HandlePawnPromotion;

                Board[1, i] = wPawn;
                Board[6, i] = bPawn;
            }

            int[,] positions = new int[3, 2] { {0, 7}, {1, 6}, {2, 5}};
            for (int i = 0; i < 2; i++)
            {     
                Board[0, positions[0, i]] = new Rook(" W_Ro ", PieceColor.White, (0, positions[0, i]));
                Board[7, positions[0, i]] = new Rook(" B_Ro ", PieceColor.Black, (7, positions[0, i]));
                Board[0, positions[1, i]] = new Knight(" W_Kn ", PieceColor.White, (0, positions[1, i]));
                Board[7, positions[1, i]] = new Knight(" B_Kn ", PieceColor.Black, (7, positions[1, i]));
                Board[0, positions[2, i]] = new Bishop(" W_Bi ", PieceColor.White, (0, positions[2, i]));
                Board[7, positions[2, i]] = new Bishop(" B_Bi ", PieceColor.Black, (7, positions[2, i]));
            }
            Board[0, 4] = new King(" W_Ki ", PieceColor.White, (0, 4));
            Board[7, 4] = new King(" B_Ki ", PieceColor.Black, (7, 4));
            Board[0, 3] = new Queen(" W_Qu ", PieceColor.White, (0, 3));
            Board[7, 3] = new Queen(" B_Qu ", PieceColor.Black, (7, 3));
        }

        private static PieceColor GetOppositeColor(PieceColor color)
        {
            return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }
    } 
}