namespace Chess
{
    class Program
    {
        static void Main(string[] args)
        {
            GameController chessBoard = new();
            chessBoard.Game();
        }
    }
}

// TODO: En passant - null enemy piece if pawn moved
// TODO: Castling
// TODO: Chess bot