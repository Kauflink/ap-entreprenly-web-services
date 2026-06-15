namespace Entreprenly.WebServices.Sales.Interfaces.Acl;

/// <summary>
///     Anti-corruption layer exposing Sales bounded context capabilities to other contexts.
/// </summary>
/// <remarks>
///     Provides a simplified integration surface for registering a sale and updating the daily cash
///     register, without leaking the Sales internal model.
/// </remarks>
public interface ISalesContextFacade
{
    /// <summary>
    ///     Registers a completed sale for the given seller and updates the seller's daily cash
    ///     register with the sale total.
    /// </summary>
    /// <param name="ownerEmail">The seller's account email that owns the sale.</param>
    /// <param name="sellerId">The seller identifier.</param>
    /// <param name="lines">The sale lines.</param>
    /// <param name="total">The sale total.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns><c>true</c> when the sale was registered, <c>false</c> otherwise.</returns>
    Task<bool> RegisterChatSale(string ownerEmail, long sellerId, List<ChatSaleLine> lines, double total,
        CancellationToken cancellationToken);
}
