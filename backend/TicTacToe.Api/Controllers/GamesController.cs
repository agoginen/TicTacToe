using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Contracts;
using TicTacToe.Api.Engine;
using TicTacToe.Api.Storage;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/games")]
public sealed class GamesController(IGameEngine gameEngine, IScoreboardStore scoreboardStore) : ControllerBase
{
    [HttpPost]
    public ActionResult<GameStateResponse> Create([FromBody] CreateGameRequest request)
    {
        var session = gameEngine.CreateGame(request.Mode);
        var response = session.ToResponse(scoreboardStore);
        return CreatedAtAction(nameof(GetById), new { id = session.Id }, response);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<GameStateResponse> GetById(Guid id)
    {
        var session = gameEngine.GetGame(id);
        return Ok(session.ToResponse(scoreboardStore));
    }

    [HttpPost("{id:guid}/moves")]
    public ActionResult<GameStateResponse> SubmitMove(Guid id, [FromBody] MoveRequest request)
    {
        var session = gameEngine.ApplyMove(id, request.Player, request.Row, request.Col);
        return Ok(session.ToResponse(scoreboardStore));
    }

    [HttpPost("{id:guid}/undo")]
    public ActionResult<GameStateResponse> Undo(Guid id)
    {
        var session = gameEngine.Undo(id);
        return Ok(session.ToResponse(scoreboardStore));
    }

    [HttpPost("{id:guid}/reset")]
    public ActionResult<GameStateResponse> Reset(Guid id)
    {
        var session = gameEngine.Reset(id);
        return Ok(session.ToResponse(scoreboardStore));
    }
}
