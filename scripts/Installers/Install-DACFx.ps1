####################################################################################
##  File:  Install-DACFx.ps1
##  Team:  CI-Platform
##  Desc:  Install SQL Server® Data-Tier Application Framework (DACFx) for Windows
####################################################################################

Import-Module -Name ImageHelpers -Force

$exitcode = Install-MSI -MsiUrl "https://download.microsoft.com/download/D/5/C/D5CFC940-DA21-44D3-84FF-A0FD147F1681/EN/x64/DacFramework.msi" -MsiName "DacFramework.msi"

exit $exitcode
