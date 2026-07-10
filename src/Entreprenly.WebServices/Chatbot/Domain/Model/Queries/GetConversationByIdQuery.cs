namespace Entreprenly.WebServices.Chatbot.Domain.Model.Queries;

/// <summary>
///     Query to retrieve a single chatbot conversation by its identifier.
/// </summary>
public record GetConversationByIdQuery(int ConversationId);
