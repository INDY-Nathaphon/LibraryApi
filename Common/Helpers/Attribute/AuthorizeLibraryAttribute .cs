using Microsoft.AspNetCore.Authorization;
using static LibraryApi.Common.Enum.Enums;

namespace LibraryApi.Common.Helpers.Attribute
{
    public class AuthorizeLibraryAttribute : AuthorizeAttribute
    {
        public AuthorizeLibraryAttribute(params UserRoles[] roles)
        {
            Policy = "LibraryAccessPolicy";
            Roles = string.Join(",", roles.Select(r => r.ToString()));
        }
    }
}
