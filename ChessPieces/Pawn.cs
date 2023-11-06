using System;
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
            Func<(int X, int Y), (int X, int Y)> move = 
                Color == PieceColor.White ? MoveForward : MoveBackward;

            (int x, int y) forwardMove = move(Position);
            if(CheckBoardPossition(board, forwardMove))
            {
                _possibleMoves.Add(forwardMove);

                forwardMove = move(_possibleMoves[0]);
                if(!isMoved && CheckBoardPossition(board, forwardMove))
                    _possibleMoves.Add(forwardMove);
            }

            return _possibleMoves;
        }

        public override void Move((int X, int Y) position)
        {
            if(!isMoved)
                isMoved = true;
            Position = position;
        }

        protected override bool CheckBoardPossition(
            Piece[,] board, (int x, int y) position
        )
        {
            var boardOnPosition = board[position.x, position.y];
            return boardOnPosition == null;
        }
    }
}