namespace Entreprenly.WebServices.Sales.Domain.Model.Commands;

/// <summary>
///     Command to open a cash register for a given business day.
/// </summary>
/// <param name="OwnerEmail">The authenticated account that owns the register.</param>
/// <param name="Date">The business day this register belongs to.</param>
/// <param name="TotalCash">The initial cash total.</param>
/// <param name="TotalDigital">The initial digital total.</param>
public record CreateCashRegisterCommand(
    string OwnerEmail,
    DateOnly Date,
    double TotalCash,
    double TotalDigital);
