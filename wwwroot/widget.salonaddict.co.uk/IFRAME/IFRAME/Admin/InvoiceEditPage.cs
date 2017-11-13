namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class InvoiceEditPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnCarryForward;
        protected Button btnPaid;
        protected Button btnVoid;
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected Literal ltrBillingPeriod;
        protected Literal ltrInvoicedDate;
        protected Literal ltrInvoiceNumber;
        protected Literal ltrOverdue;
        protected Literal ltrPaymentDate;
        protected Literal ltrPlanFee;
        protected Literal ltrPlanType;
        protected Literal ltrSalonName;
        protected Literal ltrStatus;
        protected Literal ltrSubtotalExclTax;
        protected Literal ltrTotalAdjustment;
        protected Literal ltrTotalAmountDue;
        protected Literal ltrTotalInclTax;
        protected Literal ltrTotalPlan;
        protected Literal ltrTotalTax;
        protected Literal ltrTotalWidget;
        protected Literal ltrTotalWidgetCount;
        protected Panel pnl;

        private void BindSalonInvoice(SalonInvoiceDB value)
        {
            this.ltrSalonName.Text = value.BillingCompany;
            this.ltrBillingPeriod.Text = IFRMHelper.FromUrlFriendlyDate(value.BillEndDate).ToString("MMM dd yyyy");
            this.ltrInvoicedDate.Text = value.CreatedOn.ToString("MMM dd yyyy");
            this.ltrInvoiceNumber.Text = value.InvoiceNumber;
            this.ltrPlanType.Text = value.PlanDescription;
            this.ltrPlanFee.Text = (((double) int.Parse(value.PlanFee.ToString())) / 100.0).ToString("C");
            this.ltrOverdue.Text = (((double) int.Parse(value.TotalOverdue.ToString())) / 100.0).ToString("C");
            this.ltrTotalPlan.Text = (((double) int.Parse(value.TotalPlan.ToString())) / 100.0).ToString("C");
            this.ltrTotalWidget.Text = (((double) int.Parse(value.TotalWidget.ToString())) / 100.0).ToString("C");
            this.ltrTotalWidgetCount.Text = value.TotalWidgetCount.ToString();
            this.ltrSubtotalExclTax.Text = (((double) int.Parse(value.SubtotalExclTax.ToString())) / 100.0).ToString("C");
            this.ltrTotalAdjustment.Text = (((double) int.Parse(value.TotalAdjustment.ToString())) / 100.0).ToString("C");
            if (value.TotalAdjustment <= 0)
            {
                this.ltrTotalAdjustment.Text = this.ltrTotalAdjustment.Text + " CR";
            }
            this.ltrTotalTax.Text = (((double) int.Parse(value.TotalTax.ToString())) / 100.0).ToString("C");
            this.ltrPaymentDate.Text = IFRMHelper.FromUrlFriendlyDate(value.PaymentDueDate).ToString("MMM dd yyyy");
            this.ltrTotalInclTax.Text = (((double) int.Parse(value.TotalInclTax.ToString())) / 100.0).ToString("C");
            this.ltrTotalAmountDue.Text = (((double) int.Parse(value.TotalAmountDue.ToString())) / 100.0).ToString("C");
            this.ltrStatus.Text = "ISSUED";
            if (value.PaidOn.HasValue)
            {
                this.ltrStatus.Text = "<b>PAID</b>";
            }
            if (value.Deleted)
            {
                this.ltrStatus.Text = "<b>VOID</b>";
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            int postedPageIndex = this.PostedPageIndex;
            int postedPageSize = this.PostedPageSize;
            string str = $"{"page_index"}={postedPageIndex}";
            string str2 = $"{"page_size"}={postedPageSize}";
            string uRL = IFRMHelper.GetURL("invoicesearch.aspx", new string[] { str, str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void btnCarryForward_Click(object sender, EventArgs e)
        {
            string str = $"{"inv"}={this.PostedSalonInvoiceId}";
            string uRL = IFRMHelper.GetURL("invoiceforward.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnPaid_Click(object sender, EventArgs e)
        {
            string str = $"{"inv"}={this.PostedSalonInvoiceId}";
            string uRL = IFRMHelper.GetURL("invoicepaid.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnVoid_Click(object sender, EventArgs e)
        {
            string str = $"{"inv"}={this.PostedSalonInvoiceId}";
            string uRL = IFRMHelper.GetURL("invoicevoid.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("invoicesearch.aspx", new string[0]);
            if (this.PostedSalonInvoiceId == Guid.Empty)
            {
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonInvoiceDB salonInvoiceById = IoC.Resolve<IBillingManager>().GetSalonInvoiceById(this.PostedSalonInvoiceId);
                if (salonInvoiceById == null)
                {
                    base.Response.Redirect(uRL, true);
                }
                if (salonInvoiceById.PaidOn.HasValue)
                {
                    this.btnPaid.Visible = false;
                    this.btnCarryForward.Visible = false;
                }
                if (salonInvoiceById.Deleted)
                {
                    this.btnVoid.Visible = false;
                }
                this.BindSalonInvoice(salonInvoiceById);
            }
        }

        public int PostedPageIndex
        {
            get
            {
                string str = base.Request.QueryString["page_index"];
                if (string.IsNullOrEmpty(str))
                {
                    return 1;
                }
                int result = 0;
                if (!int.TryParse(str, out result))
                {
                    return 1;
                }
                if (result < 1)
                {
                    return 1;
                }
                return result;
            }
        }

        public int PostedPageSize
        {
            get
            {
                string str = base.Request.QueryString["page_size"];
                if (string.IsNullOrEmpty(str))
                {
                    return 10;
                }
                int result = 0;
                if (!int.TryParse(str, out result))
                {
                    return 10;
                }
                if (result < 1)
                {
                    return 10;
                }
                if (result > 50)
                {
                    return 50;
                }
                return result;
            }
        }

        public Guid PostedSalonInvoiceId
        {
            get
            {
                string str = base.Request.QueryString["inv"];
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

