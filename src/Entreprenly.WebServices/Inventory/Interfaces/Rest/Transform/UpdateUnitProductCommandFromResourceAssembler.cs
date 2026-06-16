using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class UpdateUnitProductCommandFromResourceAssembler
{
    public static UpdateUnitProductCommand ToCommandFromResource(string ownerEmail, int unitProductId,
        UpdateUnitProductResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateUnitProductCommand(ownerEmail, unitProductId, resource.Name, resource.Description,
            resource.CodeQr, resource.Price, resource.WeightGrams, resource.Brand);
    }
}
