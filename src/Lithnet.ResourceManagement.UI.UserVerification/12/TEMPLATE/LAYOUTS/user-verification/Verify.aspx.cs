using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using Lithnet.ResourceManagement.Client;
using Microsoft.IdentityManagement.SmsServiceProvider;
using SD = System.Diagnostics;

namespace Lithnet.ResourceManagement.UI.UserVerification
{
    public partial class Verify : System.Web.UI.Page
    {
        private static Random random = new Random();

        private static ISmsServiceProvider provider;

        private static ISmsServiceProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    LoadSmsProvider();
                }

                return provider;
            }
        }

        private string SmsTarget
        {
            get
            {
                return (string)this.ViewState[nameof(this.SmsTarget)];
            }
            set
            {
                this.ViewState[nameof(this.SmsTarget)] = value;
            }
        }

        public string UserObjectID => this.Request.QueryString["id"];

        public string ObjectType => this.Request.QueryString["type"] ?? "Person";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.IsPostBack)
                {
                    return;
                }

                this.pageTitle.Text = (string)this.GetLocalResourceObject("PageTitle");

                this.ClearError();

                ResourceManagementClient c = new ResourceManagementClient();
                ResourceObject o = c.GetResourceByKey(this.ObjectType, AppConfigurationSection.CurrentConfig.SearchAttributeName, this.UserObjectID, new List<string> {  AppConfigurationSection.CurrentConfig.AccountNameAttributeName, AppConfigurationSection.CurrentConfig.PhoneNumberAttributeName, AppConfigurationSection.CurrentConfig.DisplayNameAttributeName });

                if (o == null)
                {
                    this.SetError((string)this.GetLocalResourceObject("ErrorUserNotFound"));
                    this.lbPhone.Text = null;
                    return;
                }


                if (o.Attributes.ContainsAttribute(AppConfigurationSection.CurrentConfig.AccountNameAttributeName))
                {
                    this.lbUser.Text = o.Attributes[AppConfigurationSection.CurrentConfig.AccountNameAttributeName].StringValue;
                }

                if (o.Attributes.ContainsAttribute(AppConfigurationSection.CurrentConfig.DisplayNameAttributeName))
                {
                    this.lbName.Text = o.Attributes[AppConfigurationSection.CurrentConfig.DisplayNameAttributeName].StringValue;
                }

                if (!o.Attributes.ContainsAttribute(AppConfigurationSection.CurrentConfig.PhoneNumberAttributeName) || o.Attributes[AppConfigurationSection.CurrentConfig.PhoneNumberAttributeName].IsNull)
                {
                    this.SetError((string)this.GetLocalResourceObject("ErrorUserHasNoMobileNumber"));
                    this.lbPhone.Text = null;
                    this.lbSecurityCode.Text = null;
                    this.rowSecurityCode.Visible = false;
                    return;
                }
                else
                {
                    this.ClearError();
                    this.SmsTarget = o.Attributes[AppConfigurationSection.CurrentConfig.PhoneNumberAttributeName].StringValue;
                    this.lbPhone.Text = this.SmsTarget;
                }
            }
            catch (Exception ex)
            {
                SD.Trace.WriteLine($"Exception in page_load\n {ex.ToString()}");
                this.SetError("An unexpected error occurred:\n" + ex.ToString());
            }
        }

        private void ClearError()
        {
            this.SetError(null);
        }

        private void SetError(string message)
        {
            if (message == null)
            {
                this.warning.Visible = false;
                this.lbWarning.Text = null;
                this.btSend.Enabled = true;
            }
            else
            {
                this.warning.Visible = true;
                this.lbWarning.Text = message;
                this.btSend.Enabled = false;
            }
        }



        private static string GenerateCode()
        {
            return random.Next(AppConfigurationSection.CurrentConfig.SmsCodeLowRange, AppConfigurationSection.CurrentConfig.SmsCodeHighRange).ToString($"D{AppConfigurationSection.CurrentConfig.SmsCodeLength}");
        }

        private static void LoadSmsProvider()
        {
            SD.Trace.Write("Loading SMS provider");

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;


            string path = Environment.ExpandEnvironmentVariables(AppConfigurationSection.CurrentConfig.SmsServiceProviderDll);

            SD.Trace.WriteLine($"Attempting to load provider from {path}");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The SMS provider DLL was not found at {path}. If the file exists in another location, specify that in the smsServiceProviderDll section of the web.config file");
            }

            Assembly assembly = Assembly.LoadFile(path);

            SD.Trace.WriteLine($"Loaded assembly");

            Type[] types = assembly.GetExportedTypes();

            SD.Trace.WriteLine($"Got types");


            foreach (Type t in types)
            {
                if (typeof(ISmsServiceProvider).IsAssignableFrom(t))
                {
                    SD.Trace.WriteLine($"Found type that implements ISmsServiceProvider");
                    Verify.provider = (ISmsServiceProvider)Activator.CreateInstance(t);
                    SD.Trace.WriteLine($"Provider loaded");
                    return;
                }
            }

            SD.Trace.WriteLine($"Did not find any types that implement ISmsServiceProvider");

            throw new InvalidOperationException("The specified SMS provider did not contain an ISmsServiceProvider interface");
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            SD.Trace.WriteLine($"Attempting to resolve assembly {args.Name}");

            string folderPath = Path.GetDirectoryName(Environment.ExpandEnvironmentVariables(AppConfigurationSection.CurrentConfig.SmsServiceProviderDll));
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath))
            {
                SD.Trace.WriteLine($"Assembly not found at path {assemblyPath}");
                return null;
            }

            SD.Trace.WriteLine($"Assembly found at path {assemblyPath}");
            return Assembly.LoadFrom(assemblyPath);
        }

        protected void btSend_Click(object sender, EventArgs e)
        {
            string code = null;

            try
            {
                if (this.SmsTarget == null)
                {
                    SD.Trace.WriteLine($"No sms target was set. Aborting send operation");
                    this.SetError((string)this.GetLocalResourceObject("ErrorNoSmsTargetAvailable"));
                    return;
                }

                this.ClearError();
                code = GenerateCode();
                SD.Trace.WriteLine($"Sending code {code} for {this.SmsTarget}");
                Provider.SendSms(this.SmsTarget, string.Format((string)this.GetLocalResourceObject("SmsContent"), code), Guid.NewGuid(), null);
                SD.Trace.WriteLine($"Sent code {code} to {this.SmsTarget}");

                this.rowSecurityCode.Visible = true;
                this.lbSecurityCode.Text = code;
                this.btSend.Text = (string)this.GetLocalResourceObject("PageButtonSendAnotherCode");

            }
            catch (Exception ex)
            {
                SD.Trace.WriteLine($"Exception sending code {code} to {this.SmsTarget}\n {ex.ToString()}");
                this.SetError(string.Format((string)this.GetLocalResourceObject("ErrorMessageSendFailure"), ex.ToString()));
            }
        }
    }
}