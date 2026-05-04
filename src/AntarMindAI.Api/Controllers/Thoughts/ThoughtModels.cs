// Modified by AI on 05/04/2026. Edit #1.
namespace AntarMindAI.Api.Controllers.Thoughts;

public record CreateThoughtRequest(string Text);

public record ThoughtResponse(
    string Id,
    string Text,
    DateTimeOffset CreatedAt,
    IReadOnlyList<string> Tags,
    string Sentiment,
    double IntensityScore
);

public record PagedThoughtsResponse(IReadOnlyList<ThoughtResponse> Items, int TotalCount, int Page, int PageSize);
