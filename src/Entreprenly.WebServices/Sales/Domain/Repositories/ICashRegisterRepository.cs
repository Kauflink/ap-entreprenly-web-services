using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Shared.Domain.Repositories;

namespace Entreprenly.WebServices.Sales.Domain.Repositories;

/// <summary>
///     Repository contract for the <see cref="CashRegister" /> aggregate. All lookups are scoped to
///     the owner account.
/// </summary>
public interface ICashRegisterRepository : IBaseRepository<CashRegister>
{
    Task<IEnumerable<CashRegister>> FindAllByOwnerEmailAsync(string ownerEmail, CancellationToken cancellationToken);

    Task<CashRegister?> FindByIdAndOwnerEmailAsync(int id, string ownerEmail, CancellationToken cancellationToken);

    Task<CashRegister?> FindByDateAndOwnerEmailAsync(DateOnly date, string ownerEmail,
        CancellationToken cancellationToken);

    Task<bool> ExistsByDateAndOwnerEmailAsync(DateOnly date, string ownerEmail, CancellationToken cancellationToken);
}
