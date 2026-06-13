namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

public record CreateWhatsappSessionCommand(int SellerId, string OwnerEmail, string BusinessName);
