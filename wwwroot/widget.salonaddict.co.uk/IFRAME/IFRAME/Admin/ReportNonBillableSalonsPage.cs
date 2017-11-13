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

    public class ReportNonBillableSalonsPage : IFRMAdminPage
    {
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected IFRMPager cntrlPager;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
            this.gv.Columns[3].HeaderText = base.GetLocaleResourceString("gv.Columns[3].HeaderText");
        }

        private void BindReport()
        {
            int postedPageIndex = this.PostedPageIndex;
            int postedPageSize = this.PostedPageSize;
            int totalRecords = 0;
            List<SalonDB> list = IoC.Resolve<IBillingManager>().GetReportNonbillableSalons("GBP", postedPageIndex, postedPageSize, out totalRecords);
            this.gv.DataSource = list;
            this.gv.DataBind();
            this.cntrlPager.TotalRowCount = totalRecords;
            this.cntrlPager.PageIndex = postedPageIndex;
            this.cntrlPager.PageSize = postedPageSize;
        }

        protected void cntrlPager_PageCreated(object sender, PageCreatedEventArgs e)
        {
            string str = $"{"page_index"}={e.NewPageIndex}";
            string str2 = $"{"page_size"}={this.cntrlPager.PageSize}";
            string uRL = IFRMHelper.GetURL("reportnonbillablesalons.aspx", new string[] { str, str2 });
            HtmlAnchor control = (HtmlAnchor) e.Control;
            control.HRef = uRL;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindReport();
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

