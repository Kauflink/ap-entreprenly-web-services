using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class UpdateUnitLotCommandFromResourceAssembler
{
    public static UpdateUnitLotCommand ToCommandFromResource(string ownerEmail, int unitLotId,
        UpdateUnitLotResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateUnitLotCommand(ownerEmail, unitLotId, resource.ProductId, resource.CodeQr,
            resource.EntryDate, resource.Quantity, resource.ExpiryDate);
    }
}
