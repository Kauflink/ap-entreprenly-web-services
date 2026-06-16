using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class WeightLotResourceFromEntityAssembler
{
    public static WeightLotResource ToResourceFromEntity(WeightLot entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new WeightLotResource(
            entity.Id,
            entity.ProductId,
            entity.CodeQr,
            entity.EntryDate,
            entity.ProductType.ToValue(),
            entity.QuantityKg);
    }
}
