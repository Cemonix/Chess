using System.Collections.Generic;

namespace Chess
{
    class Rook : Piece
    {
        public Rook(string name, PieceColor color, (int X, int Y) position) :
            base(name, color, position) {}

        public override List<(int X, int Y)> GetPossibleMoves(Piece[,] board)
        {
            (int X, int Y) faultCoor = (-1, -1);
            bool isForwardOccupied = false;
            bool isBackwardOccupied = false;
            bool isLeftOccupied = false;
            bool isRightOccupied = false;

            _possibleMoves[0] = MoveForward(Position);
            _possibleMoves[7] = MoveBackward(Position);
            _possibleMoves[14] = MoveLeft(Position);
            _possibleMoves[21] = MoveRight(Position);
            for (int i = 0; i < 8; i++)
            {
                if (board[_possibleMoves[0+i].X, _possibleMoves[0+i].Y] is not null)
                    isForwardOccupied = true;
                _possibleMoves[1+i] = isForwardOccupied ? faultCoor : MoveForward(_possibleMoves[0+i]);

                if (board[_possibleMoves[7+i].X, _possibleMoves[7+i].Y] is not null)
                    isBackwardOccupied = true;
                _possibleMoves[8+i] = isBackwardOccupied ? faultCoor : MoveBackward(_possibleMoves[7+i]);

                if (board[_possibleMoves[14+i].X, _possibleMoves[14+i].Y] is not null)
                    isLeftOccupied = true;
                _possibleMoves[15+i] = isLeftOccupied ? faultCoor : MoveLeft(_possibleMoves[14+i]);

                if (board[_possibleMoves[21+i].X, _possibleMoves[21+i].Y] is not null)
                    isRightOccupied = true;
                _possibleMoves[22+i] = isRightOccupied ? faultCoor : MoveRight(_possibleMoves[21+i]);
            }

            _possibleMoves[0] = MoveForward(Position);
            for (int i = 0; i < 8; i++)
            {
                if (board[_possibleMoves[0+i].X, _possibleMoves[0+i].Y] is not null)
                    break;
                _possibleMoves[1+i] = isForwardOccupied ? faultCoor : MoveForward(_possibleMoves[0+i]);
            }

            return _possibleMoves;
        }

        public override void Move((int X, int Y) position) => Position = position;

    }
}