using LibraryApi.Common.Constant;
using LibraryApi.Common.Exceptions;
using LibraryApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.BusinessLogic.Implement.BaseService
{
    public class BaseService<T> : IBaseService<T> where T : class
    {
        private readonly AppDbContext _context;

        public BaseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            if (id <= 0)
            {
                throw new AppException(AppErrorCode.ValidationError, "Id must be greater than 0.");
            }

            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            if (entity == null)
            {
                throw new AppException(AppErrorCode.ValidationError, "Entity cannot be null.");
            }

            _context.Set<T>().Add(entity);

            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new AppException(AppErrorCode.ValidationError, "Entity cannot be null.");
            }
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            if (id <= 0)
            {
                throw new AppException(AppErrorCode.ValidationError, "Id must be greater than 0.");
            }
            var entity = await GetByIdAsync(id);

            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }

}
