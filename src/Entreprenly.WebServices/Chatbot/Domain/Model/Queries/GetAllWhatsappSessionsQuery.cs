namespace Entreprenly.WebServices.Chatbot.Domain.Model.Queries;

/// <summary>
///     Query to retrieve all WhatsApp sessions, optionally filtered by seller.
/// </summary>
public record GetAllWhatsappSessionsQuery(int? SellerId = null);
