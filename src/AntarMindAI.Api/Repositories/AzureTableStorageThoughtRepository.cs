// Modified by AI on 05/04/2026. Edit #3.
using Azure;
using Azure.Data.Tables;
using AntarMindAI.Api.Models;

namespace AntarMindAI.Api.Repositories;

public class AzureTableStorageThoughtRepository : IThoughtRepository
{
    private const string TableName = "thoughts";
    private readonly TableClient _tableClient;

    public AzureTableStorageThoughtRepository(string connectionString)
    {
        var serviceClient = new TableServiceClient(connectionString);
        _tableClient = serviceClient.GetTableClient(TableName);
        _tableClient.CreateIfNotExists();
    }

    public async Task<ThoughtEntry> CreateAsync(ThoughtEntry entry)
    {
        var rowKey = (DateTimeOffset.MaxValue.Ticks - entry.CreatedAt.Ticks).ToString("D20");
        var tableEntity = new TableEntity(entry.UserId, rowKey)
        {
            ["Id"] = entry.Id,
            ["Text"] = entry.Text,
            ["CreatedAt"] = entry.CreatedAt,
            ["Tags"] = string.Join("|", entry.Tags),
            ["Sentiment"] = entry.Sentiment,
            ["IntensityScore"] = entry.IntensityScore
        };

        await _tableClient.AddEntityAsync(tableEntity);
        return entry;
    }

    public async Task<(IReadOnlyList<ThoughtEntry> Items, int TotalCount)> GetByUserAsync(string userId, int page, int pageSize)
    {
        var allEntries = new List<ThoughtEntry>();
        string? continuationToken = null;

        // Fetch all entries for user (partition) — needed for total count and page offset
        do
        {
            var page_ = await _tableClient.QueryAsync<TableEntity>(
                filter: $"PartitionKey eq '{userId}'",
                maxPerPage: 1000
            ).AsPages(continuationToken).FirstOrDefaultAsync();

            if (page_ is null) break;

            foreach (var entity in page_.Values)
            {
                allEntries.Add(MapToEntry(entity, userId));
            }

            continuationToken = page_.ContinuationToken;
        } while (continuationToken is not null);

        var total = allEntries.Count;
        var skip = (page - 1) * pageSize;
        // Entries are already in reverse-chronological order due to inverted RowKey
        var items = allEntries.Skip(skip).Take(pageSize).ToList();

        return (items, total);
    }

    public async Task<IReadOnlyList<ThoughtEntry>> GetAllByUserAsync(string userId)
    {
        var allEntries = new List<ThoughtEntry>();
        string? continuationToken = null;

        do
        {
            var page = await _tableClient.QueryAsync<TableEntity>(
                filter: $"PartitionKey eq '{userId}'",
                maxPerPage: 1000
            ).AsPages(continuationToken).FirstOrDefaultAsync();

            if (page is null) break;

            foreach (var entity in page.Values)
                allEntries.Add(MapToEntry(entity, userId));

            continuationToken = page.ContinuationToken;
        } while (continuationToken is not null);

        return allEntries;
    }

    public async Task<ThoughtEntry?> GetByIdAsync(string id, string userId)
    {
        var entities = _tableClient.QueryAsync<TableEntity>(
            filter: $"PartitionKey eq '{userId}' and Id eq '{id}'"
        );

        await foreach (var entity in entities)
        {
            return MapToEntry(entity, userId);
        }

        return null;
    }

    public async Task<bool> DeleteAsync(string id, string userId)
    {
        // Find the entity by Id property within the user's partition
        var entities = _tableClient.QueryAsync<TableEntity>(
            filter: $"PartitionKey eq '{userId}' and Id eq '{id}'"
        );

        await foreach (var entity in entities)
        {
            await _tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey, entity.ETag);
            return true;
        }

        return false;
    }

    public async Task<(IReadOnlyList<ThoughtEntry> Items, int TotalCount)> SearchAsync(
        string userId, string query, int page, int pageSize)
    {
        var all = await GetAllByUserAsync(userId);
        var matches = all
            .Where(e => e.Text.Contains(query, StringComparison.OrdinalIgnoreCase)
                     || e.Tags.Any(t => t.Contains(query, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var total = matches.Count;
        var items = matches.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return ((IReadOnlyList<ThoughtEntry>)items, total);
    }

    private static ThoughtEntry MapToEntry(TableEntity entity, string userId)
    {
        var tagsRaw = entity.GetString("Tags") ?? string.Empty;
        var tags = string.IsNullOrWhiteSpace(tagsRaw)
            ? (IReadOnlyList<string>)[]
            : tagsRaw.Split('|', StringSplitOptions.RemoveEmptyEntries);

        return new ThoughtEntry
        {
            Id = entity.GetString("Id") ?? string.Empty,
            UserId = userId,
            Text = entity.GetString("Text") ?? string.Empty,
            CreatedAt = entity.GetDateTimeOffset("CreatedAt") ?? DateTimeOffset.MinValue,
            Tags = tags,
            Sentiment = entity.GetString("Sentiment") ?? "Neutral",
            IntensityScore = entity.GetDouble("IntensityScore") ?? 0.0
        };
    }
}
