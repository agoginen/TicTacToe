namespace TicTacToe.Api.Engine;

// Thrown for all rejected game operations; the API layer (Phase 3) maps ErrorCode to an HTTP status.
public sealed class GameEngineException(GameErrorCode errorCode, string message) : Exception(message)
{
    public GameErrorCode ErrorCode { get; } = errorCode;
}
