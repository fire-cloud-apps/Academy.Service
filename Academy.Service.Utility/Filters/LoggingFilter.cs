using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Academy.Service.Utility.Filters;

public class LoggingFilter : IAsyncActionFilter
{
    private readonly ILogger<LoggingFilter> _logger;

    public LoggingFilter(ILogger<LoggingFilter> logger)
    {
        _logger = logger;
    }
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _logger.LogInformation("{0} - Start", context.ActionDescriptor.RouteValues["action"]);
        var actionMethod = context.ActionDescriptor;
        var name = actionMethod.ActionConstraints.FirstOrDefault();
        await next();
        _logger.LogInformation("{0} - End", context.ActionDescriptor.RouteValues["action"]);
    }
}