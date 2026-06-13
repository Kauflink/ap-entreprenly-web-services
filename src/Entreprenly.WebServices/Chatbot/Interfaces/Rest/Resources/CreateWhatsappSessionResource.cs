namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

public record CreateWhatsappSessionResource(int SellerId, string OwnerEmail, string BusinessName);
