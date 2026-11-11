namespace SeniorDesign.src.VideoCollection;

public sealed class FakeCameraController : ICameraController
{
    private readonly string _sampleClipPath;
    private readonly List<ClipInfo> _clips = new();
    private int _counter;

    public FakeCameraController(string sampleClipPath) => _sampleClipPath = sampleClipPath;

    public bool IsRecording { get; private set; }

    public Task ConnectAsync(CancellationToken ct = default) => Task.CompletedTask;

    public Task StartRecordingAsync(CancellationToken ct = default)
    {
        if (IsRecording) throw new InvalidOperationException("Already recording.");
        IsRecording = true;
        return Task.CompletedTask;
    }

    public Task StopRecordingAsync(CancellationToken ct = default)
    {
        if (!IsRecording) throw new InvalidOperationException("Not recording.");
        IsRecording = false;
        _clips.Add(new ClipInfo("100GOPRO", $"GOPR{++_counter:D4}.MP4"));
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ClipInfo>> ListClipsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<ClipInfo>>(_clips.ToList());

    public Task DownloadClipAsync(ClipInfo clip, string destinationPath, CancellationToken ct = default)
    {
        File.Copy(_sampleClipPath, destinationPath, overwrite: true);
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}