using Entreprenly.WebServices.Chatbot.Domain.Model;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Transform;

public static class ChatbotActionResultAssembler
{
    private static int ToStatusCode(ChatbotError error) => error switch
    {
        ChatbotError.SessionNotFound => StatusCodes.Status404NotFound,
        ChatbotError.ConversationNotFound => StatusCodes.Status404NotFound,
        ChatbotError.OrderNotFound => StatusCodes.Status404NotFound,
        ChatbotError.OperationCancelled => StatusCodes.Status409Conflict,
        ChatbotError.DatabaseError => StatusCodes.Status500InternalServerError,
        ChatbotError.InternalServerError => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status400BadRequest
    };

    public static IActionResult ToActionResultFromResult<T>(
        ControllerBase controller,
        Result<T> result,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<T, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);
        var statusCode = ToStatusCode((ChatbotError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResultFromNullable<T>(
        ControllerBase controller,
        T? entity,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        ChatbotError notFoundError,
        Func<T, IActionResult> successAction) where T : class
    {
        if (entity is null)
            return problemDetailsFactory.CreateProblemDetails(
                controller,
                StatusCodes.Status404NotFound,
                notFoundError,
                errorLocalizer[notFoundError.ToString()]);
        return successAction(entity);
    }
}
