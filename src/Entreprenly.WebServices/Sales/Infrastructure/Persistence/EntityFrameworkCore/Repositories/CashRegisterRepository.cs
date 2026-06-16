using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Sales.Domain.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Sales.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core repository for the <see cref="CashRegister" /> aggregate.
/// </summary>
public class CashRegisterRepository(AppDbContext context)
    : BaseRepository<CashRegister>(context), ICashRegisterRepository
{
    public async Task<IEnumerable<CashRegister>> FindAllByOwnerEmailAsync(string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<CashRegister>()
            .Where(register => register.OwnerEmail == ownerEmail)
            .ToListAsync(cancellationToken);
    }

    public async Task<CashRegister?> FindByIdAndOwnerEmailAsync(int id, string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<CashRegister>()
            .FirstOrDefaultAsync(register => register.Id == id && register.OwnerEmail == ownerEmail,
                cancellationToken);
    }

    public async Task<CashRegister?> FindByDateAndOwnerEmailAsync(DateOnly date, string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<CashRegister>()
            .FirstOrDefaultAsync(register => register.Date == date && register.OwnerEmail == ownerEmail,
                cancellationToken);
    }

    public async Task<bool> ExistsByDateAndOwnerEmailAsync(DateOnly date, string ownerEmail,
        CancellationToken cancellationToken)
    {
        return await Context.Set<CashRegister>()
            .AnyAsync(register => register.Date == date && register.OwnerEmail == ownerEmail, cancellationToken);
    }
}
