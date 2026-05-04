// Modified by AI on 05/04/2026. Edit #1.
namespace AntarMindAI.Api.Services.Insights;

public interface IFrequencyAnalyzer
{
    IReadOnlyList<InsightMessage> Analyze(PatternAnalysisContext context);
}

public interface ITimeTrendAnalyzer
{
    IReadOnlyList<InsightMessage> Analyze(PatternAnalysisContext context);
}

public interface IRepetitionDetector
{
    IReadOnlyList<InsightMessage> Analyze(PatternAnalysisContext context);
}

public interface ITriggerIdentifier
{
    IReadOnlyList<InsightMessage> Analyze(PatternAnalysisContext context);
}

public interface ICognitiveBiasDetector
{
    IReadOnlyList<InsightMessage> Analyze(PatternAnalysisContext context);
}

public interface ICrossCorrelationAnalyzer
{
    IReadOnlyList<InsightMessage> Analyze(PatternAnalysisContext context);
}
