################################################################################
##  File:  Install-Cmake.ps1
##  Team:  CI-Build
##  Desc:  Install Cmake
################################################################################

choco install cmake.install -y --installargs 'ADD_CMAKE_TO_PATH=""System""'
