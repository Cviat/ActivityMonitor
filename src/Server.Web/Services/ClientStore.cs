using System.Collections.Concurrent;
using Server.Web.Models;

namespace Server.Web.Services;

public class ClientStore
{
    private readonly ConcurrentDictionary<string, ClientInfo> _clients = new();

    public ClientInfo RegisterOrUpdate(HeartbeatRequest request)
    {
        string id = $"{request.Domain}\\{request.MachineName}\\{request.UserName}";
        
        var client = _clients.AddOrUpdate(id,
            new ClientInfo
            {
                Id = id,
                Domain = request.Domain,
                MachineName = request.MachineName,
                UserName = request.UserName,
                IpAddress = request.IpAddress,
                LastActiveTime = DateTime.UtcNow
            },
            (_, existing) =>
            {
                existing.IpAddress = request.IpAddress;
                existing.LastActiveTime = DateTime.UtcNow;
                return existing;
            });

        return client;
    }

    public List<ClientInfo> GetAllClients()
    {
        return _clients.Values.OrderByDescending(c => c.LastActiveTime).ToList();
    }

    public bool RequestScreenshot(string clientId)
    {
        if (_clients.TryGetValue(clientId, out var client))
        {
            client.ScreenshotRequested = true;
            return true;
        }
        return false;
    }

    public bool SaveScreenshot(string clientId, string imageBase64)
    {
        if (_clients.TryGetValue(clientId, out var client))
        {
            client.LatestScreenshotBase64 = imageBase64;
            client.ScreenshotRequested = false;
            return true;
        }
        return false;
    }
}
