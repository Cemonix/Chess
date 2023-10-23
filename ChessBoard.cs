using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Chess
{
    class ChessBoard
    {
        private readonly Piece[,] _board;
        private readonly string[,] _possibleMovesBoard;
        private bool _isGameEnd;
        private PieceColor _colorOfCurrPlayer;
        private const string _boardLetters  = "ABCDEFGH";
        private readonly int _padding_len;
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

                // Choose chess piece
                int pieceX, pieceY;
                do
                {
                    (pieceX, pieceY) = GetCoordinatesFromUserInput(
                        "Write the coordinates of a piece you would like to play."
                    );
                } while (!IsPieceCoorValid(pieceX, pieceY));

                Piece currentPiece = _board[pieceX, pieceY];
                List<(int X, int Y)> piecePossibleMoves = currentPiece.GetPossibleMoves(_board);
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
                
                // End of turn clear
                ClearPossibleMovesBoard();
                _colorOfCurrPlayer = _colorOfCurrPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
                // TODO: check if game end
                // _isGameEnd = true;
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
                _board[1, i] = new Pawn(" W_Pa ", PieceColor.White, (1, i));
                _board[6, i] = new Pawn(" B_Pa ", PieceColor.Black, (6, i));
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
            _board[0, 3] = new Queen(" W_Qu ", PieceColor.White, (0, 3));
            _board[7, 3] = new Queen(" B_Qu ", PieceColor.Black, (7, 3));
            _board[0, 4] = new King(" W_Ki ", PieceColor.White, (0, 4));
            _board[7, 4] = new King(" B_Ki ", PieceColor.Black, (7, 4));
        }

        private (int X, int Y) GetCoordinatesFromUserInput(string message)
        {
            (int X, int Y) coordinates;
            Console.WriteLine(message);
            while(true)
            {
                string? chosenPieceStrCoor = Console.ReadLine();
                if(!string.IsNullOrEmpty(chosenPieceStrCoor))
                {
                    coordinates = CoordinatesMapper(chosenPieceStrCoor);
                    if(coordinates.X != -1)
                        break;
                }
                Console.WriteLine($"Provided coordinates are not valid! Try again.");
            }
            return coordinates;
        }

        private (int X, int Y) CoordinatesMapper(string coordinates)
        {
            (int X, int Y) mappedCoord = (-1, -1);
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

        private bool IsPieceCoorValid(int X, int Y)
        {
            if(_board[X, Y] is null)
            {
                Console.WriteLine("No chess piece on given coordinates.");
                return false;
            }
            else if(_board[X, Y].Color != _colorOfCurrPlayer)
            {
                Console.WriteLine("This piece does not belong to you.");
                return false;
            }
            return true;
        }

        private bool IsMoveCoorValid(int X, int Y, List<(int X, int Y)> possibleMoves)
        {
            bool isValid = false;
            foreach((int possibleX, int possibleY) in possibleMoves)
            {
                if(X == possibleX && Y == possibleY)
                {
                    isValid = true;
                }
            }

            if(_board[X, Y] is not null && _board[X, Y].Color == _colorOfCurrPlayer)
            {
                Console.WriteLine("Cannot move on position where is your other chess piece.");
                isValid = false;
            }
            else if(!isValid)
                Console.WriteLine("Cannot move on this position.");

            return isValid;
        }

        private void AddPossibleMovesToBoard(List<(int X, int Y)> possibleMoves)
        {
            foreach((int X, int Y) in possibleMoves)
            {
                _possibleMovesBoard[X, Y] = "  **  ";
            }
        }

        private void ClearPossibleMovesBoard()
        {
             for (int i = 0; i < _possibleMovesBoard.GetLength(0); i++)
            {
                for (int j = 0; j < _possibleMovesBoard.GetLength(0); j++)
                {
                    _possibleMovesBoard[i, j] = null!;
                }
            }
        }
    }
}