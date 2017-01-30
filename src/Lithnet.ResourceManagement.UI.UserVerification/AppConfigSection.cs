using System;
using System.ComponentModel;
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


        [ConfigurationProperty("smsServiceProviderDll", IsRequired = false, DefaultValue = @"%ProgramFiles%\Microsoft Forefront Identity Manager\2010\Service\SmsServiceProvider.dll")]
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

        [ConfigurationProperty("phoneNumberAttributeName", IsRequired = false, DefaultValue = "msidmOneTimePasswordMobilePhone")]
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

        [ConfigurationProperty("searchAttributeName", IsRequired = false, DefaultValue = "ObjectID")]
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

        [ConfigurationProperty("displayAttributes", IsRequired = false, DefaultValue = "DisplayName,AccountName,Domain,msidmOneTimePasswordMobilePhone")]
        public string DisplayAttributes
        {
            get
            {
                return (string)this["displayAttributes"];
            }
            set
            {
                this["displayAttributes"] = value;
            }
        }

        [ConfigurationProperty("authorizationSet", IsRequired = false)]
        public string AuthorizationSet
        {
            get
            {
                return (string)this["authorizationSet"];
            }
            set
            {
                this["authorizationSet"] = value;
            }
        }

        [ConfigurationProperty("showNullAttributes", IsRequired = false, DefaultValue = false)]
        public bool ShowNullAttributes
        {
            get
            {
                return (bool)this["showNullAttributes"];
            }
            set
            {
                this["showNullAttributes"] = value;
            }
        }

        internal string[] DisplayAttributeList
        {
            get
            {
                return this.DisplayAttributes.Split(',');
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