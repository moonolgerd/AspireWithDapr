param($installPath, $toolsPath, $package, $project)

Write-Host "Uninstalling Dapr.Analyzers..."

# Remove analyzer reference
$analyzersPath = Join-Path $toolsPath "..\analyzers\dotnet\cs\Dapr.Analyzers.dll"
try {
    $project.Object.References.Remove($analyzersPath)
    Write-Host "Analyzer uninstalled successfully."
} catch {
    Write-Warning "Could not remove analyzer reference: $_"
}
