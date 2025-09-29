using Chat.Interface.Server.Domain.Entities;
using Chat.Interface.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Chat.Interface.Server.Infrastructure.Repositories;

public interface IDataSourceRepository
{
    Task<List<DataSource>> GetAllAsync();
    Task<List<DataSource>> GetByIdsAsync(List<Guid> ids);
    Task<DataSource?> GetByIdAsync(Guid id);
    Task AddAsync(DataSource dataSource);
    Task UpdateAsync(DataSource dataSource);
}

public class DataSourceRepository : IDataSourceRepository
{
    private readonly ApplicationDbContext _context;

    public DataSourceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DataSource>> GetAllAsync()
    {
        return await _context.DataSources.ToListAsync();
    }

    public async Task<List<DataSource>> GetByIdsAsync(List<Guid> ids)
    {
        return await _context.DataSources
            .Where(ds => ids.Contains(ds.Id))
            .ToListAsync();
    }

    public async Task<DataSource?> GetByIdAsync(Guid id)
    {
        return await _context.DataSources.FindAsync(id);
    }

    public async Task AddAsync(DataSource dataSource)
    {
        await _context.DataSources.AddAsync(dataSource);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DataSource dataSource)
    {
        _context.DataSources.Update(dataSource);
        await _context.SaveChangesAsync();
    }
}