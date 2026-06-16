namespace Entreprenly.WebServices.Subscription.Interfaces.Rest.Resources;

public record FiscalDataResource(
    string DocumentType,
    string DocumentNumber,
    string BusinessName,
    string ReceiptEmail,
    string FiscalAddress);
