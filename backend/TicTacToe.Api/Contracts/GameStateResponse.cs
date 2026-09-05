using TicTacToe.Api.Models;

namespace TicTacToe.Api.Contracts;

public sealed record GameStateResponse(
    Guid GameId,
    Player?[] Board,
    Player CurrentPlayer,
    GameMode Mode,
    GameStatus Status,
    Player? Winner,
    IReadOnlyList<int>? WinningCells,
    IReadOnlyList<MoveHistoryEntryResponse> MoveHistory,
    ScoreboardResponse Scoreboard);
