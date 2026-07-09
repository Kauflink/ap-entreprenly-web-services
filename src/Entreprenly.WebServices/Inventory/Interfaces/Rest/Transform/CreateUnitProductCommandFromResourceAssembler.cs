using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class CreateUnitProductCommandFromResourceAssembler
{
    public static CreateUnitProductCommand ToCommandFromResource(string ownerEmail, CreateUnitProductResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateUnitProductCommand(ownerEmail, resource.Name, resource.Description, resource.CodeQr,
            resource.Price, resource.WeightGrams, resource.Brand);
    }
}
