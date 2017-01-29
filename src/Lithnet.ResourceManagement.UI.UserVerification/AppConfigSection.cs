using System;
using System.Configuration;
using System.Reflection;

namespace Lithnet.ResourceManagement.UI.UserVerification
{
    internal class AppConfigurationSection : ConfigurationSection
    {
        private static AppConfigurationSection current;

        internal static AppConfigurationSection CurrentConfig
        {
            get
            {
                if (AppConfigurationSection.current == null)
                {
                    AppConfigurationSection.current = AppConfigurationSection.GetConfiguration();
                }

                return AppConfigurationSection.current;
            }
        }

        internal static AppConfigurationSection GetConfiguration()
        {
            AppConfigurationSection section = (AppConfigurationSection) ConfigurationManager.GetSection("lithnetUserVerification");

            if (section == null)
            {
                section = new AppConfigurationSection();
            }

            return section;
        }


        [ConfigurationProperty("smsServiceProviderDll", IsRequired = true, DefaultValue = @"%ProgramFiles%\Microsoft Forefront Identity Manager\2010\Service\SmsServiceProvider.dll")]
        public string SmsServiceProviderDll
        {
            get
            {
                return (string) this["smsServiceProviderDll"];
            }
            set
            {
                this["smsServiceProviderDll"] = value;
            }
        }

        [ConfigurationProperty("phoneNumberAttributeName", IsRequired = true, DefaultValue = "msidmOneTimePasswordMobilePhone")]
        public string PhoneNumberAttributeName
        {
            get
            {
                return (string) this["phoneNumberAttributeName"];
            }
            set
            {
                this["phoneNumberAttributeName"] = value;
            }
        }

        [ConfigurationProperty("displayNameAttributeName", IsRequired = true, DefaultValue = "DisplayName")]
        public string DisplayNameAttributeName
        {
            get
            {
                return (string) this["displayNameAttributeName"];
            }
            set
            {
                this["displayNameAttributeName"] = value;
            }
        }

        [ConfigurationProperty("searchAttributeName", IsRequired = true, DefaultValue = "AccountName")]
        public string SearchAttributeName
        {
            get
            {
                return (string)this["searchAttributeName"];
            }
            set
            {
                this["searchAttributeName"] = value;
            }
        }
    }
}