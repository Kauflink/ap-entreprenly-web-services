using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;

namespace Entreprenly.WebServices.Chatbot.Domain.Services;

public class ProductReplyComposer(ICatalogProductRepository catalog)
{
    private static readonly Dictionary<string, decimal> WordToNumber = new(StringComparer.OrdinalIgnoreCase)
    {
        ["un"] = 1, ["una"] = 1, ["dos"] = 2, ["tres"] = 3, ["cuatro"] = 4, ["cinco"] = 5,
        ["seis"] = 6, ["siete"] = 7, ["ocho"] = 8, ["nueve"] = 9, ["diez"] = 10, ["media"] = 0.5m
    };

    private readonly Dictionary<int, CatalogProduct> _lastProductByConversation = new();

    public async Task<(List<OrderItem>? items, string? reply)> TryComposeAsync(
        string text, int conversationId, int sellerId, CancellationToken ct)
    {
        var products = (await catalog.FindBySellerIdAsync(sellerId, ct)).ToList();
        if (products.Count == 0) return (null, null);

        var normalized = Normalize(text);
        var qty = ExtractQuantity(normalized);
        var best = BestMatch(normalized, products);

        if (best is null)
        {
            if (qty.HasValue && _lastProductByConversation.TryGetValue(conversationId, out var lastProduct))
                best = lastProduct;
        }

        if (best is null) return (null, null);

        _lastProductByConversation[conversationId] = best;

        if (!qty.HasValue)
        {
            var unit = best.SoldByWeight ? "kg" : "unidad";
            var stock = best.SoldByWeight
                ? $"{best.Stock:0.##} kg disponibles"
                : $"{best.Stock:0} unidades disponibles";
            return (null,
                $"Si, tenemos {best.Name} a S/{best.Price:0.00} por {unit}. {stock}. Cuantos deseas?");
        }

        var item = new OrderItem(best.Id, best.Name, best.Price, qty.Value);
        var total = item.Subtotal;
        var qtyDisplay = best.SoldByWeight ? $"{qty:0.##} kg" : $"{qty:0} unidad(es)";
        return ([item],
            $"Anotado: {qtyDisplay} de {best.Name} = S/{total:0.00}. A que direccion lo enviamos?");
    }

    public string ListProducts(IEnumerable<CatalogProduct> products)
    {
        var sb = new StringBuilder("Nuestro catalogo:\n");
        foreach (var p in products)
        {
            var unit = p.SoldByWeight ? "kg" : "unidad";
            sb.AppendLine($"- {p.Name} — S/{p.Price:0.00}/{unit}");
        }
        sb.Append("\nQue deseas pedir?");
        return sb.ToString();
    }

    private static CatalogProduct? BestMatch(string normalized, List<CatalogProduct> products)
    {
        CatalogProduct? best = null;
        var bestScore = 0;
        foreach (var p in products)
        {
            var score = 0;
            foreach (var token in Normalize(p.Name).Split(' ', StringSplitOptions.RemoveEmptyEntries))
                if (token.Length >= 3 && normalized.Contains(token, StringComparison.Ordinal))
                    score++;
            if (score > bestScore) { bestScore = score; best = p; }
        }
        return bestScore > 0 ? best : null;
    }

    private static decimal? ExtractQuantity(string text)
    {
        var match = Regex.Match(text, @"\b(\d+(?:[.,]\d+)?)\b");
        if (match.Success &&
            decimal.TryParse(match.Groups[1].Value.Replace(',', '.'),
                NumberStyles.Any, CultureInfo.InvariantCulture, out var num))
            return num;

        foreach (var (word, value) in WordToNumber)
            if (Regex.IsMatch(text, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase))
                return value;

        return null;
    }

    private static string Normalize(string text)
    {
        var sb = new StringBuilder();
        foreach (var c in text.ToLowerInvariant().Normalize(NormalizationForm.FormD))
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        return sb.ToString();
    }
}
