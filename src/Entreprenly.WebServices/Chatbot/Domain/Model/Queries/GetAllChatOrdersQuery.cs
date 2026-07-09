namespace Entreprenly.WebServices.Chatbot.Domain.Model.Queries;

/// <summary>
///     Query to retrieve all chat orders, optionally filtered by seller.
/// </summary>
public record GetAllChatOrdersQuery(int? SellerId = null);
