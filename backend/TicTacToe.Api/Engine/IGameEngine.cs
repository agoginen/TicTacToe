using TicTacToe.Api.Models;

namespace TicTacToe.Api.Engine;

public interface IGameEngine
{
    GameSession CreateGame(GameMode mode);

    GameSession GetGame(Guid id);

    GameSession ApplyMove(Guid id, Player player, int row, int col);

    GameSession Undo(Guid id);

    GameSession Reset(Guid id);
}
