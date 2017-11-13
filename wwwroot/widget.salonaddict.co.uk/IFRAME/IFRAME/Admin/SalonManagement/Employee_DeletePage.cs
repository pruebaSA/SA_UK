namespace IFRAME.Admin.SalonManagement
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class Employee_DeletePage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnDelete;
        protected SalonMenu cntlMenu;
        protected Literal ltrName;
        protected Panel pnl;

        private void BindEmployeeDetails(EmployeeDB value)
        {
            this.ltrName.Text = value.DisplayText;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("employees.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Guid id = this.PostedEmployeeId;
            SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
            EmployeeDB employee = IoC.Resolve<IEmployeeManager>().GetEmployeesBySalonId(salonById.SalonId).SingleOrDefault<EmployeeDB>(item => item.EmployeeId == id);
            employee.Deleted = true;
            employee = IoC.Resolve<IEmployeeManager>().UpdateEmployee(employee);
            IoC.Resolve<IEmployeeManager>().DeleteEmployeeServiceMappingByEmployee(employee);
            IoC.Resolve<ISchedulingManager>().DeleteTimeBlocksByEmployee(employee);
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
            if (!this.Page.IsPostBack)
            {
                if (this.PostedEmployeeId == Guid.Empty)
                {
                    string str2 = $"{"sid"}={this.PostedSalonId}";
                    string url = IFRMHelper.GetURL("employees.aspx", new string[] { str2 });
                    base.Response.Redirect(url, true);
                }
                Guid id = this.PostedEmployeeId;
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                EmployeeDB edb = IoC.Resolve<IEmployeeManager>().GetEmployeesBySalonId(salonById.SalonId).SingleOrDefault<EmployeeDB>(item => item.EmployeeId == id);
                if (edb == null)
                {
                    string str4 = $"{"sid"}={this.PostedSalonId}";
                    string str5 = IFRMHelper.GetURL("employees.aspx", new string[] { str4 });
                    base.Response.Redirect(str5, true);
                }
                this.BindEmployeeDetails(edb);
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

