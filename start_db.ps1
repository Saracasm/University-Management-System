# start_db.ps1
# This script initializes and starts the local PostgreSQL server in the background and ensures UniversityDB exists.

$ErrorActionPreference = "Stop"

$workspaceDir = "g:\WP-Project"
$pgsqlDir = Join-Path $workspaceDir "pgsql"
$dataDir = Join-Path $pgsqlDir "data"
$logFile = Join-Path $pgsqlDir "logfile"
$initdb = Join-Path $pgsqlDir "bin\initdb.exe"
$pgctl = Join-Path $pgsqlDir "bin\pg_ctl.exe"
$createdb = Join-Path $pgsqlDir "bin\createdb.exe"
$psql = Join-Path $pgsqlDir "bin\psql.exe"

# 1. Initialize DB if data directory doesn't exist
if (-not (Test-Path $dataDir)) {
    Write-Host "Initializing PostgreSQL database cluster at $dataDir..."
    New-Item -ItemType Directory -Path $dataDir -Force | Out-Null
    
    # Write temporary password file matching appsettings.json password: Mamma_mia101
    $pwFile = Join-Path $pgsqlDir "temp_pw.txt"
    "Mamma_mia101" | Out-File -FilePath $pwFile -Encoding ascii
    
    # Run initdb
    & $initdb -D $dataDir -U postgres -A scram-sha-256 --pwfile=$pwFile
    
    # Clean up password file
    Remove-Item $pwFile -Force
    Write-Host "Database cluster initialized successfully."
}

# 2. Check if database is already running
$status = & $pgctl -D $dataDir status
if ($status -match "server is running") {
    Write-Host "PostgreSQL is already running."
} else {
    Write-Host "Starting PostgreSQL in background..."
    & $pgctl -D $dataDir -l $logFile start
    # Wait for server to start up
    Start-Sleep -Seconds 3
}

# 3. Create UniversityDB if it doesn't exist
Write-Host "Checking if UniversityDB database exists..."
$env:PGPASSWORD = "Mamma_mia101"
$dbExists = & $psql -U postgres -d postgres -t -c "SELECT 1 FROM pg_database WHERE datname='UniversityDB'"
if ($dbExists -match "1") {
    Write-Host "UniversityDB already exists."
} else {
    Write-Host "Creating database UniversityDB..."
    & $createdb -U postgres UniversityDB
    Write-Host "UniversityDB created successfully!"
}

Write-Host "Local PostgreSQL is ready on port 5432."
