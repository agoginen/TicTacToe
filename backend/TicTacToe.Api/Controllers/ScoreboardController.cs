using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Contracts;
using TicTacToe.Api.Storage;

namespace TicTacToe.Api.Controllers;

[ApiController]
[Route("api/scoreboard")]
public sealed class ScoreboardController(IScoreboardStore scoreboardStore) : ControllerBase
{
    [HttpGet]
    public ActionResult<ScoreboardResponse> Get()
    {
        return Ok(scoreboardStore.Get().ToResponse());
    }

    [HttpPost("reset")]
    public ActionResult<ScoreboardResponse> Reset()
    {
        scoreboardStore.Reset();
        return Ok(scoreboardStore.Get().ToResponse());
    }
}
