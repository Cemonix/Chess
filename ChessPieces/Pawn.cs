using System;
using System.Collections.Generic;

namespace Chess
{
    class Pawn : Piece
    {
        private bool isMoved;
        public bool isEnPassant;

        public Pawn(string name, PieceColor color, (int X, int Y) position) :
            base(name, color, position)
        {
            isMoved = false;
            isEnPassant = false;
        }

        public override List<(int X, int Y)> GetPossibleMoves(Piece[,] board)
        {
            _possibleMoves.Clear();

            Func<(int X, int Y), (int X, int Y)> move = 
                Color == PieceColor.White ? MoveForward : MoveBackward;

            (int x, int y) forwardMove = move(Position);
            if(CheckBoardPossition(board, forwardMove))
            {
                _possibleMoves.Add(forwardMove);

                forwardMove = move(_possibleMoves[0]);
                if(!isMoved && CheckBoardPossition(board, forwardMove))
                    _possibleMoves.Add(forwardMove);
            }
            
            AttackInDirection(board, move, MoveLeft);
            AttackInDirection(board, move, MoveRight);

            // isEnPassant = false;
            // EnPassant(board, move, MoveRight);
            // EnPassant(board, move, MoveLeft);

            return _possibleMoves;
        }

        public override void Move((int X, int Y) position)
        {
            if(!isMoved)
                isMoved = true;
            Position = position;
        }

        protected override bool CheckBoardPossition(
            Piece[,] board, (int x, int y) position
        )
        {
            var boardOnPosition = board[position.x, position.y];
            return boardOnPosition == null;
        }

        private void AttackInDirection(
            Piece[,] board, Func<(int X, int Y), (int X, int Y)> move,
            Func<(int X, int Y), (int X, int Y)> attackDirection
        )
        {
            (int x, int y) attack = attackDirection(move(Position));
            if(!IsCoorOutOfBoard(attack.x, attack.y))
            {
                if(
                    board[attack.x, attack.y] != null &&
                    board[attack.x, attack.y].Color != Color
                )
                {
                    _possibleMoves.Add(attack);
                }
            }
        }

        private void EnPassant(
            Piece[,] board, Func<(int X, int Y), (int X, int Y)> move,
            Func<(int X, int Y), (int X, int Y)> passantDirection
        )
        {
            // TODO: Check if white piece is at 5 line or black piece at 4 line
            // TODO: Check if enemy pawn moved 2 spaces in last turn (this condition is hard to check)
            char enemyColor = Color == PieceColor.White ? 'B' : 'W';

            (int passantMoveX, int passantMoveY) = (Position.X, Position.Y + 1);
            if(!IsCoorOutOfBoard(passantMoveX, passantMoveY))
            {
                if(
                    board[passantMoveX, passantMoveY] != null &&
                    board[passantMoveX, passantMoveY].Name == $" {enemyColor}_Pa "
                )
                    _possibleMoves.Add(passantDirection(move(Position)));
                    isEnPassant = true;
            }
        }
    }
}