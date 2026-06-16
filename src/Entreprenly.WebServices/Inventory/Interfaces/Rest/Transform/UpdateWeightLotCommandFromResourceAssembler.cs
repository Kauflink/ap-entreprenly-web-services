using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class UpdateWeightLotCommandFromResourceAssembler
{
    public static UpdateWeightLotCommand ToCommandFromResource(string ownerEmail, int weightLotId,
        UpdateWeightLotResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateWeightLotCommand(ownerEmail, weightLotId, resource.ProductId, resource.CodeQr,
            resource.EntryDate, resource.QuantityKg);
    }
}
