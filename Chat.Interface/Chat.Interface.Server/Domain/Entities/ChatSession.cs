namespace Chat.Interface.Server.Domain.Entities;

public class ChatSession
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public List<ChatMessage> Messages { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}

public class ChatMessage
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public enum MessageType
{
    User,
    Assistant,
    System,
    CampaignRecommendation
}