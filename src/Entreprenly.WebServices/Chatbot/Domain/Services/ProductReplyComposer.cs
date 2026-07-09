using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;
using Entreprenly.WebServices.Chatbot.Resources;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Chatbot.Domain.Services;

public class ProductReplyComposer(
    ICatalogProductRepository catalog,
    IStringLocalizer<ChatbotMessages> botLocalizer)
{
    private static readonly Regex NumberPattern =
        new(@"(?<!\w)(\d+(?:[.,]\d+)?)(?!\w)", RegexOptions.Compiled);

    private static readonly Dictionary<string, double> NumberWords = new(StringComparer.OrdinalIgnoreCase)
    {
        ["un"] = 1, ["una"] = 1, ["uno"] = 1,
        ["dos"] = 2, ["tres"] = 3, ["cuatro"] = 4, ["cinco"] = 5, ["seis"] = 6,
        ["siete"] = 7, ["ocho"] = 8, ["nueve"] = 9, ["diez"] = 10,
        ["once"] = 11, ["doce"] = 12,
        ["media"] = 0.5, ["medio"] = 0.5,
        ["one"] = 1, ["two"] = 2, ["three"] = 3, ["four"] = 4, ["five"] = 5,
        ["six"] = 6, ["seven"] = 7, ["eight"] = 8, ["nine"] = 9, ["ten"] = 10,
        ["eleven"] = 11, ["twelve"] = 12, ["half"] = 0.5
    };

    private static readonly string[] OrderIntent =
    [
        "quiero", "quisiera", "deseo", "dame", "necesito", "comprar", "pedir", "pedido",
        "llevar", "llevo", "ponme", "mandame", "enviame", "vender", "me das",
        "kilos", "kilo", "kg", "unidad", "unidades",
        "i want", "i'd like", "i would like", "give me", "send me", "i need",
        "buy", "order", "get me", "can i get", "i'll take",
        "pounds", "lbs", "pieces", "piece", "units", "unit"
    ];

    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "quiero","quisiera","deseo","dame","necesito","comprar","pedir","pedido",
        "llevar","llevo","ponme","mandame","enviame","vender","das",
        "kilos","kilo","unidad","unidades",
        "un","una","uno","dos","tres","cuatro","cinco","seis","siete","ocho",
        "nueve","diez","once","doce","media","medio",
        "de","del","la","el","los","las","por","favor","me","kg",
        "want","need","buy","order","give","send","take","get","please",
        "i","a","an","the","of","some","few","couple",
        "one","two","three","four","five","six","seven","eight","nine","ten",
        "eleven","twelve","half","pounds","lbs","pieces","piece","units","unit"
    };

    // ── Public API ──────────────────────────────────────────────────────────────

    public async Task<IList<CatalogProduct>> FetchCatalogAsync(string ownerEmail, CancellationToken ct)
        => (await catalog.FindByOwnerEmailAsync(ownerEmail, ct)).ToList();

    public string? Compose(string text, IList<CatalogProduct> products)
    {
        if (string.IsNullOrWhiteSpace(text) || products.Count == 0) return null;
        var normalized = Normalize(text);

        var match = BestMatch(normalized, products);
        if (match is not null)
            return ReplyForProduct(normalized, match);

        if (MentionsAny(normalized, "catalogo", "productos", "que venden", "que vende", "que vendes",
                "que tienes", "que hay", "menu", "lista", "ofrecen",
                "catalog", "catalogue", "what do you sell", "what do you have", "show me",
                "what do you offer", "your products", "product list", "what can i buy",
                "what is available", "what are your products", "what items"))
            return ReplyWithCatalogue(products);

        if (MentionsAny(normalized, OrderIntent))
            return ReplyWhenProductNotFound(products, ExtractRequestedProduct(normalized));

        if (IsProductIntent(normalized))
            return ReplyWhenProductNotFound(products, null);

        return null;
    }

    public OrderItem? DetectOrder(string text, IList<CatalogProduct> products)
        => DetectOrder(text, products, null);

    public OrderItem? DetectOrder(string text, IList<CatalogProduct> products, CatalogProduct? contextProduct)
    {
        if (string.IsNullOrWhiteSpace(text) || products.Count == 0) return null;
        var normalized = Normalize(text);

        var product = BestMatch(normalized, products);
        var usingContext = product is null;
        if (usingContext) product = contextProduct;
        if (product is null) return null;

        var qty = usingContext
            ? AnyQuantity(normalized, product)
            : OrderQuantity(normalized, product);
        if (qty is null) return null;

        double q = qty.Value;
        if (q <= 0 || !product.IsInStock || q > (double)product.Stock) return null;

        return new OrderItem(product.Id, product.Name, product.Price, (decimal)q);
    }

    public CatalogProduct? MatchProduct(string text, IList<CatalogProduct> products)
    {
        if (string.IsNullOrWhiteSpace(text) || products.Count == 0) return null;
        return BestMatch(Normalize(text), products);
    }

    // ── Reply builders ──────────────────────────────────────────────────────────

    private string ReplyForProduct(string normalized, CatalogProduct product)
    {
        var priceLabel = product.SoldByWeight
            ? botLocalizer["PriceLabelPerKg"].Value
            : botLocalizer["PriceLabelEach"].Value;
        var unitLabel = product.SoldByWeight ? "kg" : botLocalizer["UnitLabelUnits"].Value;

        var qty = OrderQuantity(normalized, product);
        if (qty is not null)
        {
            double q = qty.Value;
            if (!product.IsInStock || q > (double)product.Stock)
                return string.Format(botLocalizer["ProductLimitedStockReply"].Value,
                    FormatStock(product, unitLabel), product.Name);

            double total = Math.Round(q * (double)product.Price * 100.0) / 100.0;
            return string.Format(botLocalizer["ProductQuantityConfirmReply"].Value,
                FormatQuantity(q, unitLabel), product.Name, FormatPrice(total));
        }

        if (!product.IsInStock)
            return string.Format(botLocalizer["ProductOutOfStockReply"].Value,
                product.Name, FormatPrice((double)product.Price), priceLabel);

        return string.Format(botLocalizer["ProductFoundReply"].Value,
            product.Name, FormatPrice((double)product.Price), priceLabel, FormatStock(product, unitLabel));
    }

    private string ReplyWithCatalogue(IList<CatalogProduct> products)
    {
        var inStock = products.Where(p => p.IsInStock).ToList();
        if (inStock.Count == 0)
            return botLocalizer["CatalogueEmptyReply"].Value;

        var items = string.Join(", ", inStock.Select(p =>
        {
            var pl = p.SoldByWeight
                ? botLocalizer["PriceLabelPerKg"].Value
                : botLocalizer["PriceLabelEach"].Value;
            return $"{p.Name} ({FormatPrice((double)p.Price)} {pl})";
        }));
        return string.Format(botLocalizer["CatalogueListReply"].Value, items);
    }

    private string ReplyWhenProductNotFound(IList<CatalogProduct> products, string? requested)
    {
        var inStock = products.Where(p => p.IsInStock).ToList();
        if (inStock.Count == 0)
        {
            return requested is not null
                ? string.Format(botLocalizer["ProductNotFoundNoStockReply"].Value, requested)
                : botLocalizer["NoProductsAvailableReply"].Value;
        }

        var items = string.Join(", ", inStock.Select(p =>
        {
            var pl = p.SoldByWeight
                ? botLocalizer["PriceLabelPerKg"].Value
                : botLocalizer["PriceLabelEach"].Value;
            return $"{p.Name} ({FormatPrice((double)p.Price)} {pl})";
        }));

        return requested is not null
            ? string.Format(botLocalizer["ProductNotFoundWithAlternativesReply"].Value, requested, items)
            : string.Format(botLocalizer["ProductNotFoundSuggestionsReply"].Value, items);
    }

    // ── Matching ────────────────────────────────────────────────────────────────

    private static CatalogProduct? BestMatch(string normalized, IList<CatalogProduct> products)
    {
        CatalogProduct? best = null;
        var bestScore = 0;
        foreach (var p in products)
        {
            var score = Normalize(p.Name)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Count(token => token.Length >= 3 && normalized.Contains(token, StringComparison.Ordinal));
            if (score > bestScore) { bestScore = score; best = p; }
        }
        return bestScore > 0 ? best : null;
    }

    // ── Quantity extraction ─────────────────────────────────────────────────────

    private static double? OrderQuantity(string text, CatalogProduct product)
    {
        if (!MentionsAny(text, OrderIntent)) return null;
        return AnyQuantity(text, product);
    }

    private static double? AnyQuantity(string text, CatalogProduct product)
    {
        var cleaned = text;
        foreach (var token in Normalize(product.Name).Split(' ', StringSplitOptions.RemoveEmptyEntries))
            if (token.Length >= 3) cleaned = cleaned.Replace(token, " ");

        var match = NumberPattern.Match(cleaned);
        if (match.Success &&
            double.TryParse(match.Groups[1].Value.Replace(',', '.'),
                NumberStyles.Any, CultureInfo.InvariantCulture, out var num))
            return num;

        foreach (var word in cleaned.Split([' ', '.', ',', '!', '?'], StringSplitOptions.RemoveEmptyEntries))
            if (NumberWords.TryGetValue(word, out var val)) return val;

        return null;
    }

    private static string? ExtractRequestedProduct(string text)
    {
        var sb = new StringBuilder();
        foreach (var word in text.Split([' ', '.', ',', '!', '?'], StringSplitOptions.RemoveEmptyEntries))
            if (word.Length >= 3 && !StopWords.Contains(word))
            {
                if (sb.Length > 0) sb.Append(' ');
                sb.Append(word);
            }
        return sb.Length == 0 ? null : sb.ToString();
    }

    // ── Helpers ─────────────────────────────────────────────────────────────────

    private static bool IsProductIntent(string text)
        => MentionsAny(text, "tienen", "tiene", "tienes", "precio", "cuanto cuesta", "cuesta",
            "vale", "comprar", "vendes", "venden", "producto", "stock", "disponible",
            "disponibilidad", "mercaderia", "consigo", "venta",
            "price", "how much", "buy", "sell", "product", "available", "availability",
            "in stock", "do you have", "can i get", "i want", "i need");

    private static bool MentionsAny(string text, params string[] keywords)
    {
        foreach (var k in keywords)
            if (text.Contains(k, StringComparison.Ordinal)) return true;
        return false;
    }

    private static string FormatStock(CatalogProduct p, string unitLabel)
        => p.SoldByWeight
            ? FormattableString.Invariant($"{p.Stock:0.#} kg")
            : FormattableString.Invariant($"{p.Stock:0} {unitLabel}");

    private static string FormatQuantity(double qty, string unitLabel)
        => unitLabel == "kg"
            ? FormattableString.Invariant($"{qty:0.#} kg")
            : FormattableString.Invariant($"{(long)qty} {unitLabel}");

    private static string FormatPrice(double value)
        => FormattableString.Invariant($"S/{value:0.00}");

    private static string Normalize(string text)
    {
        var sb = new StringBuilder();
        foreach (var c in text.ToLowerInvariant().Normalize(NormalizationForm.FormD))
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        return sb.ToString();
    }
}
