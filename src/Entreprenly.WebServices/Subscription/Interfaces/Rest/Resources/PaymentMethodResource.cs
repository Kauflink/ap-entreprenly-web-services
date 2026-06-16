namespace Entreprenly.WebServices.Subscription.Interfaces.Rest.Resources;

public record PaymentMethodResource(
    string Id,
    string CardBrand,
    string LastFour,
    string HolderName,
    string ExpiryMonth,
    string ExpiryYear,
    bool IsDefault);
