using LibraryApi.BusinessLogic.Implement.BaseService;
using LibraryApi.BusinessLogic.Implement.User.Interface;
using LibraryApi.Common.Constant;
using LibraryApi.Common.Exceptions;
using LibraryApi.Domain;
using Microsoft.EntityFrameworkCore;
using static LibraryApi.Common.Enum.Enums;

namespace LibraryApi.BusinessLogic.Implement.User.Service
{
    public class UserService : BaseService<Domain.Entities.User>, IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task LavelupLibrarian(long adminId, long userId)
        {
            #region Validate model

            if (adminId <= 0)
            {
                throw new AppException(AppErrorCode.ValidationError, "Admin id is required.");
            }

            if (userId <= 0)
            {
                throw new AppException(AppErrorCode.ValidationError, "User id is required.");
            }

            #endregion

            var isAdminExist = await _context.Users.AnyAsync(u => u.Id == adminId && u.Role == UserRoles.Admin);

            if (!isAdminExist)
            {
                throw new AppException(AppErrorCode.ValidationError, "Admin not found.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Role == UserRoles.User);

            if (user == null)
            {
                throw new AppException(AppErrorCode.ValidationError, "User not found.");
            }

            user.Role = UserRoles.Librarian;
            user.UpdatedDate = DateTime.UtcNow;
            user.UpdatedBy = adminId;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
