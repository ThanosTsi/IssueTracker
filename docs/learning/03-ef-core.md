# Step 03 — SQL Server and EF Core

## Goal
Replace hard-coded retrieval with relational persistence you understand.
Handbook commits 11–14. This guide is prepared now; start after steps 01–02.

## Concepts to understand first
- Container ports, DNS, volumes and disposable local infrastructure.
- DbContext options/lifetime, tracking and server-side IQueryable execution.
- Foreign keys, delete behaviors, nullability, constraints and migrations.
- Async database calls, cancellation and local configuration sources.

## Your task
1. **TODO-WF-009:** Inspect `docker-compose.yml.example`, then copy it to
   `docker-compose.yml`. Copy `.env.example` to ignored `.env`. Choose a supported
   SQL Server Linux image with an explicit tag, review its licence, choose a
   loopback host port and supply a local password yourself. Do not share it in
   chat or commit it. Start/connect with your SQL client. Recreate the container
   while retaining the named volume and show the difference from deleting data.
2. **TODO-WF-010:** Add compatible EF Core SQL Server and Design packages to
   `IssueTracker.Api.csproj`. Turn `Persistence/IssueTrackerDbContext.cs` into
   a real DbContext with entity sets/options and explicit entity configurations.
   Decide/explain relationships and delete behaviors; do not add a repository
   layer. SQL indexes are for a later measured performance lesson.
3. **TODO-WF-011:** Edit `Program.cs` and local configuration. Initialize
   user-secrets, set `ConnectionStrings:IssueTracker` locally, register the
   DbContext with an appropriate lifetime and connect. Never copy the actual
   connection string into a tracked file. Explain why the API on the host uses
   the published host port, not the Compose service's container DNS name.
4. **TODO-WF-012:** Install a compatible local `dotnet-ef` tool, generate the
   initial migration, produce/review its SQL, and apply to a disposable database.
   Inspect keys/constraints/nullability. Demonstrate rollback/recreation only
   against this local learning database. Record your inspected schema choices.
5. **TODO-WF-013:** Implement async project retrieval with EF Core in
   `Services/ProjectService.cs`, propagate a cancellation token from the handler,
   and consider no-tracking for read-only queries. Seed data through a local SQL
   script/client, restart the API, and inspect generated SQL. Revisit the earlier
   fixed-data unit test: don't silently turn it into a live database test; retire
   assertions about hard-coded fixtures when their behavior is removed. Real DB
   integration fixtures are introduced in their later phase.
6. Update README with DB startup/stop/configuration instructions and the test
   status. Leave persisted write endpoints to the next CRUD learning phase.

## TODO markers
| ID | Location |
| --- | --- |
| TODO-WF-009 | docker-compose.yml.example and .env.example |
| TODO-WF-010 | Persistence/IssueTrackerDbContext.cs |
| TODO-WF-011 | Persistence/PersistenceExercises.cs; implement wiring in Program.cs |
| TODO-WF-012 | Persistence/PersistenceExercises.cs; generate Migrations/ yourself |
| TODO-WF-013 | Persistence/PersistenceExercises.cs; implement service/handler changes |

## Acceptance criteria
- One documented command starts the local DB; credentials are excluded from Git.
- DB data survives container recreation while its named volume is retained.
- EF schema applies to an empty DB and its generated SQL has been inspected.
- API lists seeded projects after restart; generated SQL is explainable.
- Build/test succeeds with intentional updates to obsolete fixed-data tests.
- No infrastructure deployment, automatic production migration or CRUD solution.

## Verification commands
Only run these after completing the relevant prerequisites. Set secrets locally
using your chosen credential-bearing connection string; no real value is shown here.

```powershell
Copy-Item docker-compose.yml.example docker-compose.yml
Copy-Item .env.example .env
# Fill local .env before continuing.
docker compose up -d sqlserver
docker compose ps
dotnet user-secrets init --project src/IssueTracker.Api
# Set ConnectionStrings:IssueTracker locally using dotnet user-secrets.
# Add EF packages and a local dotnet-ef tool of compatible versions first.
dotnet ef migrations add InitialCreate --project src/IssueTracker.Api
dotnet ef migrations script --project src/IssueTracker.Api --output schema-review.sql
# Read schema-review.sql BEFORE applying to your disposable DB.
dotnet ef database update --project src/IssueTracker.Api
./scripts/verify.ps1
dotnet run --project src/IssueTracker.Api --launch-profile http
# In another terminal, after seeding locally:
curl.exe -i http://localhost:5062/api/projects
docker compose down
```

Reset experiment, **local disposable database only**: `docker compose down -v`
deletes the named SQL volume and its data. Alternatively, after stopping the API,
`dotnet ef database update 0 --project src/IssueTracker.Api` rolls back migrations;
follow with `dotnet ef database update --project src/IssueTracker.Api` to recreate.
Neither reset is performed by the scaffold.

## Questions you must be able to answer
1. What differs between a container's filesystem and a named volume?
2. Why does localhost mean different things on the host and inside a container?
3. Why is DbContext not a singleton?
4. When does an IQueryable actually issue SQL?
5. What does tracking cost, and when is it needed?
6. Which foreign keys and delete behaviors did your migration create?
7. What is your strategy for migrations outside local development?
8. Why do cancellation tokens and async I/O matter to a server?

## Obsidian answers before completion
Before completing this section, add or update answers to its questions in
[the Obsidian Q&A note](<C:/Users/thano/Documents/Obsidian Vault/dotNet Learning Project/Questions and Answers.md#Step 03 — SQL Server and EF Core>).
Use your actual work as examples and explain technical terms in plain language.
You can ask the agent to help write the answers, then explain them yourself.

## Suggested commit
1. `infra: add local SQL Server container`
2. `feat: add Entity Framework Core persistence`
3. `db: create initial database schema`
4. `feat: query persisted projects using Entity Framework Core`

## Do not continue until
You can recreate the local DB, inspect the schema/query SQL, explain each
configuration source/lifetime and show persisted data after restart. Request
the next learning scaffold explicitly; do not auto-advance into CRUD or cloud.
