using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Transform;

public static class CashRegisterResourceFromEntityAssembler
{
    public static CashRegisterResource ToResourceFromEntity(CashRegister entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new CashRegisterResource(
            entity.Id,
            entity.Date,
            entity.TotalCash,
            entity.TotalDigital,
            entity.SaleCount);
    }
}
