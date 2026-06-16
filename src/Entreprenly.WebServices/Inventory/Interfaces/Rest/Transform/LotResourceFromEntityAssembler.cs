using Entreprenly.WebServices.Inventory.Domain.Model.Entities;
using Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class LotResourceFromEntityAssembler
{
    public static LotResource ToResourceFromEntity(Lot entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new LotResource(
            entity.Id,
            entity.ProductId,
            entity.CodeQr,
            entity.EntryDate,
            entity.LotType.ToValue());
    }
}
