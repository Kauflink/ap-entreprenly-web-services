using Entreprenly.WebServices.Inventory.Domain.Model.Entities;
using Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class StockAlertResourceFromEntityAssembler
{
    public static StockAlertResource ToResourceFromEntity(StockAlert entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new StockAlertResource(
            entity.Id,
            entity.LotId,
            entity.ProductId,
            entity.ProductType?.ToValue(),
            entity.ProductName,
            entity.AlertType.ToValue(),
            entity.Severity.ToValue(),
            entity.Message,
            entity.CreatedAt);
    }
}
