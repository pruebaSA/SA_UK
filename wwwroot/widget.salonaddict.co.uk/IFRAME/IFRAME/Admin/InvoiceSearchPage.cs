namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using IFRAME.Controllers.Controls;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    public class InvoiceSearchPage : IFRMAdminPage
    {
        protected Button btnSearch;
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected IFRMPager cntrlPager;
        protected GridView gv;
        protected Panel pnl;
        protected TextBox txtNumber;
        protected RequiredFieldValidator valNumber;
        protected ValidatorCalloutExtender valNumberEx;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
            this.gv.Columns[3].HeaderText = base.GetLocaleResourceString("gv.Columns[3].HeaderText");
            this.gv.Columns[4].HeaderText = base.GetLocaleResourceString("gv.Columns[4].HeaderText");
        }

        private void BindInvoices()
        {
            int postedPageIndex = this.PostedPageIndex;
            int postedPageSize = this.PostedPageSize;
            List<SalonInvoiceDB> list = IoC.Resolve<BillingManagerSQL>().GetSalonInvoices(null, null, "GBP", this.PostedInvoiceNumber, DateTime.Now.AddYears(-1), DateTime.Now);
            this.gv.DataSource = list;
            this.gv.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                this.BindInvoices();
            }
        }

        protected void cntrlPager_PageCreated(object sender, PageCreatedEventArgs e)
        {
            string str = $"{"page_index"}={e.NewPageIndex}";
            string str2 = $"{"page_size"}={this.cntrlPager.PageSize}";
            string uRL = IFRMHelper.GetURL("invoicesearch.aspx", new string[] { str, str2 });
            HtmlAnchor control = (HtmlAnchor) e.Control;
            control.HRef = uRL;
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.NewEditIndex].Value.ToString());
            string str = $"{"inv"}={guid}";
            string str2 = $"{"page_index"}={this.cntrlPager.PageIndex}";
            string str3 = $"{"page_size"}={this.cntrlPager.PageSize}";
            string uRL = IFRMHelper.GetURL("invoiceedit.aspx", new string[] { str, str2, str3 });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
        }

        public string PostedInvoiceNumber =>
            this.txtNumber.Text.Trim();

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
                    return 11;
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
                    return 15;
                }
                int result = 0;
                if (!int.TryParse(str, out result))
                {
                    return 15;
                }
                if (result < 1)
                {
                    return 15;
                }
                if (result > 50)
                {
                    return 50;
                }
                return result;
            }
        }
    }
}

