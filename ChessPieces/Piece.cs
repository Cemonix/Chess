using System;
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

        protected (int X, int Y) MoveForwardRight((int X , int Y) position) => 
            MoveRight(MoveForward(position));

        protected (int X, int Y) MoveForwardLeft((int X , int Y) position) => 
            MoveLeft(MoveForward(position));

        protected (int X, int Y) MoveBackwardRight((int X , int Y) position) => 
            MoveRight(MoveBackward(position));

        protected (int X, int Y) MoveBackwardLeft((int X , int Y) position) => 
            MoveLeft(MoveBackward(position));

        protected bool IsCoorOutOfBoard(int X, int Y)
        {
            if(X > 7 || X < 0)
                return true;
            else if(Y > 7 || Y < 0)
                return true;
            return false;
        }

        protected void GetPossibleMoveInDirection(
            Piece[,] board, Func<(int x, int y), (int x, int y)> move, int directionLen = 8
        )
        {
            var newPosition = Position;
            for (int i = 0; i < directionLen; i++)
            {   
                newPosition = move(newPosition);
                if(IsCoorOutOfBoard(newPosition.X, newPosition.Y))
                    break;
                    
                var boardOnPosition = board[newPosition.X, newPosition.Y];
                if(boardOnPosition == null)
                    _possibleMoves.Add(newPosition);
                else if(boardOnPosition.Color != Color)
                {
                    _possibleMoves.Add(newPosition);
                    break;
                }
                else
                    break;
            }
        }

        protected virtual bool CheckBoardPossition(
            Piece[,] board, (int x, int y) position
        )
        {
            var boardOnPosition = board[position.x, position.y];
            return boardOnPosition == null || boardOnPosition.Color != Color;
        }

        public abstract void Move((int X , int Y) position);

        public abstract List<(int X, int Y)> GetPossibleMoves(Piece[,] board);
    }
}