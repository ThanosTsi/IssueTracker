# Step 00 — Repository and environment

## Goal
Make the learning workspace reproducible and explain its purpose before adding
behavior. Corresponds to handbook Phase 0 and the solution setup in Phase 1.

## Concepts to understand first
- SDK versus runtime; solution versus project; project references.
- Build-time package dependencies versus runtime configuration.
- Git working tree, staging, branches and ignored output.
- Pending exercises versus passing regression tests.

## Your task
1. Read README, ROADMAP, AGENTS and step 01. Inspect both `.csproj` files,
   `global.json`, `Program.cs` and `.gitignore`.
2. Run the commands below. Inspect test output and the three HTTP responses.
3. Review README's purpose in your own words and adjust it if needed. Inspect
   `git status --short --ignored` to check that bin/obj are ignored.
4. Review/stage the bootstrap and make the suggested first commit, then use a
   short-lived `feat/project-listing` branch for step 01.
5. Do not add infrastructure dependencies or ask the agent to solve the TODOs.

## TODO markers
No code TODO in step 00. First implementation marker: TODO-WF-001 in step 01.

## Acceptance criteria
- Restore and build succeed without unexplained warnings.
- NUnit discovers six explicitly skipped exercise cases; no business coverage
  is claimed until you implement and activate assertions.
- Host starts; project route returns 501, development OpenAPI 200, unknown route 404.
- Git excludes generated output/local secrets; README matches actual progress.

## Verification commands
```powershell
dotnet --version
./scripts/verify.ps1
dotnet run --project src/IssueTracker.Api --launch-profile http
# In another terminal:
curl.exe -i http://localhost:5062/api/projects
curl.exe -i http://localhost:5062/openapi/v1.json
curl.exe -i http://localhost:5062/api/unknown
git status --short --ignored
```

## Questions you must be able to answer
1. What does `Program.cs` do before the first HTTP request arrives?
2. What differs between SDK and runtime versions?
3. Why does the test project reference the API project?
4. Why can a green test command with six skips provide no behavioral confidence?
5. Which files belong in Git, and where should local credentials live?
6. Which setting enables OpenAPI here, and when is that setting read?

## Obsidian answers before completion
Before completing this section, add or update answers to its questions in
[the Obsidian Q&A note](<C:/Users/thano/Documents/Obsidian Vault/dotNet Learning Project/Questions and Answers.md#Step 00 — Repository and environment>).
Use your actual work as examples and explain technical terms in plain language.
You can ask the agent to help write the answers, then explain them yourself.

## Suggested commit
`build: scaffold IssueTracker learning workspace`

## Do not continue until
You can run the scaffold from the root, explain the intentionally incomplete
paths, and identify the first code exercise without mistaking setup for a feature.
