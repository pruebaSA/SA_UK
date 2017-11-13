namespace IFRAME.SecureArea
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class Profile_EditPage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected DropDownList ddlMobileArea;
        protected Panel pnl;
        protected TextBox txtFirstName;
        protected TextBox txtLastName;
        protected TextBox txtMobile;
        protected TextBox txtPhone;
        protected RequiredFieldValidator valFirstName;
        protected ValidatorCalloutExtender valFirstNameEX;
        protected RegularExpressionValidator valFirstNameRegEx1;
        protected ValidatorCalloutExtender valFirstNameRegExEx1;
        protected RegularExpressionValidator valLastNameRegEx1;
        protected ValidatorCalloutExtender valLastNameRegEx1Ex;
        protected RegularExpressionValidator valMobileRegEx;
        protected ValidatorCalloutExtender valMobileRegExEx;
        protected RegularExpressionValidator valPhoneRegEx;
        protected ValidatorCalloutExtender valPhoneRegExEx;

        private void BindProfileDetails()
        {
            SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
            this.txtFirstName.Text = workingUser.FirstName;
            this.txtLastName.Text = workingUser.LastName;
            this.txtPhone.Text = workingUser.PhoneNumber;
            if (!string.IsNullOrEmpty(workingUser.Mobile))
            {
                if (workingUser.Mobile.Length >= 3)
                {
                    this.ddlMobileArea.SelectedValue = workingUser.Mobile.Substring(0, 3).Trim();
                }
                if (workingUser.Mobile.Length >= 4)
                {
                    this.txtMobile.Text = workingUser.Mobile.Substring(3).Trim();
                }
            }
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
                workingUser.FirstName = this.txtFirstName.Text.Trim();
                workingUser.LastName = this.txtLastName.Text.Trim();
                workingUser.PhoneNumber = this.txtPhone.Text.Trim();
                if ((this.ddlMobileArea.SelectedValue != string.Empty) && (this.txtMobile.Text.Trim() != string.Empty))
                {
                    workingUser.Mobile = $"{this.ddlMobileArea.SelectedValue} {this.txtMobile.Text.Trim()}";
                }
                workingUser = IoC.Resolve<IUserManager>().UpdateSalonUser(workingUser);
            }
            string uRL = IFRMHelper.GetURL("profile.aspx", new string[0]);
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

