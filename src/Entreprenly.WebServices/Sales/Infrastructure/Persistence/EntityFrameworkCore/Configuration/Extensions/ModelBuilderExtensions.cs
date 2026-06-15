using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Sales.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplySalesConfiguration(this ModelBuilder builder)
    {
        // Sale aggregate
        builder.Entity<Sale>().ToTable("sales");
        builder.Entity<Sale>().HasKey(s => s.Id);
        builder.Entity<Sale>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Sale>().Property(s => s.OwnerEmail).IsRequired().HasMaxLength(120);
        builder.Entity<Sale>().Property(s => s.SellerId).IsRequired();
        builder.Entity<Sale>().Property(s => s.Total).IsRequired();
        builder.Entity<Sale>().Property(s => s.PaymentMethod).HasConversion<string>().IsRequired().HasMaxLength(20);
        builder.Entity<Sale>().Property(s => s.Status).HasConversion<string>().IsRequired().HasMaxLength(30);
        builder.Entity<Sale>().Property(s => s.CreatedAt).IsRequired();
        builder.Entity<Sale>().HasIndex(s => s.OwnerEmail);

        // Sale line items (owned collection, persisted in its own table)
        builder.Entity<Sale>().OwnsMany(s => s.Items, items =>
        {
            items.ToTable("sale_items");
            items.WithOwner().HasForeignKey("SaleId");
            items.Property<int>("Id");
            items.HasKey("Id");
            items.Property(i => i.ProductId).HasColumnName("product_id").IsRequired();
            items.Property(i => i.ProductName).HasColumnName("product_name").IsRequired().HasMaxLength(160);
            items.Property(i => i.Quantity).HasColumnName("quantity");
            items.Property(i => i.WeightKg).HasColumnName("weight_kg");
            items.Property(i => i.UnitPrice).HasColumnName("unit_price").IsRequired();
            items.Property(i => i.Subtotal).HasColumnName("subtotal").IsRequired();
        });

        // Proof of payment (optional owned value object, persisted in its own 1:1 table so a sale
        // without a confirmed payment simply has no row).
        builder.Entity<Sale>().OwnsOne(s => s.PaymentReceipt, receipt =>
        {
            receipt.ToTable("sale_payment_receipts");
            receipt.WithOwner().HasForeignKey("SaleId");
            receipt.Property(r => r.Method).HasColumnName("method").HasConversion<string>().HasMaxLength(20);
            receipt.Property(r => r.TransactionCode).HasColumnName("transaction_code").HasMaxLength(120);
            receipt.Property(r => r.Amount).HasColumnName("amount");
            receipt.Property(r => r.ConfirmedAt).HasColumnName("confirmed_at");
        });
        builder.Entity<Sale>().Navigation(s => s.PaymentReceipt).IsRequired(false);

        // CashRegister aggregate
        builder.Entity<CashRegister>().ToTable("cash_registers");
        builder.Entity<CashRegister>().HasKey(c => c.Id);
        builder.Entity<CashRegister>().Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<CashRegister>().Property(c => c.OwnerEmail).IsRequired().HasMaxLength(120);
        builder.Entity<CashRegister>().Property(c => c.Date).IsRequired();
        builder.Entity<CashRegister>().Property(c => c.TotalCash).IsRequired();
        builder.Entity<CashRegister>().Property(c => c.TotalDigital).IsRequired();
        builder.Entity<CashRegister>().Property(c => c.SaleCount).IsRequired();
        builder.Entity<CashRegister>().Ignore(c => c.TotalDay);
        // One cash register per account per business day.
        builder.Entity<CashRegister>().HasIndex(c => new { c.OwnerEmail, c.Date }).IsUnique();
    }
}
