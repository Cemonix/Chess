

using System.Collections.Generic;

namespace Chess
{
    class Knight : Piece
    {
        public Knight(string name, PieceColor color, (int X, int Y) position) :
            base(name, color, position) {}

        public override List<(int X, int Y)> GetPossibleMoves(Piece[,] board)
        {   
            _possibleMoves.Clear();

            (int fflX, int fflY) = MoveLeft(MoveForward(MoveForward(Position)));
            if(!IsCoorOutOfBoard(fflX, fflY))
                _possibleMoves.Add((fflX, fflY));

            (int ffrX, int ffrY) = MoveRight(MoveForward(MoveForward(Position)));
            if(!IsCoorOutOfBoard(ffrX, ffrY))
                _possibleMoves.Add((ffrX, ffrY));

            (int bblX, int bblY) = MoveLeft(MoveBackward(MoveBackward(Position)));
            if(!IsCoorOutOfBoard(bblX, bblY))
                _possibleMoves.Add((bblX, bblY));

            (int bbrX, int bbrY) = MoveRight(MoveBackward(MoveBackward(Position)));
            if(!IsCoorOutOfBoard(bbrX, bbrY))
                _possibleMoves.Add((bbrX, bbrY));

            (int llfX, int llfY) = MoveForward(MoveLeft(MoveLeft(Position)));
            if(!IsCoorOutOfBoard(llfX, llfY))
                _possibleMoves.Add((llfX, llfY));

            (int llbX, int llbY) = MoveBackward(MoveLeft(MoveLeft(Position)));
            if(!IsCoorOutOfBoard(llbX, llbY))
                _possibleMoves.Add((llbX, llbY));

            (int rrfX, int rrfY) = MoveForward(MoveRight(MoveRight(Position)));
            if(!IsCoorOutOfBoard(rrfX, rrfY))
                _possibleMoves.Add((rrfX, rrfY));

            (int rrbX, int rrbY) = MoveBackward(MoveRight(MoveRight(Position)));
            if(!IsCoorOutOfBoard(rrbX, rrbY))
                _possibleMoves.Add((rrbX, rrbY));

            return _possibleMoves;
        }

        public override void Move((int X, int Y) position) => Position = position;
    }
}