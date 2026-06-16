using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Shared.Application.Model;

namespace Entreprenly.WebServices.Inventory.Application.CommandServices;

/// <summary>
///     Handles unit lot commands.
/// </summary>
public interface IUnitLotCommandService
{
    Task<Result<UnitLot>> Handle(CreateUnitLotCommand command, CancellationToken cancellationToken);

    Task<Result<UnitLot>> Handle(UpdateUnitLotCommand command, CancellationToken cancellationToken);

    Task<Result<int>> Handle(DeleteUnitLotCommand command, CancellationToken cancellationToken);
}
