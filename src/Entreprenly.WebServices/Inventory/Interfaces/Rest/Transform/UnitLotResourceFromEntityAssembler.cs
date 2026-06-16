using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class UnitLotResourceFromEntityAssembler
{
    public static UnitLotResource ToResourceFromEntity(UnitLot entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new UnitLotResource(
            entity.Id,
            entity.ProductId,
            entity.CodeQr,
            entity.EntryDate,
            entity.ProductType.ToValue(),
            entity.Quantity,
            entity.ExpiryDate);
    }
}
