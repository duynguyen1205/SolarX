using Microsoft.EntityFrameworkCore;
using SolarX.REPOSITORY;

namespace SolarX.API.Behaviors;

public interface IGlobalTransactionsBehaviors
{
    public Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> task);
}

public class GlobalTransactionsBehaviors : IGlobalTransactionsBehaviors
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<GlobalTransactionsBehaviors> _logger;

    public GlobalTransactionsBehaviors(ApplicationDbContext dbContext, ILogger<GlobalTransactionsBehaviors> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>>task)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();
        
        TResult result = default;
        
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                result =  await task();
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while executing the transaction.");
                throw;
            }
        });
        
        return result;
    }
}