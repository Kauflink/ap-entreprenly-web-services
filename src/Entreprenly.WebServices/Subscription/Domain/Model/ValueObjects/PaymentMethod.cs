namespace Entreprenly.WebServices.Subscription.Domain.Model.ValueObjects;

public class PaymentMethod
{
    public PaymentMethod()
    {
        PaymentMethodId = string.Empty;
        CardBrand = string.Empty;
        LastFour = string.Empty;
        HolderName = string.Empty;
        ExpiryMonth = string.Empty;
        ExpiryYear = string.Empty;
    }

    public PaymentMethod(string paymentMethodId, string cardBrand, string lastFour, string holderName,
        string expiryMonth, string expiryYear, bool isDefault)
    {
        PaymentMethodId = paymentMethodId;
        CardBrand = cardBrand;
        LastFour = lastFour;
        HolderName = holderName;
        ExpiryMonth = expiryMonth;
        ExpiryYear = expiryYear;
        IsDefault = isDefault;
    }

    public string PaymentMethodId { get; private set; }
    public string CardBrand { get; private set; }
    public string LastFour { get; private set; }
    public string HolderName { get; private set; }
    public string ExpiryMonth { get; private set; }
    public string ExpiryYear { get; private set; }
    public bool IsDefault { get; private set; }
}
