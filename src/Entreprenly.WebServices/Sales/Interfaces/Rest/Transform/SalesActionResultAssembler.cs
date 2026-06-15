using Entreprenly.WebServices.Sales.Domain.Model;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Transform;

public static class SalesActionResultAssembler
{
    private static int ToStatusCodeFromSalesError(SalesError error)
    {
        return error switch
        {
            SalesError.OwnerRequired => StatusCodes.Status400BadRequest,
            SalesError.SellerRequired => StatusCodes.Status400BadRequest,
            SalesError.SaleItemsRequired => StatusCodes.Status400BadRequest,
            SalesError.SaleNotFound => StatusCodes.Status404NotFound,
            SalesError.BusinessDayRequired => StatusCodes.Status400BadRequest,
            SalesError.CashRegisterAlreadyExists => StatusCodes.Status409Conflict,
            SalesError.CashRegisterNotFound => StatusCodes.Status404NotFound,
            SalesError.OperationCancelled => StatusCodes.Status409Conflict,
            SalesError.DatabaseError => StatusCodes.Status500InternalServerError,
            SalesError.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }

    public static IActionResult ToActionResultFromResult<T>(
        ControllerBase controller,
        Result<T> result,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);

        var statusCode = ToStatusCodeFromSalesError((SalesError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }
}
