using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace AXDD.BuildingBlocks.Common.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        object apiResponse;
        int statusCode;

        switch (exception)
        {
            case NotFoundException notFoundEx:
                statusCode = (int)HttpStatusCode.NotFound;
                apiResponse = new
                {
                    success = false,
                    message = notFoundEx.Message,
                    statusCode
                };
                break;
            case BadRequestException badRequestEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                apiResponse = new
                {
                    success = false,
                    message = badRequestEx.Message,
                    errors = badRequestEx.Errors,
                    statusCode
                };
                break;
            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                apiResponse = new
                {
                    success = false,
                    message = "An internal server error occurred.",
                    statusCode
                };
                break;
        }

        response.StatusCode = statusCode;
        await response.WriteAsync(JsonSerializer.Serialize(apiResponse));
    }
}
