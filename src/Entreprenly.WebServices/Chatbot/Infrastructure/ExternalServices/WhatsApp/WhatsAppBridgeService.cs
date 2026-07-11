using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Entreprenly.WebServices.Chatbot.Application.Internal.OutboundServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Entreprenly.WebServices.Chatbot.Infrastructure.ExternalServices.WhatsApp;

/// <summary>
///     Sends messages through the Node.js whatsapp-web.js bridge via HTTP.
///     The bridge is an infrastructure detail; the domain only knows IWhatsAppMessagingService.
/// </summary>
public class WhatsAppBridgeService(
    HttpClient httpClient,
    IOptions<WhatsAppBridgeOptions> options,
    ILogger<WhatsAppBridgeService> logger)
    : IWhatsAppMessagingService
{
    private readonly WhatsAppBridgeOptions _options = options.Value;
    private static readonly JsonSerializerOptions BridgeJsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task SendMessageAsync(string ownerEmail, string phone, string content,
        CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(new { email = ownerEmail, phone, content });
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.BridgeUrl}/send")
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };
        request.Headers.Add("X-Bridge-Token", _options.BridgeToken);

        try
        {
            var response = await httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                logger.LogWarning("Bridge returned {Status} for {Email} → {Phone}", response.StatusCode, ownerEmail,
                    phone);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not reach WhatsApp bridge for {Email}", ownerEmail);
        }
    }

    public async Task<(string? Qr, bool Connected)> GetOrStartSessionAsync(string ownerEmail, int sellerId,
        string businessName, CancellationToken cancellationToken)
    {
        var url = $"{_options.BridgeUrl}/qr" +
                  $"?email={Uri.EscapeDataString(ownerEmail)}" +
                  $"&sellerId={sellerId}" +
                  $"&businessName={Uri.EscapeDataString(businessName)}";

        try
        {
            var response = await httpClient.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Bridge /qr returned {Status} for {Email}", response.StatusCode, ownerEmail);
                return (null, false);
            }

            var body = await response.Content.ReadFromJsonAsync<BridgeQrResponse>(BridgeJsonOptions, cancellationToken);
            return (body?.Qr, body?.Connected ?? false);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not reach WhatsApp bridge /qr for {Email}", ownerEmail);
            return (null, false);
        }
    }

    private sealed record BridgeQrResponse(string? Qr, bool Connected);
}
