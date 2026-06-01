# stop_db.ps1
# This script stops the local PostgreSQL background process.

$ErrorActionPreference = "Continue"

$workspaceDir = "g:\WP-Project"
$dataDir = Join-Path $workspaceDir "pgsql\data"
$pgctl = Join-Path $workspaceDir "pgsql\bin\pg_ctl.exe"

if (-not (Test-Path $pgctl)) {
    Write-Host "PostgreSQL binaries not found at $pgctl. Nothing to stop."
    Exit 0
}

Write-Host "Stopping PostgreSQL server..."
& $pgctl -D $dataDir stop
Write-Host "PostgreSQL stopped successfully."
