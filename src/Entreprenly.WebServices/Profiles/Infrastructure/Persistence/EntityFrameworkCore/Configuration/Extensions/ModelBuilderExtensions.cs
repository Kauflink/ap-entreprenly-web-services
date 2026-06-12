using Entreprenly.WebServices.Profiles.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyProfilesConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Profile>().HasKey(p => p.Id);
        builder.Entity<Profile>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Profile>().Property(p => p.UserId).IsRequired();
        builder.Entity<Profile>().HasIndex(p => p.UserId).IsUnique();
        builder.Entity<Profile>().Property(p => p.FirstName).IsRequired();
        builder.Entity<Profile>().Property(p => p.LastName).IsRequired();
        builder.Entity<Profile>().Property(p => p.Role).IsRequired();
        builder.Entity<Profile>().Property(p => p.Plan).IsRequired();

        builder.Entity<Profile>().OwnsOne(p => p.Preferences, preferences =>
        {
            preferences.Property(x => x.Language).HasColumnName("preferences_language");
            preferences.Property(x => x.Timezone).HasColumnName("preferences_timezone");
            preferences.Property(x => x.Theme).HasColumnName("preferences_theme");
            preferences.Property(x => x.Currency).HasColumnName("preferences_currency");
        });

        builder.Entity<Profile>().OwnsOne(p => p.NotificationSettings, notifications =>
        {
            notifications.Property(x => x.StockAlerts).HasColumnName("notifications_stock_alerts");
            notifications.Property(x => x.PaymentAlerts).HasColumnName("notifications_payment_alerts");
            notifications.Property(x => x.ChatbotMessages).HasColumnName("notifications_chatbot_messages");
        });
    }
}
