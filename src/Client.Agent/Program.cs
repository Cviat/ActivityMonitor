using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Win32;

namespace Client.Agent;

internal static class Program
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string ServerUrl = "http://localhost:5000";

    [STAThread]
    static void Main()
    {
        ConfigureAutoStart();

        var infoProvider = new SystemInfoProvider();

        // Основной фоновый цикл опроса сервера
        Task.Run(async () =>
        {
            while (true)
            {
                await ProcessHeartbeat(infoProvider);
                await Task.Delay(2000);
            }
        });

        ApplicationConfiguration.Initialize();
        Application.Run();
    }

    private static async Task ProcessHeartbeat(SystemInfoProvider info)
    {
        try
        {
            var heartbeatData = new
            {
                domain = info.Domain,
                machineName = info.MachineName,
                userName = info.UserName,
                ipAddress = info.IpAddress
            };

            var response = await _httpClient.PostAsJsonAsync($"{ServerUrl}/api/heartbeat", heartbeatData);
            if (response.IsSuccessStatusCode)
            {
                var resultJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(resultJson);
                
                if (doc.RootElement.TryGetProperty("requestScreenshot", out var prop) && prop.GetBoolean())
                {
                    byte[] screenshotBytes = SystemInfoProvider.CaptureDesktopScreenshot();
                    string base64Image = Convert.ToBase64String(screenshotBytes);

                    string clientId = $"{info.Domain}\\{info.MachineName}\\{info.UserName}";
                    var screenshotPayload = new
                    {
                        clientId = clientId,
                        imageBase64 = base64Image
                    };

                    await _httpClient.PostAsJsonAsync($"{ServerUrl}/api/screenshot", screenshotPayload);
                }
            }
        }
        catch
        {
            // Игнорируем временные сетевые сбои
        }
    }

    private static void ConfigureAutoStart()
    {
        try
        {
            string appName = "EmployeeActivityMonitorAgent";
            string appPath = Application.ExecutablePath;

            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key != null && key.GetValue(appName) == null)
            {
                key.SetValue(appName, $"\"{appPath}\"");
            }
        }
        catch
        {
        }
    }
}