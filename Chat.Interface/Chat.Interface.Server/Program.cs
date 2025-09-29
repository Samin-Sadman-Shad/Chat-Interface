using Microsoft.EntityFrameworkCore;
using Chat.Interface.Server.Infrastructure.Data;
using Chat.Interface.Server.Infrastructure.Repositories;
using Chat.Interface.Server.Infrastructure.Services;
using Chat.Interface.Server.Hubs;
using FluentValidation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ChatInterfaceDb"));

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// Repositories
builder.Services.AddScoped<IDataSourceRepository, DataSourceRepository>();
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();

// Services
builder.Services.AddScoped<IDataSourceConnector, DataSourceConnector>();
builder.Services.AddScoped<ICampaignGenerator, CampaignGenerator>();
builder.Services.AddScoped<ICampaignExecutionService, CampaignExecutionService>();

// SignalR
builder.Services.AddSignalR();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("https://localhost:51746")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.MapFallbackToFile("/index.html");

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await SeedData(context);
}

app.Run();

static async Task SeedData(ApplicationDbContext context)
{
    if (!context.DataSources.Any())
    {
        var dataSources = new[]
        {
            new Chat.Interface.Server.Domain.Entities.DataSource
            {
                Id = Guid.NewGuid(),
                Name = "Google Tag Manager",
                Type = Chat.Interface.Server.Domain.Entities.DataSourceType.GTM,
                IsConnected = false,
                CreatedAt = DateTime.UtcNow,
                Configuration = new Dictionary<string, object> { ["containerId"] = "" }
            },
            new Chat.Interface.Server.Domain.Entities.DataSource
            {
                Id = Guid.NewGuid(),
                Name = "Facebook Pixel",
                Type = Chat.Interface.Server.Domain.Entities.DataSourceType.FacebookPixel,
                IsConnected = false,
                CreatedAt = DateTime.UtcNow,
                Configuration = new Dictionary<string, object> { ["pixelId"] = "" }
            },
            new Chat.Interface.Server.Domain.Entities.DataSource
            {
                Id = Guid.NewGuid(),
                Name = "Shopify Store",
                Type = Chat.Interface.Server.Domain.Entities.DataSourceType.Shopify,
                IsConnected = false,
                CreatedAt = DateTime.UtcNow,
                Configuration = new Dictionary<string, object> { ["shopName"] = "", ["apiKey"] = "" }
            }
        };

        context.DataSources.AddRange(dataSources);
        await context.SaveChangesAsync();
    }
}
