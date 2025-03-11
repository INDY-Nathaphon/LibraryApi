namespace LibraryApi.BusinessLogic.TransactionManager
{
    public interface ITransactionManager
    {
        TResult DoworkWithTransaction<TResult>(Func<TResult> operation);
        TResult DoworkWithNoTransaction<TResult>(Func<TResult> operation);
    }
}
