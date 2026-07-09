namespace Entreprenly.WebServices.Chatbot.Domain.Model.Queries;

/// <summary>
///     Query to retrieve the WhatsApp session associated with a specific seller.
/// </summary>
public record GetWhatsappSessionBySellerIdQuery(int SellerId);
