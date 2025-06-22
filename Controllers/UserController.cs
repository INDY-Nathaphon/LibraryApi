using LibraryApi.BusinessLogic.Implement.User.Interface;
using LibraryApi.Domain.CurrentUserProvider;
using LibraryApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public class UserController : BaseController
    {
        private readonly IUserFacade userFacade;

        public UserController(ILogger<BaseController> logger, IUserFacade userFacade, ICurrentUserProvider userContext)
            : base(logger, userContext)
        {
            this.userFacade = userFacade;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(long id)
        {
            var result = await userFacade.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await userFacade.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            var result = await userFacade.AddAsync(user);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            var result = await userFacade.UpdateAsync(user);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            await userFacade.DeleteAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/lavelup-librarian")]
        public async Task<IActionResult> LavelupLibrarian(long id)
        {
            var adminId = _userContext.UserId;
            await userFacade.LavelupLibrarian(adminId, id);
            return Ok();
        }
    }
}
