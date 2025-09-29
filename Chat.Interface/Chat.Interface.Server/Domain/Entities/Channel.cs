namespace Chat.Interface.Server.Domain.Entities;

public class Channel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ChannelType Type { get; set; }
    public bool IsActive { get; set; }
    public Dictionary<string, object> Configuration { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public enum ChannelType
{
    Email,
    SMS,
    Push,
    WhatsApp,
    Voice,
    Messenger,
    Ads
}