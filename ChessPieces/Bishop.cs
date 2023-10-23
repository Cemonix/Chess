using System.Collections.Generic;

namespace Chess
{
    class Bishop : Piece
    {
        public Bishop(string name, PieceColor color, (int X, int Y) position) :
            base(name, color, position) {}

        public override List<(int X, int Y)> GetPossibleMoves(Piece[,] board)
        {
            for (int i = 7; i > 0; --i)
            {
                (int X, int Y) nextForwardLeft = Position;
                (int X, int Y) nextForwardRight = Position;
                (int X, int Y) nextBackwardLeft = Position;
                (int X, int Y) nextBackwardRight = Position;
                for (int j = i; j > 0; --j)
                {
                    nextForwardLeft = MoveLeft(MoveForward(nextForwardLeft));
                    nextForwardRight = MoveRight(MoveForward(nextForwardRight));
                    nextBackwardLeft = MoveLeft(MoveBackward(nextBackwardLeft));
                    nextBackwardRight = MoveRight(MoveBackward(nextBackwardRight));
                }
                _possibleMoves[i*4-1] = nextForwardLeft;
                _possibleMoves[i*4-2] = nextForwardRight;
                _possibleMoves[i*4-3] = nextBackwardLeft;
                _possibleMoves[i*4-4] = nextBackwardRight;
            }

            return _possibleMoves;
        }

        public override void Move((int X, int Y) position) => Position = position;
    }
}