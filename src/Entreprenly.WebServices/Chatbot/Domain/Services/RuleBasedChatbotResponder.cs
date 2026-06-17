using System.Globalization;
using System.Text;

namespace Entreprenly.WebServices.Chatbot.Domain.Services;

public class RuleBasedChatbotResponder : IChatbotResponder
{
    private static readonly (string[] Keywords, string Reply)[] Rules =
    [
        (["pago", "yape", "plin", "transferencia", "efectivo", "billetera", "como pago", "metodos de pago", "metodo de pago"],
            "Aceptamos Yape, Plin, transferencia bancaria y efectivo contra entrega.\nCuando tengas tu pedido listo, enviame la captura de tu comprobante."),

        (["comprobante", "foto", "pago enviado", "te mande", "ya pague", "te envie el pago"],
            "Perfecto, enviame la captura de tu comprobante para validar el pago."),

        (["catalogo", "productos", "que vendes", "que tienes", "menu", "lista"],
            "Con gusto te muestro nuestro catalogo. Que tipo de producto estas buscando?"),

        (["precio", "precios", "cuanto cuesta", "cuanto vale", "a cuanto", "costo"],
            "Con gusto te doy los precios. Que producto te interesa?"),

        (["horario", "hora", "atienden", "abierto", "disponible", "cuando atienden"],
            "Atendemos todos los dias de 9:00 am a 9:00 pm."),

        (["delivery", "envio", "despacho", "mandan", "hacen entrega", "reparto", "traen"],
            "Si, hacemos delivery. El costo varia segun tu zona. A que direccion lo necesitas?"),

        (["direccion", "donde estan", "ubicacion", "local", "tienda", "sucursal"],
            "Trabajamos con entrega a domicilio. Dime tu direccion y coordinamos la entrega."),

        (["pedido", "estado de mi pedido", "mi pedido", "orden", "seguimiento"],
            "Dejame revisar el estado de tu pedido. Me das tu numero de pedido?"),

        (["asesor", "persona", "humano", "hablar con alguien", "atencion al cliente", "soporte"],
            "Con gusto te transfiero con un asesor. Por favor espera un momento."),

        (["queja", "reclamo", "problema", "mal", "error", "equivocado", "incorrecto"],
            "Lamento el inconveniente. Cuentame que ocurrio para ayudarte de la mejor manera."),

        (["quien eres", "eres bot", "eres robot", "eres humano", "eres una ia", "eres un bot"],
            "Soy el asistente virtual. Estoy aqui para ayudarte con pedidos, precios y consultas."),

        (["gracias", "muchas gracias", "ok gracias", "perfecto gracias"],
            "De nada. Fue un placer atenderte. Si necesitas algo mas, aqui estare."),

        (["hola", "hi", "buenas", "buenos dias", "buenas tardes", "buenas noches", "buen dia", "saludos"],
            "Hola, bienvenido. Soy el asistente virtual. En que te puedo ayudar?\n\nPuedes preguntarme por:\n- Catalogo de productos\n- Precios\n- Delivery\n- Metodos de pago"),

        (["adios", "chao", "hasta luego", "bye", "nos vemos"],
            "Hasta luego. Fue un gusto atenderte. Vuelve cuando quieras."),
    ];

    public Task<string?> GenerateReplyAsync(string incomingMessage, int sellerId, CancellationToken cancellationToken)
    {
        var normalized = Normalize(incomingMessage);

        foreach (var (keywords, reply) in Rules)
            if (keywords.Any(k => normalized.Contains(Normalize(k), StringComparison.Ordinal)))
                return Task.FromResult<string?>(reply);

        return Task.FromResult<string?>(
            "Recibi tu mensaje. Te puedo ayudar con nuestro catalogo, precios, delivery o metodos de pago. Escribe lo que necesitas.");
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
