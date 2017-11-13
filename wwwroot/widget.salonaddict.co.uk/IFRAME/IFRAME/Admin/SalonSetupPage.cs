namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class SalonSetupPage : IFRMAdminPage
    {
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected ImageButton ibEditEmployees;
        protected ImageButton ibEditSchedule;
        protected Literal ltrEmployeeCount;
        protected Literal ltrHeader;
        protected Literal ltrScheduleCount;
        protected Panel pnl;

        private void BindSalonDetails(SalonDB value)
        {
            this.ltrHeader.Text = string.Format(base.GetLocaleResourceString("ltrHeader.Text"), value.Name);
        }

        protected void ibEdit_Command(object sender, CommandEventArgs e)
        {
            string url = string.Empty;
            string str2 = e.CommandName.ToUpperInvariant();
            string str3 = $"{"sid"}={this.PostedSalonId}";
            if (str2 == "EMPLOYEES")
            {
                url = "salonemployees-setup.aspx";
            }
            else
            {
                url = "salonschedule-setup.aspx";
            }
            string uRL = IFRMHelper.GetURL(url, new string[] { str3 });
            base.Response.Redirect(uRL, true);
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
                this.BindSalonDetails(salonById);
                List<EmployeeDB> employeesBySalonId = IoC.Resolve<IEmployeeManager>().GetEmployeesBySalonId(salonById.SalonId);
                this.ltrEmployeeCount.Text = employeesBySalonId.Count.ToString();
                int num = (from item in (from item in IoC.Resolve<ISchedulingManager>().GetSchedulingBySalonId(salonById.SalonId)
                    where item.ScheduleTypeEnum == ScheduleTypeEnum.TimeSlot
                    select item).ToList<ScheduleDB>() select item.WeekDay).Distinct<int?>().Count<int?>();
                this.ltrScheduleCount.Text = string.Format(base.GetLocaleResourceString("ltrScheduleCount.Text"), num);
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

