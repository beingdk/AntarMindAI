// Modified by AI on 05/04/2026. Edit #1.
using System.Collections.Concurrent;
using AntarMindAI.Api.Models;

namespace AntarMindAI.Api.Repositories;

public class InMemoryThoughtRepository : IThoughtRepository
{
    // Keyed by UserId → list of entries for that user
    private readonly ConcurrentDictionary<string, List<ThoughtEntry>> _store = new();
    private readonly object _lock = new();

    public Task<ThoughtEntry> CreateAsync(ThoughtEntry entry)
    {
        lock (_lock)
        {
            var list = _store.GetOrAdd(entry.UserId, _ => []);
            list.Add(entry);
        }

        return Task.FromResult(entry);
    }

    public Task<(IReadOnlyList<ThoughtEntry> Items, int TotalCount)> GetByUserAsync(string userId, int page, int pageSize)
    {
        lock (_lock)
        {
            if (!_store.TryGetValue(userId, out var list))
            {
                return Task.FromResult<(IReadOnlyList<ThoughtEntry>, int)>(([], 0));
            }

            var sorted = list.OrderByDescending(e => e.CreatedAt).ToList();
            var total = sorted.Count;
            var skip = (page - 1) * pageSize;
            var items = sorted.Skip(skip).Take(pageSize).ToList();

            return Task.FromResult<(IReadOnlyList<ThoughtEntry>, int)>((items, total));
        }
    }

    public Task<ThoughtEntry?> GetByIdAsync(string id, string userId)
    {
        lock (_lock)
        {
            if (!_store.TryGetValue(userId, out var list))
                return Task.FromResult<ThoughtEntry?>(null);

            return Task.FromResult(list.FirstOrDefault(e => e.Id == id));
        }
    }

    public Task<bool> DeleteAsync(string id, string userId)
    {
        lock (_lock)
        {
            if (!_store.TryGetValue(userId, out var list))
                return Task.FromResult(false);

            var entry = list.FirstOrDefault(e => e.Id == id);
            if (entry is null)
                return Task.FromResult(false);

            list.Remove(entry);
            return Task.FromResult(true);
        }
    }
}
