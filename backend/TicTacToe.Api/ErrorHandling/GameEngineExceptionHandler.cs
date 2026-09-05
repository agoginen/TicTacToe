using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TicTacToe.Api.Engine;

namespace TicTacToe.Api.ErrorHandling;

// Central place mapping game-engine rejections to HTTP status codes, so controllers stay free of try/catch.
public sealed class GameEngineExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not GameEngineException gameException)
        {
            return false;
        }

        var statusCode = gameException.ErrorCode == GameErrorCode.GameNotFound
            ? StatusCodes.Status404NotFound
            : StatusCodes.Status400BadRequest;

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = gameException.ErrorCode.ToString(),
            Detail = gameException.Message
        }, cancellationToken);

        return true;
    }
}
