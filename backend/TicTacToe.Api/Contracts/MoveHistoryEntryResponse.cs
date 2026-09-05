using TicTacToe.Api.Models;

namespace TicTacToe.Api.Contracts;

public sealed record MoveHistoryEntryResponse(int MoveNumber, Player Player, int Row, int Col);
