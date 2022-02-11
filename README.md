![](https://lithnet.github.io/images/logo-ex-small.png)
# Lithnet User Verification Module for FIM 2010/MIM 2016
The Lithnet User Verification Module is an extension to the FIM/MIM portal that provides the ability for your help desk staff to validate a user using an SMS code sent to their mobile number.

If your users are already registered for SMS-based self-service password reset, then this module is ready to use.

![](https://github.com/lithnet/resourcemanagement-ui-userverification/wiki/images/screen-shot1.png)

## Features
1. Uses your existing [SmsServiceProvider.dll](https://github.com/lithnet/resourcemanagement-ui-userverification/wiki/Creating-an-SmsServiceProvider) that you created for SSPR one-time SMS passwords
2. Customize the list of attributes shown on the user verification page
3. Supports localized attribute names from the FIM/MIM service
4. Integrates with the user RCDC using a UocHyperLink control
5. Can be secured to only allow users of a set to access the tool

## Requirements
* SharePoint 2013 or later
* FIM 2010 R2 or later
* .NET Framework 4.0 or later

## Getting started
The module is a simple WSP package that needs to be deployed into your SharePoint farm. The [installation guide](https://github.com/lithnet/resourcemanagement-ui-userverification/wiki/Installation-and-upgrade-steps) covers all the steps for installing and upgrading the module

## Localization
The module will match the locale of the FIM portal, rendering attribute display names in the browser's preferred language. The module itself has been localized into the following languages
- Italian
- German (Thanks to Peter Stapf)
- Danish (Thanks to Søren Granfeldt)
- Swedish (Thanks to Leo Erlandsson)

If you want to contribute a translation, see the "How can I contribute" section below.

## How can I contribute to the project?
* Found an issue and want us to fix it? [Log it](https://github.com/lithnet/resourcemanagement-ui-userverification/issues)
* Want to fix an issue yourself or add functionality? Clone the project and submit a pull request

## Enteprise support
Lithnet offer enterprise support plans for our open-source products. Deploy our tools with confidence that you have the backing of the dedicated Lithnet support team if you run into any issues, have questions, or need advice. Reach out to support@lithnet.io, let us know the number of users you are managing with your MIM implementation, and we'll put together a quote.

## Keep up to date
* [Visit our blog](http://blog.lithnet.io)
* [Follow us on twitter](https://twitter.com/lithnet_io)![](http://twitter.com/favicon.ico)
