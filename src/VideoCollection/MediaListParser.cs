namespace SeniorDesign.src.VideoCollection;

using System.Text.Json;

public static class MediaListParser
{
    public static IReadOnlyList<ClipInfo> Parse(string json)
    {
        var clips = new List<ClipInfo>();
        using var doc = JsonDocument.Parse(json);
        foreach (var folder in doc.RootElement.GetProperty("media").EnumerateArray())
        {
            var dir = folder.GetProperty("d").GetString()!;
            // UNVERIFIED - I am somewhat lost here. I would need to do more research into fs and n variables. 
            foreach (var file in folder.GetProperty("fs").EnumerateArray())
                clips.Add(new ClipInfo(dir, file.GetProperty("n").GetString()!));
        }
        return clips;
    }
}
