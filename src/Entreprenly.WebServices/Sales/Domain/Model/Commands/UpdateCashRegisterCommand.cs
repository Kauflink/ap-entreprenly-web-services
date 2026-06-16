namespace Entreprenly.WebServices.Sales.Domain.Model.Commands;

/// <summary>
///     Command to update the running totals of an existing cash register.
/// </summary>
/// <param name="OwnerEmail">The authenticated account that owns the register.</param>
/// <param name="CashRegisterId">The cash register identifier.</param>
/// <param name="TotalCash">The new cash total.</param>
/// <param name="TotalDigital">The new digital total.</param>
/// <param name="SaleCount">The new number of sales registered for the day.</param>
public record UpdateCashRegisterCommand(
    string OwnerEmail,
    int CashRegisterId,
    double TotalCash,
    double TotalDigital,
    int SaleCount);
