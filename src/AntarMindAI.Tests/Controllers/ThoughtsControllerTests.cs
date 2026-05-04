// Modified by AI on 05/04/2026. Edit #1.
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AntarMindAI.Api.Controllers.Thoughts;

namespace AntarMindAI.Tests.Controllers;

public class ThoughtsControllerTests : IClassFixture<TestApiFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public ThoughtsControllerTests(TestApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ── POST /api/thoughts ──────────────────────────────────────────────────

    [Fact]
    public async Task CreateThought_WithValidText_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/api/thoughts", new CreateThoughtRequest("Hello, world!"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateThought_ReturnsThoughtResponseBody()
    {
        var response = await _client.PostAsJsonAsync("/api/thoughts", new CreateThoughtRequest("My first thought"));
        var body = await response.Content.ReadFromJsonAsync<ThoughtResponse>(_jsonOptions);

        Assert.NotNull(body);
        Assert.NotEmpty(body.Id);
        Assert.Equal("My first thought", body.Text);
        Assert.True(body.CreatedAt > DateTimeOffset.MinValue);
    }

    [Fact]
    public async Task CreateThought_ReturnsEnrichedFields()
    {
        var response = await _client.PostAsJsonAsync("/api/thoughts", new CreateThoughtRequest("I had a great trading day with profit."));
        var body = await response.Content.ReadFromJsonAsync<ThoughtResponse>(_jsonOptions);

        Assert.NotNull(body);
        Assert.NotNull(body.Tags);
        Assert.NotNull(body.Sentiment);
        Assert.InRange(body.IntensityScore, 0.0, 1.0);
    }

    [Fact]
    public async Task CreateThought_TradingText_ReturnsTagsIncludingTrading()
    {
        var response = await _client.PostAsJsonAsync("/api/thoughts", new CreateThoughtRequest("My stock portfolio had a great trading session."));
        var body = await response.Content.ReadFromJsonAsync<ThoughtResponse>(_jsonOptions);

        Assert.NotNull(body);
        Assert.Contains("Trading", body.Tags);
    }

    [Fact]
    public async Task CreateThought_WithEmptyText_Returns400WithStandardError()
    {
        var response = await _client.PostAsJsonAsync("/api/thoughts", new CreateThoughtRequest(""));
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.TryGetProperty("correlationId", out _));
        Assert.True(body.TryGetProperty("title", out _));
    }

    [Fact]
    public async Task CreateThought_WithTextOver2000Chars_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/thoughts", new CreateThoughtRequest(new string('a', 2001)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateThought_ResponseHasCacheControlNoStore()
    {
        var response = await _client.PostAsJsonAsync("/api/thoughts", new CreateThoughtRequest("Cache test"));

        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
    }

    // ── GET /api/thoughts ───────────────────────────────────────────────────

    [Fact]
    public async Task GetThoughts_ReturnsPagedResponse()
    {
        var response = await _client.GetAsync("/api/thoughts");
        var body = await response.Content.ReadFromJsonAsync<PagedThoughtsResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.NotNull(body.Items);
    }

    [Fact]
    public async Task GetThoughts_ResponseHasCacheControlNoStore()
    {
        var response = await _client.GetAsync("/api/thoughts");

        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
    }

    [Fact]
    public async Task GetThoughts_AfterCreatingThought_IncludesIt()
    {
        await _client.PostAsJsonAsync("/api/thoughts", new CreateThoughtRequest("Retrievable thought"));

        var response = await _client.GetAsync("/api/thoughts");
        var body = await response.Content.ReadFromJsonAsync<PagedThoughtsResponse>(_jsonOptions);

        Assert.NotNull(body);
        Assert.Contains(body.Items, t => t.Text == "Retrievable thought");
    }

    // ── GET /api/thoughts/{id} ───────────────────────────────────────────────

    [Fact]
    public async Task GetThoughtById_ExistingOwned_Returns200WithBody()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/thoughts", new CreateThoughtRequest("Fetch by id"));
        var created = await createResponse.Content.ReadFromJsonAsync<ThoughtResponse>(_jsonOptions);

        var response = await _client.GetAsync($"/api/thoughts/{created!.Id}");
        var body = await response.Content.ReadFromJsonAsync<ThoughtResponse>(_jsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(created.Id, body.Id);
        Assert.Equal("Fetch by id", body.Text);
    }

    [Fact]
    public async Task GetThoughtById_NonExistent_Returns404()
    {
        var response = await _client.GetAsync("/api/thoughts/does-not-exist");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.True(body.TryGetProperty("correlationId", out _));
    }

    [Fact]
    public async Task GetThoughtById_ResponseHasCacheControlNoStore()
    {
        var response = await _client.GetAsync("/api/thoughts/does-not-exist");

        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
    }

    // ── DELETE /api/thoughts/{id} ────────────────────────────────────────────

    [Fact]
    public async Task DeleteThought_ExistingOwned_Returns204()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/thoughts", new CreateThoughtRequest("To delete"));
        var created = await createResponse.Content.ReadFromJsonAsync<ThoughtResponse>(_jsonOptions);

        var deleteResponse = await _client.DeleteAsync($"/api/thoughts/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteThought_NonExistent_Returns404WithStandardError()
    {
        var response = await _client.DeleteAsync("/api/thoughts/non-existent-id");
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(_jsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.True(body.TryGetProperty("correlationId", out _));
        Assert.True(body.TryGetProperty("title", out _));
    }

    [Fact]
    public async Task DeleteThought_ResponseHasCacheControlNoStore()
    {
        var response = await _client.DeleteAsync("/api/thoughts/non-existent-id");

        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
    }
}
