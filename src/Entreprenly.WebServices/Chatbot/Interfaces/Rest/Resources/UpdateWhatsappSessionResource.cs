namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

/// <summary>
///     Body for updating the authenticated account's WhatsApp session status.
///     SellerId and OwnerEmail are intentionally not part of this resource: they are always
///     resolved from the authenticated caller, never trusted from client input.
/// </summary>
public record UpdateWhatsappSessionResource(string BusinessName, string Status, string? Phone);
