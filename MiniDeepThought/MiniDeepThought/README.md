# Mini Deep Thought

A tiny .NET 8 console app that lets you submit an "Ultimate Question", pick a toy algorithm, shows progress (with optional cancellation), and persists job history to a local JSON file (`deepthought-jobs.json`). Includes a small xUnit test suite.

## Quickstart

```bash
# Create solution & projects
mkdir MiniDeepThought && cd MiniDeepThought

dotnet new sln -n MiniDeepThought

dotnet new console -n MiniDeepThought -o src/MiniDeepThought
# Enable nullable and implicit usings (already true by default in .NET 8 templates)

# Add xUnit test project
 dotnet new xunit -n MiniDeepThought.Tests -o tests/MiniDeepThought.Tests

# Add project refs
 dotnet sln add src/MiniDeepThought/MiniDeepThought.csproj
 dotnet sln add tests/MiniDeepThought.Tests/MiniDeepThought.Tests.csproj
 dotnet add tests/MiniDeepThought.Tests/MiniDeepThought.Tests.csproj reference src/MiniDeepThought/MiniDeepThought.csproj

# Replace generated files with the ones in this repo layout
# (copy the source files above into the specified folders)

# Run tests
 dotnet test

# Run the app
 dotnet run --project src/MiniDeepThought
```

## Menu
```
1) Submit Question
2) List Jobs
3) View Result by JobId
4) Cancel Running Job
5) Exit
```

## Notes
- Strategies implement a single `RunAsync` that reports `IProgress<int>` and honors `CancellationToken`.
- `JobRunner` persists before start and after finish/cancel/fail.
- `JobStore` writes/reads a single JSON file. JSON uses enum-as-string.
- Delays are provided by a tiny `IClock` for fast tests.
- Validation: 1–200 char question; algorithm in {Trivial, SlowCount, RandomGuess}.

## Acceptance Checklist
- [x] Submit & Complete with Trivial => 100% progress, Answer="42", saved
- [x] Cancel Running Job => Status=Canceled, Progress<100%
- [x] View Result => shows `answer`, `summary`, `durationMs`
- [x] List Jobs => prints table of statuses and progress

## Possible Extensions
- Per-job audit log
- Resume incomplete job
- Spinner while computing
- Composite strategy that chains two strategies