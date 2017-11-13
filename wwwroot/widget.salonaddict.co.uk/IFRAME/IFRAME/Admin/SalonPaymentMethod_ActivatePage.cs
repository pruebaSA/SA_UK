namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class SalonPaymentMethod_ActivatePage : IFRMAdminPage
    {
        protected Button btnActivate;
        protected Button btnCancel;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected Literal ltrCard;
        protected Literal ltrCardNumber;
        protected Literal ltrExpiry;
        protected Literal ltrSalon;
        protected Panel pnl;

        private void BindPaymentDetails(SalonPaymentMethodDB value)
        {
            SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(value.SalonId);
            this.ltrSalon.Text = salonById.Name;
            this.ltrCard.Text = value.Alias;
            string str = IoC.Resolve<ISecurityManager>().DecryptUserPassword(value.MaskedCardNumber, IFRAME.Controllers.Settings.Security_Key_3DES);
            string s = IoC.Resolve<ISecurityManager>().DecryptUserPassword(value.CardExpirationMonth, IFRAME.Controllers.Settings.Security_Key_3DES);
            string str3 = IoC.Resolve<ISecurityManager>().DecryptUserPassword(value.CardExpirationYear, IFRAME.Controllers.Settings.Security_Key_3DES);
            DateTime time = new DateTime(int.Parse(str3), int.Parse(s), 1);
            this.ltrCardNumber.Text = str;
            this.ltrExpiry.Text = time.ToString("MMM yyyy");
        }

        protected void btnActivate_Click(object sender, EventArgs e)
        {
            SalonPaymentMethodDB salonPaymentMethodById = IoC.Resolve<ISalonManager>().GetSalonPaymentMethodById(this.PostedSalonPaymentMethodId);
            salonPaymentMethodById.Active = true;
            salonPaymentMethodById = IoC.Resolve<ISalonManager>().UpdateSalonPaymentMethod(salonPaymentMethodById);
            string uRL = IFRMHelper.GetURL("reportinactivepaymentmethods.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("reportinactivepaymentmethods.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedSalonPaymentMethodId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("reportinactivepaymentmethods.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonPaymentMethodDB salonPaymentMethodById = IoC.Resolve<ISalonManager>().GetSalonPaymentMethodById(this.PostedSalonPaymentMethodId);
                if ((salonPaymentMethodById == null) || salonPaymentMethodById.Active)
                {
                    string url = IFRMHelper.GetURL("reportinactivepaymentmethods.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                this.BindPaymentDetails(salonPaymentMethodById);
            }
        }

        public Guid PostedSalonPaymentMethodId
        {
            get
            {
                string str = base.Request.QueryString["pmid"];
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

