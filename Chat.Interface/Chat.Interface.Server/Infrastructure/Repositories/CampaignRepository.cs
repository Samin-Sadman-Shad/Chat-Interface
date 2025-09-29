using Chat.Interface.Server.Domain.Entities;
using Chat.Interface.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chat.Interface.Server.Infrastructure.Repositories;

public interface ICampaignRepository
{
    Task<Campaign?> GetByIdAsync(Guid id);
    Task<List<Campaign>> GetAllAsync();
    Task AddAsync(Campaign campaign);
    Task UpdateAsync(Campaign campaign);
}

public class CampaignRepository : ICampaignRepository
{
    private readonly ApplicationDbContext _context;

    public CampaignRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Campaign?> GetByIdAsync(Guid id)
    {
        return await _context.Campaigns.FindAsync(id);
    }

    public async Task<List<Campaign>> GetAllAsync()
    {
        return await _context.Campaigns.ToListAsync();
    }

    public async Task AddAsync(Campaign campaign)
    {
        await _context.Campaigns.AddAsync(campaign);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Campaign campaign)
    {
        _context.Campaigns.Update(campaign);
        await _context.SaveChangesAsync();
    }
}