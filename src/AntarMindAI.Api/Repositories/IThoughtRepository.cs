// Modified by AI on 05/04/2026. Edit #2.
using AntarMindAI.Api.Models;

namespace AntarMindAI.Api.Repositories;

public interface IThoughtRepository
{
    Task<ThoughtEntry> CreateAsync(ThoughtEntry entry);
    Task<(IReadOnlyList<ThoughtEntry> Items, int TotalCount)> GetByUserAsync(string userId, int page, int pageSize);
    /// <summary>Returns all thoughts for a user without pagination.</summary>
    Task<IReadOnlyList<ThoughtEntry>> GetAllByUserAsync(string userId);
    /// <summary>Returns null if not found or not owned by userId.</summary>
    Task<ThoughtEntry?> GetByIdAsync(string id, string userId);
    /// <summary>Deletes a thought by id. Returns false if not found or not owned by userId.</summary>
    Task<bool> DeleteAsync(string id, string userId);
}
