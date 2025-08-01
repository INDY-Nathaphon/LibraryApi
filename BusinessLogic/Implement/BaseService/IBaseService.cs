﻿namespace LibraryApi.BusinessLogic.Implement.BaseService
{
    public interface IBaseService<T> where T : class
    {
        Task<T?> GetByIdAsync(long id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(long id);
    }

}
