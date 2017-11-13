namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class SalonUser_EditPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected DropDownList ddlMobileArea;
        protected Literal ltrSalon;
        protected Literal ltrUsername;
        protected Panel pnl;
        protected TextBox txtFirstName;
        protected TextBox txtLastName;
        protected TextBox txtMobile;
        protected TextBox txtPhone;
        protected RequiredFieldValidator valFirstName;
        protected ValidatorCalloutExtender valFirstNameEX;
        protected RegularExpressionValidator valFirstNameRegEx1;
        protected ValidatorCalloutExtender valFirstNameRegExEx1;
        protected RegularExpressionValidator valLastNameRegEx1;
        protected ValidatorCalloutExtender valLastNameRegEx1Ex;
        protected RegularExpressionValidator valMobileRegEx;
        protected ValidatorCalloutExtender valMobileRegExEx;
        protected RegularExpressionValidator valPhoneRegEx;
        protected ValidatorCalloutExtender valPhoneRegExEx;

        private void BindSalonDetails(SalonDB value)
        {
            this.ltrSalon.Text = value.Name;
        }

        private void BindUserDetails(SalonUserDB value)
        {
            this.ltrUsername.Text = value.Username;
            this.txtFirstName.Text = value.FirstName;
            this.txtLastName.Text = value.LastName;
            this.txtPhone.Text = value.PhoneNumber;
            if (!string.IsNullOrEmpty(value.Mobile))
            {
                if (value.Mobile.Length >= 3)
                {
                    this.ddlMobileArea.SelectedValue = value.Mobile.Substring(0, 3).Trim();
                }
                if (value.Mobile.Length >= 4)
                {
                    this.txtMobile.Text = value.Mobile.Substring(3).Trim();
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("salonusers.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Func<SalonUserDB, bool> predicate = null;
            if (this.Page.IsValid)
            {
                if (predicate == null)
                {
                    predicate = item => item.UserId == this.PostedUserId;
                }
                SalonUserDB user = IoC.Resolve<IUserManager>().GetSalonUsersBySalonId(this.PostedSalonId).SingleOrDefault<SalonUserDB>(predicate);
                user.FirstName = this.txtFirstName.Text.Trim();
                user.LastName = this.txtLastName.Text.Trim();
                user.PhoneNumber = this.txtPhone.Text.Trim();
                if ((this.ddlMobileArea.SelectedValue != string.Empty) && (this.txtMobile.Text.Trim() != string.Empty))
                {
                    user.Mobile = $"{this.ddlMobileArea.SelectedValue} {this.txtMobile.Text.Trim()}";
                }
                user = IoC.Resolve<IUserManager>().UpdateSalonUser(user);
            }
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("salonusers.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Func<SalonUserDB, bool> predicate = null;
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("salonusers.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (this.PostedUserId == Guid.Empty)
            {
                string url = IFRMHelper.GetURL("salonusers.aspx", new string[0]);
                base.Response.Redirect(url, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string str3 = IFRMHelper.GetURL("salonusers.aspx", new string[0]);
                    base.Response.Redirect(str3, true);
                }
                if (predicate == null)
                {
                    predicate = item => item.UserId == this.PostedUserId;
                }
                SalonUserDB rdb = IoC.Resolve<IUserManager>().GetSalonUsersBySalonId(salonById.SalonId).SingleOrDefault<SalonUserDB>(predicate);
                if (rdb == null)
                {
                    string str4 = IFRMHelper.GetURL("salonusers.aspx", new string[0]);
                    base.Response.Redirect(str4, true);
                }
                this.BindSalonDetails(salonById);
                this.BindUserDetails(rdb);
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

