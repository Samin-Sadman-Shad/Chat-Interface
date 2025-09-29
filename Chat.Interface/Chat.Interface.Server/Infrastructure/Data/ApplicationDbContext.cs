using Microsoft.EntityFrameworkCore;
using Chat.Interface.Server.Domain.Entities;
using System.Text.Json;

namespace Chat.Interface.Server.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<DataSource> DataSources { get; set; }
    public DbSet<Channel> Channels { get; set; }
    public DbSet<Campaign> Campaigns { get; set; }
    public DbSet<ChatSession> ChatSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure DataSource
        modelBuilder.Entity<DataSource>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Configuration)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                      v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());
        });

        // Configure Channel
        modelBuilder.Entity<Channel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Configuration)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                      v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());
        });

        // Configure Campaign
        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Channels)
                  .HasConversion(
                      v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                      v => JsonSerializer.Deserialize<List<ChannelType>>(v, (JsonSerializerOptions?)null) ?? new());
            entity.OwnsOne(e => e.Audience, audience =>
            {
                audience.Property(a => a.Criteria)
                        .HasConversion(
                            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                            v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());
            });
            entity.OwnsOne(e => e.Timing, timing =>
            {
                timing.Property(t => t.PreferredDays)
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                          v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new());
            });
        });

        // Configure ChatSession
        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.OwnsMany(e => e.Messages, message =>
            {
                message.Property(m => m.Metadata)
                       .HasConversion(
                           v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                           v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null) ?? new());
            });
        });

        base.OnModelCreating(modelBuilder);
    }
}