using LibraryApi.Domain.CurrentUserProvider;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected readonly ILogger<BaseController> _logger;
    protected readonly ICurrentUserProvider _userContext;

    protected BaseController(ILogger<BaseController> logger, ICurrentUserProvider userContext)
    {
        _logger = logger;
        _userContext = userContext;
    }

    protected string? UserId => _userContext.UserId; // ใช้งานได้ใน Controller อื่น

    /// <summary>
    /// จัดการ Response Format ให้อยู่ในมาตรฐานเดียวกัน
    /// </summary>
    protected IActionResult ApiResponse(object? data = null, string message = "Success", int statusCode = 200)
    {
        return StatusCode(statusCode, new
        {
            success = statusCode is >= 200 and < 300,
            message,
            data
        });
    }

    /// <summary>
    /// จัดการ Exception ทั่วไป ลดการใช้ Try-Catch ในทุก Controller
    /// </summary>
    protected IActionResult HandleException(Exception ex, string customMessage = "An error occurred.")
    {
        _logger.LogError(ex, customMessage);
        return StatusCode(500, new { success = false, message = customMessage });
    }
}
