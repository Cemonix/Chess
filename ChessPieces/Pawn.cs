using System;
using System.Collections.Generic;

namespace Chess
{
    class Pawn : Piece
    {
        public delegate void PromotionHandler(Pawn pawn, (int X, int Y) position);

        public event PromotionHandler? OnPromotion;
        public bool HasMoved { get; private set; }
        public bool isEnPassant;

        public Pawn(string name, PieceColor color, (int x, int y) position) :
            base(name, color, position)
        {
            HasMoved = false;
            isEnPassant = false;
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

            // isEnPassant = false;
            // EnPassant(board, move, MoveRight);
            // EnPassant(board, move, MoveLeft);

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

        protected override bool CanMoveTo(Piece[,] board, int x, int y)
        {
            if (!IsWithinBoard(x, y))
                return false;

            var targetPiece = board[x, y];
            return targetPiece == null;
        }

        private void EnPassant(
            Piece[,] board, Func<(int x, int y), (int x, int y)> move,
            Func<(int x, int y), (int x, int y)> passantDirection
        )
        {
            // TODO: Check if white piece is at 5 line or black piece at 4 line
            // TODO: Check if enemy pawn moved 2 spaces in last turn (this condition is hard to check)
            char enemyColor = Color == PieceColor.White ? 'B' : 'W';

            (int passantMovex, int passantMovey) = (Position.x, Position.y + 1);
            if(IsWithinBoard(passantMovex, passantMovey))
            {
                if(
                    board[passantMovex, passantMovey] != null &&
                    board[passantMovex, passantMovey].Name == $" {enemyColor}_Pa "
                )
                    PossibleMoves.Add(passantDirection(move(Position)));
                    isEnPassant = true;
            }
        }
    }
}