namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class SalonUser_DeletePage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnDelete;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected Literal ltrEmail;
        protected Literal ltrMobile;
        protected Literal ltrName;
        protected Literal ltrPhone;
        protected Literal ltrSalon;
        protected Literal ltrUsername;
        protected Panel pnl;

        private void BindSalonDetails(SalonDB value)
        {
            this.ltrSalon.Text = value.Name;
        }

        private void BindUserDetails(SalonUserDB value)
        {
            this.ltrUsername.Text = value.Username;
            this.ltrName.Text = (value.FirstName + " " + value.LastName).Trim();
            this.ltrPhone.Text = value.PhoneNumber;
            this.ltrMobile.Text = value.Mobile;
            this.ltrEmail.Text = value.Email;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("salonusers.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            SalonUserDB salonUserById = IoC.Resolve<IUserManager>().GetSalonUserById(this.PostedUserId);
            IoC.Resolve<IUserManager>().DeleteSalonUser(salonUserById);
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("salonusers.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Func<SalonUserDB, bool> predicate = null;
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("salonusers.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (this.PostedUserId == Guid.Empty)
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
                if (predicate == null)
                {
                    predicate = item => item.UserId == this.PostedUserId;
                }
                SalonUserDB rdb = IoC.Resolve<IUserManager>().GetSalonUsersBySalonId(salonById.SalonId).SingleOrDefault<SalonUserDB>(predicate);
                if (rdb == null)
                {
                    string str4 = IFRMHelper.GetURL("salonusers.aspx", new string[0]);
                    base.Response.Redirect(str4, true);
                }
                this.BindSalonDetails(salonById);
                this.BindUserDetails(rdb);
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

        public Guid PostedUserId
        {
            get
            {
                string str = base.Request.QueryString["uid"];
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

