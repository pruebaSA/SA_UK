namespace IFRAME.Admin.SalonManagement
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using IFRAME.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class Holiday_CreatePage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected SalonMenu cntlMenu;
        protected Panel pnl;
        protected DateTextBox txtDate;
        protected TextBox txtDescription;
        protected RequiredFieldValidator valDate;
        protected ValidatorCalloutExtender valDateEx;
        protected RequiredFieldValidator valDescription;
        protected ValidatorCalloutExtender valDescriptionEx;

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("holidays.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid && (this.txtDate.Date != DateTime.MinValue))
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                DateTime date = this.txtDate.Date;
                if (!IoC.Resolve<ISalonManager>().GetClosingDaysBySalonId(salonById.SalonId).Exists(item => item.Date == date))
                {
                    ClosingDayDB closingDay = new ClosingDayDB {
                        Active = true,
                        Category = "Holiday",
                        CycleLength = 1,
                        CyclePeriodType = 40,
                        SalonId = salonById.SalonId,
                        StartDateUtc = date,
                        Description = this.txtDescription.Text.Trim()
                    };
                    closingDay = IoC.Resolve<ISalonManager>().InsertClosingDay(closingDay);
                }
            }
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("holidays.aspx", new string[] { str });
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
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack && (IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId) == null))
            {
                string url = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                base.Response.Redirect(url, true);
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

