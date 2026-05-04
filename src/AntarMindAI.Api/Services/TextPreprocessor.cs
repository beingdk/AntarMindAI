// Modified by AI on 05/04/2026. Edit #1.
using System.Text.RegularExpressions;

namespace AntarMindAI.Api.Services;

public class TextPreprocessor : ITextPreprocessor
{
    private static readonly HashSet<string> StopWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a", "an", "the", "and", "or", "but", "in", "on", "at", "to", "for",
        "of", "with", "by", "from", "is", "it", "this", "that", "was", "are",
        "be", "been", "being", "have", "has", "had", "do", "does", "did", "will",
        "would", "could", "should", "may", "might", "shall", "can",
        "so", "if", "as", "up", "out", "about", "into", "than", "then", "too",
        "very", "just", "also", "its", "we", "you", "he", "she", "they", "i",
        "me", "my", "him", "her", "our", "your", "their", "what", "when", "where",
        "which", "who", "how", "all", "any", "both", "each", "more", "most",
        "other", "some", "such", "own", "same", "off", "over", "after", "before",
        "while", "through", "during", "between", "get", "got", "like", "go", "went"
    };

    public IReadOnlyList<string> Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];

        var lower = text.ToLowerInvariant();
        var noPunctuation = Regex.Replace(lower, @"[^\w\s]", " ");
        var tokens = noPunctuation.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return tokens
            .Where(t => !StopWords.Contains(t) && t.Length > 1)
            .ToList();
    }
}
