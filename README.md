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

## Getting started
The module is a simple WSP package that needs to be deployed into your SharePoint farm. The [installation guide](https://github.com/lithnet/resourcemanagement-ui-userverification/wiki/Installation-and-upgrade-steps) covers all the steps for installing and upgrading the module

## Localization
The module will match the locale of the FIM portal, rendering attribute display names in the browser's preferred language. The module itself has been localized into the following languages
- Italian
- German (Thanks to Peter Stapf)

If you want to contribute a translation, see the "How can I contribute" section below.

## How can I contribute to the project
Found an issue?
* [Log it](https://github.com/lithnet/resourcemanagement-ui-userverification/issues)

Want to fix an issue?
* Fork the project and submit a pull request 

Want to contribute a translation?
* Fill in the [localization survey](https://lithnet-my.sharepoint.com/personal/ryan_lithiumblue_com/_layouts/15/guestaccess.aspx?guestaccesstoken=QMmZhOa00BEb1QJnSnIlPvHFB4DSTBZWxh5UZAVp9aw%3d&docid=1_1fb5451aedc1842b88e2daeb1077b0ac8&wdFormId=%7B59C5F77D%2DCDEA%2D4703%2DBA86%2D81352BC45ED4%7D) and raise a [new issue](https://github.com/lithnet/resourcemanagement-ui-userverification/issues/new) requesting your language be added.

## Keep up to date
* [Visit my blog](http://blog.lithiumblue.com)
* [Follow me on twitter](https://twitter.com/RyanLNewington)![](http://twitter.com/favicon.ico)
