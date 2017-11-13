namespace IFRAME.SecureArea
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class SettingVAT_EditPage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected Label lblError;
        protected Panel pnl;
        protected TextBox txtPassword;
        protected TextBox txtVAT;
        protected RequiredFieldValidator valPassword;
        protected ValidatorCalloutExtender valPasswordEx;
        protected RequiredFieldValidator valVAT;
        protected ValidatorCalloutExtender valVATEx;
        protected RegularExpressionValidator valVATRegex1;
        protected ValidatorCalloutExtender valVATRegex1Ex;

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("settings.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
                string str = this.txtPassword.Text.Trim();
                if (IoC.Resolve<ISecurityManager>().DecryptUserPassword(workingUser.Password, IFRAME.Controllers.Settings.Security_Key_3DES) != str)
                {
                    this.lblError.Text = base.GetLocaleResourceString("lblError.Text");
                    return;
                }
                salon.VatNumber = this.txtVAT.Text.Trim();
                salon = IoC.Resolve<ISalonManager>().UpdateSalon(salon);
            }
            string uRL = IFRMHelper.GetURL("settings.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.Form.Attributes.Add("autocomplete", "off");
            if (!this.Page.IsPostBack)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                this.txtVAT.Text = salon.VatNumber;
            }
        }
    }
}

