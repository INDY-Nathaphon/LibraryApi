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

        public Task<ResponseDto> Register(RegisterDto model)
        {
            return _transactionManager.DoworkWithTransaction(() => _authenticationService.Register(model));
        }

        public Task<ResponseDto> Login(LoginDto model)
        {
            return _transactionManager.DoworkWithTransaction(() => _authenticationService.Login(model));
        }
    }
}
