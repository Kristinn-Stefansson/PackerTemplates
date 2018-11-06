if([environment]::OSVersion.version.Major -ge 6) {
  # You cannot change the network location if you are joined to a domain, so abort
  if(1,3,4,5 -contains (Get-WmiObject win32_computersystem).DomainRole) { return }

  # Get network connections
  Get-NetConnectionProfile | Where-Object {$_.NetworkCategory -ne "Private"} | ForEach-Object {
  	Write-Host $_.Name"category, alias "$_.InterfaceAlias" was previously set to"$_.NetworkCategory
    $_ | Set-NetConnectionProfile -NetworkCategory Private
  	Write-Host $_.Name"changed to category"$_.NetworkCategory
  }
}

Enable-PSRemoting -Force
winrm quickconfig -q

winrm set winrm/config/client/auth '@{Basic="true"}'
winrm set winrm/config/service/auth '@{Basic="true"}'
winrm set winrm/config/service '@{AllowUnencrypted="true"}'
winrm set winrm/config/winrs '@{MaxMemoryPerShellMB="2048"}'
Restart-Service -Name WinRM
netsh advfirewall firewall add rule name="WinRM-HTTP" dir=in localport=5985 protocol=TCP action=allow
