<!-- Modified by AI on 05/04/2026. Edit #1. -->

# Spec Tasks

## Tasks

- [x] 1. Define shared analysis domain models and context
  - [x] 1.1 Write unit tests confirming `PatternAnalysisContext` correctly surfaces thought count and time window
  - [x] 1.2 Create `InsightMessage` record and `PatternAnalysisContext` class in `src/AntarMindAI.Api/Services/Insights/`
  - [x] 1.3 Define `IFrequencyAnalyzer`, `ITimeTrendAnalyzer`, `IRepetitionDetector`, `ITriggerIdentifier` interfaces
  - [x] 1.4 Verify models compile and tests pass

- [x] 2. Implement frequency analysis engine
  - [x] 2.1 Write unit tests: dominant category ≥ 25%, multi-tag distribution, below-threshold returns no insights, empty thought list
  - [x] 2.2 Implement `FrequencyAnalyzer` with per-tag percentage calculation over configurable time window
  - [x] 2.3 Add `PatternDetection:FrequencyWindowDays` (default: 7) to `appsettings.json`
  - [x] 2.4 Register `FrequencyAnalyzer` as singleton in `Program.cs`
  - [x] 2.5 Verify all frequency unit tests pass

- [x] 3. Implement time-based trend detection
  - [x] 3.1 Write unit tests: peak negative hour window identified, peak activity day identified, sparse data (< 5 thoughts) produces no trend insights
  - [x] 3.2 Implement `TimeTrendAnalyzer` grouping thoughts by hour-of-day and day-of-week
  - [x] 3.3 Register `TimeTrendAnalyzer` as singleton in `Program.cs`
  - [x] 3.4 Verify all time trend unit tests pass

- [x] 4. Implement repetition detection
  - [x] 4.1 Write unit tests: cluster of 3 similar thoughts generates insight, unrelated thoughts produce no cluster, Jaccard similarity calculation correctness
  - [x] 4.2 Implement `RepetitionDetector` with Jaccard similarity + union-find clustering
  - [x] 4.3 Add `PatternDetection:RepetitionSimilarityThreshold` (default: 0.3) to `appsettings.json`
  - [x] 4.4 Ensure `StringBuilder` is used for cluster summary string construction
  - [x] 4.5 Register `RepetitionDetector` as singleton in `Program.cs`
  - [x] 4.6 Verify all repetition detection unit tests pass

- [x] 5. Implement trigger identification
  - [x] 5.1 Write unit tests: stress trigger identified when ≥ 50% negative rate for a tag, co-occurrence insight generated, below-threshold produces no trigger insights
  - [x] 5.2 Implement `TriggerIdentifier` with per-domain negative rate and co-occurrence analysis
  - [x] 5.3 Register `TriggerIdentifier` as singleton in `Program.cs`
  - [x] 5.4 Verify all trigger unit tests pass

- [x] 6. Implement InsightService orchestrator and InsightsController
  - [x] 6.1 Write unit tests for `InsightService`: deduplication, top-10 cap, empty return when below minimum thought threshold
  - [x] 6.2 Write integration tests for `GET /api/insights` (authenticated, empty result, populated result)
  - [x] 6.3 Create `IInsightService` interface and implement `InsightService` composing all 4 analyzers
  - [x] 6.4 Add `PatternDetection:MinimumThoughtsForInsights` (default: 10) to `appsettings.json`
  - [x] 6.5 Register `InsightService` as scoped in `Program.cs`
  - [x] 6.6 Create `InsightModels.cs` with response records
  - [x] 6.7 Implement `InsightsController` with `GetInsightsAsync` action
  - [x] 6.8 Verify all integration tests pass

- [x] 7. Build frontend insights panel
  - [x] 7.1 Add `getInsights` API client function to `frontend/src/api/insights.ts`
  - [x] 7.2 Create `InsightsPanel` component using `useQuery`; render MUI `List` with insight messages; show empty state when insights array is empty
  - [x] 7.3 Move `InsightsPanel` to dedicated `/insights` route with its own page and nav item
  - [x] 7.4 Run `npm run build` — verify no TypeScript errors
  - [x] 7.5 Manually verify: insights appear after logging 10+ thoughts; empty state shown with fewer thoughts

- [x] 8. Verify full integration and run all tests
  - [x] 8.1 Run `dotnet test` — confirm all tests pass with 0 failures (87 passed)
  - [x] 8.2 Run `npm run lint` — confirm no ESLint errors
