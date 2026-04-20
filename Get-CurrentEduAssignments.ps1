$config = Get-Content ".\ScriptValues.json" -Raw | ConvertFrom-Json
$secret_id = ConvertTo-SecureString $config.ClientSecret -AsPlainText -Force
$credential = [pscredential]::new($config.AppId, $secret_id)

$mg_params = @{
    TenantId = $config.TenantId
    ClientSecretCredential = $credential
    NoWelcome = $true
}

Connect-MgGraph @mg_params

:fltr while ($true) {
    Write-Host "Do you want to filter by student? (Y/N)" -ForegroundColor Cyan
    $resp = (Read-Host).Trim()
    switch -Regex ($resp) {
        "^(?i)y(es)?$" {
            Write-Host "Student UPN: " -ForegroundColor Cyan
            $UPN = (Read-Host).Trim().ToLowerInvariant()
            if (-not $UPN.EndsWith("@$($config.DefaultDomain)")) {
                $UPN += "@$($config.DefaultDomain)"
            }
            break fltr
        }
        "^(?i)n(o)?$" {
            break fltr
        }
        default {
            Write-Host "Invalid input. Please enter 'Y' or 'N'." -ForegroundColor Yellow -BackgroundColor Black
        }
    }
}

Write-Host "Searching for classes.`nPlease wait..." -ForegroundColor Magenta
if ($UPN) {
    $user = Get-MgUser -UserId $UPN
    $classes = Get-MgEducationUserClass -EducationUserId $user.Id
}
else {
    $classes = Get-MgEducationClass -All
}
Write-Host "Complete." -ForegroundColor DarkGreen

Write-Host "Formatting output.`nThis may take a moment..." -ForegroundColor Magenta
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
Write-Host "Complete." -ForegroundColor DarkGreen

$due_assignments = $assignments | Where-Object { $_.DueDateTime -ge (Get-Date) }
$assignments | Where-Object { $_.DueDateTime -ge (Get-Date) } | Format-Table

$curr_time = Get-Date -Format yyyy.MM.dd
:expt while($true) {
    Write-Host "Would you like this as a CSV? (Y/N)" -ForegroundColor Cyan
    $resp = (Read-Host).Trim()
    switch -Regex ($resp) {
        "^(?i)y(es)?$" {
            Write-Host "Output location: " -ForegroundColor Cyan
            $path = (Read-Host).Trim()
            $due_assignments | Export-Csv -Path "$path\$curr_time-assignments.csv" -NoTypeInformation
            Write-Host "CSV exported to $path\$curr_time-assignments.csv" -ForegroundColor DarkGreen
            break expt
        }
        "^(?i)n(o)?$" {
            break expt
        }
        default {
            Write-Host "Invalid input. Please enter 'Y' or 'N'" -ForegroundColor Yellow -BackgroundColor Black
        }
    }
}