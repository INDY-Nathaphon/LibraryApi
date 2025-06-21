using LibraryApi.BusinessLogic.Implement.Library.Interface;
using LibraryApi.BusinessLogic.Infrastructure.TransactionManager;

namespace LibraryApi.BusinessLogic.Implement.Library.Facade
{
    public class LibraryFacade : ILibraryFacade
    {
        private readonly ILibraryService _libraryService;
        private readonly ITransactionManagerService _transactionManager;
        public LibraryFacade(ILibraryService libraryService, ITransactionManagerService transactionManager)
        {
            _libraryService = libraryService;
            _transactionManager = transactionManager;
        }

        public async Task<Domain.Entities.Library> AddAsync(Domain.Entities.Library entity)
        {
            return await _transactionManager.DoworkWithTransaction(() => _libraryService.AddAsync(entity));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _transactionManager.DoworkWithTransaction(() => _libraryService.DeleteAsync(id));
        }

        public async Task<IEnumerable<Domain.Entities.Library>> GetAllAsync()
        {
            return await _transactionManager.DoworkWithTransaction(() => _libraryService.GetAllAsync());
        }

        public async Task<Domain.Entities.Library?> GetByIdAsync(int id)
        {
            return await _transactionManager.DoworkWithTransaction(() => _libraryService.GetByIdAsync(id));
        }

        public async Task<Domain.Entities.Library> UpdateAsync(Domain.Entities.Library entity)
        {
            return await _transactionManager.DoworkWithTransaction(() => _libraryService.UpdateAsync(entity));
        }
    }
}
