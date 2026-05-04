// Modified by AI on 05/04/2026. Edit #1.
using AntarMindAI.Api.Repositories;
using AntarMindAI.Api.Services;
using AntarMindAI.Api.Services.Insights;

namespace AntarMindAI.Api.Services.Reflections;

public class WeeklyReflectionService : IWeeklyReflectionService
{
    private readonly IThoughtRepository _repository;
    private readonly IInsightService _insightService;
    private readonly IAiSummaryService _aiSummaryService;
    private readonly IRecommendationService _recommendationService;

    public WeeklyReflectionService(
        IThoughtRepository repository,
        IInsightService insightService,
        IAiSummaryService aiSummaryService,
        IRecommendationService recommendationService)
    {
        _repository = repository;
        _insightService = insightService;
        _aiSummaryService = aiSummaryService;
        _recommendationService = recommendationService;
    }

    public async Task<WeeklyReflection> GetWeeklyReflectionAsync(string userId, DateTimeOffset? weekDate = null)
    {
        var reference = weekDate ?? DateTimeOffset.UtcNow;
        var (weekStart, weekEnd) = GetWeekBoundaries(reference);

        var allThoughts = await _repository.GetAllByUserAsync(userId);
        var weekThoughts = allThoughts
            .Where(t => t.CreatedAt >= weekStart && t.CreatedAt <= weekEnd)
            .ToList();

        var categoryDistribution = ComputeCategoryDistribution(weekThoughts);
        var dailySentimentTrend = ComputeDailySentimentTrend(weekThoughts, weekStart);
        var peakActivityByHour = ComputePeakActivityByHour(weekThoughts);

        // Top 5 insights from the full insight service
        var allInsights = await _insightService.GetInsightsAsync(userId);
        var topInsights = allInsights.Take(5).ToList();

        var reflection = new WeeklyReflection(
            WeekStart: weekStart,
            WeekEnd: weekEnd,
            TotalThoughts: weekThoughts.Count,
            CategoryDistribution: categoryDistribution,
            DailySentimentTrend: dailySentimentTrend,
            PeakActivityByHour: peakActivityByHour,
            TopInsights: topInsights,
            AiSummary: null,
            Recommendations: []); // filled in below

        var recommendations = _recommendationService.GetRecommendations(reflection, topInsights);
        var aiSummary = await _aiSummaryService.GenerateWeeklySummaryAsync(reflection);

        return reflection with { AiSummary = aiSummary, Recommendations = recommendations };
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static (DateTimeOffset Start, DateTimeOffset End) GetWeekBoundaries(DateTimeOffset date)
    {
        var utc = date.ToUniversalTime();
        // DayOfWeek: Sunday=0, Monday=1, ..., Saturday=6
        var daysFromMonday = ((int)utc.DayOfWeek + 6) % 7;
        var monday = new DateTimeOffset(utc.Year, utc.Month, utc.Day, 0, 0, 0, TimeSpan.Zero)
            .AddDays(-daysFromMonday);
        var sunday = monday.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);
        return (monday, sunday);
    }

    private static IReadOnlyList<CategoryCount> ComputeCategoryDistribution(
        IReadOnlyList<Models.ThoughtEntry> thoughts)
    {
        if (thoughts.Count == 0) return [];

        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var t in thoughts)
            foreach (var tag in t.Tags)
            {
                counts.TryGetValue(tag, out var n);
                counts[tag] = n + 1;
            }

        var total = counts.Values.Sum();
        return counts
            .OrderByDescending(kv => kv.Value)
            .Select(kv => new CategoryCount(kv.Key, kv.Value, total == 0 ? 0 : Math.Round((double)kv.Value / total * 100, 1)))
            .ToList();
    }

    private static IReadOnlyList<DailySentiment> ComputeDailySentimentTrend(
        IReadOnlyList<Models.ThoughtEntry> thoughts,
        DateTimeOffset weekStart)
    {
        var result = new List<DailySentiment>(7);
        for (int d = 0; d < 7; d++)
        {
            var day = weekStart.AddDays(d);
            var dayThoughts = thoughts.Where(t =>
                t.CreatedAt.Year == day.Year &&
                t.CreatedAt.Month == day.Month &&
                t.CreatedAt.Day == day.Day).ToList();

            result.Add(new DailySentiment(
                Day: day.DayOfWeek,
                Positive: dayThoughts.Count(t => t.Sentiment == "Positive"),
                Negative: dayThoughts.Count(t => t.Sentiment == "Negative"),
                Neutral: dayThoughts.Count(t => t.Sentiment == "Neutral")));
        }
        return result;
    }

    private static IReadOnlyList<HourlyActivity> ComputePeakActivityByHour(
        IReadOnlyList<Models.ThoughtEntry> thoughts)
    {
        var counts = new int[24];
        foreach (var t in thoughts)
            counts[t.CreatedAt.Hour]++;

        return Enumerable.Range(0, 24)
            .Select(h => new HourlyActivity(h, counts[h]))
            .ToList();
    }
}
