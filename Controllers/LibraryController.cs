using LibraryApi.BusinessLogic.Implement.Library.Interface;
using LibraryApi.BusinessLogic.Implement.User.Interface;
using LibraryApi.Domain;
using LibraryApi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LibraryController : BaseController
    {
        private readonly ILibraryFacade libraryFacade;

        public LibraryController(ILogger<BaseController> logger, ILibraryFacade libraryFacade, IUserContext userContext)
            :base(logger,userContext)
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

        [HttpPost]
        public async Task<IActionResult> CreateLibrary([FromBody] Library library)
        {
            var createdLibrary = await libraryFacade.AddAsync(library);
            return CreatedAtAction(nameof(createdLibrary), new { id = createdLibrary.Id }, createdLibrary);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateLibrary([FromBody] Library library)
        {
            var updatedUser = await libraryFacade.UpdateAsync(library);
            return Ok(updatedUser);
        }

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
