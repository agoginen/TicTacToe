namespace TicTacToe.Api.Models;

// Session-level scoreboard, independent of any single GameSession.
public sealed class Scoreboard
{
    public int XWins { get; set; }

    public int OWins { get; set; }

    public int Draws { get; set; }
}
