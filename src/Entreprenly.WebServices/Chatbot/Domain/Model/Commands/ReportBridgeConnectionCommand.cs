namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

/// <summary>
///     Command to report a WhatsApp bridge connection or disconnection event.
/// </summary>
public record ReportBridgeConnectionCommand(bool Connected, string? Phone, string OwnerEmail, string BusinessName, int SellerId);
