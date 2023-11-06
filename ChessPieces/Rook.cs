using System;
using System.Collections.Generic;

namespace Chess
{
    class Rook : Piece
    {
        public Rook(string name, PieceColor color, (int X, int Y) position) :
            base(name, color, position) {}

        public override List<(int X, int Y)> GetPossibleMoves(Piece[,] board)
        {
            _possibleMoves.Clear();

            GetPossibleMoveInDirection(board, MoveForward);
            GetPossibleMoveInDirection(board, MoveBackward);
            GetPossibleMoveInDirection(board, MoveLeft);
            GetPossibleMoveInDirection(board, MoveRight);

            return _possibleMoves;
        }

        public override void Move((int X, int Y) position) => Position = position;
    }
}