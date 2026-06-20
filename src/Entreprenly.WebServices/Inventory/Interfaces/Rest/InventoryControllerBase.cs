using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Microsoft.AspNetCore.Mvc;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest;

/// <summary>
///     Base controller for the Inventory bounded context. Exposes the authenticated account email,
///     used as the owner identity that scopes every inventory operation.
/// </summary>
public abstract class InventoryControllerBase : ControllerBase
{
    protected string OwnerEmail => (HttpContext.Items["User"] as User)?.Email ?? string.Empty;

    protected int OwnerUserId => (HttpContext.Items["User"] as User)?.Id ?? 0;
}
