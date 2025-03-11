using LibraryApi.BusinessLogic.Implement.BaseService;
using LibraryApi.BusinessLogic.Implement.Book.Interface;
using LibraryApi.Domain;

namespace LibraryApi.BusinessLogic.Implement.Book.Service
{
    public class BookService : BaseService<LibraryApi.Domain.Entities.Book>, IBookService
    {
        private readonly AppDbContext _context;
        public BookService(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
