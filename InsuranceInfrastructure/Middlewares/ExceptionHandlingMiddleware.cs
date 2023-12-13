using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging; 
using System;
using System.Threading.Tasks;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger; 

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger; 
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
      
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the exception using ILogger
            _logger.LogError(ex, "An unhandled exception occurred");

            // Handle the exception and send an appropriate response to the client
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync($"{ex.ToString()} Internal Server Error");

            // You can customize the error response further as needed
        }
    }
}
