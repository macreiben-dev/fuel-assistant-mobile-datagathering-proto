param(
  [Parameter(Mandatory)] [string] $ArtifactOutputPath,
  [Parameter(Mandatory)] [string] $SimhubInstallationDirectory)

$isSimhubInstallationDirectoryValid = Test-Path $SimhubInstallationDirectory

$isArtifactOutputPathValid = Test-Path $ArtifactOutputPath

if ($isSimhubInstallationDirectoryValid -eq $false) {
  Write-Error("Simub installation directory is invalid")
  exit 1
}

if ($isArtifactOutputPathValid -eq $false) {
  Write-Error("Artifact output path is invalid")
  exit 1
}

Write-Information "Simhub installation path is:   $SimhubInstallationDirectory"
Write-Information "Artifact output path is:       $isArtifactOutputPathValid"

$Source = "$ArtifactOutputPath/FuelAssistantMobile.DataGathering.*"
$Destination = $SimhubInstallationDirectory

Write-Information "-------------------------------------------------------------"
Write-Information "Source parameter:            $Source"
Write-Information "Destination parameter:       $Destination"

Copy-Item $Source $Destination -Verbose