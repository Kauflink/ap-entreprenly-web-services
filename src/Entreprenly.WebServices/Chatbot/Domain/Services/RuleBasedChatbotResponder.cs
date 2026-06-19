using System.Globalization;
using System.Text;

namespace Entreprenly.WebServices.Chatbot.Domain.Services;

public class RuleBasedChatbotResponder : IChatbotResponder
{
    public Task<string?> GenerateReplyAsync(string incomingMessage, string clientName, CancellationToken cancellationToken)
    {
        var name = string.IsNullOrWhiteSpace(clientName) ? "" : " " + clientName.Trim();
        var text = Normalize(incomingMessage);
        var words = Words(text);

        if (Phrase(text, "como pago", "como puedo pagar", "formas de pago", "metodos de pago",
                "medios de pago", "forma de pago", "metodo de pago", "aceptan yape", "aceptan plin",
                "aceptan tarjeta", "aceptan transferencia", "puedo pagar con"))
            return Reply("Aceptamos Yape, Plin, transferencia bancaria y efectivo contra entrega. " +
                         "Cuando tengas tu pedido listo, envíame la captura del pago por aquí y lo validamos.");

        if (Phrase(text, "comprobante", "ya pague", "ya pagué", "envie el pago", "envio el pago",
                "mi pago", "hice el pago", "hice la transferencia", "adjunto"))
            return Reply("Perfecto, envíame la captura o foto de tu comprobante por aquí y validamos tu pedido enseguida.");

        if (Phrase(text, "delivery", "hacen envios", "hacen envio", "envian", "reparto",
                "a domicilio", "cuanto demora", "cuanto tarda", "tiempo de entrega", "cuando llega",
                "costo de envio", "cobran envio", "tienen delivery"))
            return Reply("Sí, hacemos delivery. El tiempo y costo dependen de tu zona. " +
                         "Indícame tu dirección y con gusto te confirmo el envío.");

        if (Phrase(text, "horario", "que hora atienden", "a que hora abren", "a que hora cierran",
                "hasta que hora", "estan abiertos", "estan atendiendo", "atienden hoy", "abren hoy",
                "dias atienden", "que dias"))
            return Reply("Atendemos todos los días de 9:00 a 21:00. ¿En qué te puedo ayudar?");

        if (Phrase(text, "donde estan", "donde quedan", "ubicacion", "su direccion", "tienen local",
                "tienda fisica", "como llego", "donde los encuentro", "donde se ubican"))
            return Reply("Trabajamos con entrega a domicilio. Pásame tu dirección y coordinamos la entrega de tu pedido.");

        if (Phrase(text, "mi pedido", "mi orden", "estado de mi", "donde esta mi pedido",
                "ya salio mi", "cuando llega mi", "seguimiento", "rastrear"))
            return Reply("Déjame revisar el estado de tu pedido. ¿Me confirmas el número de pedido o el nombre con el que lo hiciste?");

        if (Phrase(text, "asesor", "hablar con alguien", "hablar con una persona", "una persona",
                "un humano", "atencion al cliente", "necesito ayuda", "soporte"))
            return Reply($"Claro, con gusto te ayudo{name}. Cuéntame qué necesitas y, si lo prefieres, derivo tu caso a un asesor.");

        if (Phrase(text, "reclamo", "queja", "no llego", "esta mal", "llego mal",
                "llego incompleto", "problema con", "no funciona", "quiero devolver", "devolucion",
                "esta dañado", "esta vencido"))
            return Reply($"Lamento el inconveniente{name}. Cuéntame qué ocurrió con tu pedido y lo resolvemos lo antes posible.");

        if (Phrase(text, "eres un bot", "eres bot", "eres real", "eres una persona", "eres humano",
                "robot", "con quien hablo", "eres una maquina"))
            return Reply($"Soy el asistente virtual de la tienda{name}. Puedo ayudarte con el catálogo, precios y pedidos. ¿Qué necesitas?");

        if (Phrase(text, "precio", "precios", "cuanto cuesta", "cuanto sale", "cuanto vale",
                "costo", "que precio"))
            return Reply("Con gusto te comparto los precios. ¿Qué producto te interesa?");

        if (Phrase(text, "stock", "disponible", "disponibilidad", "hay", "queda", "quedan", "consigo"))
            return Reply("Déjame revisar la disponibilidad. ¿De qué producto y qué cantidad necesitas?");

        if (Phrase(text, "catalogo", "productos", "que venden", "que vendes", "menu", "lista de precios",
                "que ofrecen", "que tienen para", "muestrame"))
            return Reply("Te puedo mostrar el catálogo. Dime qué categoría buscas y te paso las opciones.");

        if (Phrase(text, "pedido", "quiero", "quisiera", "comprar", "ordenar", "pedir", "necesito",
                "llevar", "me gustaria", "adquirir"))
            return Reply("Perfecto, registro tu pedido. Indícame los productos, cantidades y tu dirección de entrega.");

        if (Phrase(text, "gracias", "muchas gracias", "mil gracias", "te agradezco"))
            return Reply($"¡A ti{name}! Quedo atento para cualquier otro pedido.");

        if (Phrase(text, "adios", "hasta luego", "nos vemos", "chau", "hasta pronto", "bye"))
            return Reply($"¡Gracias por escribirnos{name}! Que tengas un excelente día. Aquí estaré para tu próximo pedido.");

        if (Phrase(text, "hola", "buenas", "buenos dias", "buenas tardes", "buenas noches", "hey", "saludos", "alo"))
            return Reply($"Hola{name}, bienvenido a la tienda. ¿Qué te gustaría pedir hoy?");

        if (HasWord(words, "si", "claro", "dale", "ok", "okay", "listo", "perfecto", "correcto", "afirmativo"))
            return Reply("¡Genial! Cuéntame qué producto y cantidad deseas y armamos tu pedido.");

        if (HasWord(words, "no", "nada", "ninguno", "ninguna"))
            return Reply($"Entendido{name}. Si necesitas algo más, aquí estoy para ayudarte con tu pedido.");

        return Reply($"Gracias por tu mensaje{name}. ¿Deseas hacer un pedido o conocer nuestros productos? Dime en qué te puedo ayudar.");
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
        => new(text.Split(new[] { ' ', '.', ',', '!', '?', ';', ':' }, StringSplitOptions.RemoveEmptyEntries));

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
