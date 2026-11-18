namespace Tests;
using System.Net;
using SeniorDesign.src.VideoCollection;

// A fake "network". The real HttpClient sends requests through a handler.
// We replace it with one that never touches the network. It records what was
// requested and returns a canned reply.
public sealed class StubHandler : HttpMessageHandler
{
    // A function (Func) that decides what reply to give for a request.
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

    // Every request that came through, so tests can check what was sent.
    public List<HttpRequestMessage> Requests { get; } = new();

    // If no reply function is given, default to "200 OK with an empty JSON body".
    // "_ =>" is a lambda: a function with one ignored parameter.
    public StubHandler(Func<HttpRequestMessage, HttpResponseMessage>? respond = null)
        => _respond = respond ?? (_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}") });

    // HttpClient calls this instead of using the network.
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        Requests.Add(request);                      // remember the request
        return Task.FromResult(_respond(request));  // hand back the canned reply
    }
}

public class GoProCameraControllerTests
{
    // A helper that builds a controller wired to a stub, so each test stays short.
    // It returns two things at once (a "tuple"): the controller and the stub.
    private static (GoProCameraController cam, StubHandler stub) Make(
        TransportType t = TransportType.WifiAp,
        Func<HttpRequestMessage, HttpResponseMessage>? respond = null)
    {
        var stub = new StubHandler(respond);
        var http = new HttpClient(stub) { BaseAddress = new Uri("http://10.5.5.9:8080") };
        return (new GoProCameraController(new CameraConnectionOptions("10.5.5.9", t), http), stub);
    }

    // [Fact] marks a method as a test. xUnit runs every [Fact] in the project.
    [Fact]
    public async Task StartRecording_SendsShutterStart_AndSetsIsRecording()
    {
        var (cam, stub) = Make();                  // Arrange: set things up
        await cam.StartRecordingAsync();           // Act: do the thing

        Assert.True(cam.IsRecording);              // Assert: check results
        // .Single() asserts there was exactly one request and returns it.
        Assert.Equal("/gopro/camera/shutter/start", stub.Requests.Single().RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task StartRecording_OnCameraError_Throws_AndStaysNotRecording()
    {
        // The stub now answers every request with a 500 error.
        var (cam, _) = Make(respond: _ => new HttpResponseMessage(HttpStatusCode.InternalServerError));

        // Expect this call to throw an HttpRequestException.
        await Assert.ThrowsAsync<HttpRequestException>(() => cam.StartRecordingAsync());

        // Even though we tried to start, the failure means we shouldn't claim to be recording.
        Assert.False(cam.IsRecording);
    }

    [Fact]
    public async Task Connect_OverUsb_EnablesWiredControl()
    {
        var (cam, stub) = Make(TransportType.Usb);
        await cam.ConnectAsync();
        Assert.Equal("/gopro/camera/control/wired_usb?p=1", stub.Requests.Single().RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task Connect_OverWifi_SendsNothing()
    {
        var (cam, stub) = Make(TransportType.WifiAp);
        await cam.ConnectAsync();
        Assert.Empty(stub.Requests);               // no requests should have been made
    }

    [Fact]
    public async Task ListClips_ParsesMediaList()
    {
        // """...""" is a raw string literal, so you can write JSON without escaping quotes.
        var json = """{"media":[{"d":"100GOPRO","fs":[{"n":"GOPR0001.MP4"},{"n":"GOPR0002.MP4"}]}]}""";
        var (cam, _) = Make(respond: _ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) });

        var clips = await cam.ListClipsAsync();

        Assert.Equal(2, clips.Count);
        // Records compare by value, so this checks both fields match.
        Assert.Equal(new ClipInfo("100GOPRO", "GOPR0001.MP4"), clips[0]);
    }

    [Fact]
    public void ForUsb_BuildsAddressFromSerial()
    {
        // The example from the Open GoPro spec: serial ending in 789 gives 172.27.189.51.
        // This test is not async because it doesn't touch the network.
        Assert.Equal("172.27.189.51", CameraConnectionOptions.ForUsb("C0000123456789").Host);
    }
}