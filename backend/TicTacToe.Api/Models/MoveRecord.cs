namespace TicTacToe.Api.Models;

// Row/Col are 0-indexed; the API/frontend layer is responsible for any 1-indexed display formatting.
public sealed record MoveRecord(int MoveNumber, Player Player, int Row, int Col);
