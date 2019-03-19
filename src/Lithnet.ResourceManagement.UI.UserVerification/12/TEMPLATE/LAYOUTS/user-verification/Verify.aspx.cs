using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Lithnet.ResourceManagement.Client;
using SD = System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Linq;

namespace Lithnet.ResourceManagement.UI.UserVerification
{
    public partial class Verify : System.Web.UI.Page
    {
        private static Random random = new Random();

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

        private string UserObjectID => this.Request.QueryString["id"];

        private string ObjectType => this.Request.QueryString["type"] ?? "Person";

        private void BuildAttributeTable(ResourceObject o)
        {
            foreach (string attributeName in AppConfigurationSection.CurrentConfig.DisplayAttributeList)
            {
                string value;

                AttributeValue attribute = o.Attributes[attributeName];

                if (attribute.IsNull)
                {
                    value = null;
                }
                else
                {
                    value = string.Join("<br/>", attribute.ValuesAsString);
                }

                this.AddRowToTable(LocalizationHelper.GetLocalizedName(attributeName, this.ObjectType), value, true);
            }
        }

        private void AddRowToTable(string header, string value, bool persist)
        {
            int rowCount = this.attributeTable.Rows.Count;

            if (value == null && !AppConfigurationSection.CurrentConfig.ShowNullAttributes)
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

        private bool IsAuthorized()
        {
            if (string.IsNullOrEmpty(AppConfigurationSection.CurrentConfig.AuthorizationSet))
            {
                return true;
            }

            ResourceObject o = UserUtils.GetCurrentUser();

            if (o == null)
            {
                SD.Trace.WriteLine("Current user was not found in the FIM service");
                return false;
            }

            Guid g;
            if (Guid.TryParse(AppConfigurationSection.CurrentConfig.AuthorizationSet, out g))
            {
                return UserUtils.IsMemberOfSet(o, g);
            }
            else
            {
                return UserUtils.IsMemberOfSet(o, AppConfigurationSection.CurrentConfig.AuthorizationSet);
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

                this.btSend.Text = (string)this.GetLocalResourceObject("PageButtonSendCode");
                this.ClearError();

                if (!this.IsAuthorized())
                {
                    this.SetError((string)this.GetLocalResourceObject("NotAuthorized"));
                    this.btSend.Visible = false;
                    SD.Trace.WriteLine("User is not authorized to use this tool");
                    return;
                }

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
                this.divWarning.Visible = false;
                this.lbWarning.Text = null;
                this.btSend.Enabled = true;
            }
            else
            {
                this.divWarning.Visible = true;
                this.lbWarning.Text = message;
                this.btSend.Enabled = false;
            }
        }

        private static string GenerateCode()
        {
            return random.Next(AppConfigurationSection.CurrentConfig.SmsCodeLowRange, AppConfigurationSection.CurrentConfig.SmsCodeHighRange).ToString($"D{AppConfigurationSection.CurrentConfig.SmsCodeLength}");
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

                string content = AppConfigurationSection.CurrentConfig.SmsContent ?? (string)this.GetLocalResourceObject("SmsContent") ?? "Your code is {0}";

                SmsProvider.SendSms(this.SmsTarget, string.Format(content, code));
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