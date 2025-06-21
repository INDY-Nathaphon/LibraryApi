using LibraryApi.BusinessLogic.Implement.User.Interface;
using LibraryApi.BusinessLogic.Infrastructure.TransactionManager;

namespace LibraryApi.BusinessLogic.Implement.User.Facade
{
    public class UserFacade : IUserFacade
    {
        private readonly IUserService _userService;
        private readonly ITransactionManagerService _transactionManager;


        public UserFacade(ITransactionManagerService transactionManage, IUserService userService)
        {
            _transactionManager = transactionManage ?? throw new ArgumentNullException(nameof(transactionManage));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<Domain.Entities.User> AddAsync(Domain.Entities.User entity)
        {
            return await _transactionManager.DoworkWithTransaction(() => _userService.AddAsync(entity));
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _transactionManager.DoworkWithTransaction(() => _userService.DeleteAsync(id));
        }

        public async Task<IEnumerable<Domain.Entities.User>> GetAllAsync()
        {
            return await _transactionManager.DoworkWithTransaction(() => _userService.GetAllAsync());
        }

        public async Task<Domain.Entities.User?> GetByIdAsync(int id)
        {
            return await _transactionManager.DoworkWithTransaction(() => _userService.GetByIdAsync(id));
        }

        public async Task<Domain.Entities.User> UpdateAsync(Domain.Entities.User entity)
        {
            return await _transactionManager.DoworkWithTransaction(() => _userService.UpdateAsync(entity));
        }
    }
}
