namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class SalonEmployees_SetupPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected CheckBox cb1;
        protected CheckBox cb10;
        protected CheckBox cb2;
        protected CheckBox cb3;
        protected CheckBox cb4;
        protected CheckBox cb5;
        protected CheckBox cb6;
        protected CheckBox cb7;
        protected CheckBox cb8;
        protected CheckBox cb9;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected LinkButton lbServiceHeader1;
        protected LinkButton lbServiceHeader10;
        protected LinkButton lbServiceHeader2;
        protected LinkButton lbServiceHeader3;
        protected LinkButton lbServiceHeader4;
        protected LinkButton lbServiceHeader5;
        protected LinkButton lbServiceHeader6;
        protected LinkButton lbServiceHeader7;
        protected LinkButton lbServiceHeader8;
        protected LinkButton lbServiceHeader9;
        protected Literal ltrHeader;
        protected Panel pnl;
        protected Panel pnlServiceContent1;
        protected Panel pnlServiceContent10;
        protected Panel pnlServiceContent2;
        protected Panel pnlServiceContent3;
        protected Panel pnlServiceContent4;
        protected Panel pnlServiceContent5;
        protected Panel pnlServiceContent6;
        protected Panel pnlServiceContent7;
        protected Panel pnlServiceContent8;
        protected Panel pnlServiceContent9;
        protected BalloonPopupExtender pnlServiceEx1;
        protected BalloonPopupExtender pnlServiceEx10;
        protected BalloonPopupExtender pnlServiceEx2;
        protected BalloonPopupExtender pnlServiceEx3;
        protected BalloonPopupExtender pnlServiceEx4;
        protected BalloonPopupExtender pnlServiceEx5;
        protected BalloonPopupExtender pnlServiceEx6;
        protected BalloonPopupExtender pnlServiceEx7;
        protected BalloonPopupExtender pnlServiceEx8;
        protected BalloonPopupExtender pnlServiceEx9;
        protected Repeater rptrServices1;
        protected Repeater rptrServices10;
        protected Repeater rptrServices2;
        protected Repeater rptrServices3;
        protected Repeater rptrServices4;
        protected Repeater rptrServices5;
        protected Repeater rptrServices6;
        protected Repeater rptrServices7;
        protected Repeater rptrServices8;
        protected Repeater rptrServices9;
        protected TextBox txtName1;
        protected TextBox txtName10;
        protected TextBox txtName2;
        protected TextBox txtName3;
        protected TextBox txtName4;
        protected TextBox txtName5;
        protected TextBox txtName6;
        protected TextBox txtName7;
        protected TextBox txtName8;
        protected TextBox txtName9;
        protected UpdatePanel up;
        protected RequiredFieldValidator valName1;
        protected RequiredFieldValidator valName10;
        protected ValidatorCalloutExtender valName10Ex;
        protected ValidatorCalloutExtender valName1Ex;
        protected RequiredFieldValidator valName2;
        protected ValidatorCalloutExtender valName2Ex;
        protected RequiredFieldValidator valName3;
        protected ValidatorCalloutExtender valName3Ex;
        protected RequiredFieldValidator valName4;
        protected ValidatorCalloutExtender valName4Ex;
        protected RequiredFieldValidator valName5;
        protected ValidatorCalloutExtender valName5Ex;
        protected RequiredFieldValidator valName6;
        protected ValidatorCalloutExtender valName6Ex;
        protected RequiredFieldValidator valName7;
        protected ValidatorCalloutExtender valName7Ex;
        protected RequiredFieldValidator valName8;
        protected ValidatorCalloutExtender valName8Ex;
        protected RequiredFieldValidator valName9;
        protected ValidatorCalloutExtender valName9Ex;

        private void BindServices(SalonDB value)
        {
            List<ServiceDB> servicesBySalonId = IoC.Resolve<IServiceManager>().GetServicesBySalonId(value.SalonId);
            this.rptrServices1.DataSource = servicesBySalonId;
            this.rptrServices1.DataBind();
            this.rptrServices2.DataSource = servicesBySalonId;
            this.rptrServices2.DataBind();
            this.rptrServices3.DataSource = servicesBySalonId;
            this.rptrServices3.DataBind();
            this.rptrServices4.DataSource = servicesBySalonId;
            this.rptrServices4.DataBind();
            this.rptrServices5.DataSource = servicesBySalonId;
            this.rptrServices5.DataBind();
            this.rptrServices6.DataSource = servicesBySalonId;
            this.rptrServices6.DataBind();
            this.rptrServices7.DataSource = servicesBySalonId;
            this.rptrServices7.DataBind();
            this.rptrServices8.DataSource = servicesBySalonId;
            this.rptrServices8.DataBind();
            this.rptrServices9.DataSource = servicesBySalonId;
            this.rptrServices9.DataBind();
            this.rptrServices10.DataSource = servicesBySalonId;
            this.rptrServices10.DataBind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("salonsetup.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                List<ServiceDB> servicesBySalonId = IoC.Resolve<IServiceManager>().GetServicesBySalonId(salonById.SalonId);
                EmployeeDB employee = null;
                Employee_Service_MappingDB mapping = null;
                employee = new EmployeeDB {
                    BookOnWeb = true,
                    BookOnMobile = true,
                    BookOnWidget = true,
                    CreatedOnUtc = DateTime.UtcNow,
                    Deleted = false,
                    DisplayText = this.txtName1.Text.Trim(),
                    SalonId = salonById.SalonId,
                    SEName = Guid.NewGuid().ToString().ToLowerInvariant(),
                    ShowOnWeb = true,
                    ShowOnMobile = true,
                    ShowOnWidget = true
                };
                employee = IoC.Resolve<IEmployeeManager>().InsertEmployee(employee);
                foreach (RepeaterItem item in this.rptrServices1.Items)
                {
                    CheckBox box = item.FindControl("cb") as CheckBox;
                    Literal literal = item.FindControl("ltrServiceId") as Literal;
                    Guid serviceID = new Guid(literal.Text);
                    servicesBySalonId.Single<ServiceDB>(s => s.ServiceId == serviceID);
                    if (((box != null) && (literal != null)) && box.Checked)
                    {
                        mapping = new Employee_Service_MappingDB {
                            EmployeeId = employee.EmployeeId,
                            ServiceId = serviceID
                        };
                        mapping = IoC.Resolve<IEmployeeManager>().InsertEmployeeServiceMapping(mapping);
                    }
                }
                if (this.cb2.Checked)
                {
                    employee = new EmployeeDB {
                        BookOnWeb = true,
                        BookOnMobile = true,
                        BookOnWidget = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Deleted = false,
                        DisplayText = this.txtName2.Text.Trim(),
                        SalonId = salonById.SalonId,
                        SEName = Guid.NewGuid().ToString().ToLowerInvariant(),
                        ShowOnWeb = true,
                        ShowOnMobile = true,
                        ShowOnWidget = true
                    };
                    employee = IoC.Resolve<IEmployeeManager>().InsertEmployee(employee);
                    foreach (RepeaterItem item2 in this.rptrServices2.Items)
                    {
                        CheckBox box2 = item2.FindControl("cb") as CheckBox;
                        Literal literal2 = item2.FindControl("ltrServiceId") as Literal;
                        Guid serviceID = new Guid(literal2.Text);
                        servicesBySalonId.Single<ServiceDB>(s => s.ServiceId == serviceID);
                        if (((box2 != null) && (literal2 != null)) && box2.Checked)
                        {
                            mapping = new Employee_Service_MappingDB {
                                EmployeeId = employee.EmployeeId,
                                ServiceId = serviceID
                            };
                            mapping = IoC.Resolve<IEmployeeManager>().InsertEmployeeServiceMapping(mapping);
                        }
                    }
                }
                if (this.cb3.Checked)
                {
                    employee = new EmployeeDB {
                        BookOnWeb = true,
                        BookOnMobile = true,
                        BookOnWidget = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Deleted = false,
                        DisplayText = this.txtName3.Text.Trim(),
                        SalonId = salonById.SalonId,
                        SEName = Guid.NewGuid().ToString().ToLowerInvariant(),
                        ShowOnWeb = true,
                        ShowOnMobile = true,
                        ShowOnWidget = true
                    };
                    employee = IoC.Resolve<IEmployeeManager>().InsertEmployee(employee);
                    foreach (RepeaterItem item3 in this.rptrServices3.Items)
                    {
                        CheckBox box3 = item3.FindControl("cb") as CheckBox;
                        Literal literal3 = item3.FindControl("ltrServiceId") as Literal;
                        Guid serviceID = new Guid(literal3.Text);
                        servicesBySalonId.Single<ServiceDB>(s => s.ServiceId == serviceID);
                        if (((box3 != null) && (literal3 != null)) && box3.Checked)
                        {
                            mapping = new Employee_Service_MappingDB {
                                EmployeeId = employee.EmployeeId,
                                ServiceId = serviceID
                            };
                            mapping = IoC.Resolve<IEmployeeManager>().InsertEmployeeServiceMapping(mapping);
                        }
                    }
                }
                if (this.cb4.Checked)
                {
                    employee = new EmployeeDB {
                        BookOnWeb = true,
                        BookOnMobile = true,
                        BookOnWidget = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Deleted = false,
                        DisplayText = this.txtName4.Text.Trim(),
                        SalonId = salonById.SalonId,
                        SEName = Guid.NewGuid().ToString().ToLowerInvariant(),
                        ShowOnWeb = true,
                        ShowOnMobile = true,
                        ShowOnWidget = true
                    };
                    employee = IoC.Resolve<IEmployeeManager>().InsertEmployee(employee);
                    foreach (RepeaterItem item4 in this.rptrServices4.Items)
                    {
                        CheckBox box4 = item4.FindControl("cb") as CheckBox;
                        Literal literal4 = item4.FindControl("ltrServiceId") as Literal;
                        Guid serviceID = new Guid(literal4.Text);
                        servicesBySalonId.Single<ServiceDB>(s => s.ServiceId == serviceID);
                        if (((box4 != null) && (literal4 != null)) && box4.Checked)
                        {
                            mapping = new Employee_Service_MappingDB {
                                EmployeeId = employee.EmployeeId,
                                ServiceId = serviceID
                            };
                            mapping = IoC.Resolve<IEmployeeManager>().InsertEmployeeServiceMapping(mapping);
                        }
                    }
                }
                if (this.cb5.Checked)
                {
                    employee = new EmployeeDB {
                        BookOnWeb = true,
                        BookOnMobile = true,
                        BookOnWidget = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Deleted = false,
                        DisplayText = this.txtName5.Text.Trim(),
                        SalonId = salonById.SalonId,
                        SEName = Guid.NewGuid().ToString().ToLowerInvariant(),
                        ShowOnWeb = true,
                        ShowOnMobile = true,
                        ShowOnWidget = true
                    };
                    employee = IoC.Resolve<IEmployeeManager>().InsertEmployee(employee);
                    foreach (RepeaterItem item5 in this.rptrServices5.Items)
                    {
                        CheckBox box5 = item5.FindControl("cb") as CheckBox;
                        Literal literal5 = item5.FindControl("ltrServiceId") as Literal;
                        Guid serviceID = new Guid(literal5.Text);
                        servicesBySalonId.Single<ServiceDB>(s => s.ServiceId == serviceID);
                        if (((box5 != null) && (literal5 != null)) && box5.Checked)
                        {
                            mapping = new Employee_Service_MappingDB {
                                EmployeeId = employee.EmployeeId,
                                ServiceId = serviceID
                            };
                            mapping = IoC.Resolve<IEmployeeManager>().InsertEmployeeServiceMapping(mapping);
                        }
                    }
                }
                if (this.cb6.Checked)
                {
                    employee = new EmployeeDB {
                        BookOnWeb = true,
                        BookOnMobile = true,
                        BookOnWidget = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Deleted = false,
                        DisplayText = this.txtName6.Text.Trim(),
                        SalonId = salonById.SalonId,
                        SEName = Guid.NewGuid().ToString().ToLowerInvariant(),
                        ShowOnWeb = true,
                        ShowOnMobile = true,
                        ShowOnWidget = true
                    };
                    employee = IoC.Resolve<IEmployeeManager>().InsertEmployee(employee);
                    foreach (RepeaterItem item6 in this.rptrServices6.Items)
                    {
                        CheckBox box6 = item6.FindControl("cb") as CheckBox;
                        Literal literal6 = item6.FindControl("ltrServiceId") as Literal;
                        Guid serviceID = new Guid(literal6.Text);
                        servicesBySalonId.Single<ServiceDB>(s => s.ServiceId == serviceID);
                        if (((box6 != null) && (literal6 != null)) && box6.Checked)
                        {
                            mapping = new Employee_Service_MappingDB {
                                EmployeeId = employee.EmployeeId,
                                ServiceId = serviceID
                            };
                            mapping = IoC.Resolve<IEmployeeManager>().InsertEmployeeServiceMapping(mapping);
                        }
                    }
                }
                if (this.cb7.Checked)
                {
                    employee = new EmployeeDB {
                        BookOnWeb = true,
                        BookOnMobile = true,
                        BookOnWidget = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Deleted = false,
                        DisplayText = this.txtName7.Text.Trim(),
                        SalonId = salonById.SalonId,
                        SEName = Guid.NewGuid().ToString().ToLowerInvariant(),
                        ShowOnWeb = true,
                        ShowOnMobile = true,
                        ShowOnWidget = true
                    };
                    employee = IoC.Resolve<IEmployeeManager>().InsertEmployee(employee);
                    foreach (RepeaterItem item7 in this.rptrServices7.Items)
                    {
                        CheckBox box7 = item7.FindControl("cb") as CheckBox;
                        Literal literal7 = item7.FindControl("ltrServiceId") as Literal;
                        Guid serviceID = new Guid(literal7.Text);
                        servicesBySalonId.Single<ServiceDB>(s => s.ServiceId == serviceID);
                        if (((box7 != null) && (literal7 != null)) && box7.Checked)
                        {
                            mapping = new Employee_Service_MappingDB {
                                EmployeeId = employee.EmployeeId,
                                ServiceId = serviceID
                            };
                            mapping = IoC.Resolve<IEmployeeManager>().InsertEmployeeServiceMapping(mapping);
                        }
                    }
                }
                if (this.cb8.Checked)
                {
                    employee = new EmployeeDB {
                        BookOnWeb = true,
                        BookOnMobile = true,
                        BookOnWidget = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Deleted = false,
                        DisplayText = this.txtName8.Text.Trim(),
                        SalonId = salonById.SalonId,
                        SEName = Guid.NewGuid().ToString().ToLowerInvariant(),
                        ShowOnWeb = true,
                        ShowOnMobile = true,
                        ShowOnWidget = true
                    };
                    employee = IoC.Resolve<IEmployeeManager>().InsertEmployee(employee);
                    foreach (RepeaterItem item8 in this.rptrServices8.Items)
                    {
                        CheckBox box8 = item8.FindControl("cb") as CheckBox;
                        Literal literal8 = item8.FindControl("ltrServiceId") as Literal;
                        Guid serviceID = new Guid(literal8.Text);
                        servicesBySalonId.Single<ServiceDB>(s => s.ServiceId == serviceID);
                        if (((box8 != null) && (literal8 != null)) && box8.Checked)
                        {
                            mapping = new Employee_Service_MappingDB {
                                EmployeeId = employee.EmployeeId,
                                ServiceId = serviceID
                            };
                            mapping = IoC.Resolve<IEmployeeManager>().InsertEmployeeServiceMapping(mapping);
                        }
                    }
                }
                if (this.cb9.Checked)
                {
                    employee = new EmployeeDB {
                        BookOnWeb = true,
                        BookOnMobile = true,
                        BookOnWidget = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Deleted = false,
                        DisplayText = this.txtName9.Text.Trim(),
                        SalonId = salonById.SalonId,
                        SEName = Guid.NewGuid().ToString().ToLowerInvariant(),
                        ShowOnWeb = true,
                        ShowOnMobile = true,
                        ShowOnWidget = true
                    };
                    employee = IoC.Resolve<IEmployeeManager>().InsertEmployee(employee);
                    foreach (RepeaterItem item9 in this.rptrServices9.Items)
                    {
                        CheckBox box9 = item9.FindControl("cb") as CheckBox;
                        Literal literal9 = item9.FindControl("ltrServiceId") as Literal;
                        Guid serviceID = new Guid(literal9.Text);
                        servicesBySalonId.Single<ServiceDB>(s => s.ServiceId == serviceID);
                        if (((box9 != null) && (literal9 != null)) && box9.Checked)
                        {
                            mapping = new Employee_Service_MappingDB {
                                EmployeeId = employee.EmployeeId,
                                ServiceId = serviceID
                            };
                            mapping = IoC.Resolve<IEmployeeManager>().InsertEmployeeServiceMapping(mapping);
                        }
                    }
                }
                if (this.cb10.Checked)
                {
                    employee = new EmployeeDB {
                        BookOnWeb = true,
                        BookOnMobile = true,
                        BookOnWidget = true,
                        CreatedOnUtc = DateTime.UtcNow,
                        Deleted = false,
                        DisplayText = this.txtName10.Text.Trim(),
                        SalonId = salonById.SalonId,
                        SEName = Guid.NewGuid().ToString().ToLowerInvariant(),
                        ShowOnWeb = true,
                        ShowOnMobile = true,
                        ShowOnWidget = true
                    };
                    employee = IoC.Resolve<IEmployeeManager>().InsertEmployee(employee);
                    foreach (RepeaterItem item10 in this.rptrServices10.Items)
                    {
                        CheckBox box10 = item10.FindControl("cb") as CheckBox;
                        Literal literal10 = item10.FindControl("ltrServiceId") as Literal;
                        Guid serviceID = new Guid(literal10.Text);
                        servicesBySalonId.Single<ServiceDB>(s => s.ServiceId == serviceID);
                        if (((box10 != null) && (literal10 != null)) && box10.Checked)
                        {
                            mapping = new Employee_Service_MappingDB {
                                EmployeeId = employee.EmployeeId,
                                ServiceId = serviceID
                            };
                            mapping = IoC.Resolve<IEmployeeManager>().InsertEmployeeServiceMapping(mapping);
                        }
                    }
                }
            }
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("salonsetup.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void cb_CheckedChanged(object sender, EventArgs e)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedSalonId == Guid.Empty)
            {
                string str = $"{"sid"}={this.PostedSalonId}";
                string uRL = IFRMHelper.GetURL("salonsetup.aspx", new string[] { str });
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string str3 = $"{"sid"}={this.PostedSalonId}";
                    string url = IFRMHelper.GetURL("salonsetup.aspx", new string[] { str3 });
                    base.Response.Redirect(url, true);
                }
                this.ltrHeader.Text = string.Format(base.GetLocaleResourceString("ltrHeader.Text"), salonById.Name);
                this.BindServices(salonById);
            }
            this.valName1.Visible = this.cb1.Checked;
            this.valName1Ex.Enabled = this.cb1.Checked;
            this.valName2.Visible = this.cb2.Checked;
            this.valName2Ex.Enabled = this.cb2.Checked;
            this.valName3.Visible = this.cb3.Checked;
            this.valName3Ex.Enabled = this.cb3.Checked;
            this.valName4.Visible = this.cb4.Checked;
            this.valName4Ex.Enabled = this.cb4.Checked;
            this.valName5.Visible = this.cb5.Checked;
            this.valName5Ex.Enabled = this.cb5.Checked;
            this.valName6.Visible = this.cb6.Checked;
            this.valName6Ex.Enabled = this.cb6.Checked;
            this.valName7.Visible = this.cb7.Checked;
            this.valName7Ex.Enabled = this.cb7.Checked;
            this.valName8.Visible = this.cb8.Checked;
            this.valName8Ex.Enabled = this.cb8.Checked;
            this.valName9.Visible = this.cb9.Checked;
            this.valName9Ex.Enabled = this.cb9.Checked;
            this.valName10.Visible = this.cb10.Checked;
            this.valName10Ex.Enabled = this.cb10.Checked;
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

