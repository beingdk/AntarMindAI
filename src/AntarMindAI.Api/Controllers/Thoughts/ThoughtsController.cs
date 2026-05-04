// Modified by AI on 05/04/2026. Edit #1.
using AntarMindAI.Api.Models;
using AntarMindAI.Api.Repositories;
using AntarMindAI.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AntarMindAI.Api.Controllers.Thoughts;

[ApiController]
[Route("api/thoughts")]
[Authorize]
public class ThoughtsController : ControllerBase
{
    private readonly IThoughtRepository _repository;
    private readonly ICurrentUserService _currentUser;
    private readonly IThoughtAnalysisPipeline _pipeline;

    public ThoughtsController(
        IThoughtRepository repository,
        ICurrentUserService currentUser,
        IThoughtAnalysisPipeline pipeline)
    {
        _repository = repository;
        _currentUser = currentUser;
        _pipeline = pipeline;
    }

    [HttpPost]
    public async Task<IActionResult> CreateThoughtAsync([FromBody] CreateThoughtRequest request)
    {
        Response.Headers.CacheControl = "no-store";

        if (string.IsNullOrWhiteSpace(request.Text) || request.Text.Length > 2000)
        {
            return BadRequest(new
            {
                correlationId = Guid.NewGuid().ToString(),
                status = "400",
                title = "Validation Error",
                detail = "Text must be between 1 and 2000 characters.",
                source = new { text = "Text must be between 1 and 2000 characters." }
            });
        }

        var userId = _currentUser.GetUserId() ?? "anonymous";
        var trimmedText = request.Text.Trim();
        var analysis = _pipeline.Analyze(trimmedText);

        var entry = new ThoughtEntry
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Text = trimmedText,
            CreatedAt = DateTimeOffset.UtcNow,
            Tags = analysis.Tags,
            Sentiment = analysis.Sentiment,
            IntensityScore = analysis.IntensityScore
        };

        var created = await _repository.CreateAsync(entry);

        return Created($"/api/thoughts/{created.Id}", MapToResponse(created));
    }

    [HttpGet]
    public async Task<IActionResult> GetThoughtsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        Response.Headers.CacheControl = "no-store";

        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var userId = _currentUser.GetUserId() ?? "anonymous";
        var (items, total) = await _repository.GetByUserAsync(userId, page, pageSize);

        var response = new PagedThoughtsResponse(
            Items: items.Select(MapToResponse).ToList(),
            TotalCount: total,
            Page: page,
            PageSize: pageSize
        );

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetThoughtByIdAsync(string id)
    {
        Response.Headers.CacheControl = "no-store";

        var userId = _currentUser.GetUserId() ?? "anonymous";
        var thought = await _repository.GetByIdAsync(id, userId);

        if (thought is null)
        {
            return NotFound(new
            {
                correlationId = Guid.NewGuid().ToString(),
                status = "404",
                title = "Not Found",
                detail = $"Thought with id '{id}' was not found or does not belong to the current user."
            });
        }

        return Ok(MapToResponse(thought));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteThoughtAsync(string id)
    {
        Response.Headers.CacheControl = "no-store";

        var userId = _currentUser.GetUserId() ?? "anonymous";
        var deleted = await _repository.DeleteAsync(id, userId);

        if (!deleted)
        {
            return NotFound(new
            {
                correlationId = Guid.NewGuid().ToString(),
                status = "404",
                title = "Not Found",
                detail = $"Thought with id '{id}' was not found or does not belong to the current user."
            });
        }

        return NoContent();
    }

    private static ThoughtResponse MapToResponse(ThoughtEntry e) =>
        new(e.Id, e.Text, e.CreatedAt, e.Tags, e.Sentiment, e.IntensityScore);
}
