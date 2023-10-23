using System.Collections.Generic;

namespace Chess
{
    class Pawn : Piece
    {
        private bool isMoved;

        public Pawn(string name, PieceColor color, (int X, int Y) position) :
            base(name, color, position)
        {
            isMoved = false;
        }

        public override List<(int X, int Y)> GetPossibleMoves(Piece[,] board)
        {
            _possibleMoves.Clear();

            // TODO: En passant
            if(Color == PieceColor.White)
            {
                _possibleMoves.Add(MoveForward(Position));
                if(!isMoved && board[_possibleMoves[0].X, _possibleMoves[0].Y] == null)
                    _possibleMoves.Add(MoveForward(_possibleMoves[0]));
            }
            else
            {
                _possibleMoves.Add(MoveBackward(Position));
                if(!isMoved && board[_possibleMoves[0].X, _possibleMoves[0].Y] == null)
                    _possibleMoves.Add(MoveBackward(_possibleMoves[0]));
            }

            return _possibleMoves;
        }

        public override void Move((int X, int Y) position)
        {
            if(!isMoved)
                isMoved = true;
            Position = position;
        }
    }
}