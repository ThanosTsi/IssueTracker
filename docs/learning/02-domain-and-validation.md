# Step 02 — Domain, contracts and validation

## Goal
Model the business vocabulary and reject invalid requests deliberately without
coupling API contracts to a future database. Handbook commits 7–10.

## Concepts to understand first
- Entity identity, relationships, invariants and UTC timestamps.
- DTOs, mutable versus server-owned fields, and over-posting.
- Input validation versus business rules; boundary-value tests.
- Deliberate 400 responses and consistent error shapes.

## Your task
1. **TODO-WF-005:** Fill or split `Domain/DomainExercises.cs`: Organisation
   has Id/Name/CreatedAt; Project adds OrganisationId/Description; WorkItem has
   Id/ProjectId/Title/Description/Status/Priority/AssignedUserId/CreatedAt/UpdatedAt.
   Add Todo/InProgress/Done and Low/Medium/High/Critical enums. Choose types and
   nullability yourself; explain which invariants belong in domain code.
2. **TODO-WF-006:** Fill or split `Contracts/ContractExercises.cs`, and extend
   `ProjectResponse.cs` where justified. Define the create/update project and
   create/response work-item contracts. Keep server-owned fields out of write DTOs.
3. **TODO-WF-007:** Choose/document minimum and maximum project-name lengths
   and description limits. Implement validation, then wire a temporary
   `POST /api/projects` path in `Program.cs` that returns consistent 400 for
   invalid input. Valid input may return an explicit 501 persistence placeholder
   until the CRUD phase; it must not claim creation or persistence yet. This path
   exists to exercise validation through HTTP, not to build business operations.
4. **TODO-WF-008:** Replace ignored bodies in `ProjectValidationTests.cs` with
   actual assertions. Add min-1/min/max/max+1 cases and description boundaries,
   and valid/null/blank/missing cases as appropriate. Remove `Ignore` only after
   the tests assert the behavior. Keep these tests independent of SQL/network.
5. Edit README/contracts documentation to explain your chosen limits and
   temporary valid-input response. Do not let the agent implement these choices.

## TODO markers
| ID | Location |
| --- | --- |
| TODO-WF-005 | Domain/DomainExercises.cs |
| TODO-WF-006 | Contracts/ContractExercises.cs and ProjectResponse.cs |
| TODO-WF-007 | Contracts/ContractExercises.cs; implement HTTP wiring in Program.cs |
| TODO-WF-008 | tests/IssueTracker.Api.Tests/ProjectValidationTests.cs |

## Acceptance criteria
- Models and contracts compile, are distinct, and use WorkItem rather than Task.
- Missing/blank and out-of-range project input returns 400 consistently.
- Valid input passes validation but does not claim persisted creation yet.
- Active unit tests cover chosen boundaries; manual HTTP checks verify wiring.
- No EF Core or navigation-property complexity has been introduced yet.

## Verification commands
```powershell
./scripts/verify.ps1
dotnet test --filter FullyQualifiedName~ProjectValidationTests
dotnet run --project src/IssueTracker.Api --launch-profile http
# In another terminal; adapt payloads if you deliberately change the contract:
Invoke-WebRequest http://localhost:5062/api/projects -Method Post -ContentType application/json -Body '{}'
Invoke-WebRequest http://localhost:5062/api/projects -Method Post -ContentType application/json -Body '{"name":"   "}'
# Also send min-1/min/max/max+1 and your chosen valid request.
```
PowerShell may throw on non-success HTTP codes; inspect the exception's response
status/body. A thrown client exception does not mean the server failed to respond.

## Questions you must be able to answer
1. What makes an entity different from a DTO?
2. Which fields should a caller be allowed to set?
3. What does OrganisationId represent before authentication is introduced?
4. Which invariants survive a change of transport or storage technology?
5. Why test both sides of each length boundary?
6. How do absent, null, empty and whitespace-only values differ?
7. Where does malformed JSON get rejected compared with a business rule?

## Obsidian answers before completion
Before completing this section, add or update answers to its questions in
[the Obsidian Q&A note](<C:/Users/thano/Documents/Obsidian Vault/dotNet Learning Project/Questions and Answers.md#Step 02 — Domain, contracts and validation>).
Use your actual work as examples and explain technical terms in plain language.
You can ask the agent to help write the answers, then explain them yourself.

## Suggested commit
1. `feat: add organisation project and work item domain models`
2. `feat: add project and work item API contracts`
3. `feat: validate project requests`
4. `test: cover project request validation`

## Do not continue until
You can explain every field and boundary, demonstrate invalid input through
HTTP, and show meaningful active NUnit assertions. Ask for review before step 03.
