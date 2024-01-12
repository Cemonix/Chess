using System;
using System.Collections.Generic;

namespace Chess
{
    class Rook : Piece
    {
        public bool HasMoved { get; private set; } = false;
        
        public Rook(string name, PieceColor color, (int x, int y) position) :
            base(name, color, position)
        {
            HasMoved = false;
        }

        public override List<(int x, int y)> GetPossibleMoves(Piece[,] board)
        {
            PossibleMoves.Clear();

            GetPossibleMoveInDirection(board, MoveForward);
            GetPossibleMoveInDirection(board, MoveBackward);
            GetPossibleMoveInDirection(board, MoveLeft);
            GetPossibleMoveInDirection(board, MoveRight);

            return PossibleMoves;
        }

        public override void Move((int x, int y) position)
        {
            base.Move(position);
            HasMoved = true;
        }
    }
}