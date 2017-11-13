namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using IFRAME.Controllers.Controls;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    public class InvoiceWTsPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected IFRMPager cntrlPager;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
            this.gv.Columns[3].HeaderText = base.GetLocaleResourceString("gv.Columns[3].HeaderText");
            this.gv.Columns[4].HeaderText = base.GetLocaleResourceString("gv.Columns[4].HeaderText");
            this.gv.Columns[5].HeaderText = base.GetLocaleResourceString("gv.Columns[5].HeaderText");
        }

        private void BindSalonInvoiceWTs(SalonInvoiceDB value)
        {
            int postedPageIndex = this.PostedPageIndex;
            int postedPageSize = this.PostedPageSize;
            int totalRecords = 0;
            List<SalonInvoiceWTDB> list = IoC.Resolve<IBillingManager>().GetSalonInvoiceWTByInvoiceId(value.InvoiceId, postedPageIndex, postedPageSize, out totalRecords);
            this.gv.DataSource = list;
            this.gv.DataBind();
            this.cntrlPager.TotalRowCount = totalRecords;
            this.cntrlPager.PageIndex = postedPageIndex;
            this.cntrlPager.PageSize = postedPageSize;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"inv"}={this.PostedSalonInvoiceId}";
            string uRL = IFRMHelper.GetURL("invoicedetails.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void cntrlPager_PageCreated(object sender, PageCreatedEventArgs e)
        {
            string str = $"{"inv"}={this.PostedSalonInvoiceId}";
            string str2 = $"{"page_index"}={e.NewPageIndex}";
            string str3 = $"{"page_size"}={this.cntrlPager.PageSize}";
            string uRL = IFRMHelper.GetURL("invoicewts.aspx", new string[] { str, str2, str3 });
            HtmlAnchor control = (HtmlAnchor) e.Control;
            control.HRef = uRL;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            string uRL = IFRMHelper.GetURL("invoicesissued.aspx", new string[0]);
            if (this.PostedSalonInvoiceId == Guid.Empty)
            {
                base.Response.Redirect(uRL, true);
            }
            SalonInvoiceDB salonInvoiceById = IoC.Resolve<IBillingManager>().GetSalonInvoiceById(this.PostedSalonInvoiceId);
            if (salonInvoiceById == null)
            {
                base.Response.Redirect(uRL, true);
            }
            if (salonInvoiceById.PaidOn.HasValue || salonInvoiceById.Deleted)
            {
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                this.BindSalonInvoiceWTs(salonInvoiceById);
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

