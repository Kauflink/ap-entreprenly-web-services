namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

public record BridgeStatusResource(bool Connected, string? Phone, string OwnerEmail, string BusinessName, int SellerId);
