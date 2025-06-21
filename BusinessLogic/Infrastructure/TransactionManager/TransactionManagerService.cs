using Microsoft.EntityFrameworkCore;

namespace LibraryApi.BusinessLogic.Infrastructure.TransactionManager
{
    #region UnitOfWork

    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }
    }

    #endregion

    public class TransactionManagerService : ITransactionManagerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TransactionManagerService> _logger;

        public TransactionManagerService(ILogger<TransactionManagerService> _logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            this._logger = _logger;
        }

        public async Task<TResult> DoworkWithTransaction<TResult>(Func<Task<TResult>> operation)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                TResult result = await operation.Invoke();
                await _unitOfWork.CommitTransactionAsync();
                return result;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<TResult> DoworkWithNoTransaction<TResult>(Func<Task<TResult>> operation)
        {
            try
            {
                return await operation.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task DoworkWithTransaction(Func<Task> operation)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                await operation.Invoke();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task DoworkWithNoTransaction(Func<Task> operation)
        {
            try
            {
                await operation.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
