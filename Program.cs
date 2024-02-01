namespace Chess
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            GameController chessBoard = new();
            chessBoard.Game();
        }
    }
}