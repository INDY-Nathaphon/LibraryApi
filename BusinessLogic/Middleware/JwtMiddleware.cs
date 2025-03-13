using LibraryApi.BusinessLogic.Service.TokenBlacklist;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITokenBlacklistService _tokenBlacklistService;

    public JwtMiddleware(RequestDelegate next, ITokenBlacklistService tokenBlacklistService)
    {
        _next = next;
        _tokenBlacklistService = tokenBlacklistService;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token) && await _tokenBlacklistService.IsTokenRevokedAsync(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Token has been revoked.");
            return;
        }

        await _next(context);
    }
}
