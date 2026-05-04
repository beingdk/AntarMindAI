// Modified by AI on 05/04/2026. Edit #1.
using AntarMindAI.Api.Models;
using AntarMindAI.Api.Repositories;

namespace AntarMindAI.Tests.Repositories;

public class InMemoryThoughtRepositoryTests
{
    private readonly InMemoryThoughtRepository _repository;

    public InMemoryThoughtRepositoryTests()
    {
        _repository = new InMemoryThoughtRepository();
    }

    [Fact]
    public async Task CreateAsync_StoresEntryAndReturnsIt()
    {
        var entry = new ThoughtEntry { Id = "1", UserId = "user-a", Text = "Hello", CreatedAt = DateTimeOffset.UtcNow };

        var result = await _repository.CreateAsync(entry);

        Assert.Equal(entry.Id, result.Id);
        Assert.Equal(entry.UserId, result.UserId);
        Assert.Equal(entry.Text, result.Text);
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsOnlyEntriesForUser()
    {
        await _repository.CreateAsync(new ThoughtEntry { Id = "1", UserId = "user-a", Text = "A1", CreatedAt = DateTimeOffset.UtcNow });
        await _repository.CreateAsync(new ThoughtEntry { Id = "2", UserId = "user-b", Text = "B1", CreatedAt = DateTimeOffset.UtcNow });
        await _repository.CreateAsync(new ThoughtEntry { Id = "3", UserId = "user-a", Text = "A2", CreatedAt = DateTimeOffset.UtcNow });

        var (items, total) = await _repository.GetByUserAsync("user-a", page: 1, pageSize: 20);

        Assert.Equal(2, total);
        Assert.All(items, i => Assert.Equal("user-a", i.UserId));
    }

    [Fact]
    public async Task GetByUserAsync_ReturnsInReverseChronologicalOrder()
    {
        var base_ = DateTimeOffset.UtcNow;
        await _repository.CreateAsync(new ThoughtEntry { Id = "1", UserId = "user-a", Text = "Oldest", CreatedAt = base_.AddMinutes(-10) });
        await _repository.CreateAsync(new ThoughtEntry { Id = "2", UserId = "user-a", Text = "Middle", CreatedAt = base_.AddMinutes(-5) });
        await _repository.CreateAsync(new ThoughtEntry { Id = "3", UserId = "user-a", Text = "Newest", CreatedAt = base_ });

        var (items, _) = await _repository.GetByUserAsync("user-a", page: 1, pageSize: 20);

        Assert.Equal("Newest", items[0].Text);
        Assert.Equal("Middle", items[1].Text);
        Assert.Equal("Oldest", items[2].Text);
    }

    [Fact]
    public async Task GetByUserAsync_PaginatesCorrectly()
    {
        var base_ = DateTimeOffset.UtcNow;
        for (int i = 0; i < 5; i++)
        {
            await _repository.CreateAsync(new ThoughtEntry { Id = i.ToString(), UserId = "user-a", Text = $"Thought {i}", CreatedAt = base_.AddMinutes(i) });
        }

        var (page1Items, total) = await _repository.GetByUserAsync("user-a", page: 1, pageSize: 2);
        var (page2Items, _) = await _repository.GetByUserAsync("user-a", page: 2, pageSize: 2);
        var (page3Items, _) = await _repository.GetByUserAsync("user-a", page: 3, pageSize: 2);

        Assert.Equal(5, total);
        Assert.Equal(2, page1Items.Count);
        Assert.Equal(2, page2Items.Count);
        Assert.Single(page3Items);
    }

    [Fact]
    public async Task DeleteAsync_RemovesExistingOwnedEntry_ReturnsTrue()
    {
        await _repository.CreateAsync(new ThoughtEntry { Id = "del-1", UserId = "user-a", Text = "To delete", CreatedAt = DateTimeOffset.UtcNow });

        var result = await _repository.DeleteAsync("del-1", "user-a");

        Assert.True(result);
        var (items, _) = await _repository.GetByUserAsync("user-a", 1, 20);
        Assert.Empty(items);
    }

    [Fact]
    public async Task DeleteAsync_NonExistentEntry_ReturnsFalse()
    {
        var result = await _repository.DeleteAsync("does-not-exist", "user-a");

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_EntryOwnedByAnotherUser_ReturnsFalse()
    {
        await _repository.CreateAsync(new ThoughtEntry { Id = "other-1", UserId = "user-b", Text = "Not yours", CreatedAt = DateTimeOffset.UtcNow });

        var result = await _repository.DeleteAsync("other-1", "user-a");

        Assert.False(result);
    }

    [Fact]
    public async Task GetByUserAsync_EmptyRepository_ReturnsEmptyList()
    {
        var (items, total) = await _repository.GetByUserAsync("user-a", 1, 20);

        Assert.Empty(items);
        Assert.Equal(0, total);
    }

    [Fact]
    public async Task SearchAsync_MatchByText_ReturnsMatchingEntries()
    {
        await _repository.CreateAsync(new ThoughtEntry { Id = "s1", UserId = "user-a", Text = "stressed about trading", Tags = [], CreatedAt = DateTimeOffset.UtcNow });
        await _repository.CreateAsync(new ThoughtEntry { Id = "s2", UserId = "user-a", Text = "great day hiking", Tags = [], CreatedAt = DateTimeOffset.UtcNow });

        var (items, total) = await _repository.SearchAsync("user-a", "trading", 1, 20);

        Assert.Equal(1, total);
        Assert.Single(items);
        Assert.Equal("s1", items[0].Id);
    }
}
