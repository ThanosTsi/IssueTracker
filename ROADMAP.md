# IssueTracker roadmap

Source: the Obsidian handbook, including the revised Docker/Kubernetes track
and learning-agent rules in sections 34–41. Initial scaffold: steps 00–03 only.

| Order | Learning outcome | Introduce when ready |
| --- | --- | --- |
| 00 | Repository and environment | Purpose, Git hygiene, verification |
| 01 | ASP.NET foundations | Routing, JSON, service extraction, NUnit, DI |
| 02 | Domain and validation | Organisation/Project/WorkItem, DTOs, boundaries |
| 03 | Persistence | SQL Server, EF Core, migrations, async querying |
| 04 | API CRUD and query quality | Create/read/update/delete, pagination, filters, errors |
| 05 | Integration testing | Real HTTP pipeline, disposable SQL/Testcontainers |
| 06 | Docker and CI | Multi-stage image, health, networking, hardening, immutable tags |
| 07 | Identity and tenancy | OIDC, policies, trusted tenant resolution, adversarial tests |
| 08 | Azure | Container Apps, Azure SQL, managed identity, budget and teardown |
| 09 | Terraform | Encode infrastructure already understood; protect state |
| 10 | Messaging | Service Bus, worker, dual-write analysis, idempotency, retries/DLQ |
| 11 | Operations | Logs, traces, metrics, threat model and measured performance |
| 12 | Kubernetes | Local objects, probes, measured resources, rollout, scaling, Kustomize |
| Later | Optional extensions | AKS comparison, outbox, concurrency, React, evidence-based Redis |

At 7–10 hours/week, use the handbook's revised timeline as a guide, not a reason
to skip mastery gates. Start Kubernetes after containers and observability are
comfortable. Container Apps remains the first Azure deployment path; AKS is optional.

Do not create later implementation code until the human asks for that phase.
Record actual architectural decisions yourself using the ADR template; no
conclusions have been filled in for you.
