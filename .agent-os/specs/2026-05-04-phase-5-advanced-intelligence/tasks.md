<!-- Modified by AI on 05/05/2026. Edit #1. -->

# Spec Tasks

## Tasks

- [x] 1. Implement full-text thought search
  - [x] 1.1 Write integration tests for `GET /api/thoughts/search` (match by text, match by tag, empty results, missing q param returns 400)
  - [x] 1.2 Add `SearchThoughtsAsync` action to `ThoughtsController` with case-insensitive in-memory filtering
  - [x] 1.3 Extend `IThoughtRepository` with `SearchAsync(userId, query, page, pageSize)` method; implement in both `InMemoryThoughtRepository` and `AzureTableStorageThoughtRepository`
  - [x] 1.4 Verify all search integration tests pass

- [x] 2. Implement data export endpoint
  - [x] 2.1 Write integration tests for `GET /api/export?format=csv` (correct headers, valid CSV content) and `GET /api/export?format=json` (valid JSON array)
  - [x] 2.2 Create `ExportController` with `ExportThoughtsAsync` action
  - [x] 2.3 Implement CSV generation using `StringBuilder`; include proper encoding for fields containing commas or quotes
  - [x] 2.4 Set correct `Content-Type` and `Content-Disposition` headers with dated filename
  - [x] 2.5 Verify all export integration tests pass

- [x] 3. Implement cognitive bias detection
  - [x] 3.1 Write unit tests for `CognitiveBiasDetector`: catastrophizing pattern triggers on ≥ 3 matching thoughts, overgeneralizing pattern triggers independently, below-threshold produces no insights
  - [x] 3.2 Create `ICognitiveBiasDetector` interface
  - [x] 3.3 Add bias pattern keyword lists to `appsettings.json` under `CognitiveBiasDetection:Patterns`
  - [x] 3.4 Implement `CognitiveBiasDetector` reading patterns from `IConfiguration`
  - [x] 3.5 Register `CognitiveBiasDetector` as singleton; integrate into `InsightService` alongside existing analyzers
  - [x] 3.6 Verify all cognitive bias unit tests pass

- [x] 4. Implement cross-domain correlation insights
  - [x] 4.1 Write unit tests for `CrossCorrelationAnalyzer`: tag co-occurrence rate > 60% generates insight, time-windowed stress/trading correlation detected, below-threshold scenarios produce no insights
  - [x] 4.2 Create `ICrossCorrelationAnalyzer` interface
  - [x] 4.3 Implement `CrossCorrelationAnalyzer` with co-occurrence rate computation and 2-hour time-window correlation
  - [x] 4.4 Register `CrossCorrelationAnalyzer` as singleton; integrate into `InsightService`
  - [x] 4.5 Verify all correlation unit tests pass

- [x] 5. Implement personalized recommendations service
  - [x] 5.1 Write unit tests for `RecommendationService`: evening stress pattern triggers correct recommendation, trading-heavy focus triggers correct recommendation, no patterns = no recommendations
  - [x] 5.2 Create `IRecommendationService` interface
  - [x] 5.3 Implement `RecommendationService` with template-based recommendations driven by insight categories and weekly reflection data
  - [x] 5.4 Integrate recommendations into `WeeklyReflectionService` and add `recommendations: string[]` to `WeeklyReflectionResponse`
  - [x] 5.5 Verify all recommendation unit tests pass

- [x] 6. Implement embedding-based similarity detection
  - [x] 6.1 Write unit tests for `NullEmbeddingService` (returns empty similar thoughts) and a mock `IEmbeddingService` verifying cosine similarity math
  - [x] 6.2 Create `IEmbeddingService` interface and `SimilarThought` record
  - [x] 6.3 Implement `NullEmbeddingService` (always returns empty array)
  - [x] 6.4 Implement `AzureOpenAiEmbeddingService` with background embedding generation and cosine similarity in-memory search
  - [x] 6.5 Extend `ThoughtEntry` with `float[]? Embedding` property; update both repository implementations to store/retrieve Base64-encoded embeddings
  - [x] 6.6 Add `AI:EmbeddingsEnabled` and `AI:EmbeddingsApiKey` to `appsettings.json`; register correct service in `Program.cs`
  - [x] 6.7 Extend `GET /api/thoughts/{id}` response with `similarThoughts` array
  - [x] 6.8 Verify embedding unit tests pass; verify POST response is not delayed by background embedding generation

- [x] 7. Implement voice-to-text input on frontend
  - [x] 7.1 Extend `ThoughtInput` component with a microphone `IconButton` using the Web Speech API
  - [x] 7.2 Hide microphone button when `'SpeechRecognition' in window === false`
  - [x] 7.3 Show pulsing recording indicator while recognition is active; provide stop button
  - [x] 7.4 Populate text field with interim results; finalize on `result` event
  - [x] 7.5 Run `npm run build` — verify no TypeScript errors

- [x] 8. Verify full integration and run all tests
  - [x] 8.1 Run `dotnet test` — confirm all tests pass with 0 failures
  - [x] 8.2 Run `npm run lint` — confirm no ESLint errors
  - [x] 8.3 Manually verify: search returns correct results, export downloads a valid file, cognitive bias insights appear after logging matching thoughts, voice input works in Chrome
