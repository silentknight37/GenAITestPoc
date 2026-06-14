# Packs GenAITest.Engine and GenAITest.AspNetCore and drops them into nuget-local/
param([switch]$Clean)

$root     = $PSScriptRoot
$localFeed = "$root\nuget-local"

if ($Clean) { Remove-Item "$localFeed\*.nupkg" -Force -ErrorAction SilentlyContinue }

Write-Host "==> Packing GenAITest.Engine ..."
dotnet pack "$root\src\GenAITest.Engine\GenAITest.Engine.csproj" `
    -c Release --no-build -o "$localFeed" -v q

Write-Host "==> Packing GenAITest.AspNetCore ..."
dotnet pack "$root\src\GenAITest.AspNetCore\GenAITest.AspNetCore.csproj" `
    -c Release -o "$localFeed" -v q

Write-Host ""
Write-Host "Packages in $localFeed :"
Get-ChildItem "$localFeed\*.nupkg" | ForEach-Object { Write-Host "  $($_.Name)" }
Write-Host ""
Write-Host "Done. Install in your API with:"
Write-Host "  dotnet add package GenAITest.AspNetCore --source `"$localFeed`""
