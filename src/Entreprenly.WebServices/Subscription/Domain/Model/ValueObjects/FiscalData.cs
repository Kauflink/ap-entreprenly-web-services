namespace Entreprenly.WebServices.Subscription.Domain.Model.ValueObjects;

public class FiscalData
{
    public FiscalData()
    {
        DocumentType = string.Empty;
        DocumentNumber = string.Empty;
        BusinessName = string.Empty;
        ReceiptEmail = string.Empty;
        FiscalAddress = string.Empty;
    }

    public FiscalData(string documentType, string documentNumber, string businessName, string receiptEmail,
        string fiscalAddress)
    {
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        BusinessName = businessName;
        ReceiptEmail = receiptEmail;
        FiscalAddress = fiscalAddress;
    }

    public string DocumentType { get; private set; }
    public string DocumentNumber { get; private set; }
    public string BusinessName { get; private set; }
    public string ReceiptEmail { get; private set; }
    public string FiscalAddress { get; private set; }
}
