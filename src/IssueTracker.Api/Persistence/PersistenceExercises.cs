namespace IssueTracker.Api.Persistence;

internal static class PersistenceExercises
{
    // TODO-WF-011 (YOU): Configure a scoped DbContext in Program.cs using a
    // connection string supplied through user-secrets/environment variables.
    // Learning goal: distinguish build-time dependencies from runtime settings.
    // Acceptance: development API can connect; no credentials enter Git.

    // TODO-WF-012 (YOU): Generate the initial migration and inspect its SQL.
    // Learning goal: version a schema and examine constraints/indexes.
    // Acceptance: migrate an empty disposable DB, roll back, then recreate it.

    // TODO-WF-013 (YOU): Replace fixed data with an async EF query and
    // propagate cancellation from HTTP. Understand server-side queries/tracking.
    // Acceptance: seeded projects survive restarts; inspect generated SQL.
    // Leave write endpoints for the later CRUD phase.
}
