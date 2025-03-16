using Azure.Core;
using LibraryApi.BusinessLogic.Implement.Authentication.Interface;
using LibraryApi.BusinessLogic.Service.TokenBlacklist;
using LibraryApi.Controllers.DTO.AUthenticationDTO;
using LibraryApi.Controllers.DTO.BaseDTO;
using LibraryApi.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IAuthenticationFacade _authenticationFacade;
        private readonly ITokenService _tokenService;
        private readonly AppSettings _appSettings;

        public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationFacade authenticationFacade, ITokenService tokenBlacklistService, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _authenticationFacade = authenticationFacade;
            _tokenService = tokenBlacklistService;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var result = await _authenticationFacade.Register(model);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("Login")]
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
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = HttpContext.Items["User"]?.ToString();

            if (string.IsNullOrWhiteSpace(userId))
            {
                return BadRequest(new { message = "Token is missing." });
            }

            int expiryMinutes = _appSettings.RedisSetting.TokenExpiryMinutes;
                
            await Task.Run(() => _tokenService.RevokeRefreshToken(userId));

            return Ok(new { message = "Logged out successfully." });
        }

        [HttpPost("RefreshToken")]
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
