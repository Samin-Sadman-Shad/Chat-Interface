namespace Chat.Interface.Server.Domain.Entities;

public class DataSource
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DataSourceType Type { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public Dictionary<string, object> Configuration { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? LastSyncAt { get; set; }
}

public enum DataSourceType
{
    GTM,
    FacebookPixel,
    GoogleAds,
    FacebookPage,
    Website,
    Shopify,
    CRM,
    TwitterPage,
    ReviewSites,
    AdManager
}
