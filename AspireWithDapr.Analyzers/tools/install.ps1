param($installPath, $toolsPath, $package, $project)

Write-Host "Installing Dapr.Analyzers..."

# Add analyzer reference
$analyzersPath = Join-Path $toolsPath "..\analyzers\dotnet\cs\Dapr.Analyzers.dll"
if (Test-Path $analyzersPath) {
    $project.Object.References.Add($analyzersPath)
    Write-Host "Analyzer installed successfully."
} else {
    Write-Warning "Analyzer assembly not found at: $analyzersPath"
}
