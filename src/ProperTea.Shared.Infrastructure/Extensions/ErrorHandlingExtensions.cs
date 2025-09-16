using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ProperTea.Infrastructure.Shared.ErrorHandling;

namespace ProperTea.Infrastructure.Shared.Extensions;

public static class ErrorHandlingExtensions
{
    public static IServiceCollection AddGlobalErrorHandling(this IServiceCollection services,
        string? serviceName = null)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                var correlationId = context.HttpContext.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                                    ?? Guid.NewGuid().ToString();

                context.ProblemDetails.Extensions["correlationId"] = correlationId;
                context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
                context.ProblemDetails.Instance = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(serviceName))
                    context.ProblemDetails.Extensions["service"] = serviceName;

                if (!string.IsNullOrEmpty(context.ProblemDetails.Status?.ToString()))
                    context.ProblemDetails.Type = $"https://httpstatuses.io/{context.ProblemDetails.Status}";
            };
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }

    public static WebApplication UseGlobalErrorHandling(this WebApplication app, string? serviceName = null)
    {
        app.UseExceptionHandler();

        app.UseStatusCodePages(async context =>
        {
            var problemDetails = StatusCodeHelpers.CreateStatusCodeProblemDetails(
                context.HttpContext,
                context.HttpContext.Response.StatusCode,
                serviceName);

            await context.HttpContext.Response.WriteAsJsonAsync(problemDetails);
        });

        return app;
    }
}