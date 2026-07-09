using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Sales.Domain.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Sales.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core repository for the <see cref="Sale" /> aggregate.
/// </summary>
public class SaleRepository(AppDbContext context) : BaseRepository<Sale>(context), ISaleRepository
{
    public async Task<IEnumerable<Sale>> FindAllByOwnerEmailAsync(string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<Sale>()
            .Where(sale => sale.OwnerEmail == ownerEmail)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sale>> FindAllByOwnerEmailAndDateAsync(string ownerEmail, DateOnly date,
        CancellationToken cancellationToken)
    {
        // Sales are bucketed by the business day in America/Lima (UTC-5, no daylight saving).
        var businessOffset = TimeSpan.FromHours(-5);
        var startOfDay = new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, businessOffset);
        var startOfNextDay = startOfDay.AddDays(1);
        return await Context.Set<Sale>()
            .Where(sale => sale.OwnerEmail == ownerEmail
                           && sale.CreatedAt >= startOfDay && sale.CreatedAt < startOfNextDay)
            .ToListAsync(cancellationToken);
    }

    public async Task<Sale?> FindByIdAndOwnerEmailAsync(int id, string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<Sale>()
            .FirstOrDefaultAsync(sale => sale.Id == id && sale.OwnerEmail == ownerEmail, cancellationToken);
    }
}
