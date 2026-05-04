// Modified by AI on 05/04/2026. Edit #3.
using AntarMindAI.Api.Auth;
using AntarMindAI.Api.Repositories;
using AntarMindAI.Api.Services;
using AntarMindAI.Api.Services.Insights;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Register ICurrentUserService — LocalDev in Development/Testing, real in production
if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddScoped<ICurrentUserService, LocalDevCurrentUserService>();
}
else
{
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
}

// Custom EasyAuth authentication scheme that delegates to ICurrentUserService
builder.Services.AddAuthentication("EasyAuth")
    .AddScheme<AuthenticationSchemeOptions, EasyAuthHandler>("EasyAuth", null);
builder.Services.AddAuthorization();

// Register IThoughtRepository — Azure Table Storage if connection string is set, else in-memory
var tableConnectionString = Environment.GetEnvironmentVariable("AZURE_TABLE_STORAGE_CONNECTION_STRING")
    ?? builder.Configuration["AzureTableStorage:ConnectionString"]
    ?? string.Empty;

if (!string.IsNullOrWhiteSpace(tableConnectionString))
{
    builder.Services.AddSingleton<IThoughtRepository>(_ =>
        new AzureTableStorageThoughtRepository(tableConnectionString));
}
else
{
    builder.Services.AddSingleton<IThoughtRepository, InMemoryThoughtRepository>();
}

// Register intelligence pipeline services
builder.Services.AddSingleton<ITextPreprocessor, TextPreprocessor>();
builder.Services.AddSingleton<ITaggingEngine, RuleBasedTaggingEngine>();
builder.Services.AddSingleton<ISentimentAnalyzer, RuleBasedSentimentAnalyzer>();
builder.Services.AddSingleton<IThoughtAnalysisPipeline, ThoughtAnalysisPipeline>();

// Register insights engine analyzers and service
builder.Services.AddSingleton<IFrequencyAnalyzer, FrequencyAnalyzer>();
builder.Services.AddSingleton<ITimeTrendAnalyzer, TimeTrendAnalyzer>();
builder.Services.AddSingleton<IRepetitionDetector, RepetitionDetector>();
builder.Services.AddSingleton<ITriggerIdentifier, TriggerIdentifier>();
builder.Services.AddScoped<IInsightService, InsightService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Serve the compiled React frontend from wwwroot/ui
app.UseStaticFiles();
app.MapControllers();

// Fall back to index.html for SPA client-side routing
app.MapFallbackToFile("ui/index.html");

app.Run();

// Required for WebApplicationFactory in integration tests
public partial class Program { }
