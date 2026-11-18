namespace SeniorDesign.src.VideoCollection;


public sealed class GoProCameraController : ICameraController
{
    private readonly CameraConnectionOptions _options;
    private readonly HttpClient _http;
    private readonly bool _ownsHttp;
    private readonly GoProHttpClient _client;

    public GoProCameraController(CameraConnectionOptions options, HttpClient? http = null)
    {
        _options = options;
        _ownsHttp = http is null;
        _http = http ?? new HttpClient { BaseAddress = options.BaseUri };
        _client = new GoProHttpClient(_http);
    }

    public bool IsRecording { get; private set; }

    public async Task ConnectAsync(CancellationToken ct = default)
    {
        if (_options.Transport == TransportType.Usb)
            await _client.EnableWiredUsbAsync(ct);
    }

    public async Task StartRecordingAsync(CancellationToken ct = default)
    {
        await _client.StartShutterAsync(ct);
        IsRecording = true;
    }

    public async Task StopRecordingAsync(CancellationToken ct = default)
    {
        await _client.StopShutterAsync(ct);
        IsRecording = false;
    }

    public async Task<IReadOnlyList<ClipInfo>> ListClipsAsync(CancellationToken ct = default)
        => MediaListParser.Parse(await _client.GetMediaListJsonAsync(ct));

    public Task DownloadClipAsync(ClipInfo clip, string destinationPath, CancellationToken ct = default)
        => _client.DownloadAsync(clip.Directory, clip.FileName, destinationPath, ct);

    public ValueTask DisposeAsync()
    {
        if (_ownsHttp) _http.Dispose();
        return ValueTask.CompletedTask;
    }
}
