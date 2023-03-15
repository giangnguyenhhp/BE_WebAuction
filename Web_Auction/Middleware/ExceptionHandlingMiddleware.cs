using System.Net;
using Newtonsoft.Json;

namespace Web_Auction.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, RequestDelegate requestDelegate)
    {
        _logger = logger;
        _requestDelegate = requestDelegate;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _requestDelegate(context);
        }
        catch (Exception e)
        {
            await HandleException(context, e);
        }
    }

    private Task HandleException(HttpContext context, Exception e)
    {
        _logger.LogError(e.ToString());
        var errorMessageObject = new { e.Message, Code = "system_error" };
        var errorMessage = JsonConvert.SerializeObject(errorMessageObject);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return context.Response.WriteAsync(errorMessage);
    }
}