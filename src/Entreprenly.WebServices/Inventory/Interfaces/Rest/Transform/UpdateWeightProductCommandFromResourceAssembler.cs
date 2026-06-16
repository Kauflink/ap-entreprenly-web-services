using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class UpdateWeightProductCommandFromResourceAssembler
{
    public static UpdateWeightProductCommand ToCommandFromResource(string ownerEmail, int weightProductId,
        UpdateWeightProductResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateWeightProductCommand(ownerEmail, weightProductId, resource.Name, resource.Description,
            resource.CodeQr, resource.PricePerKg);
    }
}
