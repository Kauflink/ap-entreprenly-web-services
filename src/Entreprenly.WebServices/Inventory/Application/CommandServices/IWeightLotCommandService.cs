using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Shared.Application.Model;

namespace Entreprenly.WebServices.Inventory.Application.CommandServices;

/// <summary>
///     Handles weight lot commands.
/// </summary>
public interface IWeightLotCommandService
{
    Task<Result<WeightLot>> Handle(CreateWeightLotCommand command, CancellationToken cancellationToken);

    Task<Result<WeightLot>> Handle(UpdateWeightLotCommand command, CancellationToken cancellationToken);

    Task<Result<int>> Handle(DeleteWeightLotCommand command, CancellationToken cancellationToken);
}
