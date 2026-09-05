using TicTacToe.Api.Engine;
using TicTacToe.Api.Models;
using Xunit;

namespace TicTacToe.Api.Tests;

public class ComputerPlayerTests
{
    private static Player?[] EmptyBoard() => new Player?[9];

    [Fact]
    public void SelectMove_WinAvailable_TakesWinningMove()
    {
        // O at 0,1; cell 2 completes the top row for O.
        var board = EmptyBoard();
        board[0] = Player.O;
        board[1] = Player.O;
        board[3] = Player.X;
        board[4] = Player.X;

        var move = ComputerPlayer.SelectMove(board);

        Assert.Equal(2, move);
    }

    [Fact]
    public void SelectMove_NoWinButXCanWin_BlocksX()
    {
        // X at 0,1 threatens to win at 2; O has no winning move of its own.
        var board = EmptyBoard();
        board[0] = Player.X;
        board[1] = Player.X;
        board[3] = Player.O;
        board[6] = Player.O;

        var move = ComputerPlayer.SelectMove(board);

        Assert.Equal(2, move);
    }

    [Fact]
    public void SelectMove_NoWinOrBlock_TakesCenter()
    {
        var board = EmptyBoard();
        board[0] = Player.X;

        var move = ComputerPlayer.SelectMove(board);

        Assert.Equal(4, move);
    }

    [Fact]
    public void SelectMove_CenterTaken_TakesCorner()
    {
        var board = EmptyBoard();
        board[4] = Player.X;

        var move = ComputerPlayer.SelectMove(board);

        Assert.Contains(move, new[] { 0, 2, 6, 8 });
    }

    [Fact]
    public void SelectMove_CenterAndCornersTaken_TakesAnyRemainingCell()
    {
        var board = EmptyBoard();
        board[4] = Player.X;
        board[0] = Player.O;
        board[2] = Player.X;
        board[6] = Player.O;
        board[8] = Player.X;

        var move = ComputerPlayer.SelectMove(board);

        Assert.Contains(move, new[] { 1, 3, 5, 7 });
    }

    [Fact]
    public void TryWin_ReturnsNull_WhenNoWinningMoveExists()
    {
        var board = EmptyBoard();
        board[0] = Player.O;

        Assert.Null(ComputerPlayer.TryWin(board, Player.O));
    }
}
