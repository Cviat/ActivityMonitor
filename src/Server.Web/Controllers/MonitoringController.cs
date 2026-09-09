using Microsoft.AspNetCore.Mvc;
using Server.Web.Models;
using Server.Web.Services;

namespace Server.Web.Controllers;

[ApiController]
[Route("api")]
public class MonitoringController : ControllerBase
{
    private readonly ClientStore _clientStore;

    public MonitoringController(ClientStore clientStore)
    {
        _clientStore = clientStore;
    }

    /// <summary>
    /// Регистрация и обновление активности клиента (Heartbeat)
    /// </summary>
    [HttpPost("heartbeat")]
    public IActionResult Heartbeat([FromBody] HeartbeatRequest request)
    {
        var client = _clientStore.RegisterOrUpdate(request);
        bool shouldTakeScreenshot = client.ScreenshotRequested;
        
        return Ok(new HeartbeatResponse { RequestScreenshot = shouldTakeScreenshot });
    }

    /// <summary>
    /// Загрузка скриншота от клиента
    /// </summary>
    [HttpPost("screenshot")]
    public IActionResult UploadScreenshot([FromBody] UploadScreenshotRequest request)
    {
        bool success = _clientStore.SaveScreenshot(request.ClientId, request.ImageBase64);
        if (success) return Ok(new { status = "success" });
        return NotFound(new { error = "Client not found" });
    }

    /// <summary>
    /// Получение списка всех подключенных клиентов
    /// </summary>
    [HttpGet("clients")]
    public IActionResult GetClients()
    {
        var clients = _clientStore.GetAllClients();
        return Ok(clients);
    }

    public class RequestScreenshotDto
    {
        public string ClientId { get; set; } = string.Empty;
    }

    /// <summary>
    /// Установка флага запроса скриншота для клиента
    /// </summary>
    [HttpPost("clients/request-screenshot")]
    public IActionResult RequestScreenshot([FromBody] RequestScreenshotDto dto)
    {
        string cleanedId = dto.ClientId.Trim('"').Replace("\\\\", "\\");
        bool success = _clientStore.RequestScreenshot(cleanedId);
        if (success) return Ok(new { status = "requested" });
        
        var allClients = _clientStore.GetAllClients();
        var match = allClients.FirstOrDefault(c => c.Id.Equals(cleanedId, StringComparison.OrdinalIgnoreCase));
        if (match != null)
        {
            match.ScreenshotRequested = true;
            return Ok(new { status = "requested" });
        }

        return NotFound(new { error = "Client not found" });
    }
}
