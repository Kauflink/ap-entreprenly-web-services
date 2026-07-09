using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;

/// <summary>
///     A product available to sell at the point of sale, with its currently available stock already
///     computed. Exposes everything the point-of-sale view needs in a single request, without
///     splitting products and lots.
/// </summary>
/// <param name="Name">The product display name.</param>
/// <param name="Price">The unit price for unit products, or the price per kilogram for weight products.</param>
/// <param name="ByWeight">Whether the product is sold by weight (kilograms) instead of by unit.</param>
/// <param name="Stock">The currently available stock (units for unit products, kilograms for weight products).</param>
[SwaggerSchema("A product available to sell, with its computed stock")]
public record SalesProductResource(
    string Name,
    double Price,
    bool ByWeight,
    double Stock);
