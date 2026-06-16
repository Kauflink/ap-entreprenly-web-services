using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyInventoryConfiguration(this ModelBuilder builder)
    {
        // Unit Product
        builder.Entity<UnitProduct>().HasKey(p => p.Id);
        builder.Entity<UnitProduct>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<UnitProduct>().Ignore(p => p.ProductType);
        builder.Entity<UnitProduct>().Property(p => p.OwnerEmail).IsRequired().HasMaxLength(120);
        builder.Entity<UnitProduct>().HasIndex(p => p.OwnerEmail);
        builder.Entity<UnitProduct>().Property(p => p.Name).IsRequired().HasMaxLength(160);
        builder.Entity<UnitProduct>().Property(p => p.Description).HasMaxLength(500);
        builder.Entity<UnitProduct>().Property(p => p.CodeQr).HasMaxLength(120);
        builder.Entity<UnitProduct>().Property(p => p.Price).IsRequired();
        builder.Entity<UnitProduct>().Property(p => p.WeightGrams).IsRequired();
        builder.Entity<UnitProduct>().Property(p => p.Brand).HasMaxLength(120);

        // Weight Product
        builder.Entity<WeightProduct>().HasKey(p => p.Id);
        builder.Entity<WeightProduct>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<WeightProduct>().Ignore(p => p.ProductType);
        builder.Entity<WeightProduct>().Property(p => p.OwnerEmail).IsRequired().HasMaxLength(120);
        builder.Entity<WeightProduct>().HasIndex(p => p.OwnerEmail);
        builder.Entity<WeightProduct>().Property(p => p.Name).IsRequired().HasMaxLength(160);
        builder.Entity<WeightProduct>().Property(p => p.Description).HasMaxLength(500);
        builder.Entity<WeightProduct>().Property(p => p.CodeQr).HasMaxLength(120);
        builder.Entity<WeightProduct>().Property(p => p.PricePerKg).IsRequired();

        // Unit Lot
        builder.Entity<UnitLot>().HasKey(l => l.Id);
        builder.Entity<UnitLot>().Property(l => l.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<UnitLot>().Ignore(l => l.ProductType);
        builder.Entity<UnitLot>().Property(l => l.OwnerEmail).IsRequired().HasMaxLength(120);
        builder.Entity<UnitLot>().HasIndex(l => l.OwnerEmail);
        builder.Entity<UnitLot>().Property(l => l.ProductId).IsRequired();
        builder.Entity<UnitLot>().Property(l => l.CodeQr).HasMaxLength(120);
        builder.Entity<UnitLot>().Property(l => l.EntryDate).IsRequired();
        builder.Entity<UnitLot>().Property(l => l.Quantity).IsRequired();
        builder.Entity<UnitLot>().Property(l => l.ExpiryDate);

        // Weight Lot
        builder.Entity<WeightLot>().HasKey(l => l.Id);
        builder.Entity<WeightLot>().Property(l => l.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<WeightLot>().Ignore(l => l.ProductType);
        builder.Entity<WeightLot>().Property(l => l.OwnerEmail).IsRequired().HasMaxLength(120);
        builder.Entity<WeightLot>().HasIndex(l => l.OwnerEmail);
        builder.Entity<WeightLot>().Property(l => l.ProductId).IsRequired();
        builder.Entity<WeightLot>().Property(l => l.CodeQr).HasMaxLength(120);
        builder.Entity<WeightLot>().Property(l => l.EntryDate).IsRequired();
        builder.Entity<WeightLot>().Property(l => l.QuantityKg).IsRequired();
    }
}
