using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class WeightProductResourceFromEntityAssembler
{
    public static WeightProductResource ToResourceFromEntity(WeightProduct entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new WeightProductResource(
            entity.Id,
            entity.Name,
            entity.Description,
            entity.CodeQr,
            entity.ProductType.ToValue(),
            entity.PricePerKg);
    }
}
