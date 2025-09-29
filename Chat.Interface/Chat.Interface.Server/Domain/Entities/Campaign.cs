namespace Chat.Interface.Server.Domain.Entities;

public class Campaign
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<ChannelType> Channels { get; set; } = new();
    public AudienceSegment Audience { get; set; } = new();
    public CampaignTiming Timing { get; set; } = new();
    public CampaignStatus Status { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? ScheduledAt { get; set; }
    public DateTime? ExecutedAt { get; set; }
}

public class AudienceSegment
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, object> Criteria { get; set; } = new();
    public int EstimatedSize { get; set; }
}

public class CampaignTiming
{
    public DateTime? OptimalTime { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public List<string> PreferredDays { get; set; } = new();
    public TimeSpan? PreferredTimeOfDay { get; set; }
}

public enum CampaignStatus
{
    Draft,
    Scheduled,
    Running,
    Completed,
    Failed,
    Paused
}