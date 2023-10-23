using System.Collections.Generic;

namespace Chess
{   
    enum PieceColor
    {
        White,
        Black
    }

    abstract class Piece
    {
        public string Name { get; set;}
        public PieceColor Color { get; }
        public (int X, int Y) Position { get; set; }
        protected List<(int X, int Y)> _possibleMoves;

        public Piece(string name, PieceColor color, (int X, int Y) position)
        {
            Name = name;
            Color = color;
            Position = position;
            _possibleMoves = new List<(int X, int Y)>();
        }

        protected (int X, int Y) MoveForward((int X, int Y) position)
        {
            position.X += 1;
            return position;
        }

        protected (int X, int Y) MoveBackward((int X, int Y) position)
        {
            position.X -= 1;
            return position;
        }

        protected (int X, int Y) MoveLeft((int X, int Y) position)
        {
            position.Y -= 1;
            return position;
        }

        protected (int X, int Y) MoveRight((int X , int Y) position)
        {
            position.Y += 1;
            return position;
        }

        public abstract void Move((int X , int Y) position);

        public abstract List<(int X, int Y)> GetPossibleMoves(Piece[,] board);

        protected bool IsCoorOutOfBoard(int X, int Y)
        {
            if(X > 7)
                return true;
            else if(X < 0)
                return true;
            else if(Y > 7)
                return true;
            else if(Y < 0)
                return true;
            return false;
        }
    }
}