using TicTacToe.Api.Models;
using TicTacToe.Api.Storage;

namespace TicTacToe.Api.Engine;

// Undo/scoreboard policy: Option A - once a game is Won/Draw, Undo is disabled and the
// scoreboard entry is final (documented in README per Phase 7).
public sealed class GameEngine(IGameSessionStore sessions, IScoreboardStore scoreboard) : IGameEngine
{
    private const int BoardDimension = 3;

    public GameSession CreateGame(GameMode mode)
    {
        var session = new GameSession { Id = Guid.NewGuid(), Mode = mode };
        sessions.Add(session);
        return session;
    }

    public GameSession GetGame(Guid id)
    {
        return sessions.Get(id) ?? throw new GameEngineException(GameErrorCode.GameNotFound, $"Game '{id}' was not found.");
    }

    public GameSession ApplyMove(Guid id, Player player, int row, int col)
    {
        var session = GetGame(id);

        if (session.Status != GameStatus.InProgress)
        {
            throw new GameEngineException(GameErrorCode.GameAlreadyCompleted, "The game has already completed.");
        }

        if (row is < 0 or >= BoardDimension || col is < 0 or >= BoardDimension)
        {
            throw new GameEngineException(GameErrorCode.CellOutOfRange, $"Row/column must be between 0 and {BoardDimension - 1}.");
        }

        var cellIndex = ToCellIndex(row, col);
        if (session.Board[cellIndex] is not null)
        {
            throw new GameEngineException(GameErrorCode.CellOccupied, $"Cell ({row},{col}) is already occupied.");
        }

        if (player != session.CurrentPlayer)
        {
            throw new GameEngineException(GameErrorCode.WrongPlayerTurn, $"It is not {player}'s turn.");
        }

        PlaceMoveAndEvaluate(session, player, row, col);

        if (session.Mode == GameMode.VsComputer && session.Status == GameStatus.InProgress && session.CurrentPlayer == Player.O)
        {
            var computerCell = ComputerPlayer.SelectMove(session.Board);
            var (computerRow, computerCol) = ToRowCol(computerCell);
            PlaceMoveAndEvaluate(session, Player.O, computerRow, computerCol);
        }

        sessions.Update(session);
        return session;
    }

    public GameSession Undo(Guid id)
    {
        var session = GetGame(id);

        if (session.Status != GameStatus.InProgress)
        {
            throw new GameEngineException(GameErrorCode.UndoNotAllowedAfterCompletion, "Undo is disabled once the game has completed.");
        }

        if (session.MoveHistory.Count == 0)
        {
            throw new GameEngineException(GameErrorCode.NoMovesToUndo, "There are no moves to undo.");
        }

        var movesToRemove = session.Mode == GameMode.VsComputer ? 2 : 1;
        movesToRemove = Math.Min(movesToRemove, session.MoveHistory.Count);
        session.MoveHistory.RemoveRange(session.MoveHistory.Count - movesToRemove, movesToRemove);

        RebuildFromHistory(session);
        sessions.Update(session);
        return session;
    }

    public GameSession Reset(Guid id)
    {
        var session = GetGame(id);

        Array.Clear(session.Board);
        session.MoveHistory.Clear();
        session.CurrentPlayer = Player.X;
        session.Status = GameStatus.InProgress;
        session.Winner = null;
        session.WinningCells = null;

        sessions.Update(session);
        return session;
    }

    private void PlaceMoveAndEvaluate(GameSession session, Player player, int row, int col)
    {
        var cellIndex = ToCellIndex(row, col);
        session.Board[cellIndex] = player;
        session.MoveHistory.Add(new MoveRecord(session.MoveHistory.Count + 1, player, row, col));

        var (status, winner, winningCells) = BoardEvaluator.Evaluate(session.Board);
        session.Status = status;
        session.Winner = winner;
        session.WinningCells = winningCells;

        switch (status)
        {
            case GameStatus.Won:
                scoreboard.Update(sb =>
                {
                    if (winner == Player.X) sb.XWins++;
                    else sb.OWins++;
                });
                break;
            case GameStatus.Draw:
                scoreboard.Update(sb => sb.Draws++);
                break;
            case GameStatus.InProgress:
                session.CurrentPlayer = player == Player.X ? Player.O : Player.X;
                break;
        }
    }

    private static void RebuildFromHistory(GameSession session)
    {
        Array.Clear(session.Board);
        foreach (var move in session.MoveHistory)
        {
            session.Board[ToCellIndex(move.Row, move.Col)] = move.Player;
        }

        var (status, winner, winningCells) = BoardEvaluator.Evaluate(session.Board);
        session.Status = status;
        session.Winner = winner;
        session.WinningCells = winningCells;
        session.CurrentPlayer = session.MoveHistory.Count % 2 == 0 ? Player.X : Player.O;
    }

    private static int ToCellIndex(int row, int col) => row * BoardDimension + col;

    private static (int Row, int Col) ToRowCol(int cellIndex) => (cellIndex / BoardDimension, cellIndex % BoardDimension);
}
