using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Chess
{
    class ChessBoard
    {
        private const string _boardLetters  = "ABCDEFGH";
        private readonly Piece[,] _board;
        private readonly string[,] _possibleMovesBoard;
        private PieceColor _currentPlayerColor;
        private readonly int _padding_len;
        private bool _isGameEnd;
        public ChessBoard()
        {
            _board = new Piece[8, 8];
            _possibleMovesBoard = new string[8, 8];
            _currentPlayerColor = PieceColor.White;
            _padding_len = 6; // Length of chess piece names
        }

        public void Game()
        {
            do
            {   
                Console.WriteLine("Welcome to the new game of chess.");
                CreateChessPiecesAndFillBoard();

                while (!_isGameEnd)
                {
                    ClearPossibleMovesFromBoard();
                    DrawBoard();

                    Console.WriteLine($"It is {_currentPlayerColor}'s turn.");
                    StartTurn();
                }
                
                _currentPlayerColor = PieceColor.White;
                _isGameEnd = false;
                ClearBoard();

            } while (GetConfirmationFromUserInput("Would you like to play another game? (y/n)"));
        }

        public void StartTurn()
        {
            var allCurrentPlayersMoves = GetAllPlayerPossibleMoves(_currentPlayerColor);
            var allOpponentPlayersMoves = GetAllPlayerPossibleMoves(GetOppositeColor(_currentPlayerColor));

            allCurrentPlayersMoves = FilterPlayerPossibleMoves(allCurrentPlayersMoves);

            King king = GetKing(_currentPlayerColor);
            bool isKingUnderAttack = king.IsKingUnderAttack(
                allOpponentPlayersMoves
            );
            
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

                // Choose chess piece to move with
                int chosenPieceX, chosenPieceY;
                List<(int x, int y)> piecePossibleMoves = new ();

                do
                {
                    (chosenPieceX, chosenPieceY) = GetCoordinatesFromUserInput(
                        "Write the coordinates of a piece you would like to play."
                    );

                    if (!IsPieceCoorValid(chosenPieceX, chosenPieceY))
                    {
                        Console.WriteLine("Invalid selection. Please choose another piece.");
                        continue;
                    }

                    if (
                        !allCurrentPlayersMoves.TryGetValue(
                            (chosenPieceX, chosenPieceY), out piecePossibleMoves!
                        )
                    )
                    {
                        Console.WriteLine("Selected piece has no legal moves. Please choose another piece.");
                    }

                } while (piecePossibleMoves == null || piecePossibleMoves.Count == 0);

                Piece chosenPiece = _board[chosenPieceX, chosenPieceY];
                AddPossibleMovesToBoard(piecePossibleMoves);
                DrawBoard();
                
                // Move chosen chess piece
                int moveX, moveY;
                do
                {
                    (moveX, moveY) = GetCoordinatesFromUserInput(
                        "Write the coordinates where you would like to move the chosen piece."
                    );
                } while (!IsMoveCoorValid(moveX, moveY, piecePossibleMoves));
                
                _board[chosenPieceX, chosenPieceY] = null!;
                _board[moveX, moveY] = chosenPiece;
                chosenPiece.Move((moveX, moveY));

                // En passant
                // if(
                //     currentPiece.GetType().Name == "Pawn" &&
                //     ((Pawn)currentPiece).isEnPassant
                // ){}
                
                // End of turn clear
                _currentPlayerColor = GetOppositeColor(_currentPlayerColor);
            }
        }

        private void DrawBoard()
        {
            string padding = new(' ', _padding_len);
            for (int i = 0; i < _board.GetLength(0); i++)
            {
                for (int j = 0; j < _board.GetLength(0); j++)
                {
                    // Board left side numbering
                    if(j == 0)
                        Console.Write((i + 1).ToString());
                    Console.Write("|");

                    // Draw empty tile or tile with possible move
                    if (_board[i, j] is null)
                        if(_possibleMovesBoard[i, j] is null)
                            Console.Write(padding);
                        else
                            Console.Write(_possibleMovesBoard[i, j]);
                    else
                    {
                        int piece_pad_len = _padding_len - _board[i, j].Name.Length;
                        if(piece_pad_len != 0)
                        {
                            string piece_padding = new(' ',  (piece_pad_len - 1) / 2 + 1);
                            Console.Write(piece_padding + _board[i, j].Name + piece_padding);
                        }
                        else
                            Console.Write(_board[i, j].Name);
                        
                    }
                }
                Console.Write("|\n");
            }

            // Board last line letters
            Console.WriteLine(new string('-', 2 + (_padding_len + 1) * 8));
            foreach (char letter in _boardLetters)
            {
                if(letter == 'A')
                    Console.Write(" |  " + letter);
                else
                    Console.Write("   |  " + letter);
            }
            Console.Write("   |");
            Console.WriteLine();
        }

        private Dictionary<(int X, int Y), List<(int X, int Y)>> GetAllPlayerPossibleMoves(
            PieceColor playerColor
        )
        {
            var allPlayerPossibleMoves = new Dictionary<(int X, int Y), List<(int X, int Y)>>();

            foreach (Piece piece in _board)
            {
                if (piece != null && piece.Color == playerColor)
                {
                    var moves = piece.GetPossibleMoves(_board);
                    if (moves.Count > 0)
                    {
                        allPlayerPossibleMoves.Add(piece.Position, moves);
                    }
                }
            }

            return allPlayerPossibleMoves;
        }

        private Dictionary<(int X, int Y), List<(int X, int Y)>> FilterPlayerPossibleMoves(
            Dictionary<(int X, int Y), List<(int X, int Y)>> playerPossibleMoves
        )
        {
            var filteredPlayerMoves = new Dictionary<(int X, int Y), List<(int X, int Y)>>();
            foreach (var possibleMovesPair in playerPossibleMoves)
            {
                (int x, int y) = possibleMovesPair.Key;
                var filteredPossibleMoves = possibleMovesPair.Value
                    .Where(move => !MoveResultsInCheck(_board[x, y], move))
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
            Piece? targetPositionPiece = _board[move.x, move.y];

            // Simulate the move
            _board[x, y] = null!;
            piece.Position = move;
            _board[move.x, move.y] = piece;

            var allOpponentMoves = GetAllPlayerPossibleMoves(GetOppositeColor(_currentPlayerColor));
            King king = GetKing(_currentPlayerColor);
            bool isInCheck = king.IsKingUnderAttack(allOpponentMoves);

            // Revert to original state
            piece.Position = (x, y);
            _board[x, y] = piece;
            _board[move.x, move.y] = targetPositionPiece;

            return isInCheck;
        } 

        private void HandlePawnPromotion(Pawn pawn, (int X, int Y) position)
        {
            _board[position.X, position.Y] = new Queen(
                $" {pawn.Color.ToString()[0]}_Qu ", pawn.Color, position
            );
        }

        private (int x, int y) GetCoordinatesFromUserInput(string message)
        {
            (int x, int y) coordinates;
            Console.WriteLine(message);
            while(true)
            {
                string? chosenPieceStrCoor = Console.ReadLine();
                if(!string.IsNullOrEmpty(chosenPieceStrCoor))
                {
                    coordinates = CoordinatesMapper(chosenPieceStrCoor);
                    if(coordinates.x != -1)
                        break;
                }
                Console.WriteLine($"Provided coordinates are not valid! Try again.");
            }
            return coordinates;
        }

        private bool GetConfirmationFromUserInput(string message)
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

        private (int x, int y) CoordinatesMapper(string coordinates)
        {
            (int x, int y) mappedCoord = (-1, -1);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if(_boardLetters[j] + (i + 1).ToString() == coordinates.ToUpper())
                    {
                        mappedCoord = (i, j);
                        return mappedCoord;
                    }
                }
            }
            return mappedCoord;
        }

        private bool IsPieceCoorValid(int x, int y)
        {
            if(_board[x, y] is null)
            {
                Console.WriteLine("No chess piece on given coordinates.");
                return false;
            }
            else if(_board[x, y].Color != _currentPlayerColor)
            {
                Console.WriteLine("This piece does not belong to you.");
                return false;
            }

            return true;
        }

        private bool IsMoveCoorValid(int x, int y, List<(int x, int y)> possibleMoves)
        {
            bool isValid = false;
            foreach((int possiblex, int possibley) in possibleMoves)
            {
                if(x == possiblex && y == possibley)
                {
                    isValid = true;
                }
            }

            if(!isValid)
                Console.WriteLine("Cannot move on this position.");

            return isValid;
        }

        private void AddPossibleMovesToBoard(List<(int x, int y)> possibleMoves)
        {
            foreach((int x, int y) in possibleMoves)
            {
                if(_board[x, y] == null)
                    _possibleMovesBoard[x, y] = "  **  ";
            }
        }

        private void ClearPossibleMovesFromBoard()
        {
            for (int i = 0; i < _possibleMovesBoard.GetLength(0); i++)
            {
                for (int j = 0; j < _possibleMovesBoard.GetLength(0); j++)
                {
                    _possibleMovesBoard[i, j] = null!;
                }
            }
        }

        private void ClearBoard()
        {
            for (int i = 0; i < _board.GetLength(0); i++)
            {
                for (int j = 0; j < _board.GetLength(0); j++)
                {
                    _board[i, j] = null!;
                }
            }
        }

        private King GetKing(PieceColor kingColor)
        {
            foreach (Piece piece in _board)
            {
                if (piece is King king && piece.Color == kingColor)
                    return king;
            }
            throw new Exception("King was not found on the board.");
        }

        private List<(int x, int y)> GetCastlingMovesForKing(King king)
        {
            var castlingMoves = new List<(int x, int y)>();
            if (
                king.HasMoved ||
                king.IsKingUnderAttack(GetAllPlayerPossibleMoves(GetOppositeColor(king.Color)))
            )
            {
                return castlingMoves;
            }

            // Check Kingside Castling
            if (CanCastle(king, 7))
            {
                castlingMoves.Add((king.Position.x, 6));
            }

            // Check Queenside Castling
            if (CanCastle(king, 0))
            {
                castlingMoves.Add((king.Position.x, 2));
            }

            return castlingMoves;
        }

        private bool CanCastle(King king, int rookColumn)
        {
            int pathStart = king.Position.y < rookColumn ? king.Position.y + 1 : rookColumn + 1;
            int pathEnd = king.Position.y < rookColumn ? rookColumn - 1 : king.Position.y - 1;

            // Check if the path between the king and the rook is clear
            for (int column = pathStart; column <= pathEnd; column++)
            {
                if (_board[king.Position.x, column] != null)
                {
                    return false;
                }
            }

            // Check if the rook has moved
            if (_board[king.Position.x, rookColumn] is not Rook rook || rook.HasMoved)
            {
                return false;
            }

            // Check if any squares that the king passes through are under attack
            for (
                int column = king.Position.y;
                column != rookColumn;
                column += rookColumn > king.Position.y ? 1 : -1
            )
            {
                if (
                    IsSquareUnderAttack(
                        (king.Position.x, column),
                        GetAllPlayerPossibleMoves(GetOppositeColor(king.Color))
                    )
                )
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsSquareUnderAttack(
            (int x, int y) square, Dictionary<(int x, int y), List<(int x, int y)>> allOpponentMoves
        )
        {
            // Check if the given square is in the list of moves for any of the opponent's pieces
            foreach (var pieceMoves in allOpponentMoves.Values)
            {
                if (pieceMoves.Contains(square))
                {
                    return true;
                }
            }

            return false;
        }

        private void CreateChessPiecesAndFillBoard()
        {
            for (int i = 0; i < 8; i++)
            {
                Pawn wPawn = new (" W_Pa ", PieceColor.White, (1, i));
                Pawn bPawn = new (" B_Pa ", PieceColor.Black, (6, i));

                wPawn.OnPromotion += HandlePawnPromotion;
                bPawn.OnPromotion += HandlePawnPromotion;

                _board[1, i] = wPawn;
                _board[6, i] = bPawn;
            }

            int[,] positions = new int[3, 2] { {0, 7}, {1, 6}, {2, 5}};
            for (int i = 0; i < 2; i++)
            {     
                _board[0, positions[0, i]] = new Rook(" W_Ro ", PieceColor.White, (0, positions[0, i]));
                _board[7, positions[0, i]] = new Rook(" B_Ro ", PieceColor.Black, (7, positions[0, i]));
                _board[0, positions[1, i]] = new Knight(" W_Kn ", PieceColor.White, (0, positions[1, i]));
                _board[7, positions[1, i]] = new Knight(" B_Kn ", PieceColor.Black, (7, positions[1, i]));
                _board[0, positions[2, i]] = new Bishop(" W_Bi ", PieceColor.White, (0, positions[2, i]));
                _board[7, positions[2, i]] = new Bishop(" B_Bi ", PieceColor.Black, (7, positions[2, i]));
            }
            _board[0, 4] = new King(" W_Ki ", PieceColor.White, (0, 4), GetCastlingMovesForKing);
            _board[7, 4] = new King(" B_Ki ", PieceColor.Black, (7, 4), GetCastlingMovesForKing);
            _board[0, 3] = new Queen(" W_Qu ", PieceColor.White, (0, 3));
            _board[7, 3] = new Queen(" B_Qu ", PieceColor.Black, (7, 3));
        }

        private PieceColor GetOppositeColor(PieceColor color)
        {
            return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }
    }
}