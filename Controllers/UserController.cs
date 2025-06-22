using LibraryApi.BusinessLogic.Implement.User.Interface;
using LibraryApi.Common.Constant;
using LibraryApi.Common.Exceptions;
using LibraryApi.Common.Helpers.Attribute;
using LibraryApi.Domain.CurrentUserProvider;
using LibraryApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LibraryApi.Common.Enum.Enums;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public class UserController : BaseController
    {
        private readonly IUserFacade _userFacade;

        public UserController(ILogger<BaseController> logger, IUserFacade userFacade, ICurrentUserProvider userContext)
            : base(logger, userContext)
        {
            this._userFacade = userFacade;
        }

        [HttpGet("{id}")]
        [AuthorizeLibrary(UserRoles.Admin, UserRoles.Librarian)]
        public async Task<IActionResult> GetUser(long id)
        {
            var result = await _userFacade.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet]
        [AuthorizeLibrary(UserRoles.Admin, UserRoles.Librarian)]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userFacade.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        [AuthorizeLibrary(UserRoles.Admin, UserRoles.Librarian)]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            var result = await _userFacade.AddAsync(user);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [AuthorizeLibrary(UserRoles.Admin, UserRoles.Librarian)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                throw new AppException(AppErrorCode.ValidationError, "Id mismatch.");
            }

            var result = await _userFacade.UpdateAsync(user);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [AuthorizeLibrary(UserRoles.Admin, UserRoles.Librarian)]
        public async Task<IActionResult> DeleteUser(long id)
        {
            await _userFacade.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/lavelup-librarian")]
        [AuthorizeLibrary(UserRoles.Admin)]
        public async Task<IActionResult> LavelupLibrarian(long id)
        {
            var adminId = _userContext.UserId;
            await _userFacade.LavelupLibrarian(adminId, id);
            return Ok();
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = _userContext.UserId;
            var profile = await _userFacade.GetByIdAsync(userId);
            return Ok(profile);
        }
    }
}
