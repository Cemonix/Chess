using System.Collections.Generic;

namespace Chess
{
    class Knight : Piece
    {
        public Knight(string name, PieceColor color, (int x, int y) position) :
            base(name, color, position) {}

        public override List<(int x, int y)> GetPossibleMoves(Piece[,] board)
        {   
            PossibleMoves.Clear();
            
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
                if(CanMoveTo(board, possiblePositions[i].x, possiblePositions[i].y))
                    PossibleMoves.Add((possiblePositions[i].x, possiblePositions[i].y));
            }

            return PossibleMoves;
        }
    }
}