using System.Transactions;

namespace LibraryApi.BusinessLogic.TransactionManager
{
    public class TransactionManager : ITransactionManager
    {
        public TResult DoworkWithTransaction<TResult>(Func<TResult> operation)
        {
            if (operation == null) throw new ArgumentNullException(nameof(operation));

            using (var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                TResult result = operation.Invoke();
                scope.Complete();
                return result;
            }
        }

        public TResult DoworkWithNoTransaction<TResult>(Func<TResult> operation)
        {
            if (operation == null) throw new ArgumentNullException(nameof(operation));
            return operation.Invoke();
        }
    }
}
