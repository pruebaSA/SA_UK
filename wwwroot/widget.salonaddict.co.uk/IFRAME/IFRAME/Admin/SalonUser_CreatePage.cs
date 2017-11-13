namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Drawing;
    using System.Web.UI.WebControls;

    public class SalonUser_CreatePage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnCheck;
        protected Button btnSave;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected DropDownList ddlMobileArea;
        protected Label lblCheck;
        protected Literal ltrSalon;
        protected Panel pnl;
        protected TextBox txtEmail;
        protected TextBox txtFirstName;
        protected TextBox txtLastName;
        protected TextBox txtMobile;
        protected TextBox txtPassword;
        protected TextBox txtPhone;
        protected TextBox txtUsername;
        protected ValidatorCalloutExtender txtUsernameEx;
        protected RegularExpressionValidator valEmailRegEx;
        protected ValidatorCalloutExtender valEmailRegExEx;
        protected RequiredFieldValidator valFirstName;
        protected ValidatorCalloutExtender valFirstNameEX;
        protected RegularExpressionValidator valFirstNameRegEx1;
        protected ValidatorCalloutExtender valFirstNameRegExEx1;
        protected RegularExpressionValidator valLastNameRegEx1;
        protected ValidatorCalloutExtender valLastNameRegEx1Ex;
        protected RegularExpressionValidator valMobileRegEx;
        protected ValidatorCalloutExtender valMobileRegExEx;
        protected RequiredFieldValidator valPassword;
        protected ValidatorCalloutExtender valPasswordEx;
        protected RegularExpressionValidator valPasswordRegex;
        protected ValidatorCalloutExtender valPasswordRegexEx;
        protected RegularExpressionValidator valPhoneRegEx;
        protected ValidatorCalloutExtender valPhoneRegExEx;
        protected RequiredFieldValidator valUsername;
        protected RegularExpressionValidator valUsernameRegex;
        protected ValidatorCalloutExtender valUsernameRegexEx;

        private void BindSalonDetails(SalonDB value)
        {
            this.ltrSalon.Text = value.Name;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("salonusers.aspx", new string[] { str });
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
                if (IoC.Resolve<IUserManager>().GetSalonUserByUsername(this.txtUsername.Text.Trim()) != null)
                {
                    this.lblCheck.ForeColor = ColorTranslator.FromHtml("#c80d0d");
                    this.lblCheck.Text = base.GetLocaleResourceString("lblCheck.NotAvailable");
                    return;
                }
                SalonUserDB user = new SalonUserDB {
                    Active = true,
                    CreatedOnUtc = DateTime.UtcNow,
                    Deleted = false,
                    PhoneNumber = this.txtPhone.Text.Trim(),
                    Username = this.txtUsername.Text.Trim()
                };
                user.DisplayText = user.FirstName ?? user.Username;
                user.Email = this.txtEmail.Text.Trim();
                user.FirstName = this.txtFirstName.Text.Trim();
                user.IsAdmin = false;
                user.IsApproved = true;
                user.IsConfirmed = true;
                user.IsGuest = false;
                user.IsLockedOut = false;
                user.IsTaxExempt = false;
                user.LastIPAddress = base.Request.UserHostAddress;
                user.LastName = this.txtLastName.Text.Trim();
                user.Password = IoC.Resolve<ISecurityManager>().EncryptUserPassword(this.txtPassword.Text.Trim(), IFRAME.Controllers.Settings.Security_Key_3DES);
                user.PasswordFormat = "ENC3DES";
                user.PasswordSalt = "";
                if ((this.ddlMobileArea.SelectedValue != string.Empty) && (this.txtMobile.Text.Trim() != string.Empty))
                {
                    user.Mobile = $"{this.ddlMobileArea.SelectedValue} {this.txtMobile.Text.Trim()}";
                }
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                user.SalonId = salonById.SalonId;
                user = IoC.Resolve<IUserManager>().InsertSalonUser(user);
            }
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("salonusers.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.valUsernameRegex.ValidationExpression = ".{" + IFRAME.Controllers.Settings.IFRMMembership_MinimumUsernameLength + "}.*";
            this.valUsernameRegex.ErrorMessage = string.Format(base.GetLocaleResourceString("valUsernameRegex.ErrorMessage"), IFRAME.Controllers.Settings.IFRMMembership_MinimumUsernameLength);
            this.valPasswordRegex.ValidationExpression = ".{" + IFRAME.Controllers.Settings.IFRMMembership_MinimumPasswordLength + "}.*";
            this.valPasswordRegex.ErrorMessage = string.Format(base.GetLocaleResourceString("valPasswordRegex.ErrorMessage"), IFRAME.Controllers.Settings.IFRMMembership_MinimumPasswordLength);
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
    }
}

