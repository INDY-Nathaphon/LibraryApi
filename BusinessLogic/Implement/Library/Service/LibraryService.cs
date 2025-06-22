using LibraryApi.BusinessLogic.Implement.BaseService;
using LibraryApi.BusinessLogic.Implement.Library.Interface;
using LibraryApi.Common.Constant;
using LibraryApi.Common.Exceptions;
using LibraryApi.Common.Infos.User;
using LibraryApi.Domain;
using LibraryApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.BusinessLogic.Implement.Library.Service
{
    public class LibraryService : BaseService<Domain.Entities.Library>, ILibraryService
    {
        private readonly AppDbContext _context;
        public LibraryService(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task JoinLibrary(long libraryId, long userId)
        {
            #region Validate model

            if (libraryId <= 0)
            {
                throw new AppException(AppErrorCode.ValidationError, "Library id is required.");
            }


            if (userId <= 0)
            {
                throw new AppException(AppErrorCode.ValidationError, "User id is required.");
            }

            #endregion

            #region Check library exist

            var isUserExist = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!isUserExist)
            {
                throw new AppException(AppErrorCode.NotFound, "User not found.");
            }

            var isLibraryExist = await _context.Libraries.AnyAsync(l => l.Id == libraryId);

            if (!isLibraryExist)
            {
                throw new AppException(AppErrorCode.NotFound, "Library not found.");
            }

            var isUserLibraryExist = await _context.UserLibraries.AnyAsync(ul => ul.LibraryId == libraryId && ul.UserId == userId);

            if (isUserLibraryExist)
            {
                throw new AppException(AppErrorCode.ValidationError, "User already joined this library.");
            }

            #endregion

            var userLibrary = new UserLibrary
            {
                LibraryId = libraryId,
                UserId = userId,
                CreatedBy = userId
            };

            await _context.UserLibraries.AddAsync(userLibrary);
            await _context.SaveChangesAsync();
        }

        public async Task JoinLibraries(List<long> libraryIds, long userId)
        {
            #region Validate model

            if (libraryIds == null || libraryIds.Count == 0)
            {
                throw new AppException(AppErrorCode.ValidationError, "Library ids are required.");
            }

            if (userId <= 0)
            {
                throw new AppException(AppErrorCode.ValidationError, "User id is required.");
            }

            #endregion

            #region Check user exist

            var isUserExist = await _context.Users.AnyAsync(u => u.Id == userId);

            if (!isUserExist)
            {
                throw new AppException(AppErrorCode.NotFound, "User not found.");
            }

            #endregion

            #region Check all libraries exist

            var existingLibraryIds = await _context.Libraries
                .Where(l => libraryIds.Contains(l.Id))
                .Select(l => l.Id)
                .ToListAsync();

            var missingLibraryIds = libraryIds.Except(existingLibraryIds).ToList();

            if (missingLibraryIds.Any())
            {
                throw new AppException(AppErrorCode.NotFound, $"Libraries not found: {string.Join(", ", missingLibraryIds)}");
            }

            #endregion

            #region Check user already joined

            var existingUserLibraryIds = await _context.UserLibraries
                .Where(ul => ul.UserId == userId && libraryIds.Contains(ul.LibraryId))
                .Select(ul => ul.LibraryId)
                .ToListAsync();

            var newLibraryIds = libraryIds.Except(existingUserLibraryIds).ToList();

            if (!newLibraryIds.Any())
            {
                throw new AppException(AppErrorCode.ValidationError, "User already joined all these libraries.");
            }

            #endregion

            #region Add new user libraries

            var userLibrariesToAdd = newLibraryIds.Select(libId => new UserLibrary
            {
                UserId = userId,
                LibraryId = libId,
                CreatedBy = userId
            });

            await _context.UserLibraries.AddRangeAsync(userLibrariesToAdd);
            await _context.SaveChangesAsync();

            #endregion
        }

        public async Task<List<UserInfo>> GetUsersByLibraryIdAsync(long libraryId)
        {
            #region Validate model

            if (libraryId <= 0)
            {
                throw new AppException(AppErrorCode.ValidationError, "Library id is required.");
            }

            #endregion

            var userInfos = await (from u in _context.Users
                                   join ul in _context.UserLibraries on u.Id equals ul.UserId
                                   where ul.LibraryId == libraryId
                                   select new UserInfo
                                   {
                                       Id = u.Id,
                                       Name = u.Name,
                                       Email = u.Email
                                   }).ToListAsync();

            return userInfos;
        }
    }
}
