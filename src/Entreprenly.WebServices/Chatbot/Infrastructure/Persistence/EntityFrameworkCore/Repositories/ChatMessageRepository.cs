using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Chatbot.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class ChatMessageRepository(AppDbContext context)
    : BaseRepository<ChatMessage>(context), IChatMessageRepository
{
    public async Task<IEnumerable<ChatMessage>> FindAllByConversationIdAsync(int conversationId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<ChatMessage>()
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.SentAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ChatMessage?> FindLastByConversationIdAsync(int conversationId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<ChatMessage>()
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.SentAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<ChatMessage>> FindAllBySellerIdAsync(int sellerId,
        CancellationToken cancellationToken)
    {
        var conversationIds = Context.Set<Conversation>()
            .Where(c => c.SellerId == sellerId)
            .Select(c => c.Id);

        return await Context.Set<ChatMessage>()
            .Where(m => conversationIds.Contains(m.ConversationId))
            .OrderBy(m => m.SentAt)
            .ToListAsync(cancellationToken);
    }
}
