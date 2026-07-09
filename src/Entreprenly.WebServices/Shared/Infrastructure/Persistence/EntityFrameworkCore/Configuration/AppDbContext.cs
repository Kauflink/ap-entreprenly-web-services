using Entreprenly.WebServices.Chatbot.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Entreprenly.WebServices.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Entreprenly.WebServices.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Entreprenly.WebServices.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Entreprenly.WebServices.Sales.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Interceptors;
using Entreprenly.WebServices.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Application database context.
/// </summary>
/// <param name="options">The options for the database context.</param>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Apply audit timestamp interceptor for all IAuditableEntity implementations
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Chatbot Context
        builder.ApplyChatbotConfiguration();

        // IAM Context
        builder.ApplyIamConfiguration();

        // Profiles Context
        builder.ApplyProfilesConfiguration();

        // Sales Context
        builder.ApplySalesConfiguration();

        // Inventory Context
        builder.ApplyInventoryConfiguration();

        // Subscription Context
        builder.ApplySubscriptionConfiguration();

        // General Naming Convention for the database objects
        builder.UseSnakeCaseNamingConvention();
    }
}
