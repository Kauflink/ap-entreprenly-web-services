namespace Entreprenly.WebServices.Chatbot.Application.Internal.OutboundServices;

public interface IWhatsAppMessagingService
{
    Task SendMessageAsync(string ownerEmail, string phone, string content, CancellationToken cancellationToken);

    /// <summary>
    ///     Ensures a bridge session exists for the given seller (starting one on first call) and
    ///     returns its current pairing QR (if still waiting to be scanned) and connection state.
    ///     The sellerId must come from the authenticated caller, never from client input, because the
    ///     bridge has no authentication of its own on this route.
    /// </summary>
    Task<(string? Qr, bool Connected)> GetOrStartSessionAsync(string ownerEmail, int sellerId,
        string businessName, CancellationToken cancellationToken);
}
