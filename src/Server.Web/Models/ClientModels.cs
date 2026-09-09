using System.Text.Json.Serialization;

namespace Server.Web.Models;

public class ClientInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("domain")]
    public string Domain { get; set; } = string.Empty;

    [JsonPropertyName("machineName")]
    public string MachineName { get; set; } = string.Empty;

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("ipAddress")]
    public string IpAddress { get; set; } = string.Empty;

    [JsonPropertyName("lastActiveTime")]
    public DateTime LastActiveTime { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("screenshotRequested")]
    public bool ScreenshotRequested { get; set; } = false;

    [JsonPropertyName("latestScreenshotBase64")]
    public string? LatestScreenshotBase64 { get; set; }
}

public class HeartbeatRequest
{
    [JsonPropertyName("domain")]
    public string Domain { get; set; } = string.Empty;

    [JsonPropertyName("machineName")]
    public string MachineName { get; set; } = string.Empty;

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("ipAddress")]
    public string IpAddress { get; set; } = string.Empty;
}

public class HeartbeatResponse
{
    [JsonPropertyName("requestScreenshot")]
    public bool RequestScreenshot { get; set; }
}

public class UploadScreenshotRequest
{
    [JsonPropertyName("clientId")]
    public string ClientId { get; set; } = string.Empty;

    [JsonPropertyName("imageBase64")]
    public string ImageBase64 { get; set; } = string.Empty;
}
