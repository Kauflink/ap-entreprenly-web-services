using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Shared.Application.Model;

namespace Entreprenly.WebServices.Inventory.Application.CommandServices;

/// <summary>
///     Handles weight product commands.
/// </summary>
public interface IWeightProductCommandService
{
    Task<Result<WeightProduct>> Handle(CreateWeightProductCommand command, CancellationToken cancellationToken);

    Task<Result<WeightProduct>> Handle(UpdateWeightProductCommand command, CancellationToken cancellationToken);

    Task<Result<int>> Handle(DeleteWeightProductCommand command, CancellationToken cancellationToken);
}
