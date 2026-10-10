# IssueTracker

A portfolio and learning project for an experienced C# desktop developer
transitioning into ASP.NET Core backend, Azure and container engineering.
The eventual product is a multi-tenant work-management API with organisations,
projects and work items. Architecture evolves only when requirements justify it.

This bootstrap follows sections 37–39 of **A .NET Backend & Azure Transition
Handbook** in the Obsidian vault. It creates learning steps 00–03; the human
implements the meaningful behavior. See [AGENTS.md](AGENTS.md) for agent rules.

## Start here

Install the .NET SDK selected by [global.json](global.json): 10.0.401, or a
newer patch in the same SDK feature band. .NET 10 is an LTS release; see the
[Microsoft support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core).
Docker and SQL Server are needed only when you reach step 03.

From the repository root:

```powershell
dotnet restore IssueTracker.sln
dotnet build IssueTracker.sln --no-restore
dotnet test IssueTracker.sln --no-build --no-restore
dotnet run --project src/IssueTracker.Api --launch-profile http
```

In another terminal:

```powershell
curl.exe -i http://localhost:5062/api/projects
curl.exe -i http://localhost:5062/openapi/v1.json
curl.exe -i http://localhost:5062/api/unknown
```

The implemented routes return **200**, **200** and **404**, respectively
(OpenAPI is available in Development). `/api/projects` returns a JSON
collection of two sample id/name pairs through an injected `ProjectService`.
`/` has no UI and returns 404. The `.http` file also contains these requests.

OpenAPI JSON is enabled only in Development; no Swagger UI is installed.
The launch profile uses loopback HTTP for this first local lesson. This is a
learning scaffold, not a deployable authenticated SaaS application.

Build/test shortcut: `./scripts/verify.ps1` on PowerShell, or
`sh scripts/verify.sh` on a POSIX shell. The project service test now contains
active assertions for the two sample projects. The five validation test cases
remain ignored exercises; skipped tests are not completed test coverage.

## Learning sequence

| Step | Guide | Outcome |
| --- | --- | --- |
| 00 | [Repository](docs/learning/00-repository.md) | Run the scaffold and review repository hygiene |
| 01 | [ASP.NET foundations](docs/learning/01-aspnet-foundations.md) | Hard-coded GET, service extraction, NUnit, DI |
| 02 | [Domain and validation](docs/learning/02-domain-and-validation.md) | Entities, DTOs and deliberate input boundaries |
| 03 | [EF Core](docs/learning/03-ef-core.md) | Local SQL, schema migrations and persisted queries |

Step 01's code exercises **TODO-WF-001** through **TODO-WF-004** are marked
`DONE`. Read the guide's explanation of service registration and request
lifetimes, then complete its verification and mastery gate before advancing.

## Repository layout

```text
IssueTracker.sln
AGENTS.md / README.md / ROADMAP.md / global.json
src/IssueTracker.Api/
  Program.cs / IssueTracker.Api.http
  Contracts/    id/name response plus DTO exercises
  Domain/       entity exercises
  Services/     ProjectService exercise
  Persistence/  EF shell and learning markers (no EF dependency yet)
tests/IssueTracker.Api.Tests/
  ProjectServiceTests.cs / ProjectValidationTests.cs
docs/
  learning/     steps 00–03
  decisions/    ADR template
scripts/        PowerShell and shell verification
infra/
  terraform/                    reserved
  kubernetes/base/               reserved
  kubernetes/overlays/local/     reserved
docker-compose.yml.example / .env.example
```

The host, development OpenAPI, sample project retrieval, service injection and
sample-data unit test are implemented. Domain fields, validation and
persistence remain exercises. Infrastructure directories contain `.gitkeep`
files only; the Compose file is a later local SQL exercise template.

## Configuration and workflow

`appsettings.json` contains non-secret logging settings. Never place passwords,
tokens or credential-bearing connection strings in tracked files. Use
user-secrets or environment variables when step 03 introduces the database.
Copy `.env.example` to ignored `.env` only when needed, and fill it locally.
User-secrets are a development convenience, not a production secret store.

Git is initialized on `main`; no commits or remote are created by the bootstrap.
Review the files, then make the first commit:

```powershell
git status --short
git add .
git diff --cached --check
git commit -m "build: scaffold IssueTracker learning workspace"
git switch -c feat/project-listing
```

Use small, coherent commits. After completing a TODO, run verification, ask for
a review, fix issues yourself, then commit. Request the next scaffold explicitly
after meeting the current guide's mastery gate. Future phases are listed in
[ROADMAP.md](ROADMAP.md).

When you finish a learning section, answer its questions in the Obsidian note
[Questions and Answers](<C:/Users/thano/Documents/Obsidian Vault/dotNet Learning Project/Questions and Answers.md>).
Ask the agent to add or update explanations based on your actual work, with
technical terms explained in plain language. Then practise explaining them
yourself. Steps 00 and 01 have reference answers; steps 02 and 03 contain only
questions. Written answers do not replace practical verification or the mastery gate.
