using LibraryApi.BusinessLogic.Implement.Authentication.Interface;
using LibraryApi.BusinessLogic.Infrastructure.TransactionManager;
using LibraryApi.Common.Infos.Authentication;
using System.Security.Claims;

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

        public async Task<AuthResult> Login(LoginInfo model)
        {
            return await _transactionManager.DoworkWithTransaction(() => _authenticationService.Login(model));
        }

        public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
        {
            return await _transactionManager.DoworkWithTransaction(() => _authenticationService.RefreshTokenAsync(refreshToken));
        }

        public async Task<bool> RevokeRefreshToken(string refreshTokenJwt)
        {
            return await _transactionManager.DoworkWithTransaction(() => _authenticationService.RevokeRefreshToken(refreshTokenJwt));
        }

        public async Task<AuthResult> GoogleResponse(List<Claim> claims)
        {
            return await _transactionManager.DoworkWithTransaction(() => _authenticationService.GoogleResponse(claims));
        }
    }
}
