using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SolarX.REPOSITORY.Abstractions;

namespace SolarX.REPOSITORY.Interceptions;

public class UpdateAuditableInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default) // 
    {
        var dbContext = eventData.Context;

        if (dbContext is null)
        {
            return base.SavingChangesAsync(
                eventData,
                result,
                cancellationToken);
        }

        var entries =
            dbContext
                .ChangeTracker
                .Entries<IAuditableEntity>();
        var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(a => a.CreatedAt).CurrentValue
                    = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, vietnamTimeZone);

                entityEntry.Property(a => a.UpdateAt).CurrentValue
                    = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, vietnamTimeZone);
            }

            if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(a => a.UpdateAt).CurrentValue
                    = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, vietnamTimeZone);
            }
        }

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken
        );
    }
}