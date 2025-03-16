using LibraryApi.BusinessLogic.Service.TokenBlacklist;

public class JwtMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
    {
        var token = context.Request.Cookies["AuthToken"];
        if (!string.IsNullOrEmpty(token))
        {
            var userId = tokenService.ValidateAccessToken(token);

            if (userId != null)
            {
                context.Items["User"] = userId;  // Store user info in HttpContext
            }
        }

        await _next(context);
    }
}

