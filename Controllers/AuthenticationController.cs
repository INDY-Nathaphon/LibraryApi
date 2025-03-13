using LibraryApi.BusinessLogic.Implement.Authentication.Interface;
using LibraryApi.BusinessLogic.Service.TokenBlacklist;
using LibraryApi.Controllers.DTO.AUthenticationDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationFacade _authenticationFacade;
private readonly IConfiguration _config;
        private readonly ITokenBlacklistService _tokenBlacklistService;


        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationFacade authenticationFacade, IConfiguration config, ITokenBlacklistService tokenBlacklistService)
        {
            _logger = logger;
            _authenticationFacade = authenticationFacade;
            _config = config;
            _tokenBlacklistService = tokenBlacklistService;
        }

        [HttpGet]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var result = await _authenticationFacade.Register(model);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _authenticationFacade.Login(model);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token is missing." });
            }

            int expiryMinutes = int.Parse(_config["Redis:TokenExpiryMinutes"] ?? "120");

            await _tokenBlacklistService.RevokeTokenAsync(token, expiryMinutes);

            return Ok(new { message = "Logged out successfully." });
        }

        [HttpGet]
        [Route("refreshtoken")]
        public async Task<IActionResult> RefreshToken()
        {
            throw new NotImplementedException();
        }
    }
}
