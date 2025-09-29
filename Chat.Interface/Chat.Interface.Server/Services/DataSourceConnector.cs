using Chat.Interface.Server.Domain.Entities;
using Polly;

namespace Chat.Interface.Server.Infrastructure.Services;

public interface IDataSourceConnector
{
    Task<bool> TestConnectionAsync(DataSource dataSource);
    Task SyncDataAsync(DataSource dataSource);
}

public class DataSourceConnector : IDataSourceConnector
{
    private readonly ResiliencePipeline _resiliencePipeline;

    public DataSourceConnector()
    {
        _resiliencePipeline = new ResiliencePipelineBuilder()
            .AddRetry(new Polly.Retry.RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2)
            })
            .AddTimeout(TimeSpan.FromSeconds(10))
            .Build();
    }

    public async Task<bool> TestConnectionAsync(DataSource dataSource)
    {
        return await _resiliencePipeline.ExecuteAsync(async _ =>
        {
            // Simulate connection test based on data source type
            return dataSource.Type switch
            {
                DataSourceType.GTM => await TestGTMConnectionAsync(dataSource),
                DataSourceType.FacebookPixel => await TestFacebookPixelConnectionAsync(dataSource),
                DataSourceType.GoogleAds => await TestGoogleAdsConnectionAsync(dataSource),
                _ => await Task.FromResult(true) // Mock successful connection
            };
        });
    }

    public async Task SyncDataAsync(DataSource dataSource)
    {
        await _resiliencePipeline.ExecuteAsync(async _ =>
        {
            
            await Task.Delay(1000);
            
        });
    }

    private async Task<bool> TestGTMConnectionAsync(DataSource dataSource)
    {
        // Mock GTM connection test
        await Task.Delay(500);
        return dataSource.Configuration.ContainsKey("containerId");
    }

    private async Task<bool> TestFacebookPixelConnectionAsync(DataSource dataSource)
    {
        // Mock Facebook Pixel connection test
        await Task.Delay(500);
        return dataSource.Configuration.ContainsKey("pixelId");
    }

    private async Task<bool> TestGoogleAdsConnectionAsync(DataSource dataSource)
    {
        // Mock Google Ads connection test
        await Task.Delay(500);
        return dataSource.Configuration.ContainsKey("customerId");
    }
}