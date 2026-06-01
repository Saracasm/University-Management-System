# setup_db.ps1
# This script downloads and extracts the portable PostgreSQL binaries locally inside the workspace.

$ErrorActionPreference = "Stop"
$ProgressPreference = "SilentlyContinue"

$workspaceDir = "g:\WP-Project"
$zipUrl = "https://get.enterprisedb.com/postgresql/postgresql-16.3-1-windows-x64-binaries.zip"
$zipPath = Join-Path $workspaceDir "postgresql-binaries.zip"
$pgsqlDir = Join-Path $workspaceDir "pgsql"

# Clean up any partial download
if (Test-Path $zipPath) {
    Remove-Item $zipPath -Force
}

# Check if pgsql directory already exists
if (Test-Path $pgsqlDir) {
    Write-Host "pgsql directory already exists at $pgsqlDir. Skipping download."
    Exit 0
}

Write-Host "Downloading PostgreSQL 16.3 portable binaries using modern fast downloader..."
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

# Use WebClient for maximum speed (ignores slow PowerShell progress bar rendering)
try {
    $webClient = New-Object System.Net.WebClient
    $webClient.DownloadFile($zipUrl, $zipPath)
} catch {
    Write-Host "WebClient failed, falling back to curl.exe..."
    & curl.exe -L -o $zipPath $zipUrl
}

Write-Host "Download complete. Extracting binaries to $workspaceDir..."
# Expand-Archive extracts everything inside a top-level 'pgsql' folder already inside the zip
Expand-Archive -Path $zipPath -DestinationPath $workspaceDir -Force

Write-Host "Cleaning up ZIP archive..."
Remove-Item -Path $zipPath -Force

Write-Host "PostgreSQL binaries setup successfully in $pgsqlDir!"
