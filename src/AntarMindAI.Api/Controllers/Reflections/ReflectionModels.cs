// Modified by AI on 05/04/2026. Edit #1.
using AntarMindAI.Api.Services.Insights;

namespace AntarMindAI.Api.Controllers.Reflections;

public record CategoryCountResponse(string Category, int Count, double Percentage);

public record DailySentimentResponse(string Day, int Positive, int Negative, int Neutral);

public record HourlyActivityResponse(int Hour, int ThoughtCount);

public record InsightMessageResponse(string Category, string Message, double Confidence);

public record WeeklyReflectionResponse(
    DateTimeOffset WeekStart,
    DateTimeOffset WeekEnd,
    int TotalThoughts,
    IReadOnlyList<CategoryCountResponse> CategoryDistribution,
    IReadOnlyList<DailySentimentResponse> DailySentimentTrend,
    IReadOnlyList<HourlyActivityResponse> PeakActivityByHour,
    IReadOnlyList<InsightMessageResponse> TopInsights,
    string? AiSummary,
    IReadOnlyList<string> Recommendations);
