namespace Chess
{
    class Program
    {
        static void Main(string[] args)
        {
            ChessBoard chessBoard = new();
            chessBoard.Game();
        }
    }
}

// TODO: King possible moves cannot be places where he is in danger
// TODO: En passant - null enemy piece if pawn moved
// TODO: Castling
// TODO: Game end, restart
// TODO: Chess bot