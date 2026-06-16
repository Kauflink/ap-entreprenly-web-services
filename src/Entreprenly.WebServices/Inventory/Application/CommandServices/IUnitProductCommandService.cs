using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Shared.Application.Model;

namespace Entreprenly.WebServices.Inventory.Application.CommandServices;

/// <summary>
///     Handles unit product commands.
/// </summary>
public interface IUnitProductCommandService
{
    Task<Result<UnitProduct>> Handle(CreateUnitProductCommand command, CancellationToken cancellationToken);

    Task<Result<UnitProduct>> Handle(UpdateUnitProductCommand command, CancellationToken cancellationToken);

    Task<Result<int>> Handle(DeleteUnitProductCommand command, CancellationToken cancellationToken);
}
