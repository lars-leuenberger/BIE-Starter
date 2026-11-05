param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("On", "Off")]
    [string]$Mode
)

$ErrorActionPreference = "Stop"
$target = Join-Path $PSScriptRoot "../src/BIE.Core/EntryPolicy.cs"
$source = Get-Content -LiteralPath $target -Raw -Encoding utf8
$correct = 'entry.Days > 30'
$broken = 'entry.Days >= 30'

if ($Mode -eq "On")
{
    $before = $correct
    $after = $broken
}
else
{
    $before = $broken
    $after = $correct
}

if ($source.Contains($after))
{
    Write-Host "Gewünschter Zustand ist bereits aktiv."
    exit 0
}

if (-not $source.Contains($before))
{
    throw "Erwartete Codezeile fehlt."
}

[IO.File]::WriteAllText(
    [IO.Path]::GetFullPath($target),
    $source.Replace($before, $after),
    [Text.UTF8Encoding]::new($false)
)

Write-Host "Regression: $Mode"
