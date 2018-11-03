################################################################################
##  File:  Install-VS2015.ps1
##  Team:  CI-Build
##  Desc:  Install Visual Studio 2015
################################################################################

Import-Module -Name ImageHelpers -Force

$InstallerURI = 'https://download.microsoft.com/download/c/a/c/cac076be-2940-4a9e-ad8a-040996fbc7fc/vs_enterprise.exe'
$InstallerName = 'vs_Enterprise.exe'
$ArgumentList = ('/Full', '/Quiet', '/NoRestart' )
$exitCode = Install-EXE -Url $InstallerURI -Name $InstallerName -ArgumentList $ArgumentList

# Find the version of VS installed for this instance
$regKey = "HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*"
$installedApplications = Get-ItemProperty -Path $regKey
$VisualStudioVersion = ($installedApplications | Where-Object { $_.DisplayName -and $_.DisplayName.toLower().Contains("visual studio enterprise") } | Select-Object -First 1).DisplayVersion
Write-Host "Visual Studio version" $VisualStudioVersion "installed"

exit $exitCode
