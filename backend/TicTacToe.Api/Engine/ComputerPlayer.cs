using TicTacToe.Api.Models;

namespace TicTacToe.Api.Engine;

// Computer always plays O; human always plays X (per problem statement).
public static class ComputerPlayer
{
    private static readonly int[] Corners = [0, 2, 6, 8];
    private const int Center = 4;

    public static int SelectMove(Player?[] board)
    {
        return TryWin(board, Player.O)
            ?? TryBlock(board, Player.X)
            ?? TryCenter(board)
            ?? TryCorner(board)
            ?? TryAny(board)
            ?? throw new InvalidOperationException("No available cell for the computer to play.");
    }

    // Priority 1: win immediately if possible.
    public static int? TryWin(Player?[] board, Player player)
    {
        foreach (var cell in EmptyCells(board))
        {
            if (BoardEvaluator.WinsAt(board, cell, player))
            {
                return cell;
            }
        }

        return null;
    }

    // Priority 2: block the opponent's winning move.
    public static int? TryBlock(Player?[] board, Player opponent) => TryWin(board, opponent);

    // Priority 3: take the center.
    public static int? TryCenter(Player?[] board) => board[Center] is null ? Center : null;

    // Priority 4: take a corner.
    public static int? TryCorner(Player?[] board)
    {
        foreach (var cell in Corners)
        {
            if (board[cell] is null)
            {
                return cell;
            }
        }

        return null;
    }

    // Priority 5: take any remaining empty cell.
    public static int? TryAny(Player?[] board)
    {
        foreach (var cell in EmptyCells(board))
        {
            return cell;
        }

        return null;
    }

    private static IEnumerable<int> EmptyCells(Player?[] board)
    {
        for (var i = 0; i < board.Length; i++)
        {
            if (board[i] is null)
            {
                yield return i;
            }
        }
    }
}
