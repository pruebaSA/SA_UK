namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class ProfilePage : IFRMSecurePage
    {
        protected Button btnEdit;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected Literal ltrEmail;
        protected Literal ltrMobile;
        protected Literal ltrName;
        protected Literal ltrPhone;
        protected Panel pnl;

        private void BindProfileDetails()
        {
            SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
            this.ltrEmail.Text = workingUser.Email;
            this.ltrMobile.Text = workingUser.Mobile;
            this.ltrName.Text = (workingUser.FirstName + " " + workingUser.LastName).Trim();
            this.ltrPhone.Text = workingUser.PhoneNumber;
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("profile-edit.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.BindProfileDetails();
            }
        }
    }
}

