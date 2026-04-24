param(
    [string]$HostName = "localhost",
    [int]$Port = 5432,
    [string]$Username = "postgresuser",
    [string]$MaintenanceDatabase = "postgres"
)

$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$setupScript = Join-Path $scriptRoot "nivad_user_service_setup.psql"

psql -h $HostName -p $Port -U $Username -d $MaintenanceDatabase -v ON_ERROR_STOP=1 -f $setupScript
