using TicTacToe.Api.Models;
using TicTacToe.Api.Storage;

namespace TicTacToe.Api.Contracts;

public static class ContractMappingExtensions
{
    public static GameStateResponse ToResponse(this GameSession session, IScoreboardStore scoreboardStore)
    {
        return new GameStateResponse(
            session.Id,
            session.Board,
            session.CurrentPlayer,
            session.Mode,
            session.Status,
            session.Winner,
            session.WinningCells,
            session.MoveHistory.Select(m => new MoveHistoryEntryResponse(m.MoveNumber, m.Player, m.Row, m.Col)).ToList(),
            scoreboardStore.Get().ToResponse());
    }

    public static ScoreboardResponse ToResponse(this Scoreboard scoreboard)
    {
        return new ScoreboardResponse(scoreboard.XWins, scoreboard.OWins, scoreboard.Draws);
    }
}
