using LibraryApi.BusinessLogic.Implement.BaseService;
using LibraryApi.Common.Infos.User;

namespace LibraryApi.BusinessLogic.Implement.Library.Interface
{
    public interface ILibraryFacade : IBaseService<Domain.Entities.Library>
    {
        public Task JoinLibrary(long libraryId, long userId);

        public Task JoinLibraries(List<long> libraryIds, long userId);

        Task<List<UserInfo>> GetUsersByLibraryIdAsync(long libraryId);
    }
}
