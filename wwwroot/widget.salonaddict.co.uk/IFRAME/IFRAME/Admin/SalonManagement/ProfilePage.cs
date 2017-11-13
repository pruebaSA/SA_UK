namespace IFRAME.Admin.SalonManagement
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class ProfilePage : IFRMAdminPage
    {
        protected Button btnEdit;
        protected SalonMenu cntlMenu;
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

