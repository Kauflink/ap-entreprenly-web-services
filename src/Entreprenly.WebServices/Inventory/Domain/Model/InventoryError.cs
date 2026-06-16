namespace Entreprenly.WebServices.Inventory.Domain.Model;

public enum InventoryError
{
    None,
    OwnerRequired,
    ProductNameRequired,
    ProductIdRequired,
    NegativeQuantity,
    UnitProductNotFound,
    WeightProductNotFound,
    UnitLotNotFound,
    WeightLotNotFound,
    ProductCodeAlreadyExists,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
