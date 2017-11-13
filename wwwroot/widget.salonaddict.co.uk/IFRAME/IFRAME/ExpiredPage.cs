namespace IFRAME
{
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class ExpiredPage : IFRMPage
    {
        protected Panel pnl;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                WSPDB wSPCurrent = IoC.Resolve<IBillingManager>().GetWSPCurrent(IFRMContext.Current.Salon.SalonId);
                if (((wSPCurrent != null) && wSPCurrent.Active) && (DateTime.Today < IFRMHelper.FromUrlFriendlyDate(wSPCurrent.PlanEndDate)))
                {
                    base.Response.Redirect(IFRMHelper.GetURL("~/", new string[0]), true);
                }
            }
        }
    }
}

