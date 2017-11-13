namespace IFRAME.SecureArea
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class Profile_Password_EditPage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected Label lblError;
        protected Panel pnl;
        protected TextBox txtPassword;
        protected TextBox txtPasswordChange;
        protected TextBox txtPasswordConfirm;
        protected RequiredFieldValidator valPassword;
        protected RequiredFieldValidator valPasswordChange;
        protected ValidatorCalloutExtender valPasswordChangeEx;
        protected RegularExpressionValidator valPasswordChangeRegEx;
        protected ValidatorCalloutExtender valPasswordChangeRegExEx;
        protected RequiredFieldValidator valPasswordConfirm;
        protected CompareValidator valPasswordConfirmComp;
        protected ValidatorCalloutExtender valPasswordConfirmCompEx;
        protected ValidatorCalloutExtender valPasswordConfirmEx;
        protected ValidatorCalloutExtender valPasswordEx;

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("profile.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
                string str = this.txtPassword.Text.Trim();
                if (IoC.Resolve<ISecurityManager>().DecryptUserPassword(workingUser.Password, IFRAME.Controllers.Settings.Security_Key_3DES) != str)
                {
                    this.lblError.Text = base.GetLocaleResourceString("lblError.Text");
                    return;
                }
                workingUser.Password = IoC.Resolve<ISecurityManager>().EncryptUserPassword(this.txtPasswordChange.Text.Trim(), IFRAME.Controllers.Settings.Security_Key_3DES);
                workingUser = IoC.Resolve<IUserManager>().UpdateSalonUser(workingUser);
            }
            string uRL = IFRMHelper.GetURL("profile.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}

