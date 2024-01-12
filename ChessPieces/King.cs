using System.Collections.Generic;

namespace Chess
{
    class King : Piece
    {
        public King(string name, PieceColor color, (int x, int y) position) :
            base(name, color, position) {}

        public override List<(int x, int y)> GetPossibleMoves(Piece[,] board)
        {
            // TODO: Castling
            PossibleMoves.Clear();

            GetPossibleMoveInDirection(board, MoveForward, 1);
            GetPossibleMoveInDirection(board, MoveBackward, 1);
            GetPossibleMoveInDirection(board, MoveLeft, 1);
            GetPossibleMoveInDirection(board, MoveRight, 1);
            GetPossibleMoveInDirection(board, MoveForwardRight, 1);
            GetPossibleMoveInDirection(board, MoveForwardLeft, 1);
            GetPossibleMoveInDirection(board, MoveBackwardRight, 1);
            GetPossibleMoveInDirection(board, MoveBackwardLeft, 1);

            return PossibleMoves;
        }
    }
}