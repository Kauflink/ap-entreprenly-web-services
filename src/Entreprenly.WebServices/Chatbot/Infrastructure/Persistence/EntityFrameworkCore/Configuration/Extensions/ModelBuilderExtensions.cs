using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Chatbot.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyChatbotConfiguration(this ModelBuilder builder)
    {
        // Conversation
        builder.Entity<Conversation>().HasKey(c => c.Id);
        builder.Entity<Conversation>().Property(c => c.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Conversation>().Property(c => c.SellerId).IsRequired();
        builder.Entity<Conversation>().Property(c => c.ClientPhone).IsRequired().HasMaxLength(20);
        builder.Entity<Conversation>().Property(c => c.ClientName).IsRequired().HasMaxLength(100);
        builder.Entity<Conversation>().Property(c => c.Status).HasConversion<string>().IsRequired();
        builder.Entity<Conversation>().Property(c => c.StartedAt).IsRequired();
        builder.Entity<Conversation>().Property(c => c.ClosedAt);

        // ChatMessage
        builder.Entity<ChatMessage>().HasKey(m => m.Id);
        builder.Entity<ChatMessage>().Property(m => m.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<ChatMessage>().Property(m => m.ConversationId).IsRequired();
        builder.Entity<ChatMessage>().Property(m => m.Content).IsRequired().HasMaxLength(4000);
        builder.Entity<ChatMessage>().Property(m => m.Sender).HasConversion<string>().IsRequired();
        builder.Entity<ChatMessage>().Property(m => m.Type).HasConversion<string>().IsRequired();
        builder.Entity<ChatMessage>().Property(m => m.SentAt).IsRequired();

        // ChatOrder
        builder.Entity<ChatOrder>().HasKey(o => o.Id);
        builder.Entity<ChatOrder>().Property(o => o.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<ChatOrder>().Property(o => o.ConversationId).IsRequired();
        builder.Entity<ChatOrder>().Property(o => o.SellerId).IsRequired();
        builder.Entity<ChatOrder>().Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
        builder.Entity<ChatOrder>().Property(o => o.ClientPhone).IsRequired().HasMaxLength(20);
        builder.Entity<ChatOrder>().Property(o => o.DeliveryAddress).IsRequired().HasMaxLength(300);
        builder.Entity<ChatOrder>().Property(o => o.ItemsJson).IsRequired().HasColumnType("longtext");
        builder.Entity<ChatOrder>().Property(o => o.Total).IsRequired().HasColumnType("decimal(10,2)");
        builder.Entity<ChatOrder>().Property(o => o.Status).HasConversion<string>().IsRequired();
        builder.Entity<ChatOrder>().Property(o => o.HasReceipt).IsRequired();
        builder.Entity<ChatOrder>().Property(o => o.ReceiptImageUrl).HasColumnType("longtext");
        builder.Entity<ChatOrder>().Property(o => o.RejectionCount).IsRequired();
        builder.Entity<ChatOrder>().Property(o => o.CreatedAt).IsRequired();
        builder.Entity<ChatOrder>().Ignore(o => o.Items);

        // WhatsappSession
        builder.Entity<WhatsappSession>().HasKey(s => s.Id);
        builder.Entity<WhatsappSession>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<WhatsappSession>().Property(s => s.SellerId).IsRequired();
        builder.Entity<WhatsappSession>().Property(s => s.OwnerEmail).IsRequired().HasMaxLength(200);
        builder.Entity<WhatsappSession>().HasIndex(s => s.OwnerEmail).IsUnique();
        builder.Entity<WhatsappSession>().Property(s => s.BusinessName).IsRequired().HasMaxLength(200);
        builder.Entity<WhatsappSession>().Property(s => s.Status).HasConversion<string>().IsRequired();
        builder.Entity<WhatsappSession>().Property(s => s.Phone).HasMaxLength(20);
        builder.Entity<WhatsappSession>().Property(s => s.ConnectedAt);
    }
}
