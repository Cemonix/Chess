using System.Collections.Generic;

namespace Chess
{
    class Queen : Piece
    {
        public Queen(string name, PieceColor color, (int x, int y) position) :
            base(name, color, position) {}

        public override List<(int x, int y)> GetPossibleMoves(Piece[,] board)
        {
            PossibleMoves.Clear();
            
            GetPossibleMoveInDirection(board, MoveForward);
            GetPossibleMoveInDirection(board, MoveBackward);
            GetPossibleMoveInDirection(board, MoveLeft);
            GetPossibleMoveInDirection(board, MoveRight);
            GetPossibleMoveInDirection(board, MoveForwardRight);
            GetPossibleMoveInDirection(board, MoveForwardLeft);
            GetPossibleMoveInDirection(board, MoveBackwardRight);
            GetPossibleMoveInDirection(board, MoveBackwardLeft);

            return PossibleMoves;
        }

    }
}