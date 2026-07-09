using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Entreprenly.WebServices.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    // The MySql.EntityFrameworkCore provider materializes MySQL `date` columns as System.DateTime, which
    // cannot be cast to DateOnly and throws InvalidCastException on read. Map the period dates through
    // DateTime explicitly while keeping the underlying column type `date` (no schema change).
    private static readonly ValueConverter<DateOnly, DateTime> DateOnlyToDateTime = new(
        dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
        dateTime => DateOnly.FromDateTime(dateTime));

    public static void ApplySubscriptionConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Domain.Model.Aggregates.Subscription>().ToTable("subscriptions");
        builder.Entity<Domain.Model.Aggregates.Subscription>().HasKey(subscription => subscription.Id);
        builder.Entity<Domain.Model.Aggregates.Subscription>().Property(subscription => subscription.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<Domain.Model.Aggregates.Subscription>().Property(subscription => subscription.UserId).IsRequired();
        builder.Entity<Domain.Model.Aggregates.Subscription>().HasIndex(subscription => subscription.UserId).IsUnique();
        builder.Entity<Domain.Model.Aggregates.Subscription>().Property(subscription => subscription.DefaultBillingCycle)
            .IsRequired()
            .HasMaxLength(20);
        builder.Entity<Domain.Model.Aggregates.Subscription>().Property(subscription => subscription.CreatedAt);
        builder.Entity<Domain.Model.Aggregates.Subscription>().Property(subscription => subscription.UpdatedAt);

        builder.Entity<Domain.Model.Aggregates.Subscription>().OwnsOne(subscription => subscription.CurrentPlan, plan =>
        {
            plan.WithOwner().HasForeignKey("Id");
            plan.Property(value => value.PlanId).HasColumnName("current_plan_id").HasMaxLength(60);
            plan.Property(value => value.Name).HasColumnName("current_plan_name").HasMaxLength(80);
            plan.Property(value => value.ShortDescription).HasColumnName("current_plan_short_description")
                .HasColumnType("MEDIUMTEXT");
            plan.Property(value => value.MonthlyPrice).HasColumnName("current_plan_monthly_price")
                .HasPrecision(10, 2);
            plan.Property(value => value.AnnualPrice).HasColumnName("current_plan_annual_price")
                .HasPrecision(10, 2);
            plan.Property(value => value.Status).HasColumnName("current_plan_status").HasMaxLength(40);
            plan.Property(value => value.StatusLabel).HasColumnName("current_plan_status_label").HasMaxLength(80);
            plan.Property(value => value.BadgeLabel).HasColumnName("current_plan_badge_label").HasMaxLength(80);
            plan.Property(value => value.Recommended).HasColumnName("current_plan_recommended");
            plan.Property(value => value.CurrentPeriodStartDate).HasColumnName("current_period_start_date")
                .HasConversion(DateOnlyToDateTime).HasColumnType("date");
            plan.Property(value => value.CurrentPeriodEndDate).HasColumnName("current_period_end_date")
                .HasConversion(DateOnlyToDateTime).HasColumnType("date");
            plan.OwnsMany(value => value.Features, feature =>
            {
                feature.ToTable("subscription_current_plan_features");
                feature.WithOwner().HasForeignKey("SubscriptionId");
                feature.Property<int>("Id").ValueGeneratedOnAdd();
                feature.HasKey("Id");
                feature.Property(value => value.Description).IsRequired().HasMaxLength(240);
                feature.Property(value => value.Available).IsRequired();
            });
        });

        builder.Entity<Domain.Model.Aggregates.Subscription>().OwnsOne(subscription => subscription.RecommendedPlan,
            plan =>
            {
                plan.WithOwner().HasForeignKey("Id");
                plan.Property(value => value.PlanId).HasColumnName("recommended_plan_id").HasMaxLength(60);
                plan.Property(value => value.Name).HasColumnName("recommended_plan_name").HasMaxLength(80);
                plan.Property(value => value.ShortDescription).HasColumnName("recommended_plan_short_description")
                    .HasColumnType("MEDIUMTEXT");
                plan.Property(value => value.MonthlyPrice).HasColumnName("recommended_plan_monthly_price")
                    .HasPrecision(10, 2);
                plan.Property(value => value.AnnualPrice).HasColumnName("recommended_plan_annual_price")
                    .HasPrecision(10, 2);
                plan.Property(value => value.Status).HasColumnName("recommended_plan_status").HasMaxLength(40);
                plan.Property(value => value.StatusLabel).HasColumnName("recommended_plan_status_label").HasMaxLength(80);
                plan.Property(value => value.BadgeLabel).HasColumnName("recommended_plan_badge_label").HasMaxLength(80);
                plan.Property(value => value.Recommended).HasColumnName("recommended_plan_recommended");
                plan.Property(value => value.CurrentPeriodStartDate).HasColumnName("recommended_period_start_date")
                    .HasConversion(DateOnlyToDateTime).HasColumnType("date");
                plan.Property(value => value.CurrentPeriodEndDate).HasColumnName("recommended_period_end_date")
                    .HasConversion(DateOnlyToDateTime).HasColumnType("date");
                plan.OwnsMany(value => value.Features, feature =>
                {
                    feature.ToTable("subscription_recommended_plan_features");
                    feature.WithOwner().HasForeignKey("SubscriptionId");
                    feature.Property<int>("Id").ValueGeneratedOnAdd();
                    feature.HasKey("Id");
                    feature.Property(value => value.Description).IsRequired().HasMaxLength(240);
                    feature.Property(value => value.Available).IsRequired();
                });
            });

        builder.Entity<Domain.Model.Aggregates.Subscription>().OwnsMany(subscription => subscription.Limits, limit =>
        {
            limit.ToTable("subscription_limits");
            limit.WithOwner().HasForeignKey("SubscriptionId");
            limit.Property<int>("Id").ValueGeneratedOnAdd();
            limit.HasKey("Id");
            limit.Property(value => value.LimitId).IsRequired().HasMaxLength(60);
            limit.Property(value => value.Label).IsRequired().HasMaxLength(80);
            limit.Property(value => value.UsedValue).IsRequired();
            limit.Property(value => value.MaxValue).IsRequired();
        });

        builder.Entity<Domain.Model.Aggregates.Subscription>().OwnsMany(subscription => subscription.Activity,
            activity =>
            {
                activity.ToTable("subscription_activity");
                activity.WithOwner().HasForeignKey("SubscriptionId");
                activity.Property<int>("Id").ValueGeneratedOnAdd();
                activity.HasKey("Id");
                activity.Property(value => value.ActivityId).IsRequired().HasMaxLength(80);
                activity.Property(value => value.Title).IsRequired().HasMaxLength(120);
                activity.Property(value => value.Detail).IsRequired().HasColumnType("MEDIUMTEXT");
            });

        builder.Entity<Domain.Model.Aggregates.Subscription>().OwnsOne(subscription => subscription.BillingSetup,
            billing =>
            {
                billing.WithOwner().HasForeignKey("Id");
                billing.Property(value => value.PaymentMethodTitle).HasColumnName("payment_method_title")
                    .HasMaxLength(120);
                billing.Property(value => value.PaymentMethodDescription)
                    .HasColumnName("payment_method_description")
                    .HasColumnType("MEDIUMTEXT");
                billing.Property(value => value.PaymentMethodActionLabel)
                    .HasColumnName("payment_method_action_label")
                    .HasMaxLength(120);
                billing.Property(value => value.FiscalDataTitle).HasColumnName("fiscal_data_title").HasMaxLength(120);
                billing.Property(value => value.FiscalDataDescription).HasColumnName("fiscal_data_description")
                    .HasColumnType("MEDIUMTEXT");
                billing.Property(value => value.FiscalDataActionLabel).HasColumnName("fiscal_data_action_label")
                    .HasMaxLength(120);
                billing.Property(value => value.HasPaymentMethod).HasColumnName("has_payment_method");
                billing.Property(value => value.HasFiscalData).HasColumnName("has_fiscal_data");

                billing.OwnsMany(value => value.PaymentMethods, paymentMethod =>
                {
                    paymentMethod.ToTable("subscription_payment_methods");
                    paymentMethod.WithOwner().HasForeignKey("SubscriptionId");
                    paymentMethod.Property<int>("Id").ValueGeneratedOnAdd();
                    paymentMethod.HasKey("Id");
                    paymentMethod.Property(value => value.PaymentMethodId).IsRequired().HasMaxLength(80);
                    paymentMethod.Property(value => value.CardBrand).IsRequired().HasMaxLength(40);
                    paymentMethod.Property(value => value.LastFour).IsRequired().HasMaxLength(4);
                    paymentMethod.Property(value => value.HolderName).IsRequired().HasMaxLength(120);
                    paymentMethod.Property(value => value.ExpiryMonth).IsRequired().HasMaxLength(2);
                    paymentMethod.Property(value => value.ExpiryYear).IsRequired().HasMaxLength(2);
                    paymentMethod.Property(value => value.IsDefault).IsRequired();
                });

                billing.OwnsOne(value => value.FiscalData, fiscalData =>
                {
                    fiscalData.WithOwner().HasForeignKey("Id");
                    fiscalData.Property(value => value.DocumentType).HasColumnName("fiscal_document_type")
                        .HasMaxLength(20);
                    fiscalData.Property(value => value.DocumentNumber).HasColumnName("fiscal_document_number")
                        .HasMaxLength(30);
                    fiscalData.Property(value => value.BusinessName).HasColumnName("fiscal_business_name")
                        .HasMaxLength(160);
                    fiscalData.Property(value => value.ReceiptEmail).HasColumnName("fiscal_receipt_email")
                        .HasMaxLength(160);
                    fiscalData.Property(value => value.FiscalAddress).HasColumnName("fiscal_address")
                        .HasColumnType("MEDIUMTEXT");
                });
            });
    }
}
