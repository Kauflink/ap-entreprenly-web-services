using Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Inventory.Domain.Model.Entities;

/// <summary>
///     Generic lot read model. Provides a unified, read-only view over both <c>UnitLot</c> and
///     <c>WeightLot</c> aggregates, exposing only the attributes they share. It is never persisted;
///     it is assembled on demand so clients can list every stock batch regardless of its
///     measurement <see cref="ProductType" />.
/// </summary>
public class Lot(int id, int productId, string? codeQr, DateTimeOffset entryDate, ProductType lotType)
{
    public int Id { get; } = id;
    public int ProductId { get; } = productId;
    public string? CodeQr { get; } = codeQr;
    public DateTimeOffset EntryDate { get; } = entryDate;
    public ProductType LotType { get; } = lotType;
}
