using LibraryApi.BusinessLogic.Service.TokenBlacklist;
using LibraryApi.Domain;
using Microsoft.AspNetCore.Authorization;

public class JwtMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, ITokenService tokenService, IUserContext userContext)
    {
        var endpoint = context.GetEndpoint();
        var hasAuthorizeAttribute = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>() != null;

        if (hasAuthorizeAttribute) // ทำงานเฉพาะที่มี [Authorize]
        {
            var token = context.Request.Cookies["AuthToken"];

            if (!string.IsNullOrEmpty(token))
            {
                var userId = tokenService.ValidateAccessToken(token);

                if (userId != null)
                {
                    userContext.SetUserId(userId);
                }
            }
        }

        await _next(context);
    }
}

