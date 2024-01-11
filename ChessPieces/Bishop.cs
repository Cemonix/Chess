using System.Collections.Generic;

namespace Chess
{
    class Bishop : Piece
    {
        public Bishop(string name, PieceColor color, (int x, int y) position) :
            base(name, color, position) {}

        public override List<(int x, int y)> GetPossibleMoves(Piece[,] board)
        {
            PossibleMoves.Clear();
            
            GetPossibleMoveInDirection(board, MoveForwardRight);
            GetPossibleMoveInDirection(board, MoveForwardLeft);
            GetPossibleMoveInDirection(board, MoveBackwardRight);
            GetPossibleMoveInDirection(board, MoveBackwardLeft);

            return PossibleMoves;
        }
    }
}