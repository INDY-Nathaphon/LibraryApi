using LibraryApi.BusinessLogic.Implement.Authentication.Interface;
using LibraryApi.Common.Constant;
using LibraryApi.Common.Exceptions;
using LibraryApi.Common.Infos.Authentication;
using LibraryApi.Domain;
using LibraryApi.Domain.CurrentUserProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using static LibraryApi.Common.Enum.Enums;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : BaseController
    {
        private readonly IAuthenticationFacade _authenticationFacade;
        private readonly AppSettings _appSettings;

        public AuthController(
            ILogger<BaseController> logger,
            ICurrentUserProvider userContext,
            IAuthenticationFacade authenticationFacade,
            IOptions<AppSettings> appSettings) : base(logger, userContext)
        {
            _authenticationFacade = authenticationFacade;
            _appSettings = appSettings.Value;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterInfo model)
        {
            await _authenticationFacade.Register(model);
            // หากต้องการคืน 201 Created พร้อม Location header ก็ทำได้ แต่ที่นี่คืน Ok() ก็เพียงพอ
            return Ok(new { message = "Registration successful" });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginInfo model)
        {
            var result = await _authenticationFacade.Login(model);

            if (result == null)
            {
                throw new AppException(AppErrorCode.Unauthorized, "Invalid credentials.");
            }

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(new { AccessToken = result.AccessToken });
        }

        // GET: api/auth/login/google
        [HttpGet("login/google")]
        public IActionResult LoginGoogle()
        {
            var redirectUrl = Url.Action(nameof(GoogleResponse));
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // GET: api/auth/google-response
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authResult.Succeeded)
            {
                throw new AppException(AppErrorCode.Unauthorized, "Google authentication failed.");
            }

            var claims = authResult.Principal.Claims.ToList();

            var response = await _authenticationFacade.GoogleResponse(claims);

            SetRefreshTokenCookie(response.RefreshToken);

            return Ok(new { AccessToken = response.AccessToken });
        }

        // POST: api/auth/logout
        [Authorize(Roles = nameof(UserRoles.Admin))] // หรือเปลี่ยนตาม policy ที่ต้องการ
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _authenticationFacade.RevokeRefreshToken(refreshToken);
            }

            RevokeRefreshTokenCookie();

            return Ok(new { message = "Logged out successfully" });
        }

        // POST: api/auth/refresh-token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenInfo model)
        {
            var result = await _authenticationFacade.RefreshTokenAsync(model.RefreshToken);

            if (result == null)
            {
                throw new AppException(AppErrorCode.Unauthorized, "Invalid refresh token.");
            }

            SetRefreshTokenCookie(result.RefreshToken);

            return Ok(new { AccessToken = result.AccessToken });
        }

        #region Private Methods for Refresh Token Cookie

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,              // ป้องกันไม่ให้ JavaScript อ่าน cookie ได้ (XSS protection)
                Expires = DateTime.UtcNow.AddDays(1), // กำหนดวันหมดอายุของ cookie
                IsEssential = true,           // สำหรับ GDPR compliance, cookie นี้จำเป็นกับการทำงานของระบบ
                SameSite = SameSiteMode.Strict, // ป้องกัน CSRF
                Secure = true                 // ต้องใช้ HTTPS ใน production
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private void RevokeRefreshTokenCookie()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(-1), // กำหนดวันหมดอายุ cookie ให้ลบ
                IsEssential = true,
                SameSite = SameSiteMode.Strict,
                Secure = true
            };

            Response.Cookies.Append("refreshToken", "", cookieOptions);
        }

        #endregion
    }
}
