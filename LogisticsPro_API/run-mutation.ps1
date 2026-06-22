# Runs Stryker.NET mutation testing on the generated test project.
#
# IMPORTANT: stop the host API first. Stryker rebuilds the API project it mutates, and a
# running LogisticsPro_API instance locks its own binary (bin\Debug\net8.0\LogisticsPro_API.exe).
# That is why the in-app "Mutation Test" button cannot do this — it runs inside the very process
# being mutated. Run this script from a terminal instead.
#
# Usage:
#   ./run-mutation.ps1                                  # mutates the controllers (fast-ish)
#   ./run-mutation.ps1 -Mutate "**/JobController.cs"    # mutate a single file
#
param(
    [string]$Mutate = "**/Controllers/*.cs"
)

$ErrorActionPreference = "Stop"

$testProject = Join-Path $PSScriptRoot "LogisticsPro_API.GenAI.Tests.Integration"
if (-not (Test-Path $testProject)) {
    Write-Error "Generated test project not found at: $testProject  (generate + write tests first)."
    exit 1
}

# Refuse to run while the API is up — it would lock the build and fail.
$running = Get-Process LogisticsPro_API -ErrorAction SilentlyContinue
if ($running) {
    Write-Warning "LogisticsPro_API is running (PID $($running.Id))."
    Write-Warning "Stop it first (Visual Studio: Shift+F5, or close the run window), then re-run this script."
    exit 1
}

# Stryker must be installed: dotnet tool install -g dotnet-stryker
if (-not (dotnet tool list --global | Select-String -SimpleMatch "dotnet-stryker")) {
    Write-Warning "Stryker.NET not installed. Installing it now..."
    dotnet tool install -g dotnet-stryker
}

Push-Location $testProject
try {
    Write-Host "Running Stryker.NET (mutate: $Mutate)..." -ForegroundColor Cyan
    dotnet stryker --mutate "$Mutate" --reporter html --reporter json --reporter progress
    Write-Host ""
    Write-Host "Done. HTML report is under: $testProject\StrykerOutput\<timestamp>\reports\mutation-report.html" -ForegroundColor Green
}
finally {
    Pop-Location
}