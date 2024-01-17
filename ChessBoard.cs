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
        private readonly int _padding_len;
        public ChessBoard(Piece[,] board)
        {
            _board = board;
            _possibleMovesBoard = new string[8, 8];
            _padding_len = 6; // Length of chess piece names
        }

        public void DrawBoard()
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

        public static (int x, int y) CoordinatesMapper(string coordinates)
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

        public bool IsPieceCoorValid(int x, int y, PieceColor currentPlayer)
        {
            if(_board[x, y] is null)
            {
                Console.WriteLine("No chess piece on given coordinates.");
                return false;
            }
            else if(_board[x, y].Color != currentPlayer)
            {
                Console.WriteLine("This piece does not belong to you.");
                return false;
            }

            return true;
        }

        public static bool IsMoveCoorValid(int x, int y, List<(int x, int y)> possibleMoves)
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

        public void AddPossibleMovesToBoard(List<(int x, int y)> possibleMoves)
        {
            foreach((int x, int y) in possibleMoves)
            {
                if(_board[x, y] == null)
                    _possibleMovesBoard[x, y] = "  **  ";
            }
        }

        public void ClearPossibleMovesFromBoard()
        {
            for (int i = 0; i < _possibleMovesBoard.GetLength(0); i++)
            {
                for (int j = 0; j < _possibleMovesBoard.GetLength(0); j++)
                {
                    _possibleMovesBoard[i, j] = null!;
                }
            }
        }

        public void ClearBoard()
        {
            for (int i = 0; i < _board.GetLength(0); i++)
            {
                for (int j = 0; j < _board.GetLength(0); j++)
                {
                    _board[i, j] = null!;
                }
            }
        }

        public King GetKing(PieceColor kingColor)
        {
            foreach (Piece piece in _board)
            {
                if (piece is King king && piece.Color == kingColor)
                    return king;
            }
            throw new Exception("King was not found on the board.");
        }

        public static bool IsSquareUnderAttack(
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
    }
}