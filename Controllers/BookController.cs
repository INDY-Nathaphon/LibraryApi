using LibraryApi.BusinessLogic.Implement.Book.Interface;
using LibraryApi.Domain.CurrentUserProvider;
using LibraryApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class BookController : BaseController
    {
        private readonly IBookFacade bookFacade;
        public BookController(ILogger<BaseController> logger, IBookFacade bookFacade, ICurrentUserProvider userContext)
            : base(logger, userContext)
        {
            this.bookFacade = bookFacade;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var user = await bookFacade.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            var createdUser = await bookFacade.AddAsync(book);
            return CreatedAtAction(nameof(GetBook), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBook([FromBody] Book book)
        {
            var updatedBook = await bookFacade.UpdateAsync(book);
            return Ok(updatedBook);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var success = await bookFacade.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
