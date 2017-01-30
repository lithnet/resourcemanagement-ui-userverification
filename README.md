![](https://lithnet.github.io/images/logo-ex-small.png)
# Lithnet User Verification Module for FIM 2010/MIM 2016
The Lithnet User Verification Module is an extension to the FIM/MIM portal that provides the ability for your help desk staff to validate a user using an SMS code sent to their mobile number.

If your users are already registered for SMS-based self-service password reset, then this module is ready to use.

The module makes use of the same SmsServiceProvider dll that you created for SSPR one-time SMS passwords.

## Installation Steps
1. Download the SharePoint WSP package from the [releases](https://github.com/lithnet/resourcemanagement-ui-userverification/releases) page
2. Install the module into the SharePoint farm

#### Using the SharePoint Management Shell
```powershell
Add-SPSolution -LiteralPath "D:\temp\user-verification.wsp"
Install-SPSolution -Identity user-verification.wsp -WebApplication "http://mimportal.my.domain" -Force -GACDeployment -FullTrustBinDeployment
Enable-SPFeature -Identity user-verification -Url "http://mimportal.my.domain/IdentityManagement"
iisreset
```
3. Modify the SharePoint web.config file to include the location of the SMS provider DLL (if the FIM portal and FIM service is installed on the same host, and the SmsServiceProvider.dll file is in the default location, then you can skip this step

In the `<configSections>` element, add the following line
```xml
<section name="lithnetUserVerification" type="Lithnet.ResourceManagement.UI.UserVerification.AppConfigurationSection, Lithnet.ResourceManagement.UI.UserVerification"/>
```
Add the following section to the end of the file, replacing the path to the SMS provider DLL as appropriate
```xml
 <lithnetUserVerification smsServiceProviderDll="%ProgramFiles%\Microsoft Forefront Identity Manager\2010\Service\SmsServiceProvider.dll" />
```

4. Configure optional properties
The following properties are optional, and can be configured as required
```xml
 <lithnetUserVerification
    smsServiceProviderDll="%ProgramFiles%\Microsoft Forefront Identity Manager\2010\Service\SmsServiceProvider.dll"
    phoneNumberAttributeName="msidmOneTimePasswordMobilePhone"
    searchAttributeName="ObjectID"
    displayAttributes="DisplayName,AccountName,Domain,msidmOneTimePasswordMobilePhone"
    smsCodeLength="6"
    showNullAttributes="false"/>
```
## Localization
The module will match the locale of the FIM portal, rendering attribute display names in the browser's preferred language. The module itself has been localized into the following languages
- Italian
- German

If you want to contribute a translation, please fill in the [localization survey](https://lithnet-my.sharepoint.com/personal/ryan_lithiumblue_com/_layouts/15/guestaccess.aspx?guestaccesstoken=QMmZhOa00BEb1QJnSnIlPvHFB4DSTBZWxh5UZAVp9aw%3d&docid=1_1fb5451aedc1842b88e2daeb1077b0ac8&wdFormId=%7B59C5F77D%2DCDEA%2D4703%2DBA86%2D81352BC45ED4%7D) and send me a message.

Alternatively, if you are comfortable with git and visual studio, clone the repository, copy the `Verify.aspx.resx` file, naming it `Verify.aspx.CODE.resx`, where CODE is your two letter language code, modify the strings in the file, and submit a pull request.

## Upgrade Steps
Using the SharePoint Management Shell
```
Update-SPSolution -Identity user-verification.wsp -LiteralPath D:\temp\user-verification.wsp -GACDeployment -FullTrustBinDeployment
iisreset
```
