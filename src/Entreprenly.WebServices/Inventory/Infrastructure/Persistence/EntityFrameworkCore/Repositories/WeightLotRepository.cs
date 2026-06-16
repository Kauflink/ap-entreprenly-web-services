using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core repository for the <see cref="WeightLot" /> aggregate.
/// </summary>
public class WeightLotRepository(AppDbContext context)
    : BaseRepository<WeightLot>(context), IWeightLotRepository
{
    public async Task<IEnumerable<WeightLot>> FindAllByOwnerEmailAsync(string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<WeightLot>().Where(lot => lot.OwnerEmail == ownerEmail).ToListAsync(cancellationToken);
    }

    public async Task<WeightLot?> FindByIdAndOwnerEmailAsync(int id, string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<WeightLot>()
            .FirstOrDefaultAsync(lot => lot.Id == id && lot.OwnerEmail == ownerEmail, cancellationToken);
    }

    public async Task<bool> ExistsByIdAndOwnerEmailAsync(int id, string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<WeightLot>()
            .AnyAsync(lot => lot.Id == id && lot.OwnerEmail == ownerEmail, cancellationToken);
    }

    public async Task RemoveByProductIdAndOwnerEmailAsync(int productId, string ownerEmail,
        CancellationToken cancellationToken)
    {
        var lots = await Context.Set<WeightLot>()
            .Where(lot => lot.ProductId == productId && lot.OwnerEmail == ownerEmail).ToListAsync(cancellationToken);
        Context.Set<WeightLot>().RemoveRange(lots);
    }
}
