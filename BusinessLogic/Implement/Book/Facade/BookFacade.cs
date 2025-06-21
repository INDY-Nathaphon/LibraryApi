using LibraryApi.BusinessLogic.Implement.Book.Interface;
using LibraryApi.BusinessLogic.Infrastructure.TransactionManager;

namespace LibraryApi.BusinessLogic.Implement.Book.Facade
{
    public class BookFacade : IBookFacade
    {
        private readonly IBookService _bookService;

        private readonly ITransactionManagerService _transactionManager;
        public BookFacade(IBookService bookService, ITransactionManagerService transactionManager)
        {
            _bookService = bookService;
            _transactionManager = transactionManager;
        }

        public async Task<LibraryApi.Domain.Entities.Book> AddAsync(LibraryApi.Domain.Entities.Book entity)
        {
            return await _transactionManager.DoworkWithTransaction(() => _bookService.AddAsync(entity));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _transactionManager.DoworkWithTransaction(() => _bookService.DeleteAsync(id));
        }


        public async Task<IEnumerable<LibraryApi.Domain.Entities.Book>> GetAllAsync()
        {
            return await _transactionManager.DoworkWithTransaction(() => _bookService.GetAllAsync());
        }

        public async Task<LibraryApi.Domain.Entities.Book?> GetByIdAsync(int id)
        {
            return await _transactionManager.DoworkWithTransaction(() => _bookService.GetByIdAsync(id));
        }

        public async Task<LibraryApi.Domain.Entities.Book> UpdateAsync(LibraryApi.Domain.Entities.Book entity)
        {
            return await _transactionManager.DoworkWithTransaction(() => _bookService.UpdateAsync(entity));
        }
    }
}
