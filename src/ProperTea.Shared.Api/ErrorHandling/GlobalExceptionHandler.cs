using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProperTea.Shared.Domain.Exceptions;

namespace ProperTea.Shared.Api.ErrorHandling;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var correlationId = httpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault() ??
                            Guid.NewGuid().ToString();

        logger.LogError(exception,
            "An unhandled exception occurred. CorrelationId: {CorrelationId}, RequestPath: {RequestPath}, Method: {Method}",
            correlationId, httpContext.Request.Path, httpContext.Request.Method);

        var problemDetails = CreateProblemDetails(httpContext, exception, correlationId);

        httpContext.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
        httpContext.Response.Headers.Append("X-Correlation-ID", correlationId);

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails CreateProblemDetails(HttpContext httpContext, Exception exception,
        string correlationId)
    {
        var (statusCode, title, detail) = exception switch
        {
            DomainException deX => ((int)HttpStatusCode.BadRequest, "Domain error", deX.Message),
            UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, "Unauthorized",
                "Authentication is required to access this resource."),
            ArgumentException argEx => ((int)HttpStatusCode.BadRequest, "Bad Request", argEx.Message),
            InvalidOperationException => ((int)HttpStatusCode.BadRequest, "Bad Request",
                "The requested operation is not valid."),
            TimeoutException => ((int)HttpStatusCode.RequestTimeout, "Request Timeout", "The request has timed out."),
            HttpRequestException httpEx => ((int)HttpStatusCode.BadGateway, "Bad Gateway",
                "An error occurred while processing the upstream request."),
            _ => ((int)HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred.")
        };

        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
            Type = $"https://httpstatuses.io/{statusCode}",
            Extensions =
            {
                ["correlationId"] = correlationId,
                ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            }
        };
    }
}