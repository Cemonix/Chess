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
            
            List<(int x, int y)> possiblePositions = new()
            {
                MoveLeft(MoveForward(MoveForward(Position))),
                MoveRight(MoveBackward(MoveBackward(Position))),
                MoveForward(MoveLeft(MoveLeft(Position))),
                MoveBackward(MoveLeft(MoveLeft(Position))),
                MoveForward(MoveRight(MoveRight(Position))),
                MoveBackward(MoveRight(MoveRight(Position))),
                MoveRight(MoveForward(MoveForward(Position))),
                MoveLeft(MoveBackward(MoveBackward(Position)))
            };

            for (int i = 0; i < possiblePositions.Count; i++)
            {
                if(
                    !IsCoorOutOfBoard(possiblePositions[i].x, possiblePositions[i].y) && 
                    CheckBoardPossition(board, possiblePositions[i])
                )
                    _possibleMoves.Add((possiblePositions[i].x, possiblePositions[i].y));
            }

            return _possibleMoves;
        }

        public override void Move((int X, int Y) position) => Position = position;
    }
}