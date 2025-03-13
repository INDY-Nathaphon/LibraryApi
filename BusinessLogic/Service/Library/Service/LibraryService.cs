using LibraryApi.BusinessLogic.Implement.BaseService;
using LibraryApi.BusinessLogic.Implement.Library.Interface;
using LibraryApi.Domain;

namespace LibraryApi.BusinessLogic.Implement.Library.Service
{
    public class LibraryService : BaseService<Domain.Entities.Library>,ILibraryService
    {
        private readonly AppDbContext _context;
        public LibraryService(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
