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
                "aceptan tarjeta", "aceptan transferencia", "puedo pagar con",
                "how do i pay", "payment methods", "how to pay", "payment options",
                "do you accept", "can i pay", "how can i pay"))
            return Reply(localizer["PaymentMethodsReply"].Value);

        if (Phrase(text, "comprobante", "ya pague", "ya pagué", "envie el pago", "envio el pago",
                "mi pago", "hice el pago", "hice la transferencia", "adjunto",
                "i already paid", "i paid", "sent payment", "sent the payment", "payment sent",
                "proof of payment", "transfer done", "i made the transfer"))
            return Reply(localizer["ReceiptReceivedReply"].Value);

        if (Phrase(text, "delivery", "hacen envios", "hacen envio", "envian", "reparto",
                "a domicilio", "cuanto demora", "cuanto tarda", "tiempo de entrega", "cuando llega",
                "costo de envio", "cobran envio", "tienen delivery",
                "do you deliver", "home delivery", "shipping", "can you ship", "delivery cost",
                "how long does it take", "delivery time", "do you ship"))
            return Reply(localizer["DeliveryReply"].Value);

        if (Phrase(text, "horario", "que hora atienden", "a que hora abren", "a que hora cierran",
                "hasta que hora", "estan abiertos", "estan atendiendo", "atienden hoy", "abren hoy",
                "dias atienden", "que dias",
                "opening hours", "business hours", "what time", "when do you open", "when do you close",
                "are you open", "hours of operation", "schedule"))
            return Reply(localizer["HoursReply"].Value);

        if (Phrase(text, "donde estan", "donde quedan", "ubicacion", "su direccion", "tienen local",
                "tienda fisica", "como llego", "donde los encuentro", "donde se ubican",
                "where are you", "your address", "your location", "where are you located",
                "physical store", "how do i get there", "where do i find you"))
            return Reply(localizer["LocationReply"].Value);

        if (Phrase(text, "mi pedido", "mi orden", "estado de mi", "donde esta mi pedido",
                "ya salio mi", "cuando llega mi", "seguimiento", "rastrear",
                "my order", "order status", "where is my order", "track my order",
                "order number", "when will it arrive", "has my order"))
            return Reply(localizer["OrderStatusReply"].Value);

        if (Phrase(text, "asesor", "hablar con alguien", "hablar con una persona", "una persona",
                "un humano", "atencion al cliente", "necesito ayuda", "soporte",
                "speak to someone", "talk to a person", "human agent", "customer service",
                "i need help", "support", "real person", "talk to a human"))
            return Reply(string.Format(localizer["AdvisorReply"].Value, name));

        if (Phrase(text, "reclamo", "queja", "no llego", "esta mal", "llego mal",
                "llego incompleto", "problema con", "no funciona", "quiero devolver", "devolucion",
                "esta dañado", "esta vencido",
                "complaint", "i have a problem", "it arrived wrong", "wrong order", "damaged",
                "not delivered", "want to return", "return policy", "bad experience", "issue with"))
            return Reply(string.Format(localizer["ComplaintReply"].Value, name));

        if (Phrase(text, "eres un bot", "eres bot", "eres real", "eres una persona", "eres humano",
                "robot", "con quien hablo", "eres una maquina",
                "are you a bot", "are you real", "are you human", "are you a robot",
                "who am i talking to", "is this a bot", "talking to a machine"))
            return Reply(string.Format(localizer["BotIdentityReply"].Value, name));

        if (Phrase(text, "precio", "precios", "cuanto cuesta", "cuanto sale", "cuanto vale",
                "costo", "que precio",
                "price", "prices", "how much", "how much does", "how much is", "cost", "what does it cost"))
            return Reply(localizer["PriceInfoReply"].Value);

        if (Phrase(text, "stock", "disponible", "disponibilidad", "hay", "queda", "quedan", "consigo",
                "in stock", "availability", "available", "do you have", "is it available"))
            return Reply(localizer["StockReply"].Value);

        if (Phrase(text, "catalogo", "productos", "que venden", "que vendes", "menu", "lista de precios",
                "que ofrecen", "que tienen para", "muestrame",
                "catalog", "catalogue", "what do you sell", "what do you have", "show me",
                "what do you offer", "your products", "product list", "what can i buy", "what is available"))
            return Reply(localizer["CatalogueReply"].Value);

        if (Phrase(text, "pedido", "quiero", "quisiera", "comprar", "ordenar", "pedir", "necesito",
                "llevar", "me gustaria", "adquirir",
                "i want", "i would like", "i'd like", "i want to buy", "place an order",
                "i want to order", "purchase", "i need", "can i get", "i want to get"))
            return Reply(localizer["PlaceOrderReply"].Value);

        if (Phrase(text, "gracias", "muchas gracias", "mil gracias", "te agradezco",
                "thank you", "thanks", "thank u", "many thanks", "appreciate it"))
            return Reply(string.Format(localizer["ThanksReply"].Value, name));

        if (Phrase(text, "adios", "hasta luego", "nos vemos", "chau", "hasta pronto", "bye",
                "goodbye", "see you", "farewell", "take care", "see you later", "cya"))
            return Reply(string.Format(localizer["GoodbyeReply"].Value, name));

        if (Phrase(text, "hola", "buenas", "buenos dias", "buenas tardes", "buenas noches", "hey", "saludos", "alo",
                "hello", "hi", "good morning", "good afternoon", "good evening", "greetings", "howdy"))
            return Reply(string.Format(localizer["GreetingReply"].Value, name));

        if (HasWord(words, "si", "claro", "dale", "ok", "okay", "listo", "perfecto", "correcto", "afirmativo",
                "yes", "yeah", "yep", "sure", "alright", "correct", "exactly", "right"))
            return Reply(localizer["AffirmativeReply"].Value);

        if (HasWord(words, "no", "nada", "ninguno", "ninguna", "nope", "never", "nothing", "none"))
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
