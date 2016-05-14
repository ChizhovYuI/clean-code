using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace Chess
{
    public class ChessProblem
    {
        private static Board board;
        public static ChessStatus ChessStatus;

        public static void LoadFrom(string[] lines)
        {
            board = new Board(lines);
        }

        // Определяет мат, шах или пат белым.
        public static void CalculateChessStatus()
        {
            ChessStatus = !IsCheckForWhite()
                ? (HasMoves() ? ChessStatus.Ok : ChessStatus.Stalemate)
                : (HasMoves() ? ChessStatus.Check : ChessStatus.Mate);
        }

        private static bool HasMoves() => (
            from locFrom in board.GetPieces(PieceColor.White)
            from locTo in GetAllMovesForPiece(locFrom)
            select IsSafeMove(locFrom, locTo))
            .Any(x => x);

        private static bool IsSafeMove(Location locFrom, Location locTo)
        {
            var hasMoves = false;
            var old = board.Get(locTo);
            SetMove(locFrom, locTo, ColoredPiece.Empty);
            if (!IsCheckForWhite())
                hasMoves = true;
            SetMove(locTo, locFrom, old);
            return hasMoves;
        }

        private static void SetMove(Location locFrom, Location locTo, ColoredPiece piece)
        {
            board.Set(locTo, board.Get(locFrom));
            board.Set(locFrom, piece);
        }

        private static bool IsCheckForWhite()
        {
            return board.GetPieces(PieceColor.Black)
                .SelectMany(GetAllMovesForPiece)
                .Any(IsWhiteKingLocation);
        }

        private static IEnumerable<Location> GetAllMovesForPiece(Location loc)
            => board.Get(loc).Piece.GetMoves(loc, board);

        private static bool IsWhiteKingLocation(Location loc)
            => board.Get(loc).Is(PieceColor.White, Piece.King);
    }
}