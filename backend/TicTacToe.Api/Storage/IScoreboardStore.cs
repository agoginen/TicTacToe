using TicTacToe.Api.Models;

namespace TicTacToe.Api.Storage;

public interface IScoreboardStore
{
    // Returns a snapshot; mutate the live scoreboard only via Update/Reset.
    Scoreboard Get();

    void Update(Action<Scoreboard> mutate);

    void Reset();
}
