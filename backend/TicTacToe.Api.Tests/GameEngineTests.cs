using TicTacToe.Api.Engine;
using TicTacToe.Api.Models;
using TicTacToe.Api.Storage;
using Xunit;

namespace TicTacToe.Api.Tests;

public class GameEngineTests
{
    private static GameEngine CreateEngine(out IScoreboardStore scoreboardStore)
    {
        scoreboardStore = new InMemoryScoreboardStore();
        return new GameEngine(new InMemoryGameSessionStore(), scoreboardStore);
    }

    [Fact]
    public void ApplyMove_ValidMove_PlacesMarkAndSwitchesTurn()
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.TwoPlayer);

        var result = engine.ApplyMove(game.Id, Player.X, 0, 0);

        Assert.Equal(Player.X, result.Board[0]);
        Assert.Equal(Player.O, result.CurrentPlayer);
        Assert.Equal(GameStatus.InProgress, result.Status);
        Assert.Single(result.MoveHistory);
        Assert.Equal(new MoveRecord(1, Player.X, 0, 0), result.MoveHistory[0]);
    }

    [Fact]
    public void ApplyMove_InvalidMoveDoesNotChangeTurn()
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.TwoPlayer);
        engine.ApplyMove(game.Id, Player.X, 0, 0);

        Assert.Throws<GameEngineException>(() => engine.ApplyMove(game.Id, Player.O, 0, 0));

        var current = engine.GetGame(game.Id);
        Assert.Equal(Player.O, current.CurrentPlayer);
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(0, -1)]
    [InlineData(3, 0)]
    [InlineData(0, 3)]
    public void ApplyMove_CellOutOfRange_Throws(int row, int col)
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.TwoPlayer);

        var ex = Assert.Throws<GameEngineException>(() => engine.ApplyMove(game.Id, Player.X, row, col));
        Assert.Equal(GameErrorCode.CellOutOfRange, ex.ErrorCode);
    }

    [Fact]
    public void ApplyMove_OccupiedCell_Throws()
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.TwoPlayer);
        engine.ApplyMove(game.Id, Player.X, 0, 0);

        var ex = Assert.Throws<GameEngineException>(() => engine.ApplyMove(game.Id, Player.O, 0, 0));
        Assert.Equal(GameErrorCode.CellOccupied, ex.ErrorCode);
    }

    [Fact]
    public void ApplyMove_WrongPlayerTurn_Throws()
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.TwoPlayer);

        var ex = Assert.Throws<GameEngineException>(() => engine.ApplyMove(game.Id, Player.O, 0, 0));
        Assert.Equal(GameErrorCode.WrongPlayerTurn, ex.ErrorCode);
    }

    [Fact]
    public void ApplyMove_AfterCompletion_Throws()
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.TwoPlayer);
        PlayRowWinForX(engine, game.Id);

        var ex = Assert.Throws<GameEngineException>(() => engine.ApplyMove(game.Id, Player.O, 1, 2));
        Assert.Equal(GameErrorCode.GameAlreadyCompleted, ex.ErrorCode);
    }

    [Fact]
    public void ApplyMove_RowWin_SetsWonStatusAndWinningCells()
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.TwoPlayer);

        var result = PlayRowWinForX(engine, game.Id);

        Assert.Equal(GameStatus.Won, result.Status);
        Assert.Equal(Player.X, result.Winner);
        Assert.Equal([0, 1, 2], result.WinningCells);
    }

    [Fact]
    public void ApplyMove_ColumnWin_SetsWonStatus()
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.TwoPlayer);

        engine.ApplyMove(game.Id, Player.X, 0, 0);
        engine.ApplyMove(game.Id, Player.O, 0, 1);
        engine.ApplyMove(game.Id, Player.X, 1, 0);
        engine.ApplyMove(game.Id, Player.O, 1, 1);
        var result = engine.ApplyMove(game.Id, Player.X, 2, 0);

        Assert.Equal(GameStatus.Won, result.Status);
        Assert.Equal(Player.X, result.Winner);
        Assert.Equal([0, 3, 6], result.WinningCells);
    }

    [Fact]
    public void ApplyMove_DiagonalWin_SetsWonStatus()
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.TwoPlayer);

        engine.ApplyMove(game.Id, Player.X, 0, 0);
        engine.ApplyMove(game.Id, Player.O, 0, 1);
        engine.ApplyMove(game.Id, Player.X, 1, 1);
        engine.ApplyMove(game.Id, Player.O, 0, 2);
        var result = engine.ApplyMove(game.Id, Player.X, 2, 2);

        Assert.Equal(GameStatus.Won, result.Status);
        Assert.Equal(Player.X, result.Winner);
        Assert.Equal([0, 4, 8], result.WinningCells);
    }

    [Fact]
    public void ApplyMove_AllCellsFilledNoWinner_SetsDrawStatus()
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.TwoPlayer);

        engine.ApplyMove(game.Id, Player.X, 0, 0);
        engine.ApplyMove(game.Id, Player.O, 0, 1);
        engine.ApplyMove(game.Id, Player.X, 0, 2);
        engine.ApplyMove(game.Id, Player.O, 1, 1);
        engine.ApplyMove(game.Id, Player.X, 1, 0);
        engine.ApplyMove(game.Id, Player.O, 1, 2);
        engine.ApplyMove(game.Id, Player.X, 2, 1);
        engine.ApplyMove(game.Id, Player.O, 2, 0);
        var result = engine.ApplyMove(game.Id, Player.X, 2, 2);

        Assert.Equal(GameStatus.Draw, result.Status);
        Assert.Null(result.Winner);
    }

    [Fact]
    public void ApplyMove_VsComputerMode_ComputerRepliesAutomatically()
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.VsComputer);

        var result = engine.ApplyMove(game.Id, Player.X, 0, 0);

        Assert.Equal(2, result.MoveHistory.Count);
        Assert.Equal(Player.O, result.MoveHistory[1].Player);
        Assert.Equal(Player.X, result.CurrentPlayer);
    }

    [Fact]
    public void Reset_ClearsGameStateButKeepsScoreboard()
    {
        var engine = CreateEngine(out var scoreboardStore);
        var game = engine.CreateGame(GameMode.TwoPlayer);
        PlayRowWinForX(engine, game.Id);
        Assert.Equal(1, scoreboardStore.Get().XWins);

        var result = engine.Reset(game.Id);

        Assert.All(result.Board, cell => Assert.Null(cell));
        Assert.Empty(result.MoveHistory);
        Assert.Equal(GameStatus.InProgress, result.Status);
        Assert.Null(result.Winner);
        Assert.Null(result.WinningCells);
        Assert.Equal(Player.X, result.CurrentPlayer);
        Assert.Equal(1, scoreboardStore.Get().XWins);
    }

    [Fact]
    public void Undo_TwoPlayerMode_RemovesOnlyLastMove()
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.TwoPlayer);
        engine.ApplyMove(game.Id, Player.X, 0, 0);
        engine.ApplyMove(game.Id, Player.O, 1, 1);

        var result = engine.Undo(game.Id);

        Assert.Single(result.MoveHistory);
        Assert.Equal(Player.X, result.Board[0]);
        Assert.Null(result.Board[4]);
        Assert.Equal(Player.O, result.CurrentPlayer);
    }

    [Fact]
    public void Undo_ComputerMode_RemovesComputerAndPrecedingHumanMoveTogether()
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.VsComputer);
        engine.ApplyMove(game.Id, Player.X, 0, 0);

        var result = engine.Undo(game.Id);

        Assert.Empty(result.MoveHistory);
        Assert.All(result.Board, cell => Assert.Null(cell));
        Assert.Equal(Player.X, result.CurrentPlayer);
    }

    [Fact]
    public void Undo_NoMovesToUndo_Throws()
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.TwoPlayer);

        var ex = Assert.Throws<GameEngineException>(() => engine.Undo(game.Id));
        Assert.Equal(GameErrorCode.NoMovesToUndo, ex.ErrorCode);
    }

    [Fact]
    public void Undo_AfterCompletion_Throws()
    {
        var engine = CreateEngine(out _);
        var game = engine.CreateGame(GameMode.TwoPlayer);
        PlayRowWinForX(engine, game.Id);

        var ex = Assert.Throws<GameEngineException>(() => engine.Undo(game.Id));
        Assert.Equal(GameErrorCode.UndoNotAllowedAfterCompletion, ex.ErrorCode);
    }

    [Fact]
    public void Scoreboard_UpdatesExactlyOncePerCompletedGame()
    {
        var engine = CreateEngine(out var scoreboardStore);
        var game = engine.CreateGame(GameMode.TwoPlayer);

        PlayRowWinForX(engine, game.Id);

        var scoreboard = scoreboardStore.Get();
        Assert.Equal(1, scoreboard.XWins);
        Assert.Equal(0, scoreboard.OWins);
        Assert.Equal(0, scoreboard.Draws);
    }

    private static GameSession PlayRowWinForX(IGameEngine engine, Guid gameId)
    {
        engine.ApplyMove(gameId, Player.X, 0, 0);
        engine.ApplyMove(gameId, Player.O, 1, 0);
        engine.ApplyMove(gameId, Player.X, 0, 1);
        engine.ApplyMove(gameId, Player.O, 1, 1);
        return engine.ApplyMove(gameId, Player.X, 0, 2);
    }
}
