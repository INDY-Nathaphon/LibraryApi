using LibraryApi.BusinessLogic.Implement.User.Interface;
using LibraryApi.BusinessLogic.Infrastructure.TransactionManager;

namespace LibraryApi.BusinessLogic.Implement.User.Facade
{
    public class UserFacade : IUserFacade
    {
        private readonly IUserService _userService;
        private readonly ITransactionManager _transactionManager;


        public UserFacade(ITransactionManager transactionManage, IUserService userService)
        {
            _transactionManager = transactionManage ?? throw new ArgumentNullException(nameof(transactionManage));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public Task<Domain.Entities.User> AddAsync(Domain.Entities.User entity)
        {
            return _transactionManager.DoworkWithTransaction(() => _userService.AddAsync(entity));
        }

        public Task<bool> DeleteAsync(int id)
        {
            return _transactionManager.DoworkWithTransaction(() => _userService.DeleteAsync(id));
        }

        public Task<IEnumerable<Domain.Entities.User>> GetAllAsync()
        {
            return _transactionManager.DoworkWithTransaction(() => _userService.GetAllAsync());
        }

        public Task<Domain.Entities.User?> GetByIdAsync(int id)
        {
            return _transactionManager.DoworkWithTransaction(() => _userService.GetByIdAsync(id));
        }

        public Task<Domain.Entities.User> UpdateAsync(Domain.Entities.User entity)
        {
            return _transactionManager.DoworkWithTransaction(() => _userService.UpdateAsync(entity));
        }
    }
}
