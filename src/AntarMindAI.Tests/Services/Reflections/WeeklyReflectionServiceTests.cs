// Modified by AI on 05/04/2026. Edit #1.
using AntarMindAI.Api.Models;
using AntarMindAI.Api.Repositories;
using AntarMindAI.Api.Services;
using AntarMindAI.Api.Services.Insights;
using AntarMindAI.Api.Services.Reflections;

namespace AntarMindAI.Tests.Services.Reflections;

public class WeeklyReflectionServiceTests
{
    private static ThoughtEntry T(string sentiment, string[] tags, DateTimeOffset at) =>
        new() { Id = Guid.NewGuid().ToString(), Text = "t", Sentiment = sentiment, Tags = tags, CreatedAt = at };

    private static DateTimeOffset ThisMonday()
    {
        var now = DateTimeOffset.UtcNow;
        var daysFromMonday = ((int)now.DayOfWeek + 6) % 7;
        return new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, TimeSpan.Zero).AddDays(-daysFromMonday);
    }

    private static IWeeklyReflectionService BuildService(IReadOnlyList<ThoughtEntry> thoughts)
    {
        var repo = new FakeThoughtRepository(thoughts);
        var insightSvc = new FakeInsightService();
        var aiSvc = new NullAiSummaryService();
        var recSvc = new RecommendationService();
        return new WeeklyReflectionService(repo, insightSvc, aiSvc, recSvc);
    }

    [Fact]
    public async Task GetWeeklyReflection_WeekBoundaries_AreCorrectMondayToSunday()
    {
        var monday = ThisMonday();
        var svc = BuildService([]);

        var result = await svc.GetWeeklyReflectionAsync("user1");

        Assert.Equal(DayOfWeek.Monday, result.WeekStart.DayOfWeek);
        Assert.Equal(DayOfWeek.Sunday, result.WeekEnd.DayOfWeek);
        Assert.Equal(0, result.WeekStart.Hour);
        Assert.Equal(23, result.WeekEnd.Hour);
    }

    [Fact]
    public async Task GetWeeklyReflection_CategoryDistribution_CalculatesCorrectPercentages()
    {
        var monday = ThisMonday();
        var thoughts = new List<ThoughtEntry>
        {
            T("Positive", ["Work"], monday.AddHours(9)),
            T("Positive", ["Work"], monday.AddHours(10)),
            T("Negative", ["Trading"], monday.AddHours(11)),
            T("Neutral", ["Trading"], monday.AddHours(12)),
        };
        var svc = BuildService(thoughts);

        var result = await svc.GetWeeklyReflectionAsync("user1");

        var work = result.CategoryDistribution.First(c => c.Category == "Work");
        var trading = result.CategoryDistribution.First(c => c.Category == "Trading");
        Assert.Equal(2, work.Count);
        Assert.Equal(50.0, work.Percentage);
        Assert.Equal(2, trading.Count);
        Assert.Equal(50.0, trading.Percentage);
    }

    [Fact]
    public async Task GetWeeklyReflection_DailySentimentTrend_HasSevenItems()
    {
        var svc = BuildService([]);

        var result = await svc.GetWeeklyReflectionAsync("user1");

        Assert.Equal(7, result.DailySentimentTrend.Count);
    }

    [Fact]
    public async Task GetWeeklyReflection_PeakActivityByHour_Has24Items()
    {
        var svc = BuildService([]);

        var result = await svc.GetWeeklyReflectionAsync("user1");

        Assert.Equal(24, result.PeakActivityByHour.Count);
    }

    [Fact]
    public async Task GetWeeklyReflection_ZeroThoughts_ReturnsEmptyAggregations()
    {
        var svc = BuildService([]);

        var result = await svc.GetWeeklyReflectionAsync("user1");

        Assert.Equal(0, result.TotalThoughts);
        Assert.Empty(result.CategoryDistribution);
        Assert.All(result.DailySentimentTrend, d => Assert.Equal(0, d.Positive + d.Negative + d.Neutral));
        Assert.All(result.PeakActivityByHour, h => Assert.Equal(0, h.ThoughtCount));
    }

    [Fact]
    public async Task GetWeeklyReflection_ThoughtsOutsideWeek_AreExcluded()
    {
        var monday = ThisMonday();
        var thoughts = new List<ThoughtEntry>
        {
            T("Positive", ["Work"], monday.AddDays(-7)), // last week
            T("Positive", ["Work"], monday.AddHours(9)), // this week
        };
        var svc = BuildService(thoughts);

        var result = await svc.GetWeeklyReflectionAsync("user1");

        Assert.Equal(1, result.TotalThoughts);
    }

    // ── Fakes ─────────────────────────────────────────────────────────────────

    private class FakeThoughtRepository(IReadOnlyList<ThoughtEntry> thoughts) : IThoughtRepository
    {
        public Task<IReadOnlyList<ThoughtEntry>> GetAllByUserAsync(string userId) =>
            Task.FromResult(thoughts);
        public Task<ThoughtEntry> CreateAsync(ThoughtEntry entry) => throw new NotImplementedException();
        public Task<(IReadOnlyList<ThoughtEntry> Items, int TotalCount)> GetByUserAsync(string userId, int page, int pageSize) => throw new NotImplementedException();
        public Task<ThoughtEntry?> GetByIdAsync(string id, string userId) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(string id, string userId) => throw new NotImplementedException();
        public Task<(IReadOnlyList<ThoughtEntry> Items, int TotalCount)> SearchAsync(string userId, string query, int page, int pageSize) => throw new NotImplementedException();
    }

    private class FakeInsightService : IInsightService
    {
        public Task<IReadOnlyList<InsightMessage>> GetInsightsAsync(string userId) =>
            Task.FromResult<IReadOnlyList<InsightMessage>>([]);
    }
}
