using TicTacToe.Api.Models;

namespace TicTacToe.Api.Storage;

public interface IGameSessionStore
{
    void Add(GameSession session);

    GameSession? Get(Guid id);

    void Update(GameSession session);
}
