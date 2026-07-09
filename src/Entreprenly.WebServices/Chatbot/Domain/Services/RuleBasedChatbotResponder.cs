using System.Globalization;
using System.Text;
using Entreprenly.WebServices.Chatbot.Resources;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Chatbot.Domain.Services;

public class RuleBasedChatbotResponder(IStringLocalizer<ChatbotMessages> localizer) : IChatbotResponder
{
    public Task<string?> GenerateReplyAsync(string incomingMessage, string clientName, CancellationToken cancellationToken)
    {
        var name = string.IsNullOrWhiteSpace(clientName) ? "" : " " + clientName.Trim();
        var text = Normalize(incomingMessage);
        var words = Words(text);

        if (Phrase(text, "como pago", "como puedo pagar", "formas de pago", "metodos de pago",
                "medios de pago", "forma de pago", "metodo de pago", "aceptan yape", "aceptan plin",
                "aceptan tarjeta", "aceptan transferencia", "puedo pagar con"))
            return Reply(localizer["PaymentMethodsReply"].Value);

        if (Phrase(text, "comprobante", "ya pague", "ya pagué", "envie el pago", "envio el pago",
                "mi pago", "hice el pago", "hice la transferencia", "adjunto"))
            return Reply(localizer["ReceiptReceivedReply"].Value);

        if (Phrase(text, "delivery", "hacen envios", "hacen envio", "envian", "reparto",
                "a domicilio", "cuanto demora", "cuanto tarda", "tiempo de entrega", "cuando llega",
                "costo de envio", "cobran envio", "tienen delivery"))
            return Reply(localizer["DeliveryReply"].Value);

        if (Phrase(text, "horario", "que hora atienden", "a que hora abren", "a que hora cierran",
                "hasta que hora", "estan abiertos", "estan atendiendo", "atienden hoy", "abren hoy",
                "dias atienden", "que dias"))
            return Reply(localizer["HoursReply"].Value);

        if (Phrase(text, "donde estan", "donde quedan", "ubicacion", "su direccion", "tienen local",
                "tienda fisica", "como llego", "donde los encuentro", "donde se ubican"))
            return Reply(localizer["LocationReply"].Value);

        if (Phrase(text, "mi pedido", "mi orden", "estado de mi", "donde esta mi pedido",
                "ya salio mi", "cuando llega mi", "seguimiento", "rastrear"))
            return Reply(localizer["OrderStatusReply"].Value);

        if (Phrase(text, "asesor", "hablar con alguien", "hablar con una persona", "una persona",
                "un humano", "atencion al cliente", "necesito ayuda", "soporte"))
            return Reply(string.Format(localizer["AdvisorReply"].Value, name));

        if (Phrase(text, "reclamo", "queja", "no llego", "esta mal", "llego mal",
                "llego incompleto", "problema con", "no funciona", "quiero devolver", "devolucion",
                "esta dañado", "esta vencido"))
            return Reply(string.Format(localizer["ComplaintReply"].Value, name));

        if (Phrase(text, "eres un bot", "eres bot", "eres real", "eres una persona", "eres humano",
                "robot", "con quien hablo", "eres una maquina"))
            return Reply(string.Format(localizer["BotIdentityReply"].Value, name));

        if (Phrase(text, "precio", "precios", "cuanto cuesta", "cuanto sale", "cuanto vale",
                "costo", "que precio"))
            return Reply(localizer["PriceInfoReply"].Value);

        if (Phrase(text, "stock", "disponible", "disponibilidad", "hay", "queda", "quedan", "consigo"))
            return Reply(localizer["StockReply"].Value);

        if (Phrase(text, "catalogo", "productos", "que venden", "que vendes", "menu", "lista de precios",
                "que ofrecen", "que tienen para", "muestrame"))
            return Reply(localizer["CatalogueReply"].Value);

        if (Phrase(text, "pedido", "quiero", "quisiera", "comprar", "ordenar", "pedir", "necesito",
                "llevar", "me gustaria", "adquirir"))
            return Reply(localizer["PlaceOrderReply"].Value);

        if (Phrase(text, "gracias", "muchas gracias", "mil gracias", "te agradezco"))
            return Reply(string.Format(localizer["ThanksReply"].Value, name));

        if (Phrase(text, "adios", "hasta luego", "nos vemos", "chau", "hasta pronto", "bye"))
            return Reply(string.Format(localizer["GoodbyeReply"].Value, name));

        if (Phrase(text, "hola", "buenas", "buenos dias", "buenas tardes", "buenas noches", "hey", "saludos", "alo"))
            return Reply(string.Format(localizer["GreetingReply"].Value, name));

        if (HasWord(words, "si", "claro", "dale", "ok", "okay", "listo", "perfecto", "correcto", "afirmativo"))
            return Reply(localizer["AffirmativeReply"].Value);

        if (HasWord(words, "no", "nada", "ninguno", "ninguna"))
            return Reply(string.Format(localizer["NegativeReply"].Value, name));

        return Reply(string.Format(localizer["DefaultReply"].Value, name));
    }

    private static bool Phrase(string text, params string[] keywords)
    {
        foreach (var k in keywords)
            if (text.Contains(k, StringComparison.Ordinal)) return true;
        return false;
    }

    private static bool HasWord(HashSet<string> words, params string[] candidates)
    {
        foreach (var c in candidates)
            if (words.Contains(c)) return true;
        return false;
    }

    private static HashSet<string> Words(string text)
        => new(text.Split([' ', '.', ',', '!', '?', ';', ':'], StringSplitOptions.RemoveEmptyEntries));

    private static string Normalize(string text)
    {
        var sb = new StringBuilder();
        foreach (var c in text.ToLowerInvariant().Normalize(NormalizationForm.FormD))
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        return sb.ToString();
    }

    private static Task<string?> Reply(string text) => Task.FromResult<string?>(text);
}
