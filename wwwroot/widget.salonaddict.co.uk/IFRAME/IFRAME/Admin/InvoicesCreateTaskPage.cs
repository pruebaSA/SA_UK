namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class InvoicesCreateTaskPage : IFRMAdminPage
    {
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected Image imgLoader;
        protected LinkButton lbStartTask;
        protected Panel pnl;

        private void GenerateInvoice(BillableSalonDB value)
        {
            SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
            SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(value.SalonId);
            if (IoC.Resolve<IBillingManager>().GetReportSalonInvoiceOverdueCountBySalonId(salonById.SalonId, "R") <= 0)
            {
                WSPDB wSPCurrent = IoC.Resolve<IBillingManager>().GetWSPCurrent(value.SalonId);
                if ((wSPCurrent.PlanType != "30") && (wSPCurrent.PlanType != "100"))
                {
                    throw new Exception("Invalid plan type.");
                }
                int num2 = 0;
                int num3 = (num2 + IoC.Resolve<IBillingManager>().GetSalonInvoiceCount()) + 1;
                SalonInvoiceDB edb = IoC.Resolve<IBillingManager>().GetSalonInvoiceCurrent(salonById.SalonId, null, false);
                if ((edb != null) && (edb.PlanId != wSPCurrent.PlanId))
                {
                    edb = null;
                }
                DateTime time = IFRMHelper.FromUrlFriendlyDate(wSPCurrent.PlanStartDate);
                DateTime time2 = IFRMHelper.FromUrlFriendlyDate(wSPCurrent.PlanEndDate);
                DateTime time3 = IFRMHelper.FromUrlFriendlyDate(wSPCurrent.BillStartDate);
                DateTime time4 = IFRMHelper.FromUrlFriendlyDate(wSPCurrent.BillEndDate);
                string billStartDate = wSPCurrent.BillStartDate;
                string billEndDate = wSPCurrent.BillEndDate;
                if ((wSPCurrent.CancelDate != string.Empty) && (IFRMHelper.FromUrlFriendlyDate(wSPCurrent.CancelDate) < time4))
                {
                    billEndDate = wSPCurrent.CancelDate;
                }
                List<BillableSalonWTDB> source = IoC.Resolve<IBillingManager>().GetReportBillableSalonWT(value.SalonId, billStartDate, billEndDate);
                int num4 = source.Sum<BillableSalonWTDB>(item => item.ItemPrice);
                int num5 = source.Count<BillableSalonWTDB>();
                DateTime time5 = time;
                DateTime time6 = time2;
                DateTime time7 = time3.AddMonths(1);
                DateTime time8 = time7.AddMonths(1).AddDays(-1.0);
                SalonInvoiceInsertXML invoice = new SalonInvoiceInsertXML {
                    BillingAddressLine1 = salonById.AddressLine1,
                    BillingAddressLine2 = salonById.AddressLine2,
                    BillingAddressLine3 = salonById.AddressLine3,
                    BillingAddressLine4 = salonById.AddressLine4,
                    BillingAddressLine5 = salonById.AddressLine5,
                    BillingCompany = salonById.Name,
                    BillingEmail = salonById.Email,
                    BillingFirstName = string.Empty,
                    BillingLastName = string.Empty,
                    BillingMobile = string.Empty,
                    BillingPhoneNumber = salonById.PhoneNumber,
                    BillingPhoneNumberType = string.Empty,
                    BillEndDate = wSPCurrent.BillEndDate,
                    BillStartDate = wSPCurrent.BillStartDate,
                    CreatedBy = workingUser.Username,
                    CreatedOn = DateTime.Now,
                    CurrencyCode = "GBP",
                    InvoiceNumber = $"SLN{time4.ToString("yyyyMMdd")}-{num3 + 1}",
                    InvoiceType = "R",
                    NextBillEndDate = IFRMHelper.ToUrlFriendlyDate(time8),
                    NextBillStartDate = IFRMHelper.ToUrlFriendlyDate(time7),
                    PaymentDueDate = IFRMHelper.ToUrlFriendlyDate(DateTime.Today.AddDays(15.0)),
                    PlanFee = wSPCurrent.PlanPrice,
                    PlanId = wSPCurrent.PlanId,
                    PlanStartDate = wSPCurrent.PlanStartDate,
                    PlanType = wSPCurrent.PlanType,
                    PlanDescription = wSPCurrent.Description,
                    PlanEndDate = wSPCurrent.PlanEndDate,
                    ParentId = (edb == null) ? null : new Guid?(edb.InvoiceId),
                    Published = true,
                    SalonId = wSPCurrent.SalonId,
                    Status = string.Empty,
                    TotalWidget = num4,
                    TotalWidgetCount = num5,
                    VATNumber = salonById.VatNumber,
                    TotalPlan = 0
                };
                if ((time7 >= IFRMHelper.FromUrlFriendlyDate(wSPCurrent.PlanEndDate)) && (wSPCurrent.CancelDate == string.Empty))
                {
                    invoice.TotalPlan = wSPCurrent.PlanPrice;
                    if (wSPCurrent.PlanType == "30")
                    {
                        time6 = time6.AddMonths(1);
                    }
                    else if (wSPCurrent.PlanType == "100")
                    {
                        time6 = time6.AddYears(1);
                    }
                }
                invoice.NextPlanEndDate = IFRMHelper.ToUrlFriendlyDate(time6);
                invoice.NextPlanStartDate = IFRMHelper.ToUrlFriendlyDate(time5);
                invoice.SubtotalExclTax = invoice.TotalWidget + invoice.TotalPlan;
                invoice.TaxRate = "0.2000";
                if (!string.IsNullOrEmpty(invoice.VATNumber))
                {
                    invoice.TaxRate = "0.0000";
                }
                invoice.TotalAdjustment = 0;
                invoice.TotalExclTax = invoice.SubtotalExclTax + invoice.TotalAdjustment;
                invoice.TotalTax = 0;
                if (invoice.TotalExclTax > 0)
                {
                    invoice.TotalTax = (int) Math.Round((double) (invoice.TotalExclTax * double.Parse(invoice.TaxRate)), 0, MidpointRounding.AwayFromZero);
                }
                invoice.TotalInclTax = invoice.TotalExclTax + invoice.TotalTax;
                invoice.TotalOverdue = 0;
                if (((edb != null) && !edb.Deleted) && edb.PaidOn.HasValue)
                {
                    invoice.TotalOverdue = edb.TotalAmountDue - edb.TotalPaid;
                }
                invoice.TotalAmountDue = invoice.TotalInclTax + invoice.TotalOverdue;
                invoice.Transactions = source;
                IoC.Resolve<IBillingManager>().GenerateSalonInvoice(invoice);
            }
        }

        protected void lbStartTask_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            foreach (BillableSalonDB ndb in IoC.Resolve<IBillingManager>().GetReportBillableSalons("GBP"))
            {
                this.GenerateInvoice(ndb);
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
    }
}

