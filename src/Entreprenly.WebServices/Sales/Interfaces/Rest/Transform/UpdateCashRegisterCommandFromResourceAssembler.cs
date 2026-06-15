using Entreprenly.WebServices.Sales.Domain.Model.Commands;
using Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Transform;

public static class UpdateCashRegisterCommandFromResourceAssembler
{
    public static UpdateCashRegisterCommand ToCommandFromResource(string ownerEmail, int cashRegisterId,
        UpdateCashRegisterResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateCashRegisterCommand(
            ownerEmail,
            cashRegisterId,
            resource.TotalCash,
            resource.TotalDigital,
            resource.SaleCount);
    }
}
