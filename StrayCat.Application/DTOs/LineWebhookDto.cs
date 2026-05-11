namespace StrayCat.Application.DTOs;

public class LineWebhookRequest
{
    public string Destination { get; set; } = string.Empty;
    public List<LineWebhookEvent> Events { get; set; } = new();
}

public class LineWebhookEvent
{
    public string Type { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public long Timestamp { get; set; }
    public LineWebhookSource Source { get; set; } = null!;
    public LineWebhookMessage? Message { get; set; }
    public string? ReplyToken { get; set; }
    public string? WebhookEventId { get; set; }
}

public class LineWebhookSource
{
    public string Type { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}

public class LineWebhookMessage
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Text { get; set; }
}

public class LineMessageRequest
{
    public string To { get; set; } = string.Empty;
    public List<LineMessage> Messages { get; set; } = new();
}

public class LineMessage
{
    public string Type { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
