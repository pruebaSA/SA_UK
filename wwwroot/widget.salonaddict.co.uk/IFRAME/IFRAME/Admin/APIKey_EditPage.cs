namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class APIKey_EditPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected Literal ltrSalon;
        protected Literal ltrVerificationToken;
        protected Panel pnl;
        protected TextBox txtHttpReferer;

        private void BindAPIDetails(WidgetApiKeyDB value)
        {
            this.ltrVerificationToken.Text = value.VerificationToken;
            this.txtHttpReferer.Text = value.HttpReferer;
        }

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
                WidgetApiKeyDB widgetApiKeyById = IoC.Resolve<IUserManager>().GetWidgetApiKeyById(this.PostedWApiKeyId);
                widgetApiKeyById.HttpReferer = this.txtHttpReferer.Text.Trim();
                widgetApiKeyById = IoC.Resolve<IUserManager>().UpdateWidgetApiKey(widgetApiKeyById);
            }
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("salonusers.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("salonusers.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (this.PostedWApiKeyId == Guid.Empty)
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
                WidgetApiKeyDB widgetApiKeyById = IoC.Resolve<IUserManager>().GetWidgetApiKeyById(this.PostedWApiKeyId);
                if (widgetApiKeyById == null)
                {
                    string str4 = IFRMHelper.GetURL("salonusers.aspx", new string[0]);
                    base.Response.Redirect(str4, true);
                }
                if (widgetApiKeyById.SalonId != salonById.SalonId)
                {
                    string str5 = IFRMHelper.GetURL("salonusers.aspx", new string[0]);
                    base.Response.Redirect(str5, true);
                }
                this.BindSalonDetails(salonById);
                this.BindAPIDetails(widgetApiKeyById);
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

        public Guid PostedWApiKeyId
        {
            get
            {
                string str = base.Request.QueryString["wapi"];
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

