using LibraryApi.BusinessLogic.Implement.Book.Interface;
using LibraryApi.BusinessLogic.Infrastructure.TransactionManager;

namespace LibraryApi.BusinessLogic.Implement.Book.Facade
{
    public class BookFacade : IBookFacade
    {
        private readonly IBookService _bookService;

        private readonly ITransactionManager _transactionManager;
        public BookFacade(IBookService bookService, ITransactionManager transactionManager)
        {
            _bookService = bookService;
            _transactionManager = transactionManager;
        }

        public Task<LibraryApi.Domain.Entities.Book> AddAsync(LibraryApi.Domain.Entities.Book entity)
        {
            return _transactionManager.DoworkWithTransaction(() => _bookService.AddAsync(entity));
        }

        public Task<bool> DeleteAsync(int id)
        {
            return _transactionManager.DoworkWithTransaction(() => _bookService.DeleteAsync(id));
        }


        public Task<IEnumerable<LibraryApi.Domain.Entities.Book>> GetAllAsync()
        {
            return _transactionManager.DoworkWithTransaction(() => _bookService.GetAllAsync());
        }

        public Task<LibraryApi.Domain.Entities.Book?> GetByIdAsync(int id)
        {
            return _transactionManager.DoworkWithTransaction(() => _bookService.GetByIdAsync(id));
        }

        public Task<LibraryApi.Domain.Entities.Book> UpdateAsync(LibraryApi.Domain.Entities.Book entity)
        {
            return _transactionManager.DoworkWithTransaction(() => _bookService.UpdateAsync(entity));
        }
    }
}
