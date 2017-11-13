namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class InvoiceDetailsPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected Literal ltrBillingPeriod;
        protected Literal ltrInvoicedDate;
        protected Literal ltrInvoiceNumber;
        protected Literal ltrOverdue;
        protected Literal ltrPaymentDate;
        protected Literal ltrPlanFee;
        protected Literal ltrPlanType;
        protected Literal ltrSalonName;
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
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (this.ReferringPage == "d")
            {
                int postedPageIndex = this.PostedPageIndex;
                int postedPageSize = this.PostedPageSize;
                string postedInvoiceType = this.PostedInvoiceType;
                int postedOrderBy = this.PostedOrderBy;
                string str2 = $"{"type"}={postedInvoiceType}";
                string str3 = $"{"page_index"}={postedPageIndex}";
                string str4 = $"{"page_size"}={postedPageSize}";
                string str5 = $"{"sort"}={postedOrderBy}";
                string uRL = IFRMHelper.GetURL("invoicesissued.aspx", new string[] { str2, str3, str4, str5 });
                base.Response.Redirect(uRL, true);
            }
            else
            {
                string url = IFRMHelper.GetURL("invoicesearch.aspx", new string[0]);
                base.Response.Redirect(url, true);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("invoicesissued.aspx", new string[0]);
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
                if (salonInvoiceById.PaidOn.HasValue || salonInvoiceById.Deleted)
                {
                    base.Response.Redirect(uRL, true);
                }
                this.BindSalonInvoice(salonInvoiceById);
            }
        }

        public string PostedInvoiceType
        {
            get
            {
                string str2;
                string str = base.Request.QueryString["type"];
                str = str ?? string.Empty;
                str = str.ToLower();
                if (((str2 = str) == null) || ((str2 != "r") && (str2 != "u")))
                {
                    return string.Empty;
                }
                return str.ToUpper();
            }
        }

        public int PostedOrderBy
        {
            get
            {
                string str2;
                string s = base.Request.QueryString["sort"];
                if (((str2 = s) == null) || (((str2 != "10") && (str2 != "60")) && (str2 != "70")))
                {
                    return 10;
                }
                return int.Parse(s);
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

        public string ReferringPage
        {
            get
            {
                string str2;
                string str = base.Request.QueryString["ref"];
                str = str ?? string.Empty;
                str = str.ToLowerInvariant();
                if (((str2 = str) == null) || ((str2 != "d") && (str2 != "e")))
                {
                    return "d";
                }
                return str;
            }
        }
    }
}

