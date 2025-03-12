using LibraryApi.BusinessLogic.Implement.Authentication.Interface;
using LibraryApi.Controllers.DTO.AUthenticationDTO;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationFacade _authenticationFacade;

        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationFacade authenticationFacade)
        {
            _logger = logger;
            _authenticationFacade = authenticationFacade;
        }

        [HttpGet]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var result = await _authenticationFacade.RegisterUserAsync(model);

            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("refreshtoken")]
        public async Task<IActionResult> RefreshToken()
        {
            throw new NotImplementedException();
        }
    }
}
