using System.Collections.Generic;

namespace Chess
{
    class King : Piece
    {
        public King(string name, PieceColor color, (int X, int Y) position) :
            base(name, color, position) {}

        public override List<(int X, int Y)> GetPossibleMoves(Piece[,] board)
        {
            // TODO: Castling
            // TODO: Check mate - must not step into field where enemy can move 
            //       (go through every enemy piece on board and set each possible move to -1, -1
            //        if enemy can move there)
            _possibleMoves[0] = MoveForward(Position);
            _possibleMoves[1] = MoveBackward(Position);
            _possibleMoves[2] = MoveLeft(Position);
            _possibleMoves[3] = MoveRight(Position);
            _possibleMoves[4] = MoveLeft(MoveForward(Position));
            _possibleMoves[5] = MoveRight(MoveForward(Position));
            _possibleMoves[6] = MoveLeft(MoveBackward(Position));
            _possibleMoves[7] = MoveRight(MoveBackward(Position));

            return _possibleMoves;
        }

        public override void Move((int X, int Y) position) => Position = position;
        
    }
}