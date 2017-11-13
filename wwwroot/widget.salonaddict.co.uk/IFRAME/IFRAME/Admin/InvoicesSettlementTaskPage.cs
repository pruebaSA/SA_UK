namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using SA.Payments.Realex.RealVault;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class InvoicesSettlementTaskPage : IFRMAdminPage
    {
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected Image imgLoader;
        protected LinkButton lbStartTask;
        protected Panel pnl;

        protected void lbStartTask_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            foreach (SalonInvoiceDB edb in IoC.Resolve<IBillingManager>().GetReportSalonInvoiceDue("R", "GBP"))
            {
                this.SettleInvoice(edb);
                if (DateTime.Now.AddSeconds(-20.0) > now)
                {
                    break;
                }
            }
            string uRL = IFRMHelper.GetURL("billing.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "START_TASK", ("window.setTimeout(function() {" + this.lbStartTask.ClientID + ".click();}, 500);").ToString(), true);
            }
        }

        private void SettleInvoice(SalonInvoiceDB value)
        {
            if (!value.Deleted)
            {
                SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
                if (value.TotalAmountDue > 0)
                {
                    if (value.TotalAmountDue > 0x186a0)
                    {
                        return;
                    }
                    List<SalonPaymentMethodDB> salonPaymentMethodsBySalonId = IoC.Resolve<ISalonManager>().GetSalonPaymentMethodsBySalonId(value.SalonId);
                    SalonPaymentMethodDB paymentMethod = salonPaymentMethodsBySalonId.SingleOrDefault<SalonPaymentMethodDB>(item => item.IsPrimary);
                    if ((paymentMethod == null) || !paymentMethod.Active)
                    {
                        paymentMethod = salonPaymentMethodsBySalonId.SingleOrDefault<SalonPaymentMethodDB>(item => !item.IsPrimary && item.Active);
                    }
                    if ((paymentMethod == null) || !paymentMethod.Active)
                    {
                        return;
                    }
                    value.CardAlias = paymentMethod.Alias;
                    value.CardName = paymentMethod.CardName;
                    value.CardNumber = paymentMethod.CardNumber;
                    value.CardType = paymentMethod.CardType;
                    value.MaskedCardNumber = paymentMethod.MaskedCardNumber;
                    string orderID = "INV" + Guid.NewGuid().ToString().ToUpperInvariant().Replace("-", string.Empty);
                    SettingsProviderConfig settings = new SettingsProviderConfig();
                    ReceiptInResponse response = new ReceiptInRequest(orderID, paymentMethod.RealexPayerRef, paymentMethod.RealexCardRef, value.CurrencyCode, value.TotalAmountDue.ToString(), true, settings).GetResponse<ReceiptInResponse>();
                    value.AuthorizationTransactionCode = orderID;
                    value.AuthorizationTransactionId = orderID;
                    value.CaptureTransactionId = orderID;
                    if (!response.HasErrors() && response.IsValid(settings))
                    {
                        value.AuthorizationTransactionResult = response.Result;
                        value.CaptureTransactionResult = response.Result;
                        value.TotalPaid = value.TotalAmountDue;
                        value.PaidOn = new DateTime?(DateTime.Now);
                    }
                    else
                    {
                        value.AuthorizationTransactionResult = response.Message;
                        value.CaptureTransactionResult = response.Message;
                        paymentMethod.Active = false;
                        paymentMethod = IoC.Resolve<ISalonManager>().UpdateSalonPaymentMethod(paymentMethod);
                    }
                }
                else
                {
                    value.TotalPaid = 0;
                    value.PaidOn = new DateTime?(DateTime.Now);
                }
                value.UpdatedOn = DateTime.Now;
                value.UpdatedBy = workingUser.Username;
                value = IoC.Resolve<IBillingManager>().UpdateSalonInvoice(value);
            }
        }
    }
}

