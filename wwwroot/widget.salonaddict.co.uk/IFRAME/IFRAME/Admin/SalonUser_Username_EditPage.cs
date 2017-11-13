namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Drawing;
    using System.Web.UI.WebControls;

    public class SalonUser_Username_EditPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnCheck;
        protected Button btnSave;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected Label lblCheck;
        protected Label lblError;
        protected Literal ltrSalon;
        protected Panel pnl;
        protected TextBox txtPassword;
        protected TextBox txtUsername;
        protected ValidatorCalloutExtender txtUsernameEx;
        protected RequiredFieldValidator valPassword;
        protected ValidatorCalloutExtender valPasswordEx;
        protected RequiredFieldValidator valUsername;
        protected RegularExpressionValidator valUsernameRegex;
        protected ValidatorCalloutExtender valUsernameRegexEx;

        private void BindSalonDetails(SalonDB value)
        {
            this.ltrSalon.Text = value.Name;
        }

        private void BindUserDetails(SalonUserDB value)
        {
            this.txtUsername.Text = value.Username;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string str2 = $"{"uid"}={this.PostedUserId}";
            string uRL = IFRMHelper.GetURL("salonuser-edit.aspx", new string[] { str, str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void btnCheck_Click(object sender, EventArgs e)
        {
            string username = this.txtUsername.Text.Trim();
            if ((username != string.Empty) && (IoC.Resolve<IUserManager>().GetSalonUserByUsername(username) == null))
            {
                this.lblCheck.ForeColor = Color.Green;
                this.lblCheck.Text = base.GetLocaleResourceString("lblCheck.Available");
            }
            else
            {
                this.lblCheck.ForeColor = ColorTranslator.FromHtml("#c80d0d");
                this.lblCheck.Text = base.GetLocaleResourceString("lblCheck.NotAvailable");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonUserDB salonUserById = IoC.Resolve<IUserManager>().GetSalonUserById(this.PostedUserId);
                string str = this.txtPassword.Text.Trim();
                if (IoC.Resolve<ISecurityManager>().DecryptUserPassword(IFRMContext.Current.WorkingUser.Password, IFRAME.Controllers.Settings.Security_Key_3DES) != str)
                {
                    this.lblError.Text = base.GetLocaleResourceString("lblError.Text");
                    return;
                }
                salonUserById.Username = this.txtUsername.Text.Trim();
                salonUserById = IoC.Resolve<IUserManager>().UpdateSalonUser(salonUserById);
            }
            string str3 = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("salonusers.aspx", new string[] { str3 });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.valUsernameRegex.ValidationExpression = ".{" + IFRAME.Controllers.Settings.IFRMMembership_MinimumUsernameLength + "}.*";
            this.valUsernameRegex.ErrorMessage = string.Format(base.GetLocaleResourceString("valUsernameRegex.ErrorMessage"), IFRAME.Controllers.Settings.IFRMMembership_MinimumUsernameLength);
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("salonusers.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string url = IFRMHelper.GetURL("salonusers.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                this.BindSalonDetails(salonById);
                SalonUserDB salonUserById = IoC.Resolve<IUserManager>().GetSalonUserById(this.PostedUserId);
                if (salonUserById == null)
                {
                    string str3 = IFRMHelper.GetURL("salonusers.aspx", new string[0]);
                    base.Response.Redirect(str3, true);
                }
                this.BindUserDetails(salonUserById);
            }
        }

        public Guid PostedSalonId
        {
            get
            {
                string str = base.Request.QueryString["sid"];
                if (!string.IsNullOrEmpty(str))
                {
                    try
                    {
                        return new Guid(str);
                    }
                    catch
                    {
                    }
                }
                return Guid.Empty;
            }
        }

        public Guid PostedUserId
        {
            get
            {
                string str = base.Request.QueryString["uid"];
                if (!string.IsNullOrEmpty(str))
                {
                    try
                    {
                        return new Guid(str);
                    }
                    catch
                    {
                    }
                }
                return Guid.Empty;
            }
        }
    }
}

