using Azure.Core;
using LibraryApi.BusinessLogic.Implement.Authentication.Interface;
using LibraryApi.BusinessLogic.Service.TokenBlacklist;
using LibraryApi.Controllers.DTO.AUthenticationDTO;
using LibraryApi.Controllers.DTO.BaseDTO;
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
        private readonly ITokenService _tokenService;


        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationFacade authenticationFacade, IConfiguration config, ITokenService tokenBlacklistService)
        {
            _logger = logger;
            _authenticationFacade = authenticationFacade;
            _config = config;
            _tokenService = tokenBlacklistService;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _authenticationFacade.Login(model);

            if (result.Success)
            {
                Response.Cookies.Append("AuthToken", result.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(15)
                });
                return Ok(result.RefreshToken);
            }

            return Unauthorized();
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = HttpContext.Items["User"]?.ToString();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new { message = "Token is missing." });
            }

            int expiryMinutes = int.Parse(_config["Redis:TokenExpiryMinutes"] ?? "120");

            await Task.Run(() => _tokenService.RevokeRefreshToken(userId));

            return Ok(new { message = "Logged out successfully." });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
            var refreshToken = model.RefreshToken;
            var userId = _tokenService.GetUserIdFromRefreshToken(refreshToken);

            var result = await _authenticationFacade.RefreshTokenAsync( model.UserId, model.RefreshToken);

            if (result.Success)
            {
                Response.Cookies.Append("AuthToken", result.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(15)
                });

                return Ok(result.RefreshToken);
            }

            return Unauthorized();
        }

    }
}
