namespace Entreprenly.WebServices.Chatbot.Domain.Model.Queries;

/// <summary>
///     Query to retrieve all chatbot conversations, optionally filtered by seller.
/// </summary>
public record GetAllConversationsQuery(int? SellerId = null);
