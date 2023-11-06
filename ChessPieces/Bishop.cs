using System.Collections.Generic;

namespace Chess
{
    class Bishop : Piece
    {
        public Bishop(string name, PieceColor color, (int X, int Y) position) :
            base(name, color, position) {}

        public override List<(int X, int Y)> GetPossibleMoves(Piece[,] board)
        {
            _possibleMoves.Clear();
            
            GetPossibleMoveInDirection(board, MoveForwardRight);
            GetPossibleMoveInDirection(board, MoveForwardLeft);
            GetPossibleMoveInDirection(board, MoveBackwardRight);
            GetPossibleMoveInDirection(board, MoveBackwardLeft);

            return _possibleMoves;
        }

        public override void Move((int X, int Y) position) => Position = position;
    }
}