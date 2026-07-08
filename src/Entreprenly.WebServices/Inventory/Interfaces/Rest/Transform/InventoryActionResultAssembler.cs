using Entreprenly.WebServices.Inventory.Domain.Model;
using Entreprenly.WebServices.Shared.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;

public static class InventoryActionResultAssembler
{
    private static int ToStatusCodeFromInventoryError(InventoryError error)
    {
        return error switch
        {
            InventoryError.UnitProductNotFound => StatusCodes.Status404NotFound,
            InventoryError.WeightProductNotFound => StatusCodes.Status404NotFound,
            InventoryError.UnitLotNotFound => StatusCodes.Status404NotFound,
            InventoryError.WeightLotNotFound => StatusCodes.Status404NotFound,
            InventoryError.ProductCodeAlreadyExists => StatusCodes.Status409Conflict,
            InventoryError.OwnerRequired => StatusCodes.Status400BadRequest,
            InventoryError.ProductNameRequired => StatusCodes.Status400BadRequest,
            InventoryError.ProductIdRequired => StatusCodes.Status400BadRequest,
            InventoryError.NegativeQuantity => StatusCodes.Status400BadRequest,
            InventoryError.OperationCancelled => StatusCodes.Status409Conflict,
            InventoryError.DatabaseError => StatusCodes.Status500InternalServerError,
            InventoryError.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }

    public static IActionResult ToActionResultFromResult<T>(
        ControllerBase controller,
        Result<T> result,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);

        var statusCode = ToStatusCodeFromInventoryError((InventoryError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResultFromEntity<T>(
        ControllerBase controller,
        T? entity,
        InventoryError notFoundError,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction)
    {
        if (entity is null)
            return problemDetailsFactory.CreateProblemDetails(
                controller,
                ToStatusCodeFromInventoryError(notFoundError),
                notFoundError,
                errorLocalizer[notFoundError.ToString()]);
        return successAction(entity);
    }
}
