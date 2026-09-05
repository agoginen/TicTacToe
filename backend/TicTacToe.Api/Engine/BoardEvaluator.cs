using TicTacToe.Api.Models;

namespace TicTacToe.Api.Engine;

// Pure board evaluation: rows, columns, and diagonals, row-major index = row * 3 + col.
public static class BoardEvaluator
{
    public static readonly IReadOnlyList<int[]> WinningLines =
    [
        [0, 1, 2], [3, 4, 5], [6, 7, 8], // rows
        [0, 3, 6], [1, 4, 7], [2, 5, 8], // columns
        [0, 4, 8], [2, 4, 6]             // diagonals
    ];

    public static (GameStatus Status, Player? Winner, IReadOnlyList<int>? WinningCells) Evaluate(Player?[] board)
    {
        foreach (var line in WinningLines)
        {
            var (a, b, c) = (board[line[0]], board[line[1]], board[line[2]]);
            if (a is not null && a == b && b == c)
            {
                return (GameStatus.Won, a, line);
            }
        }

        return board.All(cell => cell is not null)
            ? (GameStatus.Draw, null, null)
            : (GameStatus.InProgress, null, null);
    }

    // True if placing `player` at `cellIndex` on `board` would produce a win.
    public static bool WinsAt(Player?[] board, int cellIndex, Player player)
    {
        var trial = (Player?[])board.Clone();
        trial[cellIndex] = player;
        return Evaluate(trial).Status == GameStatus.Won;
    }
}
