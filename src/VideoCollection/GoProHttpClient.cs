namespace SeniorDesign.src.VideoCollection;

public sealed class GoProHttpClient
{
    private readonly HttpClient _http;   // BaseAddress is set by whoever creates it

    public GoProHttpClient(HttpClient http) => _http = http;

    public Task EnableWiredUsbAsync(CancellationToken ct = default) => SendAsync("/gopro/camera/control/wired_usb?p=1", ct);
    public Task StartShutterAsync(CancellationToken ct = default)   => SendAsync("/gopro/camera/shutter/start", ct);
    public Task StopShutterAsync(CancellationToken ct = default)    => SendAsync("/gopro/camera/shutter/stop", ct); // UNVERIFIED path

    public async Task<string> GetMediaListJsonAsync(CancellationToken ct = default)
    {
        using var r = await _http.GetAsync("/gopro/media/list", ct);
        r.EnsureSuccessStatusCode();
        return await r.Content.ReadAsStringAsync(ct);
    }

    public async Task DownloadAsync(string dir, string file, string destPath, CancellationToken ct = default)
    {
        using var r = await _http.GetAsync($"/videos/DCIM/{dir}/{file}", HttpCompletionOption.ResponseHeadersRead, ct);
        r.EnsureSuccessStatusCode();
        await using var src = await r.Content.ReadAsStreamAsync(ct);
        await using var dst = File.Create(destPath);
        await src.CopyToAsync(dst, ct);
    }

    private async Task SendAsync(string path, CancellationToken ct)
    {
        using var r = await _http.GetAsync(path, ct);
        r.EnsureSuccessStatusCode();
    }
}
