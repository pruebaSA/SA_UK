namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class AccountPage : IFRMSecurePage
    {
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected Literal ltrAccount;
        protected Literal ltrPlanEndDate;
        protected Literal ltrPlanType;
        protected Literal ltrStatus;
        protected MultiView mvStatus;
        protected Panel pnl;
        protected System.Web.UI.WebControls.View v0;
        protected System.Web.UI.WebControls.View v1;

        private void BindWSP(WSPDB value)
        {
            this.ltrPlanEndDate.Text = IFRMHelper.FromUrlFriendlyDate(value.PlanEndDate).ToString("dd MMM yyyy");
            this.ltrPlanType.Text = value.Description;
            if ((value.CancelDate != string.Empty) && (DateTime.Today >= IFRMHelper.FromUrlFriendlyDate(value.CancelDate)))
            {
                this.mvStatus.ActiveViewIndex = 1;
                this.ltrStatus.Text = "Cancelled";
                this.ltrPlanEndDate.Text = IFRMHelper.FromUrlFriendlyDate(value.CancelDate).ToString("dd MMM yyyy");
            }
            else if (value.Active)
            {
                this.mvStatus.ActiveViewIndex = 1;
                this.ltrStatus.Text = "Active";
            }
            else
            {
                this.mvStatus.ActiveViewIndex = 0;
                this.ltrStatus.Text = "Inactive";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                this.ltrAccount.Text = salon.Name;
                WSPDB wSPCurrent = IoC.Resolve<IBillingManager>().GetWSPCurrent(salon.SalonId);
                this.BindWSP(wSPCurrent);
            }
        }
    }
}

