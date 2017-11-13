namespace IFRAME.Modules
{
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class Footer : IFRMUserControl
    {
        protected Literal ltrPartnerStatement;
        protected LoginView lv;

        protected void Page_Load(object sender, EventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            if (salon == null)
            {
                this.lv.Visible = false;
            }
            if (salon != null)
            {
                this.ltrPartnerStatement.Text = string.Format(base.GetLocaleResourceString("ltrPartnerStatement.Text"), salon.Name);
            }
        }
    }
}

