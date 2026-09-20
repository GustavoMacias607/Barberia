namespace Barberia.Application.Interfaces.Transactions;

public interface ITransactionManager
{
    Task<T> ExecuteSerializableAsync<T>(Func<Task<T>> action);

}
