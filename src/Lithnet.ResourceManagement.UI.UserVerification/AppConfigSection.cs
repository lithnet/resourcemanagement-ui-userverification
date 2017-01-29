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


        [ConfigurationProperty("accountNameAttributeName", IsRequired = true, DefaultValue = "AccountName")]
        public string AccountNameAttributeName
        {
            get
            {
                return (string)this["accountNameAttributeName"];
            }
            set
            {
                this["accountNameAttributeName"] = value;
            }
        }

        [ConfigurationProperty("searchAttributeName", IsRequired = true, DefaultValue = "ObjectID")]
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

        [ConfigurationProperty("smsCodeLength", IsRequired = true, DefaultValue = 6)]
        public int SmsCodeLength
        {
            get
            {
                int val = (int) this["smsCodeLength"];

                if (val <= 0)
                {
                    return 6;
                }
                else if (val > 9)
                {
                    return 9;
                }
                else
                {
                    return val;
                }
            }
            set
            {
                this["smsCodeLength"] = value;
            }
        }

        internal int SmsCodeLowRange => Convert.ToInt32("1".PadRight(this.SmsCodeLength, '0'));

        internal int SmsCodeHighRange => Convert.ToInt32("9".PadRight(this.SmsCodeLength, '9'));
    }
}