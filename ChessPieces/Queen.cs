using System.Collections.Generic;

namespace Chess
{
    class Queen : Piece
    {
        public Queen(string name, PieceColor color, (int X, int Y) position) :
            base(name, color, position) {}

        public override List<(int X, int Y)> GetPossibleMoves(Piece[,] board)
        {
            for (int i = 7; i > 0; --i)
            {
                (int X, int Y) nextForward = Position;
                (int X, int Y) nextBackward = Position;
                (int X, int Y) nextLeft = Position;
                (int X, int Y) nextRight = Position;
                (int X, int Y) nextForwardLeft = Position;
                (int X, int Y) nextForwardRight = Position;
                (int X, int Y) nextBackwardLeft = Position;
                (int X, int Y) nextBackwardRight = Position;
                for (int j = i; j > 0; --j)
                {
                    nextForward = MoveForward(nextForward);
                    nextBackward = MoveBackward(nextBackward);
                    nextLeft = MoveLeft(nextLeft);
                    nextRight = MoveRight(nextRight);
                    nextForwardLeft = MoveLeft(MoveForward(nextForwardLeft));
                    nextForwardRight = MoveRight(MoveForward(nextForwardRight));
                    nextBackwardLeft = MoveLeft(MoveBackward(nextBackwardLeft));
                    nextBackwardRight = MoveRight(MoveBackward(nextBackwardRight));
                }
                _possibleMoves[i*8-1] = nextForward;
                _possibleMoves[i*8-2] = nextBackward;
                _possibleMoves[i*8-3] = nextLeft;
                _possibleMoves[i*8-4] = nextRight;
                _possibleMoves[i*8-5] = nextForwardLeft;
                _possibleMoves[i*8-6] = nextForwardRight;
                _possibleMoves[i*8-7] = nextBackwardLeft;
                _possibleMoves[i*8-8] = nextBackwardRight;
            }
            
            return _possibleMoves;
        }

        public override void Move((int X, int Y) position) => Position = position;

    }
}