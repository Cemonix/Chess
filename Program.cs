namespace Chess{
    class Program
    {
        static void Main(string[] args)
        {
            ChessBoard chessBoard = new();
            chessBoard.Game();
        }
    }
}

// FIXME: GetPossibleMoves - finish it in every chess piece or find out new solution
// TODO: King possible moves cannot be places where he is in danger
// TODO: Make queen out of pawn if on last line
// TODO: En passant
// TODO: Castling
// TODO: Game end, restart
// TODO: Chess bot
