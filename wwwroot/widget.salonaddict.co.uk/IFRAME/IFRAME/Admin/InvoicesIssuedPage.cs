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

    public class InvoicesIssuedPage : IFRMAdminPage
    {
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected IFRMPager cntrlPager;
        protected DropDownList ddlInvoiceType;
        protected DropDownList ddlSortBy;
        protected GridView gv;
        protected Panel pnl;

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
            string postedInvoiceType = this.PostedInvoiceType;
            this.ddlInvoiceType.SelectedValue = postedInvoiceType;
            int postedOrderBy = this.PostedOrderBy;
            int postedPageIndex = this.PostedPageIndex;
            int postedPageSize = this.PostedPageSize;
            int totalRecords = 0;
            List<SalonInvoiceDB> list = IoC.Resolve<IBillingManager>().GetSalonInvoicesIssued((postedInvoiceType == string.Empty) ? null : postedInvoiceType, "GBP", postedOrderBy, postedPageIndex, postedPageSize, out totalRecords);
            this.gv.DataSource = list;
            this.gv.DataBind();
            this.cntrlPager.TotalRowCount = totalRecords;
            this.cntrlPager.PageIndex = postedPageIndex;
            this.cntrlPager.PageSize = postedPageSize;
            this.ddlSortBy.SelectedValue = this.PostedOrderBy.ToString();
        }

        protected void cntrlPager_PageCreated(object sender, PageCreatedEventArgs e)
        {
            string str = $"{"type"}={this.ddlInvoiceType.SelectedValue.ToLower()}";
            string str2 = $"{"page_index"}={e.NewPageIndex}";
            string str3 = $"{"page_size"}={this.cntrlPager.PageSize}";
            string str4 = $"{"sort"}={this.ddlSortBy.SelectedValue}";
            string uRL = IFRMHelper.GetURL("invoicesissued.aspx", new string[] { str, str2, str3, str4 });
            HtmlAnchor control = (HtmlAnchor) e.Control;
            control.HRef = uRL;
        }

        protected void ddlInvoiceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = $"{"type"}={this.ddlInvoiceType.SelectedValue.ToLower()}";
            string str2 = $"{"page_index"}={1}";
            string str3 = $"{"page_size"}={this.cntrlPager.PageSize}";
            string str4 = $"{"sort"}={this.ddlSortBy.SelectedValue}";
            string uRL = IFRMHelper.GetURL("invoicesissued.aspx", new string[] { str, str2, str3, str4 });
            base.Response.Redirect(uRL, true);
        }

        protected void ddlSortBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = $"{"type"}={this.ddlInvoiceType.SelectedValue.ToLower()}";
            string str2 = $"{"page_index"}={1}";
            string str3 = $"{"page_size"}={this.cntrlPager.PageSize}";
            string str4 = $"{"sort"}={this.ddlSortBy.SelectedValue}";
            string uRL = IFRMHelper.GetURL("invoicesissued.aspx", new string[] { str, str2, str3, str4 });
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.NewEditIndex].Value.ToString());
            string str = $"{"inv"}={guid}";
            string str2 = $"{"type"}={this.ddlInvoiceType.SelectedValue.ToLower()}";
            string str3 = $"{"page_index"}={this.cntrlPager.PageIndex}";
            string str4 = $"{"page_size"}={this.cntrlPager.PageSize}";
            string str5 = $"{"sort"}={this.ddlSortBy.SelectedValue}";
            string str6 = $"{"ref"}={"d"}";
            string uRL = IFRMHelper.GetURL("invoicedetails.aspx", new string[] { str, str2, str3, str4, str5, str6 });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindInvoices();
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

