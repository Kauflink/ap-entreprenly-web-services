namespace Entreprenly.WebServices.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get a unit lot owned by an account by its identifier.
/// </summary>
public record GetUnitLotByIdQuery(string OwnerEmail, int UnitLotId);
