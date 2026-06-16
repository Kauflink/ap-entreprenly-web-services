namespace Entreprenly.WebServices.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get a single lot from the combined view by its identifier.
/// </summary>
public record GetLotByIdQuery(string OwnerEmail, int LotId);
