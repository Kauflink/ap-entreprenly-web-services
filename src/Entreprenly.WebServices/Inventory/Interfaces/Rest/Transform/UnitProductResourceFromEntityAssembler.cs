using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class UnitProductResourceFromEntityAssembler
{
    public static UnitProductResource ToResourceFromEntity(UnitProduct entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new UnitProductResource(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.CodeQr,
            entity.ProductType.ToValue(),
            entity.Price,
            entity.WeightGrams,
            entity.Brand);
    }
}
