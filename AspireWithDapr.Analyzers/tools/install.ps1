param($installPath, $toolsPath, $package, $project)

Write-Host "Installing Dapr.Analyzers..."

# Add analyzer references
$analyzersPath = Join-Path $toolsPath "..\analyzers\dotnet\cs"
if (Test-Path $analyzersPath) {
    foreach ($analyzerFilePath in Get-ChildItem -Path "$analyzersPath\*.dll" -Exclude *.resources.dll) {
        if ($project.Object.AnalyzerReferences) {
            $project.Object.AnalyzerReferences.Add($analyzerFilePath.FullName)
            Write-Host "Added analyzer: $($analyzerFilePath.Name)"
        }
    }
    Write-Host "Dapr.Analyzers installed successfully."
} else {
    Write-Warning "Analyzers directory not found at: $analyzersPath"
}
