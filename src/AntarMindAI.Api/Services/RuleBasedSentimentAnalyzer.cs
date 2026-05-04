// Modified by AI on 05/04/2026. Edit #1.
using Microsoft.Extensions.Configuration;

namespace AntarMindAI.Api.Services;

public class RuleBasedSentimentAnalyzer : ISentimentAnalyzer
{
    private readonly Dictionary<string, double> _positiveKeywords;
    private readonly Dictionary<string, double> _negativeKeywords;

    // Unambiguous negation words preserved after stop-word removal.
    // Contractions (cant, didnt) are intentionally excluded to avoid misclassifying
    // intensifier expressions such as "can't believe how great this is".
    private static readonly HashSet<string> NegationWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "not", "no", "never", "neither", "nor", "nobody", "nothing", "nowhere", "none"
    };

    public RuleBasedSentimentAnalyzer(IConfiguration configuration)
    {
        _positiveKeywords = LoadWeights(configuration, "SentimentAnalyzer:PositiveKeywords");
        _negativeKeywords = LoadWeights(configuration, "SentimentAnalyzer:NegativeKeywords");
    }

    public SentimentResult Analyze(IReadOnlyList<string> tokens)
    {
        if (tokens.Count == 0)
            return new SentimentResult("Neutral", 0.0);

        double score = 0.0;
        int negationCountdown = 0;

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            if (NegationWords.Contains(token))
            {
                negationCountdown = 3;
                continue;
            }

            bool isNegated = negationCountdown > 0;
            if (negationCountdown > 0) negationCountdown--;

            if (_positiveKeywords.TryGetValue(token, out var posWeight))
                score += isNegated ? -posWeight : posWeight;
            else if (_negativeKeywords.TryGetValue(token, out var negWeight))
                score -= isNegated ? -negWeight : negWeight;
        }

        var sentiment = score > 0.1 ? "Positive" : score < -0.1 ? "Negative" : "Neutral";
        var intensityScore = Sigmoid(Math.Abs(score));

        return new SentimentResult(sentiment, intensityScore);
    }

    // Sigmoid maps [0, ∞) → [0, 1) with higher sensitivity near zero than Math.Min clipping.
    private static double Sigmoid(double x) => 2.0 / (1.0 + Math.Exp(-2.0 * x)) - 1.0;

    private static Dictionary<string, double> LoadWeights(IConfiguration configuration, string sectionPath)
    {
        var dict = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        var section = configuration.GetSection(sectionPath);

        foreach (var entry in section.GetChildren())
        {
            if (double.TryParse(entry.Value, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var weight))
            {
                dict[entry.Key] = weight;
            }
        }

        return dict;
    }
}
