namespace SeniorDesign.src.VideoCollection;

public sealed record ClipInfo(string Directory, string FileName);

public interface ICameraController : IAsyncDisposable
{
    bool IsRecording { get; }
    Task ConnectAsync(CancellationToken ct = default);
    Task StartRecordingAsync(CancellationToken ct = default);
    Task StopRecordingAsync(CancellationToken ct = default);
    Task<IReadOnlyList<ClipInfo>> ListClipsAsync(CancellationToken ct = default);
    Task DownloadClipAsync(ClipInfo clip, string destinationPath, CancellationToken ct = default);
}
