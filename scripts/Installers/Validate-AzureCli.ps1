################################################################################
##  File:  Validate-AzureCli.ps1
##  Team:  ReleaseManagement
##  Desc:  Validate Azure CLI
################################################################################

if(Get-Command -Name 'az')
{
    Write-Host "Azure Cli $(az --version) on path"
}
else
{
    Write-Error "Azure Cli not on path"
    exit 1
}


if( $($(az --version) | out-string)  -match  'azure-cli \((?<version>.*)\)' )
{
   $azureCliVersion = $Matches.version
}

# Adding description of the software to Markdown
$SoftwareName = "Azure CLI"

$Description = @"
_Version:_ $azureCliVersion<br/>
_Environment:_
* PATH: contains location of az.cmd
"@

Add-SoftwareDetailsToMarkdown -SoftwareName $SoftwareName -DescriptionMarkdown $Description