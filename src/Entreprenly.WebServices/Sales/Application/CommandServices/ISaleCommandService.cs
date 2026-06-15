using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Sales.Domain.Model.Commands;
using Entreprenly.WebServices.Shared.Application.Model;

namespace Entreprenly.WebServices.Sales.Application.CommandServices;

/// <summary>
///     Handles sale commands.
/// </summary>
public interface ISaleCommandService
{
    Task<Result<Sale>> Handle(CreateSaleCommand command, CancellationToken cancellationToken);
}
