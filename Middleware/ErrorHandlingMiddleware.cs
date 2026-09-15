using System.Net;
using System.Text.Json;
using FinanceApi.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApi.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (status, title, detail) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Resource not found", exception.Message),
            ValidationException => (HttpStatusCode.BadRequest, "Validation error", exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred", "An internal error occurred while processing your request.")
        };

        var problemDetails = new ProblemDetails
        {
            Status = (int)status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status.Value;

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}
