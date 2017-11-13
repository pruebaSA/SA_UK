namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using SA.Payments.Realex.RealVault;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class WSP_PaymentPage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected Label lblError;
        protected Literal ltrAccount;
        protected Literal ltrAmountDue;
        protected Literal ltrPeriod;
        protected Literal ltrPlanPrice;
        protected Literal ltrPlanType;
        protected Literal ltrVat;
        protected Panel pnl;

        private void BindWSP(WSPDB value)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            this.ltrPeriod.Text = IFRMHelper.FromUrlFriendlyDate(value.PlanStartDate).ToString("dd MMM yyyy") + " - " + IFRMHelper.FromUrlFriendlyDate(value.PlanEndDate).ToString("dd MMM yyyy");
            this.ltrPlanPrice.Text = (value.PlanPrice / 100M).ToString("C");
            this.ltrPlanType.Text = value.Description;
            int planPrice = value.PlanPrice;
            int num2 = 0;
            if (planPrice > 0)
            {
                string s = string.IsNullOrEmpty(salon.VatNumber) ? "0.2000" : "0.0000";
                num2 = (int) Math.Round((double) (planPrice * double.Parse(s)), 0, MidpointRounding.AwayFromZero);
            }
            int num4 = planPrice + num2;
            this.ltrVat.Text = (num2 / 100M).ToString("C");
            this.ltrAmountDue.Text = (num4 / 100M).ToString("C");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("wsp-history.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
            List<SalonPaymentMethodDB> salonPaymentMethodsBySalonId = IoC.Resolve<ISalonManager>().GetSalonPaymentMethodsBySalonId(salon.SalonId);
            SalonPaymentMethodDB paymentMethod = salonPaymentMethodsBySalonId.SingleOrDefault<SalonPaymentMethodDB>(item => item.IsPrimary);
            if ((paymentMethod == null) || !paymentMethod.Active)
            {
                paymentMethod = salonPaymentMethodsBySalonId.SingleOrDefault<SalonPaymentMethodDB>(item => !item.IsPrimary && item.Active);
            }
            if ((paymentMethod == null) || !paymentMethod.Active)
            {
                this.lblError.Text = base.GetLocaleResourceString("Error.PaymentMethodActive");
            }
            else
            {
                WSPDB wSPById = IoC.Resolve<IBillingManager>().GetWSPById(this.PostedPlanId);
                SalonInvoiceDB edb = IoC.Resolve<IBillingManager>().GetSalonInvoicesByPlanId(wSPById.PlanId, "U", "GBP").First<SalonInvoiceDB>();
                if (!edb.PaidOn.HasValue && (edb.TotalAmountDue > 0))
                {
                    if (edb.TotalAmountDue > 0x186a0)
                    {
                        this.lblError.Text = base.GetLocaleResourceString("Error.SettlementLimit");
                        return;
                    }
                    string orderID = "WSP" + Guid.NewGuid().ToString().ToUpperInvariant().Replace("-", string.Empty);
                    SettingsProviderConfig settings = new SettingsProviderConfig();
                    ReceiptInResponse response = new ReceiptInRequest(orderID, paymentMethod.RealexPayerRef, paymentMethod.RealexCardRef, "GBP", edb.TotalAmountDue.ToString(), true, settings).GetResponse<ReceiptInResponse>();
                    if (response.HasErrors() || !response.IsValid(settings))
                    {
                        edb.AuthorizationTransactionResult = response.Message;
                        edb.CaptureTransactionResult = response.Message;
                        edb.UpdatedOn = DateTime.Now;
                        edb.UpdatedBy = workingUser.Username;
                        edb = IoC.Resolve<IBillingManager>().UpdateSalonInvoice(edb);
                        paymentMethod.Active = false;
                        paymentMethod = IoC.Resolve<ISalonManager>().UpdateSalonPaymentMethod(paymentMethod);
                        LogDB log = new LogDB {
                            CreatedOn = DateTime.Now,
                            Exception = "Authorization Failed.",
                            LogType = "REALEX",
                            Message = response.Message,
                            PageURL = base.Request.RawUrl,
                            UserHostAddress = base.Request.UserHostAddress,
                            UserId = new Guid?(workingUser.UserId)
                        };
                        log = IoC.Resolve<ILogManager>().InsertError(log);
                        this.lblError.Text = base.GetLocaleResourceString("Error.PaymentFail");
                        return;
                    }
                    edb.AuthorizationTransactionResult = response.Result;
                    edb.CaptureTransactionResult = response.Result;
                    edb.TotalPaid = edb.TotalAmountDue;
                    edb.PaidOn = new DateTime?(DateTime.Now);
                    edb.UpdatedOn = DateTime.Now;
                    edb.UpdatedBy = workingUser.Username;
                    edb = IoC.Resolve<IBillingManager>().UpdateSalonInvoice(edb);
                    wSPById.Active = true;
                    wSPById = IoC.Resolve<IBillingManager>().UpdateWSP(wSPById);
                }
                string uRL = IFRMHelper.GetURL("wsp-history.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedPlanId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("wsp-history.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                WSPDB wSPById = IoC.Resolve<IBillingManager>().GetWSPById(this.PostedPlanId);
                if (wSPById == null)
                {
                    string url = IFRMHelper.GetURL("wsp-history.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                if (wSPById.SalonId != salon.SalonId)
                {
                    string str3 = IFRMHelper.GetURL("wsp-history.aspx", new string[0]);
                    base.Response.Redirect(str3, true);
                }
                if (wSPById.Active)
                {
                    string str4 = IFRMHelper.GetURL("wsp-history.aspx", new string[0]);
                    base.Response.Redirect(str4, true);
                }
                List<SalonInvoiceDB> source = IoC.Resolve<IBillingManager>().GetSalonInvoicesByPlanId(wSPById.PlanId, "U", "GBP");
                if (source.Count != 1)
                {
                    string str5 = IFRMHelper.GetURL("wsp-history.aspx", new string[0]);
                    base.Response.Redirect(str5, true);
                }
                if (source.First<SalonInvoiceDB>().PaidOn.HasValue)
                {
                    string str6 = IFRMHelper.GetURL("wsp-history.aspx", new string[0]);
                    base.Response.Redirect(str6, true);
                }
                if (IoC.Resolve<ISalonManager>().GetSalonPaymentMethodsBySalonId(salon.SalonId).Count == 0)
                {
                    this.lblError.Text = base.GetLocaleResourceString("Error.PaymentMethod");
                }
                this.ltrAccount.Text = salon.Name;
                this.BindWSP(wSPById);
            }
        }

        public Guid PostedPlanId
        {
            get
            {
                string str = base.Request.QueryString["wspid"];
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

