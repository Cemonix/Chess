using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    class ChessBoard
    {
        private const string _boardLetters  = "ABCDEFGH";
        private readonly Piece[,] _board;
        private readonly string[,] _possibleMovesBoard;
        private PieceColor _colorOfCurrPlayer;
        private readonly int _padding_len;
        private bool _isGameEnd;
        public ChessBoard()
        {
            _board = new Piece[8, 8];
            _possibleMovesBoard = new string[8, 8];
            _colorOfCurrPlayer = PieceColor.White;
            _padding_len = 6; // Length of chess piece names

            CreateChessPiecesAndFillBoard();
        }

        public void Game()
        {
            Console.WriteLine("Welcome to the new game of chess.");
            while(!_isGameEnd)
            {
                DrawBoard();
                Console.WriteLine($"It is {_colorOfCurrPlayer}'s turn.");

                if (IsKingUnderAttack(_colorOfCurrPlayer))
                {
                    Console.WriteLine("Your king is in check.");
                    // TODO: 1) Move the king
                    // TODO: 2) Take figure which is attacking the king
                    // TODO: 3) Place other figure before king

                    // TODO: No possible moves - end of game
                }
                // TODO: if no possible moves for any piece -> tie

                // Choose chess piece
                int pieceX, pieceY;
                do
                {
                    (pieceX, pieceY) = GetCoordinatesFromUserInput(
                        "Write the coordinates of a piece you would like to play."
                    );
                } while (!IsPieceCoorValid(pieceX, pieceY));

                Piece currentPiece = _board[pieceX, pieceY];
                List<(int x, int y)> piecePossibleMoves = currentPiece.GetPossibleMoves(_board);

                piecePossibleMoves = piecePossibleMoves
                    .Where(move => !MoveResultsInCheck(currentPiece, move))
                    .ToList();

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
                
                _board[pieceX, pieceY] = null!;
                _board[moveX, moveY] = currentPiece;
                currentPiece.Move((moveX, moveY));

                // En passant
                // if(
                //     currentPiece.GetType().Name == "Pawn" &&
                //     ((Pawn)currentPiece).isEnPassant
                // ){}
                
                // End of turn clear
                ClearPossibleMovesFromBoard();
                _colorOfCurrPlayer = GetOppositeColor(_colorOfCurrPlayer);
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
            _board[0, 4] = new King(" W_Ki ", PieceColor.White, (0, 4));
            _board[7, 4] = new King(" B_Ki ", PieceColor.Black, (7, 4));
            _board[0, 3] = new Queen(" W_Qu ", PieceColor.White, (0, 3));
            _board[7, 3] = new Queen(" B_Qu ", PieceColor.Black, (7, 3));
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
            else if(_board[x, y].Color != _colorOfCurrPlayer)
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

        private bool IsKingUnderAttack(PieceColor kingColor)
        {
            King king = GetKing(kingColor);
            PieceColor opponentColor = GetOppositeColor(kingColor);

            foreach (Piece piece in _board)
            {
                if (piece != null && piece.Color == opponentColor)
                {
                    var moves = piece.GetPossibleMoves(_board);
                    if (moves.Contains(king.Position))
                    {
                        return true;
                    }
                }
            }
            return false;
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

        private bool MoveResultsInCheck(Piece piece, (int x, int y) move)
        {
            // Save the original state
            var (x, y) = piece.Position;
            Piece? targetPositionPiece = _board[move.x, move.y];

            // Simulate the move
            _board[x, y] = null!;
            _board[move.x, move.y] = piece;

            bool isInCheck = IsKingUnderAttack(piece.Color);

            // Revert to original state
            _board[x, y] = piece;
            _board[move.x, move.y] = targetPositionPiece;

            return isInCheck;
        }

        private PieceColor GetOppositeColor(PieceColor color)
        {
            return color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }
    }
}