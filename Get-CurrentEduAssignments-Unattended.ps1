$config = Get-Content ".\ScriptValues.json" -Raw | ConvertFrom-Json
$secret_id = ConvertTo-SecureString $config.ClientSecret -AsPlainText -Force
$credential = [pscredential]::new($config.AppId, $secret_id)

$mgParams = @{
    TenantId = $config.TenantId
    ClientSecretCredential = $credential
    NoWelcome = $true
}

Connect-MgGraph @mgParams

$classes = Get-MgEducationClass -All

$assignments = foreach ($class in $classes) {
    Get-MgEducationClassAssignment -EducationClassId $class.Id |
    Select-Object (
        @{ n = "ClassId"; e = { $class.Id } },
        @{ n = "ClassName"; e = { $class.DisplayName } },
        "Id",
        "DisplayName",
        "Status",
        "DueDateTime"
    )
}

$curr_time = Get-Date -Format yyyy.MM.dd

if (Test-Path $config.OutputFolder) {
    $assignments | Where-Object { $_.DueDateTime -ge (Get-Date) } |
    Export-Csv -Path "$($config.OutputFolder)\$curr_time-assignments.csv" -NoTypeInformation
}
else {
    $errPath = Join-Path $env:TEMP "Get-CurrentEduAssignments"
    if (-not (Test-Path $errPath)) {
        New-Item -ItemType Directory -Force | Out-Null
    }
    $errLog = Join-Path $errPath "$curr_time-Errors.txt"
    if (-not (Test-Path $errLog)) {
        New-Item -Path $errLog -ItemType File -Force | Out-Null
    }
    "[$(Get-Date -Format "yyyy.MM.dd HH:mm:ss")] Couldn't reach Ouput Folder." | Add-Content -Path $errLog
}