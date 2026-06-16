namespace Entreprenly.WebServices.Sales.Domain.Model.Aggregates;

/// <summary>
///     Cash register aggregate root.
/// </summary>
/// <remarks>
///     Aggregates the takings of a single business day, split between cash and digital payments,
///     together with the number of sales registered that day. It belongs to the account
///     (<see cref="OwnerEmail" />) that opened it, so each account keeps its own daily register.
///     <see cref="TotalDay" /> is derived from both totals.
/// </remarks>
public partial class CashRegister
{
    public CashRegister()
    {
        OwnerEmail = string.Empty;
    }

    public CashRegister(string ownerEmail, DateOnly date, double totalCash, double totalDigital, int saleCount)
    {
        OwnerEmail = ownerEmail;
        Date = date;
        TotalCash = totalCash;
        TotalDigital = totalDigital;
        SaleCount = saleCount;
    }

    public int Id { get; }
    public string OwnerEmail { get; private set; }
    public DateOnly Date { get; private set; }
    public double TotalCash { get; private set; }
    public double TotalDigital { get; private set; }
    public int SaleCount { get; private set; }

    /// <summary>
    ///     The total takings of the day (cash plus digital).
    /// </summary>
    public double TotalDay => Math.Round((TotalCash + TotalDigital) * 100.0) / 100.0;

    /// <summary>
    ///     Replaces the running totals and sale count of this register.
    /// </summary>
    public CashRegister UpdateTotals(double totalCash, double totalDigital, int saleCount)
    {
        TotalCash = totalCash;
        TotalDigital = totalDigital;
        SaleCount = saleCount;
        return this;
    }
}
