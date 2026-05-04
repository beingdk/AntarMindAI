<!-- Modified by AI on 05/04/2026. Edit #1. -->

# Spec Tasks

## Tasks

- [x] 1. Implement weekly reflection domain models and aggregation service
  - [x] 1.1 Write unit tests for `WeeklyReflectionService`: correct week boundary calculation, category distribution percentages, daily sentiment grouping, peak hour computation, zero-thought week returns empty aggregations
  - [x] 1.2 Create `WeeklyReflection`, `CategoryCount`, `DailySentiment`, `HourlyActivity` records in `src/AntarMindAI.Api/Services/Reflections/`
  - [x] 1.3 Create `IWeeklyReflectionService` interface
  - [x] 1.4 Implement `WeeklyReflectionService` with week boundary logic (Monday–Sunday UTC) and all aggregations
  - [x] 1.5 Register `WeeklyReflectionService` as scoped in `Program.cs`
  - [x] 1.6 Verify all reflection service unit tests pass

- [x] 2. Implement optional AI summary service
  - [x] 2.1 Write unit tests for `NullAiSummaryService` (always returns null) and verify `OpenAiSummaryService` gracefully returns null on exception
  - [x] 2.2 Create `IAiSummaryService` interface
  - [x] 2.3 Implement `NullAiSummaryService` (default — always returns null)
  - [x] 2.4 Implement `OpenAiSummaryService` constructing a prompt from aggregated stats only (no raw thought text); catch all exceptions and return null
  - [x] 2.5 Add `AI:WeeklySummaryEnabled` and `AI:ApiKey` to `appsettings.json` (defaults: false, empty)
  - [x] 2.6 Register correct implementation in `Program.cs` based on `AI:WeeklySummaryEnabled` and `AI:ApiKey` configuration
  - [x] 2.7 Verify AI service unit tests pass

- [x] 3. Implement ReflectionsController
  - [x] 3.1 Write integration tests for `GET /api/reflections/weekly` (no date param, valid date param, invalid date param format, week with no thoughts)
  - [x] 3.2 Create `ReflectionModels.cs` with all response records
  - [x] 3.3 Implement `ReflectionsController` with `GetWeeklyReflectionAsync` action; parse optional `weekDate` query param; return 400 on invalid format
  - [x] 3.4 Verify all controller integration tests pass

- [x] 4. Install chart library and build Dashboard page
  - [x] 4.1 Run `npm install @mui/x-charts@7` in `frontend/` (v7 compatible with MUI v6)
  - [x] 4.2 Create `frontend/src/api/reflections.ts` with `getWeeklyReflection` API client function
  - [x] 4.3 Create `CategoryChart` component using MUI X `PieChart` showing category distribution
  - [x] 4.4 Create `SentimentTrendChart` component using MUI X `LineChart` with 3 series (Positive, Negative, Neutral) by day of week
  - [x] 4.5 Create `ActivityHeatmap` component using MUI X `BarChart` showing thought count by hour
  - [x] 4.6 Create `PatternHighlights` component showing top 3 insights as MUI `Card` items
  - [x] 4.7 Create `DashboardPage` assembling all four components with `useQuery` for reflection data; show `CircularProgress` while loading
  - [x] 4.8 Add AI summary section: show text if `aiSummary` is non-null; show "AI Summary Not Configured" chip if null
  - [x] 4.9 Add `/dashboard` route to `App.tsx`
  - [x] 4.10 Add Dashboard navigation link to `AppHeader`

- [x] 5. Verify full integration and run all tests
  - [x] 5.1 Run `npm run build` — no TypeScript errors (build succeeded)
  - [x] 5.2 Run `dotnet test` — 100 tests passed, 0 failures
  - [x] 5.3 Run `npm run lint` — no ESLint errors
  - [x] 5.4 Manually verify: Dashboard page loads, all four charts render with data, empty state shown when no thoughts exist for the week
