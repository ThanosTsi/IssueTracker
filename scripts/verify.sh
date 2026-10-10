#!/usr/bin/env sh
set -eu
cd "$(dirname "$0")/.."
dotnet restore IssueTracker.sln
dotnet build IssueTracker.sln --no-restore
dotnet test IssueTracker.sln --no-build --no-restore
printf '%s\n' 'Verification succeeded. Skipped exercises are not completed coverage.'
