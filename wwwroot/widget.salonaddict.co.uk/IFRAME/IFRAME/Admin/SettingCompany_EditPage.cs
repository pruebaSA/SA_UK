namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class SettingCompany_EditPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected SalonMenu cntlMenu;
        protected Label lblError;
        protected Panel pnl;
        protected TextBox txtCompany;
        protected TextBox txtPassword;
        protected RequiredFieldValidator valCompany;
        protected ValidatorCalloutExtender valCompanyEx;
        protected RequiredFieldValidator valPassword;
        protected ValidatorCalloutExtender valPasswordEx;

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("settings.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
                string str = this.txtPassword.Text.Trim();
                if (IoC.Resolve<ISecurityManager>().DecryptUserPassword(workingUser.Password, IFRAME.Controllers.Settings.Security_Key_3DES) != str)
                {
                    this.lblError.Text = base.GetLocaleResourceString("lblError.Text");
                    return;
                }
                salonById.Company = this.txtCompany.Text.Trim();
                salonById = IoC.Resolve<ISalonManager>().UpdateSalon(salonById);
            }
            string str3 = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("settings.aspx", new string[] { str3 });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            this.Page.Form.Attributes.Add("autocomplete", "off");
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                this.txtCompany.Text = string.IsNullOrEmpty(salonById.Company) ? salonById.Name : salonById.Company;
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

