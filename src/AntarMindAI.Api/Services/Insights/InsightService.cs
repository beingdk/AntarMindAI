// Modified by AI on 05/04/2026. Edit #1.
using AntarMindAI.Api.Repositories;
using Microsoft.Extensions.Configuration;

namespace AntarMindAI.Api.Services.Insights;

public class InsightService : IInsightService
{
    private readonly IThoughtRepository _repository;
    private readonly IFrequencyAnalyzer _frequencyAnalyzer;
    private readonly ITimeTrendAnalyzer _timeTrendAnalyzer;
    private readonly IRepetitionDetector _repetitionDetector;
    private readonly ITriggerIdentifier _triggerIdentifier;
    private readonly ICognitiveBiasDetector _cognitiveBiasDetector;
    private readonly ICrossCorrelationAnalyzer _crossCorrelationAnalyzer;
    private readonly int _minimumThoughts;
    private readonly int _windowDays;

    public InsightService(
        IThoughtRepository repository,
        IFrequencyAnalyzer frequencyAnalyzer,
        ITimeTrendAnalyzer timeTrendAnalyzer,
        IRepetitionDetector repetitionDetector,
        ITriggerIdentifier triggerIdentifier,
        ICognitiveBiasDetector cognitiveBiasDetector,
        ICrossCorrelationAnalyzer crossCorrelationAnalyzer,
        IConfiguration configuration)
    {
        _repository = repository;
        _frequencyAnalyzer = frequencyAnalyzer;
        _timeTrendAnalyzer = timeTrendAnalyzer;
        _repetitionDetector = repetitionDetector;
        _triggerIdentifier = triggerIdentifier;
        _cognitiveBiasDetector = cognitiveBiasDetector;
        _crossCorrelationAnalyzer = crossCorrelationAnalyzer;
        _minimumThoughts = configuration.GetValue("PatternDetection:MinimumThoughtsForInsights", 10);
        _windowDays = configuration.GetValue("PatternDetection:FrequencyWindowDays", 7);
    }

    public async Task<IReadOnlyList<InsightMessage>> GetInsightsAsync(string userId)
    {
        var thoughts = await _repository.GetAllByUserAsync(userId);

        if (thoughts.Count < _minimumThoughts)
            return [];

        var context = new PatternAnalysisContext(thoughts, _windowDays);

        var allInsights = new List<InsightMessage>();
        allInsights.AddRange(_frequencyAnalyzer.Analyze(context));
        allInsights.AddRange(_timeTrendAnalyzer.Analyze(context));
        allInsights.AddRange(_repetitionDetector.Analyze(context));
        allInsights.AddRange(_triggerIdentifier.Analyze(context));
        allInsights.AddRange(_cognitiveBiasDetector.Analyze(context));
        allInsights.AddRange(_crossCorrelationAnalyzer.Analyze(context));

        // Deduplicate by message text, sort by confidence desc, return top 10
        return allInsights
            .GroupBy(i => i.Message, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .OrderByDescending(i => i.Confidence)
            .Take(10)
            .ToList();
    }
}
