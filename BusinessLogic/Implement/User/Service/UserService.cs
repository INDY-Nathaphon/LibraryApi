using LibraryApi.BusinessLogic.Implement.BaseService;
using LibraryApi.BusinessLogic.Implement.User.Interface;
using LibraryApi.Domain;

namespace LibraryApi.BusinessLogic.Implement.User.Service
{
    public class UserService : BaseService<Domain.Entities.User>, IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
