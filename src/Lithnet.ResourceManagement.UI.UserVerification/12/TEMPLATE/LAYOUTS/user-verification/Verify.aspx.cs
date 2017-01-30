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
using System.Globalization;
using System.Threading;

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

        private Dictionary<string, string> RowItems
        {
            get
            {
                Dictionary<string, string> items = this.ViewState["Items"] as Dictionary<string, string>;

                if (items == null)
                {
                    items = new Dictionary<string, string>();
                    this.ViewState["Items"] = items;
                }

                return items;
            }
            set
            {
                this.ViewState["Items"] = value;
            }
        }

        public string UserObjectID => this.Request.QueryString["id"];

        public string ObjectType => this.Request.QueryString["type"] ?? "Person";

        private string GetLocalizedName(string attributeName, string objectType)
        {
            ResourceObject o = this.GetLocalizedObjectType(objectType);
            ResourceObject a = this.GetLocalizedAttributeType(attributeName);
            ResourceObject b = this.GetLocalizedBinding(o, a);

            return b.DisplayName;
        }

        private ResourceObject GetLocalizedBinding(ResourceObject objectType, ResourceObject attributeType)
        {
            ResourceManagementClient c = new ResourceManagementClient();

            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add("BoundAttributeType", attributeType.ObjectID.Value);
            values.Add("BoundObjectType", objectType.ObjectID.Value);

            return c.GetResourceByKey("BindingDescription", values, new List<string>() { "DisplayName" }, CultureInfo.CurrentCulture);

        }

        private ResourceObject GetLocalizedObjectType(string objectType)
        {
            ResourceManagementClient c = new ResourceManagementClient();

            ResourceObject o = c.GetResourceByKey("ObjectTypeDescription", "Name", objectType, CultureInfo.CurrentCulture);

            if (o == null)
            {
                throw new InvalidOperationException($"The objectType {objectType} was not found in the schema");
            }

            return o;
        }

        private ResourceObject GetLocalizedAttributeType(string attributeName)
        {
            ResourceManagementClient c = new ResourceManagementClient();

            ResourceObject o = c.GetResourceByKey("AttributeTypeDescription", "Name", attributeName, CultureInfo.CurrentCulture);

            if (o == null)
            {
                throw new InvalidOperationException($"The attribute {attributeName} was not found in the schema");
            }

            return o;
        }

        private void BuildAttributeTable(ResourceObject o)
        {
            foreach (string attributeName in AppConfigurationSection.CurrentConfig.DisplayAttributeList)
            {
                this.AddRowToTable(this.GetLocalizedName(attributeName, this.ObjectType), o.Attributes[attributeName].StringValue, true);
            }
        }

        private void AddRowToTable(string header, string value, bool persist)
        {
            int rowCount = this.attributeTable.Rows.Count;

            if (value == null)
            {
                SD.Trace.WriteLine($"Ignoring row add request for {header} as its value was null");
                return;
            }

            TableRow row = new TableRow();
            row.ID = $"row{rowCount}";

            TableHeaderCell hc = new TableHeaderCell { Text = header };
            hc.ID = $"th{rowCount}";
            row.Cells.Add(hc);

            TableCell tc = new TableCell { Text = value };
            tc.ID = $"tc{rowCount}";
            row.Cells.Add(tc);

            this.attributeTable.Rows.Add(row);

            if (persist)
            {
                this.RowItems.Add(header, value);
            }

            SD.Trace.WriteLine($"Row {rowCount} added");
        }

        private void ReloadTableStructure()
        {
            if (!this.IsPostBack)
            {
                return;
            }

            SD.Trace.WriteLine($"Reloading table structure");
            
            foreach (KeyValuePair<string, string> kvp in this.RowItems)
            {
                int i = this.attributeTable.Rows.Count;

                TableRow row = new TableRow();
                row.ID = $"row{i}";

                TableHeaderCell hc = new TableHeaderCell();
                hc.ID = $"th{i}";
                hc.Text = kvp.Key;
                row.Cells.Add(hc);

                TableCell tc = new TableCell();
                tc.ID = $"tc{i}";
                tc.Text = kvp.Value;
                row.Cells.Add(tc);

                this.attributeTable.Rows.Add(row);

                SD.Trace.WriteLine($"Row {i} re-added");
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.IsPostBack)
                {
                    this.ReloadTableStructure();
                    return;
                }

                SD.Trace.WriteLine($"Loading page for {Thread.CurrentPrincipal.Identity.Name} authenticated using {Thread.CurrentPrincipal.Identity.AuthenticationType} authentication in culture {CultureInfo.CurrentCulture}");

                this.pageTitle.Text = (string)this.GetLocalResourceObject("PageTitle");
                this.lbHeader.Text = (string)this.GetLocalResourceObject("PageTitle");
                this.btSend.Text = (string)this.GetLocalResourceObject("PageButtonSendCode");
                this.ClearError();

                ResourceManagementClient c = new ResourceManagementClient();
                List<string> attributeList = new List<string>();
                attributeList.AddRange(AppConfigurationSection.CurrentConfig.DisplayAttributeList);
                attributeList.Add(AppConfigurationSection.CurrentConfig.PhoneNumberAttributeName);

                ResourceObject o = c.GetResourceByKey(this.ObjectType, AppConfigurationSection.CurrentConfig.SearchAttributeName, this.UserObjectID, attributeList);

                if (o == null)
                {
                    this.SetError((string)this.GetLocalResourceObject("ErrorUserNotFound"));
                    return;
                }

                this.BuildAttributeTable(o);

                if (!o.Attributes.ContainsAttribute(AppConfigurationSection.CurrentConfig.PhoneNumberAttributeName) || o.Attributes[AppConfigurationSection.CurrentConfig.PhoneNumberAttributeName].IsNull)
                {
                    this.SetError((string)this.GetLocalResourceObject("ErrorUserHasNoMobileNumber"));
                    return;
                }
                else
                {
                    this.ClearError();
                    this.SmsTarget = o.Attributes[AppConfigurationSection.CurrentConfig.PhoneNumberAttributeName].StringValue;
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

                this.AddRowToTable((string)this.GetLocalResourceObject("SecurityCode"), code, false);
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