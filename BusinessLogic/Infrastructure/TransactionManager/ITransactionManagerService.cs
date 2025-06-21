namespace LibraryApi.BusinessLogic.Infrastructure.TransactionManager
{
    public interface ITransactionManagerService
    {
        Task<TResult> DoworkWithTransaction<TResult>(Func<Task<TResult>> operation);
        Task<TResult> DoworkWithNoTransaction<TResult>(Func<Task<TResult>> operation);
        Task DoworkWithTransaction(Func<Task> operation);
        Task DoworkWithNoTransaction(Func<Task> operation);
    }
}
