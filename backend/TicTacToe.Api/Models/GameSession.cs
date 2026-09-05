namespace TicTacToe.Api.Models;

// Mutable session record; the game engine (Phase 2) owns all transitions and recomputes
// derived state (status/winner/winning cells) from Board + MoveHistory rather than patching fields ad hoc.
public sealed class GameSession
{
    public required Guid Id { get; init; }

    public required GameMode Mode { get; init; }

    // 9-cell board, row-major: index = row * 3 + col. Null means the cell is empty.
    public Player?[] Board { get; } = new Player?[9];

    public Player CurrentPlayer { get; set; } = Player.X;

    public GameStatus Status { get; set; } = GameStatus.InProgress;

    public Player? Winner { get; set; }

    // Board indices (0-8) that make up the winning row/column/diagonal, if any.
    public IReadOnlyList<int>? WinningCells { get; set; }

    public List<MoveRecord> MoveHistory { get; } = new();
}
