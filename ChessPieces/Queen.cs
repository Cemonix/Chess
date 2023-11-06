using System.Collections.Generic;

namespace Chess
{
    class Queen : Piece
    {
        public Queen(string name, PieceColor color, (int X, int Y) position) :
            base(name, color, position) {}

        public override List<(int X, int Y)> GetPossibleMoves(Piece[,] board)
        {
            _possibleMoves.Clear();
            
            GetPossibleMoveInDirection(board, MoveForward);
            GetPossibleMoveInDirection(board, MoveBackward);
            GetPossibleMoveInDirection(board, MoveLeft);
            GetPossibleMoveInDirection(board, MoveRight);
            GetPossibleMoveInDirection(board, MoveForwardRight);
            GetPossibleMoveInDirection(board, MoveForwardLeft);
            GetPossibleMoveInDirection(board, MoveBackwardRight);
            GetPossibleMoveInDirection(board, MoveBackwardLeft);

            return _possibleMoves;
        }

        public override void Move((int X, int Y) position) => Position = position;

    }
}