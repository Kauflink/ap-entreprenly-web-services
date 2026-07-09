using Entreprenly.WebServices.Shared.Domain.Model;

namespace Entreprenly.WebServices.Inventory.Domain.Model.Errors;

/// <summary>
///     Catalog of Inventory domain errors, exposed as reusable <see cref="Error" /> instances.
/// </summary>
public static class InventoryErrors
{
    public static readonly Error OwnerRequired =
        new("Inventory.OwnerRequired", "An authenticated owner is required.");

    public static readonly Error ProductNameRequired =
        new("Inventory.ProductNameRequired", "A product name is required.");

    public static readonly Error ProductIdRequired =
        new("Inventory.ProductIdRequired", "A product is required.");

    public static readonly Error NegativeQuantity =
        new("Inventory.NegativeQuantity", "Quantity cannot be negative.");

    public static readonly Error UnitProductNotFound =
        new("Inventory.UnitProductNotFound", "Unit product not found.");

    public static readonly Error WeightProductNotFound =
        new("Inventory.WeightProductNotFound", "Weight product not found.");

    public static readonly Error UnitLotNotFound =
        new("Inventory.UnitLotNotFound", "Unit lot not found.");

    public static readonly Error WeightLotNotFound =
        new("Inventory.WeightLotNotFound", "Weight lot not found.");

    public static readonly Error ProductCodeAlreadyExists =
        new("Inventory.ProductCodeAlreadyExists", "A product already exists with this QR code.");
}
