using LibraryApi.BusinessLogic.Implement.Book.Interface;
using LibraryApi.BusinessLogic.Implement.User.Facade;
using LibraryApi.Domain;
using LibraryApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController :  BaseController
    {
        private readonly IBookFacade bookFacade;
        public BookController(ILogger<BaseController> logger, IBookFacade bookFacade, IUserContext userContext)
            :base(logger,userContext)
        {
            this.bookFacade = bookFacade;
        }

        [Authorize]
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            var createdUser = await bookFacade.AddAsync(book);
            return CreatedAtAction(nameof(GetBook), new { id = createdUser.Id }, createdUser);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateBook([FromBody] Book book)
        {
            var updatedBook = await bookFacade.UpdateAsync(book);
            return Ok(updatedBook);
        }

        [Authorize]
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
