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

    public class ReportPlanExpiryPage : IFRMAdminPage
    {
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected IFRMPager cntrlPager;
        protected DropDownList ddlExpiry;
        protected DropDownList ddlPlanType;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
        }

        private void BindReport()
        {
            string postedPlanType = this.PostedPlanType;
            int postedExpiryDays = this.PostedExpiryDays;
            int postedPageIndex = this.PostedPageIndex;
            int postedPageSize = this.PostedPageSize;
            int totalRecords = 0;
            List<WSPExpiringDB> list = IoC.Resolve<IReportManager>().GetWSPExpiryReport(postedPlanType, "GBP", postedExpiryDays, postedPageIndex, postedPageSize, out totalRecords);
            this.gv.DataSource = list;
            this.gv.DataBind();
            this.cntrlPager.TotalRowCount = totalRecords;
            this.cntrlPager.PageIndex = postedPageIndex;
            this.cntrlPager.PageSize = postedPageSize;
            this.ddlPlanType.SelectedValue = postedPlanType;
            this.ddlExpiry.SelectedValue = postedExpiryDays.ToString();
        }

        protected void cntrlPager_PageCreated(object sender, PageCreatedEventArgs e)
        {
            string str = $"{"pln"}={this.ddlPlanType.SelectedValue}";
            string str2 = $"{"exp"}={this.ddlExpiry.SelectedValue}";
            string str3 = $"{"page_index"}={1}";
            string str4 = $"{"page_size"}={this.cntrlPager.PageSize}";
            string uRL = IFRMHelper.GetURL("reportplanexpiry.aspx", new string[] { str3, str4, str, str2 });
            HtmlAnchor control = (HtmlAnchor) e.Control;
            control.HRef = uRL;
        }

        protected void ddlExpiry_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = $"{"pln"}={this.ddlPlanType.SelectedValue}";
            string str2 = $"{"exp"}={this.ddlExpiry.SelectedValue}";
            string str3 = $"{"page_index"}={1}";
            string str4 = $"{"page_size"}={this.cntrlPager.PageSize}";
            string uRL = IFRMHelper.GetURL("reportplanexpiry.aspx", new string[] { str3, str4, str, str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void ddlPlanType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = $"{"pln"}={this.ddlPlanType.SelectedValue}";
            string str2 = $"{"exp"}={this.ddlExpiry.SelectedValue}";
            string str3 = $"{"page_index"}={1}";
            string str4 = $"{"page_size"}={this.cntrlPager.PageSize}";
            string uRL = IFRMHelper.GetURL("reportplanexpiry.aspx", new string[] { str3, str4, str, str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindReport();
            }
        }

        public int PostedExpiryDays
        {
            get
            {
                string s = base.Request.QueryString["exp"];
                int result = 0;
                int.TryParse(s, out result);
                if (result < 1)
                {
                    result = 1;
                }
                if (result > 0x1c)
                {
                    result = 0x1c;
                }
                return result;
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

        public string PostedPlanType
        {
            get
            {
                string str2;
                string str = base.Request.QueryString["pln"];
                if (((str2 = str) == null) || (((str2 != "10") && (str2 != "30")) && (str2 != "100")))
                {
                    return "10";
                }
                return str;
            }
        }
    }
}

