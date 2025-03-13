using LibraryApi.BusinessLogic.Implement.Library.Interface;
using LibraryApi.BusinessLogic.TransactionManager;

namespace LibraryApi.BusinessLogic.Implement.Library.Facade
{
    public class LibraryFacade : ILibraryFacade
    {
        private readonly ILibraryService _libraryService;
        private readonly ITransactionManager _transactionManager;
        public LibraryFacade(ILibraryService libraryService, ITransactionManager transactionManager)
        {
            _libraryService = libraryService;
            _transactionManager = transactionManager;
        }

        public Task<Domain.Entities.Library> AddAsync(Domain.Entities.Library entity)
        {
            return _transactionManager.DoworkWithTransaction(() => _libraryService.AddAsync(entity));
        }

        public Task<bool> DeleteAsync(int id)
        {
            return _transactionManager.DoworkWithTransaction(() => _libraryService.DeleteAsync(id));
        }

        public Task<IEnumerable<Domain.Entities.Library>> GetAllAsync()
        {
            return _transactionManager.DoworkWithTransaction(() => _libraryService.GetAllAsync());
        }

        public Task<Domain.Entities.Library?> GetByIdAsync(int id)
        {
            return _transactionManager.DoworkWithTransaction(() => _libraryService.GetByIdAsync(id));
        }

        public Task<Domain.Entities.Library> UpdateAsync(Domain.Entities.Library entity)
        {
            return _transactionManager.DoworkWithTransaction(() => _libraryService.UpdateAsync(entity));
        }
    }
}
