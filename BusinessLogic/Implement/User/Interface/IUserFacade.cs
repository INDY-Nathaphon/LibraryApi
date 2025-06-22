using LibraryApi.BusinessLogic.Implement.BaseService;

namespace LibraryApi.BusinessLogic.Implement.User.Interface
{
    public interface IUserFacade : IBaseService<Domain.Entities.User>
    {
        Task LavelupLibrarian(long adminId, long userId);
    }
}
