using System;
using System.Collections.Generic;

namespace Chess
{
    class King : Piece
    {
        public bool HasMoved { get; private set; }

        public King(
            string name, PieceColor color, (int x, int y) position
        ) : base(name, color, position) 
        {
            HasMoved = false;
        }

        public override List<(int x, int y)> GetPossibleMoves(Piece[,] board)
        {
            PossibleMoves.Clear();

            GetPossibleMoveInDirection(board, MoveForward, 1);
            GetPossibleMoveInDirection(board, MoveBackward, 1);
            GetPossibleMoveInDirection(board, MoveLeft, 1);
            GetPossibleMoveInDirection(board, MoveRight, 1);
            GetPossibleMoveInDirection(board, MoveForwardRight, 1);
            GetPossibleMoveInDirection(board, MoveForwardLeft, 1);
            GetPossibleMoveInDirection(board, MoveBackwardRight, 1);
            GetPossibleMoveInDirection(board, MoveBackwardLeft, 1);

            return PossibleMoves;
        }

        public override void Move((int x, int y) position)
        {
            base.Move(position);
            HasMoved = true;
        }

        public List<(int x, int y)> GetCastlingMoves(
            Piece[,] board,
            Dictionary<(int x, int y), List<(int x, int y)>> opponentPossibleMoves
        )
        {
            var castlingMoves = new List<(int x, int y)>();
            if (
                HasMoved ||
                ChessBoard.IsSquareUnderAttack(Position, opponentPossibleMoves) // Is king square under attack
            )
            {
                return castlingMoves;
            }

            // Check Kingside Castling
            if (CanCastle(board, 7, opponentPossibleMoves))
            {
                castlingMoves.Add((Position.x, 6));
            }

            // Check Queenside Castling
            if (CanCastle(board, 0, opponentPossibleMoves))
            {
                castlingMoves.Add((Position.x, 2));
            }

            return castlingMoves;
        }

        public bool CanCastle(
            Piece[,] board, int rookColumn,
            Dictionary<(int x, int y), List<(int x, int y)>> opponentPossibleMoves
        )
        {
            int pathStart = Position.y < rookColumn ? Position.y + 1 : rookColumn + 1;
            int pathEnd = Position.y < rookColumn ? rookColumn - 1 : Position.y - 1;

            // Check if the path between the king and the rook is clear
            for (int column = pathStart; column <= pathEnd; column++)
            {
                if (board[Position.x, column] != null)
                {
                    return false;
                }
            }

            // Check if the rook has moved
            if (board[Position.x, rookColumn] is not Rook rook || rook.HasMoved)
            {
                return false;
            }

            // Check if any squares that the king passes through are under attack
            for (
                int column = Position.y;
                column != rookColumn;
                column += rookColumn > Position.y ? 1 : -1
            )
            {
                if (
                    ChessBoard.IsSquareUnderAttack(
                        (Position.x, column), opponentPossibleMoves
                    )
                )
                {
                    return false;
                }
            }

            return true;
        }
    }
}