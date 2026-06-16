using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core repository for the <see cref="UnitLot" /> aggregate.
/// </summary>
public class UnitLotRepository(AppDbContext context)
    : BaseRepository<UnitLot>(context), IUnitLotRepository
{
    public async Task<IEnumerable<UnitLot>> FindAllByOwnerEmailAsync(string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<UnitLot>().Where(lot => lot.OwnerEmail == ownerEmail).ToListAsync(cancellationToken);
    }

    public async Task<UnitLot?> FindByIdAndOwnerEmailAsync(int id, string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<UnitLot>()
            .FirstOrDefaultAsync(lot => lot.Id == id && lot.OwnerEmail == ownerEmail, cancellationToken);
    }

    public async Task<bool> ExistsByIdAndOwnerEmailAsync(int id, string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<UnitLot>()
            .AnyAsync(lot => lot.Id == id && lot.OwnerEmail == ownerEmail, cancellationToken);
    }

    public async Task RemoveByProductIdAndOwnerEmailAsync(int productId, string ownerEmail,
        CancellationToken cancellationToken)
    {
        var lots = await Context.Set<UnitLot>()
            .Where(lot => lot.ProductId == productId && lot.OwnerEmail == ownerEmail).ToListAsync(cancellationToken);
        Context.Set<UnitLot>().RemoveRange(lots);
    }
}
