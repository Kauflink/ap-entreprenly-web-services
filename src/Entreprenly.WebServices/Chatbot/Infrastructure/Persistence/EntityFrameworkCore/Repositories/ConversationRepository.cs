using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Chatbot.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class ConversationRepository(AppDbContext context)
    : BaseRepository<Conversation>(context), IConversationRepository
{
    public async Task<Conversation?> FindByClientPhoneAndSellerIdAsync(string clientPhone, int sellerId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<Conversation>()
            .Where(c => c.ClientPhone == clientPhone && c.SellerId == sellerId)
            .OrderByDescending(c => c.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Conversation>> FindAllBySellerIdAsync(int sellerId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<Conversation>()
            .Where(c => c.SellerId == sellerId)
            .OrderByDescending(c => c.StartedAt)
            .ToListAsync(cancellationToken);
    }
}
