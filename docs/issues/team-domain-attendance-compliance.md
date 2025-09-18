# [Domain] Team: Track Attendance Compliance

## Bounded Context
Team

## Domain Capability
Track attendance compliance for team members across scheduled events

## Business Motivation / Outcome
Team leads need to know which members consistently miss required sessions so they can intervene early and keep projects staffed appropriately.

## Domain Model Changes
- Entities:
  - Team
  - TeamMember
  - AttendanceRecord
  - ComplianceSummary
- Value Objects:
  - AttendanceStatus
  - AttendanceWindow
- Aggregates:
  - TeamAttendanceAggregate (Team root governing attendance policies and records)
- Invariants:
  - Attendance records must be immutable once logged.
  - Compliance summaries must cover a consistent rolling window.
- Domain Services:
  - AttendanceComplianceCalculator

## Application Use Cases
- Commands:
  - RecordMemberAttendance(teamId, memberId, eventId, status)
  - RecalculateComplianceSummaries(teamId)
- Queries:
  - GetTeamComplianceSnapshot(teamId)
  - GetMemberAttendanceHistory(teamId, memberId)
- Orchestration:
  - Schedule nightly compliance recalculation for active teams.

## Domain Events
- MemberAttendanceRecorded
- TeamComplianceSummaryUpdated
- MemberBecameNonCompliant

## Invariants & Policies
- A team member is considered non-compliant after missing 3 mandatory events within a rolling 30-day window.
- Attendance records older than 90 days are archived and no longer affect compliance scoring.

## Boundaries & Integration
- Consumes scheduling context events to learn about mandatory sessions.
- Publishes compliance summaries to Reporting context for dashboards.
- Exposes read models for the People Ops context to monitor staffing risks.

## Acceptance Criteria
- Given a team member misses three mandatory events within 30 days, when compliance is recalculated, then the member is flagged as non-compliant.
- Given a team member attends a make-up session within the 30-day window, when compliance is recalculated, then the missed session is offset and the member returns to compliant status.
- Given compliance summaries are generated nightly, when I view the team dashboard the next morning, then the compliance snapshot reflects the latest attendance records.

## Non-Functional Considerations
- Recalculation job must complete within 5 minutes for teams up to 500 members.
- Attendance events should be idempotent to handle duplicate submissions.
- Compliance data should be auditable for 1 year.

## Breaking Change & Migration Checklist
- [ ] Backfill historical attendance records into the new aggregate.
- [ ] Migrate reporting pipelines to consume new compliance summaries.

## Testing Strategy
- Unit tests for AttendanceComplianceCalculator business rules.
- Integration tests to ensure scheduling events trigger attendance recording.
- End-to-end tests covering compliance recalculation job and dashboard read models.

## Priority
P1 (high)

## Additional Context / References
- Related discussion: Aligning Team compliance metrics with People Ops quarterly review (see internal doc DOC-42).
