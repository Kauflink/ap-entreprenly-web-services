using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class CreateWeightLotCommandFromResourceAssembler
{
    public static CreateWeightLotCommand ToCommandFromResource(string ownerEmail, CreateWeightLotResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateWeightLotCommand(ownerEmail, resource.ProductId, resource.CodeQr, resource.EntryDate,
            resource.QuantityKg);
    }
}
