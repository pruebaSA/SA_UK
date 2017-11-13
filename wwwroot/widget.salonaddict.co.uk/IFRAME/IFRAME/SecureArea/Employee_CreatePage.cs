namespace IFRAME.SecureArea
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class Employee_CreatePage : IFRMSecurePage
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
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
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

        private void BindEmployeeAvailability()
        {
            SalonDB salon = IFRMContext.Current.Salon;
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
            }
        }

        private void BindServices()
        {
            SalonDB salon = IFRMContext.Current.Salon;
            List<ServiceDB> servicesBySalonId = IoC.Resolve<IServiceManager>().GetServicesBySalonId(salon.SalonId);
            this.gv.DataSource = servicesBySalonId;
            this.gv.DataBind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("employees.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                EmployeeDB employee = new EmployeeDB {
                    BookOnWeb = true,
                    BookOnMobile = true,
                    BookOnWidget = true,
                    CreatedOnUtc = DateTime.UtcNow,
                    Deleted = false,
                    DisplayText = this.txtName.Text.Trim(),
                    SalonId = salon.SalonId,
                    SEName = Guid.NewGuid().ToString().ToLowerInvariant(),
                    ShowOnWeb = true,
                    ShowOnMobile = true,
                    ShowOnWidget = true
                };
                employee = IoC.Resolve<IEmployeeManager>().InsertEmployee(employee);
                List<Employee_Service_MappingDB> mappings = new List<Employee_Service_MappingDB>();
                foreach (GridViewRow row in this.gv.Rows)
                {
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox box = row.FindControl("cb") as CheckBox;
                        if ((box != null) && box.Checked)
                        {
                            Employee_Service_MappingDB item = new Employee_Service_MappingDB();
                            Guid guid = new Guid(this.gv.DataKeys[row.RowIndex].Value.ToString());
                            item.EmployeeId = employee.EmployeeId;
                            item.ServiceId = guid;
                            mappings.Add(item);
                        }
                    }
                }
                if (mappings.Count > 0)
                {
                    IoC.Resolve<IEmployeeManager>().InsertEmployeeServiceMappingMultiple(mappings);
                }
                if (!this.cbMonday.Checked)
                {
                    TimeBlockDB block = new TimeBlockDB {
                        BlockTypeId = 3,
                        CycleLength = 1,
                        CyclePeriodType = 10,
                        EmployeeDisplayText = employee.DisplayText,
                        EmployeeId = new Guid?(employee.EmployeeId),
                        EndTime = TimeSpan.Parse("23:59"),
                        SalonId = salon.SalonId,
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
                        SalonId = salon.SalonId,
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
                        SalonId = salon.SalonId,
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
                        SalonId = salon.SalonId,
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
                        SalonId = salon.SalonId,
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
                        SalonId = salon.SalonId,
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
                        SalonId = salon.SalonId,
                        StartDateUtc = DateTime.Now.Date,
                        StartTime = TimeSpan.Parse("00:00"),
                        WeekDay = 1
                    };
                    kdb7 = IoC.Resolve<ISchedulingManager>().InsertTimeBlock(kdb7);
                }
            }
            string uRL = IFRMHelper.GetURL("employees.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.BindEmployeeAvailability();
                this.BindServices();
            }
        }
    }
}

