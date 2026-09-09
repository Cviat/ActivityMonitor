using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;

namespace Client.Agent;

public class SystemInfoProvider
{
    public string Domain { get; }
    public string MachineName { get; }
    public string UserName { get; }
    public string IpAddress { get; }

    public SystemInfoProvider()
    {
        Domain = Environment.UserDomainName;
        MachineName = Environment.MachineName;
        UserName = Environment.UserName;
        IpAddress = GetLocalIpAddress();
    }

    private static string GetLocalIpAddress()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
        }
        catch
        {
        }
        return "127.0.0.1";
    }

    /// <summary>
    /// Делает честный скриншот экрана через родной CopyFromScreen с отключенной отладкой
    /// </summary>
    public static byte[] CaptureDesktopScreenshot()
    {
        var bounds = Screen.PrimaryScreen?.Bounds ?? new Rectangle(0, 0, 1920, 1080);
        
        using var bitmap = new Bitmap(bounds.Width, bounds.Height);
        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
        }

        using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Jpeg);
        return ms.ToArray();
    }
}
