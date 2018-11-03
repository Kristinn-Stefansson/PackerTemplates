################################################################################
##  File:  Validate-Python.ps1
##  Team:  CI-X
##  Desc:  Configure python on path based on what VS installs
##         Must run after VS is installed
################################################################################

if(Get-Command -Name 'python')
{
    Write-Host "Python $(python --version) on path"
}
else
{
     Write-Host "Python is not on path"
    exit 1
}

$Python3Verison = $(python --version)

$Python2Path = "C:\Python27amd64"
$env:Path = $Python2Path + ";" + $env:Path

$Python2Version = & $env:comspec "/s /c python --version 2>&1"

# Adding description of the software to Markdown
$SoftwareName = "Python (64 bit)"

$Description = @"
#### $Python3Verison
_Environment:_
* PATH: contains location of python.exe

#### $Python2Version

_Location:_ $Python2Path
"@

Add-SoftwareDetailsToMarkdown -SoftwareName $SoftwareName -DescriptionMarkdown $Description