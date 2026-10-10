$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
Push-Location $repoRoot
try {
    dotnet restore IssueTracker.sln
    if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
    dotnet build IssueTracker.sln --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
    dotnet test IssueTracker.sln --no-build --no-restore
    if ($LASTEXITCODE -ne 0) { throw 'Tests failed.' }
    Write-Host 'Verification succeeded. Skipped exercises are not completed coverage.'
}
finally {
    Pop-Location
}
