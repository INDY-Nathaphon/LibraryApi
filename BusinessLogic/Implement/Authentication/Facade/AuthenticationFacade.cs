using LibraryApi.BusinessLogic.Implement.Authentication.Interface;
using LibraryApi.BusinessLogic.TransactionManager;
using LibraryApi.Controllers.DTO.AUthenticationDTO;
using LibraryApi.Controllers.DTO.BaseDTO;

namespace LibraryApi.BusinessLogic.Implement.Authentication.Facade
{
    public class AuthenticationFacade : IAuthenticationFacade
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly ITransactionManager _transactionManager;
        public AuthenticationFacade(ITransactionManager transactionManager, IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _transactionManager = transactionManager;
        }

        #region Not Implemented

        public Task<ResponseDto> RegisterUserAsync(RegisterDto model)
        {
            return _transactionManager.DoworkWithTransaction(() => _authenticationService.RegisterUserAsync(model));
        }

        public Task<LibraryApi.Domain.Entities.BaseEntity> AddAsync(LibraryApi.Domain.Entities.Book entity)
        {
            throw new NotImplementedException(  );
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<LibraryApi.Domain.Entities.BaseEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<LibraryApi.Domain.Entities.BaseEntity?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<LibraryApi.Domain.Entities.BaseEntity> UpdateAsync(LibraryApi.Domain.Entities.BaseEntity entity)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
