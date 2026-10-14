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
5. Keep README/contracts documentation aligned with the chosen limits and
   temporary valid-input response. TODO-WF-007 was implemented by the agent
   at your explicit request; explain its rules yourself before the mastery gate.

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
- Descriptions remain optional. The HTTP boundary now applies the length limits
  documented below in TODO-WF-007.

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
400 responses; TODO-WF-007 now performs explicit boundary validation and HTTP
error handling as documented below.
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
persist data or verify referenced entities. TODO-WF-007 now validates incoming
create-project requests. TODO-WF-008 and the full section mastery gate remain unfinished.

## Implemented project input validation (TODO-WF-007)

These are explicit learning-project choices, not limits imposed by .NET:

| Input | Rule |
| --- | --- |
| OrganisationId | Positive integer; missing, zero and negative values are rejected |
| Name | Required; 3–100 UTF-16 code units after trimming surrounding whitespace |
| Description | Optional; at most 2,000 UTF-16 code units including whitespace |

C# `string.Length` counts UTF-16 code units; some characters, such as emoji,
use two units. Name trimming is used for validation only: the DTO is not
modified, and no trimmed value is persisted by this placeholder endpoint.
Descriptions may be absent, null, empty or whitespace-only if within the limit.
The name range provides a simple meaningful boundary exercise; the description
cap keeps free-form text bounded. Revisit these product choices when requirements emerge.

Actual validator excerpt:

```csharp
var name = request.Name?.Trim();
if (string.IsNullOrEmpty(name))
    errors["name"] = ["Name is required and cannot be blank."];
else if (name.Length < MinNameLength || name.Length > MaxNameLength)
    errors["name"] = [$"Name must contain between {MinNameLength} and {MaxNameLength} characters after trimming."];
```
`?.` allows the validator to handle missing/null input at runtime even though
the request property is annotated as non-nullable. The validator returns
field-specific messages, rather than constructing a domain entity or saving data.

Actual endpoint excerpt (shortened):

```csharp
var errors = ProjectRequestValidation.Validate(request);
if (errors.Count > 0)
    return Results.ValidationProblem(errors);

return Results.Problem(
    statusCode: StatusCodes.Status501NotImplemented,
    title: "Project persistence is not implemented",
    detail: "The request passed validation. No project was created or saved.");
```
Invalid JSON and wrong field types fail during ASP.NET Core binding before
this handler runs. RouteHandlerOptions.ThrowOnBadRequest is explicitly enabled
in both environments, and middleware catches HTTP 400 binding failures to
return the same Validation Problem Details structure, using a `request` error key.
Invalid field values use their own field names. All these errors return
`application/problem+json` with status 400 and an `errors` object.

A valid request returns 501 Problem Details. It does not return 201, assign an
id, check that the organisation exists, or modify the sample GET results.
Unsupported content types remain ASP.NET Core HTTP 415 responses.

HTTP verification passed for 28 POST cases in each of Development and
Production, covering name min-1/min/max/max+1, description max-1/max/max+1,
omitted/null/blank values, nonpositive organisation ids and malformed/wrong-type
JSON. GET still returned 200 and an unknown route returned 404. These are manual
runtime checks; the five ignored NUnit validation exercises remain TODO-WF-008.

## TODO markers
| ID | Location |
| --- | --- |
| TODO-WF-005 | Marker in Domain/Organisation.cs; models in Organisation.cs, Project.cs, WorkItem.cs; enums in WorkItemStatus.cs and WorkItemPriority.cs |
| TODO-WF-006 | Marker in Contracts/ContractExercises.cs; worked examples in CreateProjectRequest.cs, UpdateProjectRequest.cs, CreateWorkItemRequest.cs, WorkItemResponse.cs and ProjectResponse.cs |
| TODO-WF-007 | Marker in Contracts/ProjectRequestValidation.cs; HTTP wiring and binding error handling in Program.cs |
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
Invoke-WebRequest http://localhost:5062/api/projects -Method Post -ContentType application/json -Body '{"organisationId":1,"name":"   "}'
# Valid input: 501; no data is persisted.
Invoke-WebRequest http://localhost:5062/api/projects -Method Post -ContentType application/json -Body '{"organisationId":1,"name":"Tracker"}'
# Also send name lengths 2/3/100/101 and description lengths 1999/2000/2001.
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
