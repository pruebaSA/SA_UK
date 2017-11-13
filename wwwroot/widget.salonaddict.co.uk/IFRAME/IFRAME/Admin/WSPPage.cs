namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class WSPPage : IFRMAdminPage
    {
        protected SalonMenu cntrlMenu;
        protected Literal ltrFeeWT;
        protected Literal ltrPlan;
        protected Literal ltrPlanPeriod;
        protected Literal ltrPrice;
        protected Literal ltrSalon;
        protected Literal ltrStatus;
        protected Panel pnl;
        protected Panel pnlPlan;

        private void BindCurrentPlan(WSPDB plan)
        {
            this.ltrPlan.Text = plan.Description;
            this.ltrPrice.Text = (((double) plan.PlanPrice) / 100.0).ToString("C");
            this.ltrPlanPeriod.Text = $"{IFRMHelper.FromUrlFriendlyDate(plan.PlanStartDate).ToString("dd MMM yyyy")} - {IFRMHelper.FromUrlFriendlyDate(plan.PlanEndDate).ToString("dd MMM yyyy")}";
            this.ltrFeeWT.Text = (((double) plan.ExcessFeeWT) / 100.0).ToString("C");
            if ((plan.CancelDate != string.Empty) && (DateTime.Today >= IFRMHelper.FromUrlFriendlyDate(plan.CancelDate)))
            {
                this.ltrStatus.Text = "CANCELLED";
                this.ltrPlanPeriod.Text = $"{IFRMHelper.FromUrlFriendlyDate(plan.PlanStartDate).ToString("dd MMM yyyy")} - {IFRMHelper.FromUrlFriendlyDate(plan.CancelDate).ToString("dd MMM yyyy")}";
            }
            else
            {
                this.ltrStatus.Text = plan.Active ? "ACTIVE" : "INACTIVE";
            }
        }

        private void BindSalonDetails(SalonDB value)
        {
            this.ltrSalon.Text = value.Name;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("salons.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string url = IFRMHelper.GetURL("salons.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                WSPDB wSPCurrent = IoC.Resolve<IBillingManager>().GetWSPCurrent(salonById.SalonId);
                this.TrialMode = wSPCurrent.PlanType == "10";
                this.EditMode = wSPCurrent.CancelDate == string.Empty;
                this.BindSalonDetails(salonById);
                this.BindCurrentPlan(wSPCurrent);
            }
        }

        public bool EditMode
        {
            get => 
                ((this.ViewState["EditMode"] == null) || Convert.ToBoolean(this.ViewState["EditMode"].ToString()));
            set
            {
                this.ViewState["EditMode"] = value.ToString();
            }
        }

        public Guid PostedSalonId
        {
            get
            {
                string str = base.Request.QueryString["sid"];
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

        public bool TrialMode
        {
            get => 
                ((this.ViewState["TrialMode"] == null) || Convert.ToBoolean(this.ViewState["TrialMode"].ToString()));
            set
            {
                this.ViewState["TrialMode"] = value.ToString();
            }
        }
    }
}

