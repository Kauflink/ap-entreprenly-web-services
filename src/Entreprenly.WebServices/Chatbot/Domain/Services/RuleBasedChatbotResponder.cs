namespace Entreprenly.WebServices.Chatbot.Domain.Services;

/// <summary>
///     Simple rule-based responder. Replies to common keywords in Spanish.
///     Extend this class or replace it with an AI-backed implementation without touching the domain interface.
/// </summary>
public class RuleBasedChatbotResponder : IChatbotResponder
{
    private static readonly Dictionary<string[], string> Rules = new(new StringArrayComparer())
    {
        { ["hola", "hi", "buenas", "buenos días", "buen día"], "¡Hola! 👋 Bienvenido. ¿En qué te puedo ayudar hoy?" },
        { ["catálogo", "productos", "que vendes", "qué vendes", "menu", "menú"], "📋 Puedo mostrarte nuestro catálogo. ¿Qué tipo de producto buscas?" },
        { ["precio", "precios", "cuánto", "cuanto", "costo", "vale"], "💰 Los precios dependen del producto. ¿Cuál te interesa?" },
        { ["pedido", "pedir", "ordenar", "quiero", "comprar", "llevar"], "🛒 ¡Con gusto! ¿Qué producto deseas pedir y en qué cantidad?" },
        { ["pagar", "pago", "yape", "plin", "efectivo", "transferencia"], "💳 Aceptamos Yape, Plin y efectivo. Cuando estés listo, envía tu comprobante de pago." },
        { ["gracias", "thanks", "ok", "listo", "perfecto"], "¡De nada! 😊 Si necesitas algo más, aquí estaré." },
        { ["dirección", "donde", "dónde", "delivery", "envío"], "🚚 Hacemos delivery. ¿Cuál es tu dirección de entrega?" },
        { ["horario", "abierto", "atienden"], "⏰ Atendemos de lunes a sábado de 8:00 am a 8:00 pm." },
    };

    public Task<string?> GenerateReplyAsync(string incomingMessage, int sellerId, CancellationToken cancellationToken)
    {
        var lower = incomingMessage.ToLowerInvariant().Trim();

        foreach (var (keywords, reply) in Rules)
            if (keywords.Any(k => lower.Contains(k)))
                return Task.FromResult<string?>(reply);

        return Task.FromResult<string?>(
            "🤖 Recibí tu mensaje. Un momento, pronto te atenderemos. Si tienes una pregunta específica, escríbela y haré lo posible por ayudarte.");
    }

    private sealed class StringArrayComparer : IEqualityComparer<string[]>
    {
        public bool Equals(string[]? x, string[]? y) => ReferenceEquals(x, y);
        public int GetHashCode(string[] obj) => obj.GetHashCode();
    }
}
