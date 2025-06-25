param($installPath, $toolsPath, $package, $project)

Write-Host "Uninstalling Dapr.Analyzers..."

# Remove analyzer references
$analyzersPath = Join-Path $toolsPath "..\analyzers\dotnet\cs"
if (Test-Path $analyzersPath) {
    foreach ($analyzerFilePath in Get-ChildItem -Path "$analyzersPath\*.dll" -Exclude *.resources.dll) {
        try {
            if ($project.Object.AnalyzerReferences) {
                $project.Object.AnalyzerReferences.Remove($analyzerFilePath.FullName)
                Write-Host "Removed analyzer: $($analyzerFilePath.Name)"
            }
        } catch {
            Write-Warning "Could not remove analyzer reference for $($analyzerFilePath.Name): $_"
        }
    }
    Write-Host "Dapr.Analyzers uninstalled successfully."
} else {
    Write-Warning "Analyzers directory not found at: $analyzersPath"
}
