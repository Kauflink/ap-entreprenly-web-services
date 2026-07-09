using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class CreateUnitLotCommandFromResourceAssembler
{
    public static CreateUnitLotCommand ToCommandFromResource(string ownerEmail, CreateUnitLotResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateUnitLotCommand(ownerEmail, resource.ProductId, resource.CodeQr, resource.EntryDate,
            resource.Quantity, resource.ExpiryDate);
    }
}
