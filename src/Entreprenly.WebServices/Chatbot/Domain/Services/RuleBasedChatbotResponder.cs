using System.Globalization;
using System.Text;

namespace Entreprenly.WebServices.Chatbot.Domain.Services;

public class RuleBasedChatbotResponder : IChatbotResponder
{
    private static readonly (string[] Keywords, string Reply)[] Rules =
    [
        (["pago", "yape", "plin", "transferencia", "efectivo", "billetera", "cómo pago", "como pago", "métodos de pago"],
            "💳 Aceptamos:\n• Yape\n• Plin\n• Transferencia bancaria\n• Efectivo contra entrega\n\nCuando tengas tu pedido listo, envíame la captura de tu comprobante. 📸"),

        (["comprobante", "foto", "pago enviado", "te mandé", "te mande", "ya pagué", "ya pague", "te envié el pago"],
            "✅ ¡Perfecto! Envíame la captura de tu comprobante para validar el pago."),

        (["catálogo", "catalogo", "productos", "qué vendes", "que vendes", "qué tienes", "que tienes", "menú", "menu", "lista"],
            "📋 Con gusto te muestro nuestro catálogo. ¿Qué tipo de producto estás buscando?"),

        (["precio", "precios", "cuánto cuesta", "cuanto cuesta", "cuánto vale", "cuanto vale", "a cuánto", "a cuanto", "costo"],
            "💰 Con gusto te doy los precios. ¿Qué producto te interesa?"),

        (["horario", "hora", "atienden", "abierto", "disponible", "cuándo atienden", "cuando atienden"],
            "⏰ Atendemos todos los días de 9:00 am a 9:00 pm. ¡Estamos aquí para ayudarte!"),

        (["delivery", "envío", "envio", "despacho", "mandan", "hacen entrega", "reparto", "traen"],
            "🚚 Sí, hacemos delivery. El costo varía según tu zona. ¿A qué dirección lo necesitas?"),

        (["dirección", "direccion", "dónde están", "donde estan", "ubicación", "ubicacion", "local", "tienda", "sucursal"],
            "📍 Trabajamos con entrega a domicilio. Dime tu dirección y coordinamos la entrega. 🏠"),

        (["pedido", "estado de mi pedido", "mi pedido", "orden", "seguimiento", "tracking"],
            "📦 Déjame revisar el estado de tu pedido. ¿Me das tu número de pedido o el teléfono con el que hiciste el pedido?"),

        (["asesor", "persona", "humano", "hablar con alguien", "atención al cliente", "soporte"],
            "👤 Con gusto te transfiero con un asesor. Por favor espera un momento."),

        (["queja", "reclamo", "problema", "mal", "error", "equivocado", "incorrecto"],
            "😔 Lamento mucho el inconveniente. Por favor cuéntame qué ocurrió para ayudarte de la mejor manera."),

        (["quién eres", "quien eres", "eres bot", "eres robot", "eres humano", "eres una ia", "eres un bot"],
            "🤖 Soy el asistente virtual. Estoy aquí para ayudarte con pedidos, precios y consultas. ¿En qué te puedo ayudar?"),

        (["gracias", "muchas gracias", "thank you", "thanks", "ok gracias", "perfecto gracias"],
            "😊 ¡De nada! Ha sido un placer atenderte. Si necesitas algo más, aquí estaré."),

        (["hola", "hi", "buenas", "buenos días", "buenos dias", "buenas tardes", "buenas noches", "buen día", "buen dia", "saludos"],
            "¡Hola! 👋 Bienvenido. Soy el asistente virtual. ¿En qué te puedo ayudar hoy?\n\nPuedes preguntarme por:\n• 📋 Catálogo de productos\n• 💰 Precios\n• 🚚 Delivery\n• 💳 Métodos de pago"),

        (["adiós", "adios", "chao", "hasta luego", "bye", "nos vemos"],
            "👋 ¡Hasta luego! Fue un gusto atenderte. Vuelve cuando quieras. 😊"),
    ];

    public Task<string?> GenerateReplyAsync(string incomingMessage, int sellerId, CancellationToken cancellationToken)
    {
        var normalized = Normalize(incomingMessage);

        foreach (var (keywords, reply) in Rules)
            if (keywords.Any(k => normalized.Contains(Normalize(k), StringComparison.Ordinal)))
                return Task.FromResult<string?>(reply);

        return Task.FromResult<string?>(
            "🤖 Recibí tu mensaje. ¿Te puedo ayudar con nuestro catálogo, precios, delivery o métodos de pago? Escribe lo que necesitas.");
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
