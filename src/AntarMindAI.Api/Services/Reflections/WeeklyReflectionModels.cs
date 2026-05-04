// Modified by AI on 05/04/2026. Edit #1.
using AntarMindAI.Api.Services.Insights;

namespace AntarMindAI.Api.Services.Reflections;

public record CategoryCount(string Category, int Count, double Percentage);

public record DailySentiment(DayOfWeek Day, int Positive, int Negative, int Neutral);

public record HourlyActivity(int Hour, int ThoughtCount);

public record WeeklyReflection(
    DateTimeOffset WeekStart,
    DateTimeOffset WeekEnd,
    int TotalThoughts,
    IReadOnlyList<CategoryCount> CategoryDistribution,
    IReadOnlyList<DailySentiment> DailySentimentTrend,
    IReadOnlyList<HourlyActivity> PeakActivityByHour,
    IReadOnlyList<InsightMessage> TopInsights,
    string? AiSummary,
    IReadOnlyList<string> Recommendations);
