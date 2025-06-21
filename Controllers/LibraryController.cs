using LibraryApi.BusinessLogic.Implement.Library.Interface;
using LibraryApi.Domain.CurrentUserProvider;
using LibraryApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LibraryController : BaseController
    {
        private readonly ILibraryFacade libraryFacade;

        public LibraryController(ILogger<BaseController> logger, ILibraryFacade libraryFacade, ICurrentUserProvider userContext)
            : base(logger, userContext)
        {
            this.libraryFacade = libraryFacade;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLibrary(int id)
        {
            var library = await libraryFacade.GetByIdAsync(id);
            if (library == null)
            {
                return NotFound();
            }
            return Ok(library);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateLibrary([FromBody] Library library)
        {
            var createdLibrary = await libraryFacade.AddAsync(library);
            return CreatedAtAction(nameof(createdLibrary), new { id = createdLibrary.Id }, createdLibrary);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateLibrary([FromBody] Library library)
        {
            var updatedUser = await libraryFacade.UpdateAsync(library);
            return Ok(updatedUser);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLibrary(int id)
        {
            var success = await libraryFacade.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
