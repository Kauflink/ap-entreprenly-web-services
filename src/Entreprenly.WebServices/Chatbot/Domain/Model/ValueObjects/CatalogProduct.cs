namespace Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

public record CatalogProduct(int Id, string Name, decimal Price, bool SoldByWeight, decimal Stock)
{
    public bool IsInStock => Stock > 0;
}
