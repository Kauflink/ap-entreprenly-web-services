namespace Entreprenly.WebServices.Subscription.Domain.Model.ValueObjects;

public class BillingSetup
{
    private readonly List<PaymentMethod> _paymentMethods = [];

    public BillingSetup()
    {
        PaymentMethodTitle = string.Empty;
        PaymentMethodDescription = string.Empty;
        PaymentMethodActionLabel = string.Empty;
        FiscalDataTitle = string.Empty;
        FiscalDataDescription = string.Empty;
        FiscalDataActionLabel = string.Empty;
    }

    public BillingSetup(string paymentMethodTitle, string paymentMethodDescription, string paymentMethodActionLabel,
        string fiscalDataTitle, string fiscalDataDescription, string fiscalDataActionLabel, bool hasPaymentMethod,
        bool hasFiscalData, IEnumerable<PaymentMethod> paymentMethods, FiscalData? fiscalData)
    {
        PaymentMethodTitle = paymentMethodTitle;
        PaymentMethodDescription = paymentMethodDescription;
        PaymentMethodActionLabel = paymentMethodActionLabel;
        FiscalDataTitle = fiscalDataTitle;
        FiscalDataDescription = fiscalDataDescription;
        FiscalDataActionLabel = fiscalDataActionLabel;
        HasPaymentMethod = hasPaymentMethod;
        HasFiscalData = hasFiscalData;
        _paymentMethods = paymentMethods.ToList();
        FiscalData = fiscalData;
    }

    public string PaymentMethodTitle { get; private set; }
    public string PaymentMethodDescription { get; private set; }
    public string PaymentMethodActionLabel { get; private set; }
    public string FiscalDataTitle { get; private set; }
    public string FiscalDataDescription { get; private set; }
    public string FiscalDataActionLabel { get; private set; }
    public bool HasPaymentMethod { get; private set; }
    public bool HasFiscalData { get; private set; }
    public IReadOnlyCollection<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();
    public FiscalData? FiscalData { get; private set; }

    public static BillingSetup Empty()
    {
        return new BillingSetup(
            "Metodo de pago",
            "Aun no hay tarjeta o medio de pago registrado.",
            "Agregar metodos de pago",
            "Datos de facturacion",
            "Completa RUC, razon social y correo de comprobantes.",
            "Completar datos",
            false,
            false,
            [],
            null);
    }
}
