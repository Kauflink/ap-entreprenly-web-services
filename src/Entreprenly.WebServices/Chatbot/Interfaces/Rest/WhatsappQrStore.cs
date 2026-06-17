using System.Collections.Concurrent;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest;

internal static class WhatsappQrStore
{
    private static readonly ConcurrentDictionary<string, string> Store =
        new(StringComparer.OrdinalIgnoreCase);

    internal static void Set(string ownerEmail, string qrDataUrl) => Store[ownerEmail] = qrDataUrl;
    internal static string? Get(string ownerEmail) => Store.TryGetValue(ownerEmail, out var v) ? v : null;
    internal static void Clear(string ownerEmail) => Store.TryRemove(ownerEmail, out _);
}
