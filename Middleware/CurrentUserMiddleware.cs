using LibraryApi.Domain.CurrentUserProvider;
using System.Security.Claims;

namespace LibraryApi.Middleware
{
    public class CurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICurrentUserProvider currentUser)
        {
            var user = context.User;

            if (user.Identity?.IsAuthenticated ?? false)
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = user.FindFirst(ClaimTypes.Email)?.Value;
                var role = user.FindFirst(ClaimTypes.Role)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    currentUser.SetUser(userId, email, role);
                }
            }

            await _next(context);
        }
    }

}
