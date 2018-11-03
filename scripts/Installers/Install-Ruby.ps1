################################################################################
##  File:  Install-Ruby.ps1
##  Team:  CI-Platform
##  Desc:  Install Ruby for Windows
################################################################################

Import-Module -Name ImageHelpers

# Ruby versions are already available in the tool cache.

# Add the latest available version of Ruby to the path.
Add-MachinePathItem "C:\hostedtoolcache\windows\Ruby\2.5.0\x64\bin"
$env:Path = Get-MachinePath
exit 0
