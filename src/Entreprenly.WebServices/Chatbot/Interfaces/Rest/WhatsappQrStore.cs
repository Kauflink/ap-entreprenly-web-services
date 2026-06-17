using System.Collections.Concurrent;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest;

internal static class WhatsappQrStore
{
    private static readonly ConcurrentDictionary<int, string> Store = new();

    internal static void Set(int sellerId, string qrDataUrl) => Store[sellerId] = qrDataUrl;
    internal static string? Get(int sellerId) => Store.TryGetValue(sellerId, out var v) ? v : null;
    internal static void Clear(int sellerId) => Store.TryRemove(sellerId, out _);
}
