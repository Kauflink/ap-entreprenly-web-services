namespace Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

public record CatalogProduct(int Id, string Name, decimal Price, bool SoldByWeight, decimal Stock);
