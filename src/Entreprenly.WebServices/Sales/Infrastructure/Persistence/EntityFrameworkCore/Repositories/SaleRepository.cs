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

    public async Task<Sale?> FindByIdAndOwnerEmailAsync(int id, string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<Sale>()
            .FirstOrDefaultAsync(sale => sale.Id == id && sale.OwnerEmail == ownerEmail, cancellationToken);
    }
}
