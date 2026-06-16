using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core repository for the <see cref="UnitProduct" /> aggregate.
/// </summary>
public class UnitProductRepository(AppDbContext context)
    : BaseRepository<UnitProduct>(context), IUnitProductRepository
{
    public async Task<IEnumerable<UnitProduct>> FindAllByOwnerEmailAsync(string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<UnitProduct>().Where(product => product.OwnerEmail == ownerEmail)
            .ToListAsync(cancellationToken);
    }

    public async Task<UnitProduct?> FindByIdAndOwnerEmailAsync(int id, string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<UnitProduct>()
            .FirstOrDefaultAsync(product => product.Id == id && product.OwnerEmail == ownerEmail, cancellationToken);
    }

    public async Task<bool> ExistsByIdAndOwnerEmailAsync(int id, string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<UnitProduct>()
            .AnyAsync(product => product.Id == id && product.OwnerEmail == ownerEmail, cancellationToken);
    }

    public async Task<bool> ExistsByCodeQrAndOwnerEmailAsync(string codeQr, string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<UnitProduct>()
            .AnyAsync(product => product.CodeQr == codeQr && product.OwnerEmail == ownerEmail, cancellationToken);
    }
}
