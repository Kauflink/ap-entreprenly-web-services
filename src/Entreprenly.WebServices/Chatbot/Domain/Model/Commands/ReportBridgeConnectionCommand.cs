namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

public record ReportBridgeConnectionCommand(bool Connected, string? Phone, string OwnerEmail, string BusinessName, int SellerId);
