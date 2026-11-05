$ErrorActionPreference = "Stop"
$root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$files = @(
    "tests/BIE.UnitTests/LearnerUnitTests.cs.todo",
    "tests/BIE.IntegrationTests/LearnerIntegrationTests.cs.todo",
    "tests/BIE.E2ETests/LearnerE2ETests.cs.todo"
)

foreach ($relative in $files)
{
    $source = Join-Path $root $relative
    $target = $source.Substring(0, $source.Length - 5)

    if (Test-Path -LiteralPath $target)
    {
        Write-Host "Bereits aktiv: $target"
        continue
    }

    if (-not (Test-Path -LiteralPath $source))
    {
        throw "Aufgabendatei fehlt: $source"
    }

    Move-Item -LiteralPath $source -Destination $target
}

Write-Host "Aufgaben aktiv. Die drei Platzhalter schlagen bis zur Bearbeitung fehl."
