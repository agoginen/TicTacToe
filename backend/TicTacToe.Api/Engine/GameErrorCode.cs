namespace TicTacToe.Api.Engine;

public enum GameErrorCode
{
    GameNotFound,
    CellOutOfRange,
    CellOccupied,
    GameAlreadyCompleted,
    WrongPlayerTurn,
    NoMovesToUndo,
    UndoNotAllowedAfterCompletion
}
