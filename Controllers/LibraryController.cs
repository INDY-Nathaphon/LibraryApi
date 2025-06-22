using LibraryApi.BusinessLogic.Implement.Library.Interface;
using LibraryApi.Common.Constant;
using LibraryApi.Common.Exceptions;
using LibraryApi.Common.Helpers.Attribute;
using LibraryApi.Domain.CurrentUserProvider;
using LibraryApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LibraryApi.Common.Enum.Enums;

[Authorize]
[ApiController]
[Route("api/libraries")]
public class LibraryController : BaseController
{
    private readonly ILibraryFacade libraryFacade;

    public LibraryController(ILogger<BaseController> logger, ILibraryFacade libraryFacade, ICurrentUserProvider userContext)
        : base(logger, userContext)
    {
        this.libraryFacade = libraryFacade;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLibraries()
    {
        var result = await libraryFacade.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLibrary(long id)
    {
        var result = await libraryFacade.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [AuthorizeLibrary(UserRoles.Admin)]
    public async Task<IActionResult> CreateLibrary([FromBody] Library library)
    {
        var result = await libraryFacade.AddAsync(library);
        return CreatedAtAction(nameof(GetLibrary), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [AuthorizeLibrary(UserRoles.Admin, UserRoles.Librarian)]
    public async Task<IActionResult> UpdateLibrary(long id, [FromBody] Library library)
    {
        if (id != library.Id)
        {
            throw new AppException(AppErrorCode.ValidationError, "Id mismatch.");
        }

        var result = await libraryFacade.UpdateAsync(library);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [AuthorizeLibrary(UserRoles.Admin)]
    public async Task<IActionResult> DeleteLibrary(long id)
    {
        await libraryFacade.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/join")]
    public async Task<IActionResult> JoinLibrary(long id)
    {
        var userId = _userContext.UserId;
        await libraryFacade.JoinLibrary(id, userId);
        return Ok();
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinLibraries([FromBody] List<long> ids)
    {
        var userId = _userContext.UserId;
        await libraryFacade.JoinLibraries(ids, userId);
        return Ok();
    }
}
