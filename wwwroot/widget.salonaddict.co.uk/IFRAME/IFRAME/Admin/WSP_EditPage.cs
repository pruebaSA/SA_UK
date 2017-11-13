namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class WSP_EditPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected SalonMenu cntrlMenu;
        protected Literal ltrAccount;
        protected Literal ltrPeriod;
        protected Literal ltrPlanPrice;
        protected Literal ltrPlanType;
        protected Literal ltrStatus;
        protected Literal ltrTransFee;
        protected Panel pnl;
        protected TabContainer pnlTabs;
        protected TabPanel tab1;

        private void BindWSP(SalonDB salon, WSPDB plan)
        {
            if (salon == null)
            {
                throw new ArgumentNullException("value");
            }
            if (plan == null)
            {
                throw new ArgumentNullException("plan");
            }
            this.ltrAccount.Text = salon.Name;
            this.ltrPlanType.Text = plan.Description;
            this.ltrPeriod.Text = IFRMHelper.FromUrlFriendlyDate(plan.PlanEndDate).ToString("dd MMM yyyy");
            this.ltrPlanPrice.Text = (Convert.ToDouble(plan.PlanPrice) / 100.0).ToString("C");
            this.ltrTransFee.Text = (Convert.ToDouble(plan.ExcessFeeWT) / 100.0).ToString("C");
            if ((plan.CancelDate != string.Empty) && (DateTime.Today >= IFRMHelper.FromUrlFriendlyDate(plan.CancelDate)))
            {
                this.ltrStatus.Text = $"Cancelled ({IFRMHelper.FromUrlFriendlyDate(plan.CancelDate).ToString("dd MMM yyyy")})";
            }
            else if (plan.Active)
            {
                this.ltrStatus.Text = "Active";
            }
            else
            {
                this.ltrStatus.Text = "Inactive";
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("wsp-history.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("salons.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (this.PostedPlanId == Guid.Empty)
            {
                string str2 = $"{"sid"}={this.PostedSalonId}";
                string url = IFRMHelper.GetURL("wsp.aspx", new string[] { str2 });
                base.Response.Redirect(url, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string str4 = IFRMHelper.GetURL("salons.aspx", new string[0]);
                    base.Response.Redirect(str4, true);
                }
                WSPDB wSPById = IoC.Resolve<IBillingManager>().GetWSPById(this.PostedPlanId);
                if (wSPById == null)
                {
                    string str5 = $"{"sid"}={this.PostedSalonId}";
                    string str6 = IFRMHelper.GetURL("wsp.aspx", new string[] { str5 });
                    base.Response.Redirect(str6, true);
                }
                this.BindWSP(salonById, wSPById);
            }
        }

        public Guid PostedPlanId
        {
            get
            {
                string str = base.Request.QueryString["wspid"];
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
    }
}

