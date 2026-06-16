namespace Entreprenly.WebServices.Subscription.Interfaces.Rest.Resources;

public record BillingSetupResource(
    string PaymentMethodTitle,
    string PaymentMethodDescription,
    string PaymentMethodActionLabel,
    string FiscalDataTitle,
    string FiscalDataDescription,
    string FiscalDataActionLabel,
    bool HasPaymentMethod,
    bool HasFiscalData,
    IEnumerable<PaymentMethodResource> PaymentMethods,
    FiscalDataResource? FiscalData);
