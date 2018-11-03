################################################################################
##  File:  Install-Miniconda.ps1
##  Team:  CI-X
##  Desc:  Install the latest version of Miniconda and set $env:CONDA
################################################################################

Import-Module -Name ImageHelpers -Force

$url = "https://repo.continuum.io/miniconda/Miniconda3-latest-Windows-x86_64.exe"
$name = $Url.Split('/')[-1]
$destination = "C:\Miniconda"

Install-EXE -Url $url -Name $name -ArgumentList "/S /AddToPath=0 /RegisterPython=0 /D=$destination"
Set-SystemVariable -SystemVariable "CONDA" -Value $destination
