namespace IFRAME.SecureArea
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class Profile_Email_EditPage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected Literal ltrEmail;
        protected Panel pnl;
        protected TextBox txtEmailChanged;
        protected TextBox txtEmailConfirm;
        protected RequiredFieldValidator valEmailChanged;
        protected ValidatorCalloutExtender valEmailChangedEx;
        protected RegularExpressionValidator valEmailChangedRegEx;
        protected ValidatorCalloutExtender valEmailChangedRegExEx;
        protected RequiredFieldValidator valEmailConfirm;
        protected CompareValidator valEmailConfirmComp;
        protected ValidatorCalloutExtender valEmailConfirmCompEx;
        protected ValidatorCalloutExtender valEmailConfirmEx;

        private void BindEmailDetails()
        {
            SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
            this.ltrEmail.Text = workingUser.Email;
        }

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
                workingUser.Email = this.txtEmailChanged.Text.Trim();
                workingUser = IoC.Resolve<IUserManager>().UpdateSalonUser(workingUser);
            }
            string uRL = IFRMHelper.GetURL("profile.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.BindEmailDetails();
            }
        }
    }
}

