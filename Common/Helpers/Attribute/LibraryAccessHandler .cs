using LibraryApi.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryApi.Common.Helpers.Attribute
{

    public class LibraryAccessRequirement : IAuthorizationRequirement { }

    public class LibraryAccessHandler : AuthorizationHandler<LibraryAccessRequirement, long>
    {
        private readonly AppDbContext _context;

        public LibraryAccessHandler(AppDbContext context)
        {
            _context = context;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            LibraryAccessRequirement requirement,
            long libraryId)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                context.Fail();
                return;
            }

            var userId = long.Parse(userIdClaim.Value);

            var isMember = await _context.UserLibraries
                .AnyAsync(ul => ul.UserId == userId && ul.LibraryId == libraryId);

            if (isMember)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
