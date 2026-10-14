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
1. **TODO-WF-005:** Use the separate entity and enum files in `Domain/`: Organisation
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

## Implemented domain rules (TODO-WF-005)
An **invariant** is a rule that stays true for every usable instance of an entity.
The domain checks it at construction and whenever the relevant value changes.
An HTTP request, a background job and a future database integration all have to
respect the same rules, so these checks belong in the entity itself.

- Names and titles cannot be null, empty or whitespace-only.
- Entity ids cannot be negative; 0 represents an unsaved entity. Identity is set
  at initialization and cannot be changed later.
- Parent ids must be positive and are fixed at construction. A positive id does
  not prove that the parent exists or authorize access to it.
- Assignment is optional (`null`); an assigned user id must be positive.
- Status and priority must be defined enum values. Casting `(WorkItemStatus)99`
  is legal C#, so the entity explicitly rejects it. New items start as Todo/Low,
  preserving the earlier enum defaults.
- Descriptions remain optional. Length limits remain your TODO-WF-007 exercise.

Example of using the implemented entities:

```csharp
var project = new Project(1, "Issue Tracker");
var item = new WorkItem(2, "Fix login bug");
item.Title = "Fix sign-in bug"; // Accepted; UpdatedAt is refreshed.
item.Title = "   "; // Throws; the previous title stays intact.
```
The constructor requires valid starting values. The setter checks later edits
too. A non-nullable `string` annotation alone does not reject blank strings or
stop a caller passing null at runtime.

Actual excerpt from the WorkItem title setter:

```csharp
ArgumentException.ThrowIfNullOrWhiteSpace(value);
if (_title == value) return;
_title = value;
Touch();
```
The check runs before assignment. Invalid input cannot replace the existing
title. An identical value makes no change; a changed title updates UpdatedAt.

### Timestamp ownership
`DateTimeOffset` stores a date/time together with its offset from UTC.
`DateTimeOffset.UtcNow` supplies the current instant with a zero offset.
`DateTimeUtc` is not a built-in .NET type.

Actual excerpt from the WorkItem constructor:

```csharp
CreatedAt = DateTimeOffset.UtcNow;
UpdatedAt = CreatedAt;
```
Both timestamps begin at the same instant. CreatedAt has no setter; UpdatedAt
has a private setter. Callers cannot overwrite either timestamp. Changes to
title, description, status, priority or assignment update UpdatedAt.
The private Touch method prevents UpdatedAt from moving backwards if the
system clock is corrected; it may stay equal when the clock has not advanced.

Domain exceptions protect object state. They do not automatically become HTTP
400 responses; boundary validation and HTTP error handling remain TODO-WF-007.
DomainInvariantTests verify these rules without HTTP or SQL. The ignored
TODO-WF-008 tests remain unfinished API validation exercises.

## Implemented API contracts (TODO-WF-006)

Each contract now has its own file in `Contracts/`, with comments explaining
which fields the caller supplies and which fields the server returns.

| Contract | Meaning |
| --- | --- |
| CreateProjectRequest | OrganisationId, Name and optional Description |
| UpdateProjectRequest | Replacement Name and optional Description; project identity comes from the intended route |
| CreateWorkItemRequest | ProjectId, Title and optional Description; domain defaults supply Todo/Low/unassigned |
| ProjectResponse | Id, OrganisationId, Name, Description and CreatedAt |
| WorkItemResponse | Identity, parent, text, status, priority, assignment and both timestamps |

`UpdateProjectRequest` describes replacement, not a partial patch. Null or an
omitted description means clear the description. This documents the intended
future operation; no update endpoint has been implemented.

Actual contract declaration (comments omitted):

```csharp
public sealed record UpdateProjectRequest(
    string Name,
    string? Description = null);
```
The route identifies which project to edit, while this body describes its new
editable values. Excluding Id avoids two competing identities in route and body.
OrganisationId and CreatedAt are not editable through this operation.

ProjectResponse now uses integer ids to match the domain. The existing sample
GET endpoint returns the expanded fields with fixed illustrative timestamps;
the data is still hard-coded. WorkItemResponse reuses the domain enums for now.
Under the current JSON settings those enums serialize as numbers.

The request records only describe input. They do not perform validation,
persist data or verify referenced entities. TODO-WF-007 and TODO-WF-008 remain
unfinished, and the full learning section is not complete.

## TODO markers
| ID | Location |
| --- | --- |
| TODO-WF-005 | Marker in Domain/Organisation.cs; models in Organisation.cs, Project.cs, WorkItem.cs; enums in WorkItemStatus.cs and WorkItemPriority.cs |
| TODO-WF-006 | Marker in Contracts/ContractExercises.cs; worked examples in CreateProjectRequest.cs, UpdateProjectRequest.cs, CreateWorkItemRequest.cs, WorkItemResponse.cs and ProjectResponse.cs |
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
