namespace Academy.Service.Utility.Authorization;

using Microsoft.AspNetCore.Http;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IJwtUtils jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userData = jwtUtils.ValidateToken(token);
        if (userData != null)
        {
            // attach user to context on successful jwt validation
            //context.Items["User"] = userService.GetById(userId.Value);
            context.Items["User"] = userData;
        }

        await _next(context);
    }
}