namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class ProfilePage : IFRMAdminPage
    {
        protected IFRAME.Admin.Modules.Menu cntlMenu;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.BindProfileDetails();
            }
        }
    }
}

