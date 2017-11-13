namespace IFRAME.SecureArea
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class PaymentMethod_EditPage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected Label lblError;
        protected Literal ltrCardName;
        protected Literal ltrCardNumber;
        protected Literal ltrCardType;
        protected Literal ltrExpiry;
        protected Panel pnl;
        protected TextBox txtAlias;
        protected RequiredFieldValidator valAlias;
        protected ValidatorCalloutExtender valAliasEx;

        private void BindPaymentMethodDetails(SalonPaymentMethodDB value)
        {
            this.txtAlias.Text = value.Alias;
            this.ltrCardName.Text = IoC.Resolve<ISecurityManager>().DecryptUserPassword(value.CardName, IFRAME.Controllers.Settings.Security_Key_3DES);
            this.ltrCardNumber.Text = IoC.Resolve<ISecurityManager>().DecryptUserPassword(value.MaskedCardNumber, IFRAME.Controllers.Settings.Security_Key_3DES);
            this.ltrCardType.Text = this.ConvertToFriendlyCardName(IoC.Resolve<ISecurityManager>().DecryptUserPassword(value.CardType, IFRAME.Controllers.Settings.Security_Key_3DES));
            this.ltrExpiry.Text = $"{IoC.Resolve<ISecurityManager>().DecryptUserPassword(value.CardExpirationMonth, IFRAME.Controllers.Settings.Security_Key_3DES)}/{IoC.Resolve<ISecurityManager>().DecryptUserPassword(value.CardExpirationYear, IFRAME.Controllers.Settings.Security_Key_3DES)}";
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("paymentmethods.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                string str = this.txtAlias.Text.Trim();
                SalonDB salon = IFRMContext.Current.Salon;
                SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
                SalonPaymentMethodDB salonPaymentMethodById = IoC.Resolve<ISalonManager>().GetSalonPaymentMethodById(this.PostedPaymentMethodID);
                if (salonPaymentMethodById.SalonId == salon.SalonId)
                {
                    salonPaymentMethodById.Alias = str;
                    salonPaymentMethodById.UpdatedOn = DateTime.Now;
                    salonPaymentMethodById.UpdatedBy = workingUser.Username;
                    salonPaymentMethodById = IoC.Resolve<ISalonManager>().UpdateSalonPaymentMethod(salonPaymentMethodById);
                }
            }
            string uRL = IFRMHelper.GetURL("paymentmethods.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        private string ConvertToFriendlyCardName(string value)
        {
            switch (value)
            {
                case "VISA":
                    return "Visa";

                case "MASTERCARD":
                    return "MasterCard";

                case "LASER":
                    return "Laser";

                case "MAESTRO":
                    return "Maestro";
            }
            return value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedPaymentMethodID == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("paymentmethods.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonPaymentMethodDB salonPaymentMethodById = IoC.Resolve<ISalonManager>().GetSalonPaymentMethodById(this.PostedPaymentMethodID);
                if (salonPaymentMethodById == null)
                {
                    string url = IFRMHelper.GetURL("paymentmethods.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                SalonDB salon = IFRMContext.Current.Salon;
                if (salonPaymentMethodById.SalonId != salon.SalonId)
                {
                    string str3 = IFRMHelper.GetURL("paymentmethods.aspx", new string[0]);
                    base.Response.Redirect(str3, true);
                }
                this.BindPaymentMethodDetails(salonPaymentMethodById);
            }
        }

        public Guid PostedPaymentMethodID
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

