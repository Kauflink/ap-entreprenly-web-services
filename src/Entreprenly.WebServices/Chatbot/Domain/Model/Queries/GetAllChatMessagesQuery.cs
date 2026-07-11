namespace Entreprenly.WebServices.Chatbot.Domain.Model.Queries;

/// <summary>
///     Query to retrieve all chat messages across every conversation belonging to a seller.
/// </summary>
public record GetAllChatMessagesQuery(int SellerId);
