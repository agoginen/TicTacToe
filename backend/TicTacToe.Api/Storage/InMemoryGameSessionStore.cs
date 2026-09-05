using System.Collections.Concurrent;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Storage;

public sealed class InMemoryGameSessionStore : IGameSessionStore
{
    private readonly ConcurrentDictionary<Guid, GameSession> _sessions = new();

    public void Add(GameSession session)
    {
        if (!_sessions.TryAdd(session.Id, session))
        {
            throw new InvalidOperationException($"A game session with id '{session.Id}' already exists.");
        }
    }

    public GameSession? Get(Guid id)
    {
        return _sessions.TryGetValue(id, out var session) ? session : null;
    }

    public void Update(GameSession session)
    {
        _sessions[session.Id] = session;
    }
}
