using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class CreateWeightProductCommandFromResourceAssembler
{
    public static CreateWeightProductCommand ToCommandFromResource(string ownerEmail,
        CreateWeightProductResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateWeightProductCommand(ownerEmail, resource.Name, resource.Description, resource.CodeQr,
            resource.PricePerKg);
    }
}
