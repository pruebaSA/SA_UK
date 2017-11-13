namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class ReportPlanTotalsPage : IFRMAdminPage
    {
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected GridView gv;
        protected Literal ltrSalonCount;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
        }

        private void BindReport()
        {
            List<PlanTotalsDB> planTotalsReport = IoC.Resolve<IReportManager>().GetPlanTotalsReport("GBP");
            List<object> list2 = new List<object>();
            int total = 0;
            int num2 = 0;
            int num3 = 0;
            foreach (PlanTotalsDB sdb in planTotalsReport)
            {
                string planType = sdb.PlanType;
                if (planType != null)
                {
                    if (planType != "10")
                    {
                        if (planType == "30")
                        {
                            goto Label_0072;
                        }
                        if (planType == "100")
                        {
                            goto Label_007C;
                        }
                    }
                    else
                    {
                        total = sdb.Total;
                    }
                }
                continue;
            Label_0072:
                num2 = sdb.Total;
                continue;
            Label_007C:
                num3 = sdb.Total;
            }
            list2.Add(new { 
                OverallTrial = total,
                OverallMonthly = num2,
                OverallAnnual = num3
            });
            this.gv.DataSource = list2;
            this.gv.DataBind();
            this.ltrSalonCount.Text = IoC.Resolve<IReportManager>().GetSalonCount("GBP").ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindReport();
            }
        }
    }
}

