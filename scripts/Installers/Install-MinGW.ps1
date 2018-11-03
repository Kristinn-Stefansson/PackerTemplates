################################################################################
##  File:  Install-MinGW.ps1
##  Team:  CI-X
##  Desc:  Install GNU tools for Windows to C:\tools\mingw64
################################################################################

Import-Module -Name ImageHelpers -Force

choco install -y mingw
$installDir = "C:\tools\mingw64\bin"
Add-MachinePathItem $installDir

# Hard link mingw32-make.exe to make.exe, which is a more discoverable name
# and so the same command line can be used on Windows as on macOS and Linux
New-Item -Path $installDir -ItemType HardLink -Name "make.exe" -Value "$installDir\mingw32-make.exe"
