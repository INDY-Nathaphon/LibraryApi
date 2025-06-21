using LibraryApi.BusinessLogic.Implement.Authentication.Interface;
using LibraryApi.BusinessLogic.Infrastructure.TransactionManager;
using LibraryApi.Common.Infos.Authentication;

namespace LibraryApi.BusinessLogic.Implement.Authentication.Facade
{
    public class AuthenticationFacade : IAuthenticationFacade
    {
        private readonly IAuthenticationService _authenticationService;

        private readonly ITransactionManagerService _transactionManager;

        public AuthenticationFacade(ITransactionManagerService transactionManager, IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            _transactionManager = transactionManager;
        }

        public async Task Register(RegisterInfo model)
        {
            await _transactionManager.DoworkWithTransaction(() => _authenticationService.Register(model));
        }

        public async Task<LoginRespInfo> Login(LoginInfo model)
        {
            return await _transactionManager.DoworkWithTransaction(() => _authenticationService.Login(model));
        }

        public async Task<RefrachTokenRespInfo> RefreshTokenAsync(string userId, string refreshToken)
        {
            return await _transactionManager.DoworkWithTransaction(() => _authenticationService.RefreshTokenAsync(userId, refreshToken));
        }
    }
}
