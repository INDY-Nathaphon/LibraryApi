using LibraryApi.BusinessLogic.Implement.Book.Interface;
using LibraryApi.Domain.CurrentUserProvider;
using LibraryApi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/books")]
    public class BookController : BaseController
    {
        private readonly IBookFacade bookFacade;
        public BookController(ILogger<BaseController> logger, IBookFacade bookFacade, ICurrentUserProvider userContext)
            : base(logger, userContext)
        {
            this.bookFacade = bookFacade;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(long id)
        {
            var result = await bookFacade.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] Book book)
        {
            var result = await bookFacade.AddAsync(book);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBook([FromBody] Book book)
        {
            var result = await bookFacade.UpdateAsync(book);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(long id)
        {
            await bookFacade.DeleteAsync(id);
            return NoContent();
        }
    }
}
