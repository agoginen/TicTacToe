using TicTacToe.Api.Models;

namespace TicTacToe.Api.Storage;

public sealed class InMemoryScoreboardStore : IScoreboardStore
{
    private readonly Lock _lock = new();
    private readonly Scoreboard _scoreboard = new();

    public Scoreboard Get()
    {
        lock (_lock)
        {
            return new Scoreboard
            {
                XWins = _scoreboard.XWins,
                OWins = _scoreboard.OWins,
                Draws = _scoreboard.Draws
            };
        }
    }

    public void Update(Action<Scoreboard> mutate)
    {
        lock (_lock)
        {
            mutate(_scoreboard);
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            _scoreboard.XWins = 0;
            _scoreboard.OWins = 0;
            _scoreboard.Draws = 0;
        }
    }
}
