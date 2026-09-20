using Barberia.Application.Interfaces.Transactions;
using Barberia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Data;


namespace Barberia.Infrastructure.Transactions;

public class EfTransactionManager : ITransactionManager
{
    private readonly BarberiaDbContext _dbContext;

    public EfTransactionManager(BarberiaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T> ExecuteSerializableAsync<T>(Func<Task<T>> action)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        try
        {
            var result = await action();
            await transaction.CommitAsync();
            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
