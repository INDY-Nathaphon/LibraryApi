using LibraryApi.Common.Constant;
using LibraryApi.Common.Exceptions;
using LibraryApi.Common.Helpers;
using LibraryApi.Common.Infos.Base;
using System.Net;
using System.Text.Json;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception caught in middleware");

            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int statusCode;
        AppErrorCode errorCode;
        string message;

        if (exception is AppException appEx)
        {
            errorCode = appEx.MessageId;
            message = string.IsNullOrEmpty(appEx.Message)
                        ? ErrorHelper.GetMessage(errorCode)
                        : appEx.Message;
            statusCode = MapStatusCode((int)errorCode);
        }
        else
        {
            errorCode = AppErrorCode.InternalError;
            message = ErrorHelper.GetMessage(errorCode);
            statusCode = (int)HttpStatusCode.InternalServerError;
        }

        var errorResponse = new ApiResponse<object>
        {
            MessageId = (int)errorCode,
            Message = message,
            Success = false,
            Data = null
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var result = JsonSerializer.Serialize(errorResponse);
        return context.Response.WriteAsync(result);
    }

    private static int MapStatusCode(int messageId)
    {
        return messageId switch
        {
            (int)AppErrorCode.NotFound => (int)HttpStatusCode.NotFound,
            (int)AppErrorCode.ValidationError => (int)HttpStatusCode.BadRequest,
            (int)AppErrorCode.Unauthorized => (int)HttpStatusCode.Unauthorized,
            (int)AppErrorCode.Conflict => (int)HttpStatusCode.Conflict,
            _ => (int)HttpStatusCode.InternalServerError,
        };
    }
}
