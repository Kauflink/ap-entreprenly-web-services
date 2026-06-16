using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Chatbot.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class ChatOrderRepository(AppDbContext context)
    : BaseRepository<ChatOrder>(context), IChatOrderRepository
{
    public async Task<ChatOrder?> FindByConversationIdAsync(int conversationId, CancellationToken cancellationToken)
    {
        return await Context.Set<ChatOrder>()
            .FirstOrDefaultAsync(o => o.ConversationId == conversationId, cancellationToken);
    }

    public async Task<IEnumerable<ChatOrder>> FindAllBySellerIdAsync(int sellerId, CancellationToken cancellationToken)
    {
        return await Context.Set<ChatOrder>()
            .Where(o => o.SellerId == sellerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
