using System;
using System.Collections.Generic;

namespace Chess
{
    class King : Piece
    {
        public bool HasMoved { get; private set; }
        private readonly Func<King, List<(int x, int y)>> _getCastlingMoves;

        public King(
            string name, PieceColor color, (int x, int y) position,
            Func<King, List<(int x, int y)>> getCastlingMoves
        ) : base(name, color, position) 
        {
            HasMoved = false;
            _getCastlingMoves = getCastlingMoves;
        }

        public override List<(int x, int y)> GetPossibleMoves(Piece[,] board)
        {
            PossibleMoves.Clear();

            GetPossibleMoveInDirection(board, MoveForward, 1);
            GetPossibleMoveInDirection(board, MoveBackward, 1);
            GetPossibleMoveInDirection(board, MoveLeft, 1);
            GetPossibleMoveInDirection(board, MoveRight, 1);
            GetPossibleMoveInDirection(board, MoveForwardRight, 1);
            GetPossibleMoveInDirection(board, MoveForwardLeft, 1);
            GetPossibleMoveInDirection(board, MoveBackwardRight, 1);
            GetPossibleMoveInDirection(board, MoveBackwardLeft, 1);

            // FIXME: infinite recursion - will need refactor of chessBoard class - split into:
            //  1) chessBoard
            //  2) game manager
            // Castling 
            // PossibleMoves.AddRange(_getCastlingMoves(this));

            return PossibleMoves;
        }

        public override void Move((int x, int y) position)
        {
            base.Move(position);
            HasMoved = true;
        }

        public bool IsKingUnderAttack(
            Dictionary<(int x, int y), List<(int x, int y)>> allOpponentMoves
        )
        {
            foreach (var moves in allOpponentMoves.Values)
            {
                if (moves.Contains(Position))
                    return true;    
            }

            return false;
        }
    }
}