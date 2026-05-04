// Modified by AI on 05/04/2026. Edit #2.
namespace AntarMindAI.Api.Models;

public class ThoughtEntry
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public IReadOnlyList<string> Tags { get; set; } = [];
    public string Sentiment { get; set; } = "Neutral";
    public double IntensityScore { get; set; } = 0.0;
    public float[]? Embedding { get; set; }
}
