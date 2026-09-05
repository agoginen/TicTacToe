using System.ComponentModel.DataAnnotations;
using TicTacToe.Api.Models;

namespace TicTacToe.Api.Contracts;

public sealed record MoveRequest(Player Player, [Range(0, 2)] int Row, [Range(0, 2)] int Col);
