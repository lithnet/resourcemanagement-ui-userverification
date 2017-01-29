# resourcemanagement-ui-userverification
User verification module for FIM2010/MIM2016

## Installation Steps
Using the SharePoint Management Shell
```
Add-SPSolution -LiteralPath "D:\temp\user-verification.wsp"
Install-SPSolution -Identity user-verification.wsp -WebApplication "http://mimportal.my.domain" -Force -GACDeployment -FullTrustBinDeployment
Enable-SPFeature -Identity user-verification -Url "http://mimportal.my.domain/IdentityManagement"
iisreset
```

## Upgrade Steps
Using the SharePoint Management Shell
```
Update-SPSolution -Identity user-verification.wsp -LiteralPath D:\temp\user-verification.wsp -GACDeployment -FullTrustBinDeployment
iisreset
```
