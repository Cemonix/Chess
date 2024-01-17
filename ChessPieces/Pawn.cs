using System;
using System.Collections.Generic;

namespace Chess
{
    class Pawn : Piece
    {
        public delegate void PromotionHandler(Pawn pawn, (int X, int Y) position);

        public event PromotionHandler? OnPromotion;
        public bool HasMoved { get; private set; }
        public bool HasMovedTwoSquares { get; set; }

        public Pawn(string name, PieceColor color, (int x, int y) position) :
            base(name, color, position)
        {
            HasMoved = false;
            HasMovedTwoSquares = false;
        }

        public override List<(int x, int y)> GetPossibleMoves(Piece[,] board)
        {
            PossibleMoves.Clear();

            Func<(int x, int y), (int x, int y)> move = 
                Color == PieceColor.White ? MoveForward : MoveBackward;

            (int x, int y) forwardMove = move(Position);
            if(CanMoveTo(board, forwardMove.x, forwardMove.y))
            {
                PossibleMoves.Add(forwardMove);

                forwardMove = move(PossibleMoves[0]);
                if(!HasMoved && CanMoveTo(board, forwardMove.x, forwardMove.y))
                    PossibleMoves.Add(forwardMove);
            }
            
            AttackInDirection(board, move, MoveLeft);
            AttackInDirection(board, move, MoveRight);

            var enPassantMove = GetEnPassantPossibleMove(board);
            if (enPassantMove.HasValue)
            {
                PossibleMoves.Add(enPassantMove.Value);
            }

            return PossibleMoves;
        }

        public override void Move((int x, int y) position)
        {
            base.Move(position);
            HasMoved = true;

            if (
                (Color == PieceColor.White && position.x == 7) ||
                (Color == PieceColor.Black && position.x == 0)
            )
            {
                OnPromotion?.Invoke(this, position);
            }
        }

        public bool IsTwoSquareMove(int moveX)
        {
            if (!HasMoved && Math.Abs(Position.x - moveX) == 2)
            {
                return true;
            }
            return false;
        }

        public (int x, int y)? GetEnPassantPossibleMove(Piece[,] board)
        {
            int direction = Color == PieceColor.White ? 1 : -1;
            
            if (
                IsWithinBoard(Position.x, Position.y - 1) &&
                board[Position.x, Position.y - 1] is Pawn pawnLeft && 
                pawnLeft.HasMovedTwoSquares
            )
            {
                return (Position.x + direction, Position.y - 1);
            }
            else if (
                IsWithinBoard(Position.x, Position.y + 1) &&
                board[Position.x, Position.y + 1] is Pawn pawnRight &&
                pawnRight.HasMovedTwoSquares
            )
            {
                return (Position.x + direction, Position.y + 1);
            }

            return null;
        }

        protected override bool CanMoveTo(Piece[,] board, int x, int y)
        {
            if (!IsWithinBoard(x, y))
                return false;

            var targetPiece = board[x, y];
            return targetPiece == null;
        }

        private void AttackInDirection(
            Piece[,] board, Func<(int x, int y), (int x, int y)> move,
            Func<(int x, int y), (int x, int y)> attackDirection
        )
        {
            (int x, int y) attack = attackDirection(move(Position));
            if(IsWithinBoard(attack.x, attack.y))
            {
                if(
                    board[attack.x, attack.y] != null &&
                    board[attack.x, attack.y].Color != Color
                )
                {
                    PossibleMoves.Add(attack);
                }
            }
        }
    }
}