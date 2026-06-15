using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Chatbot.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class WhatsappSessionRepository(AppDbContext context)
    : BaseRepository<WhatsappSession>(context), IWhatsappSessionRepository
{
    public async Task<WhatsappSession?> FindByOwnerEmailAsync(string ownerEmail, CancellationToken cancellationToken)
    {
        return await Context.Set<WhatsappSession>()
            .FirstOrDefaultAsync(s => s.OwnerEmail == ownerEmail, cancellationToken);
    }

    public async Task<WhatsappSession?> FindBySellerIdAsync(int sellerId, CancellationToken cancellationToken)
    {
        return await Context.Set<WhatsappSession>()
            .FirstOrDefaultAsync(s => s.SellerId == sellerId, cancellationToken);
    }

    public async Task<IEnumerable<WhatsappSession>> FindAllBySellerIdAsync(int sellerId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<WhatsappSession>()
            .Where(s => s.SellerId == sellerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByOwnerEmailAsync(string ownerEmail, CancellationToken cancellationToken)
    {
        return await Context.Set<WhatsappSession>()
            .AnyAsync(s => s.OwnerEmail == ownerEmail, cancellationToken);
    }
}
