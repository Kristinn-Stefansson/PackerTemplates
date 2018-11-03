################################################################################
##  File:  Install-SSDT.ps1
##  Team:  CI-Platform
##  Desc:  Install SQL Server Data Tools for Windows
################################################################################

Import-Module -Name ImageHelpers -Force

#SQL Server Data Tools in Visual Studio 2015 (Version: 14.0.61021.0)
$InstallerURI = 'https://download.microsoft.com/download/9/C/7/9C749FF7-7AD2-409A-BF75-69238295A668/Dev14/EN/SSDTSetup.exe'
$InstallerName = 'SSDT-Setup-ENU.exe'
$ArgumentList = ('INSTALLALL=1', '/passive', '/norestart' )

$exitCode = Install-EXE -Url $InstallerURI -Name $InstallerName -ArgumentList $ArgumentList

exit $exitCode
