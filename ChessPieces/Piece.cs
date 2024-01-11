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
        public (int x, int y) Position { get; set; }
        protected List<(int x, int y)> PossibleMoves { get; set; }

        public Piece(string name, PieceColor color, (int x, int y) position)
        {
            Name = name;
            Color = color;
            Position = position;
            PossibleMoves = new List<(int x, int y)>();
        }

        public virtual void Move((int x, int y) position) => Position = position;

        public abstract List<(int x, int y)> GetPossibleMoves(Piece[,] board);

        protected (int x, int y) MoveForward((int x, int y) position)
        {
            position.x += 1;
            return position;
        }

        protected (int x, int y) MoveBackward((int x, int y) position)
        {
            position.x -= 1;
            return position;
        }

        protected (int x, int y) MoveLeft((int x, int y) position)
        {
            position.y -= 1;
            return position;
        }

        protected (int x, int y) MoveRight((int x , int y) position)
        {
            position.y += 1;
            return position;
        }

        protected (int x, int y) MoveForwardRight((int x , int y) position) => 
            MoveRight(MoveForward(position));

        protected (int x, int y) MoveForwardLeft((int x , int y) position) => 
            MoveLeft(MoveForward(position));

        protected (int x, int y) MoveBackwardRight((int x , int y) position) => 
            MoveRight(MoveBackward(position));

        protected (int x, int y) MoveBackwardLeft((int x , int y) position) => 
            MoveLeft(MoveBackward(position));

        protected void GetPossibleMoveInDirection(
            Piece[,] board, Func<(int x, int y), (int x, int y)> moveDirection, int maxSteps = 8)
        {
            var currentPosition = Position;

            for (int i = 0; i < maxSteps; i++)
            {
                currentPosition = moveDirection(currentPosition);

                if (!IsWithinBoard(currentPosition.x, currentPosition.y))
                    break;

                if (board[currentPosition.x, currentPosition.y] != null)
                {
                    // If it's an enemy piece, add the move, but stop there.
                    if (board[currentPosition.x, currentPosition.y].Color != Color)
                        PossibleMoves.Add(currentPosition);

                    break;
                }
                
                PossibleMoves.Add(currentPosition);
            }
        }

        protected bool IsWithinBoard(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }

        protected bool CanMoveTo(Piece[,] board, int x, int y)
        {
            if (!IsWithinBoard(x, y))
                return false;

            var targetPiece = board[x, y];
            return targetPiece == null || targetPiece.Color != Color;
        }
    }
}