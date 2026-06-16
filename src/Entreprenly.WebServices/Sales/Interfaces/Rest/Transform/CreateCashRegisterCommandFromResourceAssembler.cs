using Entreprenly.WebServices.Sales.Domain.Model.Commands;
using Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Transform;

public static class CreateCashRegisterCommandFromResourceAssembler
{
    public static CreateCashRegisterCommand ToCommandFromResource(string ownerEmail,
        CreateCashRegisterResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateCashRegisterCommand(
            ownerEmail,
            resource.Date,
            resource.TotalCash,
            resource.TotalDigital);
    }
}
