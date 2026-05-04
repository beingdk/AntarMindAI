// Modified by AI on 05/04/2026. Edit #1.
using Microsoft.Extensions.Configuration;

namespace AntarMindAI.Api.Services;

public class RuleBasedTaggingEngine : ITaggingEngine
{
    private readonly Dictionary<string, HashSet<string>> _domainKeywords;
    private readonly Dictionary<string, HashSet<string>> _domainBigrams;

    public RuleBasedTaggingEngine(IConfiguration configuration)
    {
        _domainKeywords = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        _domainBigrams = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

        var keywordsSection = configuration.GetSection("TaggingEngine:Keywords");
        foreach (var domain in keywordsSection.GetChildren())
        {
            var keywords = domain.Get<string[]>() ?? [];
            _domainKeywords[domain.Key] = new HashSet<string>(keywords, StringComparer.OrdinalIgnoreCase);
        }

        var bigramsSection = configuration.GetSection("TaggingEngine:Bigrams");
        foreach (var domain in bigramsSection.GetChildren())
        {
            var bigrams = domain.Get<string[]>() ?? [];
            _domainBigrams[domain.Key] = new HashSet<string>(bigrams, StringComparer.OrdinalIgnoreCase);
        }
    }

    public IReadOnlyList<string> GetTags(IReadOnlyList<string> tokens)
    {
        var tags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var tokenSet = new HashSet<string>(tokens, StringComparer.OrdinalIgnoreCase);
        var bigramSet = BuildBigramSet(tokens);

        foreach (var (domain, keywords) in _domainKeywords)
        {
            if (tokenSet.Overlaps(keywords))
                tags.Add(domain);
        }

        foreach (var (domain, bigrams) in _domainBigrams)
        {
            if (bigramSet.Overlaps(bigrams))
                tags.Add(domain);
        }

        return tags.ToList();
    }

    // Bigrams are stored as preprocessed token pairs joined by a single space,
    // e.g. "pull request", "machine learning", "working myself".
    private static HashSet<string> BuildBigramSet(IReadOnlyList<string> tokens)
    {
        var bigrams = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < tokens.Count - 1; i++)
            bigrams.Add($"{tokens[i]} {tokens[i + 1]}");
        return bigrams;
    }
}
