namespace IFRAME.SecureArea
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using IFRAME.Modules;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class Holiday_CreatePage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected Panel pnl;
        protected DateTextBox txtDate;
        protected TextBox txtDescription;
        protected RequiredFieldValidator valDate;
        protected ValidatorCalloutExtender valDateEx;
        protected RequiredFieldValidator valDescription;
        protected ValidatorCalloutExtender valDescriptionEx;

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("holidays.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid && (this.txtDate.Date != DateTime.MinValue))
            {
                SalonDB salon = IFRMContext.Current.Salon;
                DateTime date = this.txtDate.Date;
                if (!IoC.Resolve<ISalonManager>().GetClosingDaysBySalonId(salon.SalonId).Exists(item => item.Date == date))
                {
                    ClosingDayDB closingDay = new ClosingDayDB {
                        Active = true,
                        Category = "Holiday",
                        CycleLength = 1,
                        CyclePeriodType = 40,
                        SalonId = salon.SalonId,
                        StartDateUtc = date,
                        Description = this.txtDescription.Text.Trim()
                    };
                    closingDay = IoC.Resolve<ISalonManager>().InsertClosingDay(closingDay);
                }
            }
            string uRL = IFRMHelper.GetURL("holidays.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!this.Page.IsPostBack)
            {
                this.txtDate.Text = string.Empty;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}

