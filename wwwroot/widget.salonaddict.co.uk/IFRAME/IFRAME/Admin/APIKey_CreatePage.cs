namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class APIKey_CreatePage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected Label lblError;
        protected Literal ltrSalon;
        protected Panel pnl;
        protected TextBox txtHttpReferer;
        protected TextBox txtVerificationToken;
        protected RequiredFieldValidator valVerificationToken;
        protected ValidatorCalloutExtender valVerificationTokenEx;

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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                string verificationToken = this.txtVerificationToken.Text.Trim();
                if (IoC.Resolve<IUserManager>().GetWidgetApiKeyByVerificationToken(verificationToken) != null)
                {
                    this.lblError.Text = base.GetLocaleResourceString("Error.InUse");
                    return;
                }
                WidgetApiKeyDB apiKey = new WidgetApiKeyDB {
                    Active = true,
                    CreatedOnUtc = DateTime.UtcNow,
                    Deleted = false,
                    HttpReferer = this.txtHttpReferer.Text.Trim(),
                    SalonId = this.PostedSalonId,
                    VerificationToken = verificationToken
                };
                apiKey = IoC.Resolve<IUserManager>().InsertWidgetApiKey(apiKey);
            }
            string str2 = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("salonusers.aspx", new string[] { str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
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
                this.txtVerificationToken.Text = Guid.NewGuid().ToString().Replace("-", string.Empty);
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

