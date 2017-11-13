namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class SalonUser_Password_DetailsPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected Label lblError;
        protected Literal ltrSalon;
        protected Literal ltrUsername;
        protected Panel pnl;
        protected TextBox txtPassword;
        protected RequiredFieldValidator valPassword;
        protected ValidatorCalloutExtender valPasswordEx;

        private void BindSalonDetails(SalonDB value)
        {
            this.ltrSalon.Text = value.Name;
        }

        private void BindUserDetails(SalonUserDB value)
        {
            this.ltrUsername.Text = value.Username;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string str2 = $"{"uid"}={this.PostedUserId}";
            string uRL = IFRMHelper.GetURL("salonuser-edit.aspx", new string[] { str, str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonUserDB salonUserById = IoC.Resolve<IUserManager>().GetSalonUserById(this.PostedUserId);
                string str = this.txtPassword.Text.Trim();
                if (IoC.Resolve<ISecurityManager>().DecryptUserPassword(IFRMContext.Current.WorkingUser.Password, IFRAME.Controllers.Settings.Security_Key_3DES) != str)
                {
                    this.FailedAttempts++;
                    if (this.FailedAttempts > 3)
                    {
                        base.Response.Redirect(IFRMHelper.GetURL("~/logout.aspx", new string[0]), true);
                    }
                    this.lblError.Text = base.GetLocaleResourceString("lblError.Text");
                }
                else
                {
                    this.txtPassword.TextMode = TextBoxMode.SingleLine;
                    this.txtPassword.Text = IoC.Resolve<ISecurityManager>().DecryptUserPassword(salonUserById.Password, IFRAME.Controllers.Settings.Security_Key_3DES);
                    this.btnCancel.Text = base.GetLocaleResourceString("btnBack.Text");
                    this.btnSave.Visible = false;
                }
            }
            else
            {
                string str3 = $"{"sid"}={this.PostedSalonId}";
                string uRL = IFRMHelper.GetURL("salonusers.aspx", new string[] { str3 });
                base.Response.Redirect(uRL, true);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Func<SalonUserDB, bool> predicate = null;
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("salons.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (this.PostedUserId == Guid.Empty)
            {
                string url = IFRMHelper.GetURL("salons.aspx", new string[0]);
                base.Response.Redirect(url, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string str3 = IFRMHelper.GetURL("salons.aspx", new string[0]);
                    base.Response.Redirect(str3, true);
                }
                if (predicate == null)
                {
                    predicate = item => item.UserId == this.PostedUserId;
                }
                SalonUserDB rdb = IoC.Resolve<IUserManager>().GetSalonUsersBySalonId(salonById.SalonId).SingleOrDefault<SalonUserDB>(predicate);
                if (rdb == null)
                {
                    string str4 = IFRMHelper.GetURL("salons.aspx", new string[0]);
                    base.Response.Redirect(str4, true);
                }
                this.BindSalonDetails(salonById);
                this.BindUserDetails(rdb);
            }
        }

        public int FailedAttempts
        {
            get
            {
                if (this.ViewState["FailedAttempts"] == null)
                {
                    return 0;
                }
                return int.Parse(this.ViewState["FailedAttempts"].ToString());
            }
            set
            {
                this.ViewState["FailedAttempts"] = value.ToString();
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

