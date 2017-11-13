namespace IFRAME.Admin.SalonManagement
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class Employee_EditPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected CheckBox cbFriday;
        protected CheckBox cbMonday;
        protected CheckBox cbSaturday;
        protected CheckBox cbSunday;
        protected CheckBox cbThursday;
        protected CheckBox cbTuesday;
        protected CheckBox cbWednesday;
        protected SalonMenu cntlMenu;
        protected GridView gv;
        protected Panel pnl;
        protected Panel pnl2;
        protected Panel pnlFriday;
        protected TabPanel pnlInfo;
        protected Panel pnlMonday;
        protected Panel pnlSaturday;
        protected TabPanel pnlServices;
        protected Panel pnlSunday;
        protected TabContainer pnlTabs;
        protected Panel pnlThursday;
        protected Panel pnlTuesday;
        protected Panel pnlWednesday;
        protected TextBox txtName;
        protected RequiredFieldValidator valName;
        protected ValidatorCalloutExtender valNameEx;

        private void BindEmployeeAvailability(SalonDB salon, EmployeeDB employee)
        {
            OpeningHoursDB openingHoursBySalonId = IoC.Resolve<ISalonManager>().GetOpeningHoursBySalonId(salon.SalonId);
            if (openingHoursBySalonId != null)
            {
                if (IFRMHelper.IsSalonClosed(DayOfWeek.Monday, openingHoursBySalonId))
                {
                    this.pnlMonday.Visible = false;
                }
                if (IFRMHelper.IsSalonClosed(DayOfWeek.Tuesday, openingHoursBySalonId))
                {
                    this.pnlTuesday.Visible = false;
                }
                if (IFRMHelper.IsSalonClosed(DayOfWeek.Wednesday, openingHoursBySalonId))
                {
                    this.pnlWednesday.Visible = false;
                }
                if (IFRMHelper.IsSalonClosed(DayOfWeek.Thursday, openingHoursBySalonId))
                {
                    this.pnlThursday.Visible = false;
                }
                if (IFRMHelper.IsSalonClosed(DayOfWeek.Friday, openingHoursBySalonId))
                {
                    this.pnlFriday.Visible = false;
                }
                if (IFRMHelper.IsSalonClosed(DayOfWeek.Saturday, openingHoursBySalonId))
                {
                    this.pnlSaturday.Visible = false;
                }
                if (IFRMHelper.IsSalonClosed(DayOfWeek.Sunday, openingHoursBySalonId))
                {
                    this.pnlSunday.Visible = false;
                }
                foreach (TimeBlockDB kdb in (from item in IoC.Resolve<ISchedulingManager>().GetTimeBlocksBySalonId(salon.SalonId)
                    where (item.EmployeeId == employee.EmployeeId) && item.WeekDay.HasValue
                    select item).ToList<TimeBlockDB>())
                {
                    switch ((kdb.WeekDay.Value - 1))
                    {
                        case DayOfWeek.Sunday:
                            this.cbSunday.Checked = false;
                            break;

                        case DayOfWeek.Monday:
                            this.cbMonday.Checked = false;
                            break;

                        case DayOfWeek.Tuesday:
                            this.cbTuesday.Checked = false;
                            break;

                        case DayOfWeek.Wednesday:
                            this.cbWednesday.Checked = false;
                            break;

                        case DayOfWeek.Thursday:
                            this.cbThursday.Checked = false;
                            break;

                        case DayOfWeek.Friday:
                            this.cbFriday.Checked = false;
                            break;

                        case DayOfWeek.Saturday:
                            this.cbSaturday.Checked = false;
                            break;
                    }
                }
            }
        }

        private void BindEmployeeDetails(EmployeeDB employee)
        {
            this.txtName.Text = employee.DisplayText;
        }

        private void BindEmployeeServiceMappings(EmployeeDB employee)
        {
            SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(employee.SalonId);
            IEnumerable<Employee_Service_MappingDB> source = from item in IoC.Resolve<IEmployeeManager>().GetEmployeeServiceMappingBySalonId(salonById.SalonId)
                where item.EmployeeId == employee.EmployeeId
                select item;
            foreach (GridViewRow row in this.gv.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox box = row.FindControl("cb") as CheckBox;
                    if (box != null)
                    {
                        Guid id = new Guid(this.gv.DataKeys[row.RowIndex].Value.ToString());
                        if (source.SingleOrDefault<Employee_Service_MappingDB>(item => (item.ServiceId == id)) != null)
                        {
                            box.Checked = true;
                        }
                    }
                }
            }
        }

        private void BindServices(SalonDB salon)
        {
            List<ServiceDB> servicesBySalonId = IoC.Resolve<IServiceManager>().GetServicesBySalonId(salon.SalonId);
            this.gv.DataSource = servicesBySalonId;
            this.gv.DataBind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("employees.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                Guid id = this.PostedEmployeeId;
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                EmployeeDB employee = IoC.Resolve<IEmployeeManager>().GetEmployeesBySalonId(salonById.SalonId).SingleOrDefault<EmployeeDB>(item => item.EmployeeId == id);
                employee.DisplayText = this.txtName.Text.Trim();
                employee = IoC.Resolve<IEmployeeManager>().UpdateEmployee(employee);
                List<Employee_Service_MappingDB> mappings = new List<Employee_Service_MappingDB>();
                IoC.Resolve<IEmployeeManager>().DeleteEmployeeServiceMappingByEmployee(employee);
                foreach (GridViewRow row in this.gv.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox box = row.FindControl("cb") as CheckBox;
                        if ((box != null) && box.Checked)
                        {
                            Employee_Service_MappingDB gdb = new Employee_Service_MappingDB();
                            Guid guid = new Guid(this.gv.DataKeys[row.RowIndex].Value.ToString());
                            gdb.EmployeeId = employee.EmployeeId;
                            gdb.ServiceId = guid;
                            mappings.Add(gdb);
                        }
                    }
                }
                if (mappings.Count > 0)
                {
                    IoC.Resolve<IEmployeeManager>().InsertEmployeeServiceMappingMultiple(mappings);
                }
                IoC.Resolve<ISchedulingManager>().DeleteTimeBlocksByEmployee(employee);
                if (!this.cbMonday.Checked)
                {
                    TimeBlockDB block = new TimeBlockDB {
                        BlockTypeId = 3,
                        CycleLength = 1,
                        CyclePeriodType = 10,
                        EmployeeDisplayText = employee.DisplayText,
                        EmployeeId = new Guid?(employee.EmployeeId),
                        EndTime = TimeSpan.Parse("23:59"),
                        SalonId = salonById.SalonId,
                        StartDateUtc = DateTime.Now.Date,
                        StartTime = TimeSpan.Parse("00:00"),
                        WeekDay = 2
                    };
                    block = IoC.Resolve<ISchedulingManager>().InsertTimeBlock(block);
                }
                if (!this.cbTuesday.Checked)
                {
                    TimeBlockDB kdb2 = new TimeBlockDB {
                        BlockTypeId = 3,
                        CycleLength = 1,
                        CyclePeriodType = 10,
                        EmployeeDisplayText = employee.DisplayText,
                        EmployeeId = new Guid?(employee.EmployeeId),
                        EndTime = TimeSpan.Parse("23:59"),
                        SalonId = salonById.SalonId,
                        StartDateUtc = DateTime.Now.Date,
                        StartTime = TimeSpan.Parse("00:00"),
                        WeekDay = 3
                    };
                    kdb2 = IoC.Resolve<ISchedulingManager>().InsertTimeBlock(kdb2);
                }
                if (!this.cbWednesday.Checked)
                {
                    TimeBlockDB kdb3 = new TimeBlockDB {
                        BlockTypeId = 3,
                        CycleLength = 1,
                        CyclePeriodType = 10,
                        EmployeeDisplayText = employee.DisplayText,
                        EmployeeId = new Guid?(employee.EmployeeId),
                        EndTime = TimeSpan.Parse("23:59"),
                        SalonId = salonById.SalonId,
                        StartDateUtc = DateTime.Now.Date,
                        StartTime = TimeSpan.Parse("00:00"),
                        WeekDay = 4
                    };
                    kdb3 = IoC.Resolve<ISchedulingManager>().InsertTimeBlock(kdb3);
                }
                if (!this.cbThursday.Checked)
                {
                    TimeBlockDB kdb4 = new TimeBlockDB {
                        BlockTypeId = 3,
                        CycleLength = 1,
                        CyclePeriodType = 10,
                        EmployeeDisplayText = employee.DisplayText,
                        EmployeeId = new Guid?(employee.EmployeeId),
                        EndTime = TimeSpan.Parse("23:59"),
                        SalonId = salonById.SalonId,
                        StartDateUtc = DateTime.Now.Date,
                        StartTime = TimeSpan.Parse("00:00"),
                        WeekDay = 5
                    };
                    kdb4 = IoC.Resolve<ISchedulingManager>().InsertTimeBlock(kdb4);
                }
                if (!this.cbFriday.Checked)
                {
                    TimeBlockDB kdb5 = new TimeBlockDB {
                        BlockTypeId = 3,
                        CycleLength = 1,
                        CyclePeriodType = 10,
                        EmployeeDisplayText = employee.DisplayText,
                        EmployeeId = new Guid?(employee.EmployeeId),
                        EndTime = TimeSpan.Parse("23:59"),
                        SalonId = salonById.SalonId,
                        StartDateUtc = DateTime.Now.Date,
                        StartTime = TimeSpan.Parse("00:00"),
                        WeekDay = 6
                    };
                    kdb5 = IoC.Resolve<ISchedulingManager>().InsertTimeBlock(kdb5);
                }
                if (!this.cbSaturday.Checked)
                {
                    TimeBlockDB kdb6 = new TimeBlockDB {
                        BlockTypeId = 3,
                        CycleLength = 1,
                        CyclePeriodType = 10,
                        EmployeeDisplayText = employee.DisplayText,
                        EmployeeId = new Guid?(employee.EmployeeId),
                        EndTime = TimeSpan.Parse("23:59"),
                        SalonId = salonById.SalonId,
                        StartDateUtc = DateTime.Now.Date,
                        StartTime = TimeSpan.Parse("00:00"),
                        WeekDay = 7
                    };
                    kdb6 = IoC.Resolve<ISchedulingManager>().InsertTimeBlock(kdb6);
                }
                if (!this.cbSunday.Checked)
                {
                    TimeBlockDB kdb7 = new TimeBlockDB {
                        BlockTypeId = 3,
                        CycleLength = 1,
                        CyclePeriodType = 10,
                        EmployeeDisplayText = employee.DisplayText,
                        EmployeeId = new Guid?(employee.EmployeeId),
                        EndTime = TimeSpan.Parse("23:59"),
                        SalonId = salonById.SalonId,
                        StartDateUtc = DateTime.Now.Date,
                        StartTime = TimeSpan.Parse("00:00"),
                        WeekDay = 1
                    };
                    kdb7 = IoC.Resolve<ISchedulingManager>().InsertTimeBlock(kdb7);
                }
            }
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("employees.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (this.PostedEmployeeId == Guid.Empty)
            {
                string str2 = $"{"sid"}={this.PostedSalonId}";
                string url = IFRMHelper.GetURL("employees.aspx", new string[] { str2 });
                base.Response.Redirect(url, true);
            }
            Guid id = this.PostedEmployeeId;
            SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
            EmployeeDB employee = IoC.Resolve<IEmployeeManager>().GetEmployeesBySalonId(salonById.SalonId).SingleOrDefault<EmployeeDB>(item => item.EmployeeId == id);
            if (employee == null)
            {
                string str4 = $"{"sid"}={this.PostedSalonId}";
                string str5 = IFRMHelper.GetURL("employees.aspx", new string[] { str4 });
                base.Response.Redirect(str5, true);
            }
            if (!this.Page.IsPostBack)
            {
                this.BindEmployeeDetails(employee);
                this.BindEmployeeAvailability(salonById, employee);
                this.BindServices(salonById);
                this.BindEmployeeServiceMappings(employee);
            }
        }

        public Guid PostedEmployeeId
        {
            get
            {
                string str = base.Request.QueryString["eid"];
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

