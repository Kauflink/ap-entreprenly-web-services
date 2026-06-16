namespace Entreprenly.WebServices.Chatbot.Infrastructure.ExternalServices.WhatsApp;

public class WhatsAppBridgeOptions
{
    public string BridgeUrl   { get; set; } = "http://localhost:3001";
    public string BridgeToken { get; set; } = "entreprenly-bridge-secret";
}
