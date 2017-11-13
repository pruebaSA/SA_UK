namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class DefaultPage : IFRMSecurePage
    {
        protected Overview cntlOverview;
        protected Literal ltrHeader;
        protected Panel pnl;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            this.ltrHeader.Text = string.Format(base.GetLocaleResourceString("ltrHeader.Text"), salon.Name);
        }
    }
}

