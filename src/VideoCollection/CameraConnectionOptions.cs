namespace SeniorDesign.src.VideoCollection;

using System.Text.Json;

public enum TransportType { Usb, WifiAp }

public sealed record CameraConnectionOptions(string Host, TransportType Transport, int Port = 8080)
{
    public Uri BaseUri => new($"http://{Host}:{Port}");

    public static CameraConnectionOptions ForWifiAp() => new("10.5.5.9", TransportType.WifiAp);

    public static CameraConnectionOptions ForUsb(string serial)
    {
        var d = serial[^3..];   // last three digits of the serial number
        // formula came from here: gopro.github.io/OpenGoPro/http
        return new($"172.2{d[0]}.1{d[1]}{d[2]}.51", TransportType.Usb);
    }
}