namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class Employee_DeletePage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnDelete;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected Literal ltrName;
        protected Panel pnl;

        private void BindEmployeeDetails(EmployeeDB value)
        {
            this.ltrName.Text = value.DisplayText;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("employees.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            Guid id = this.PostedEmployeeId;
            EmployeeDB employee = IoC.Resolve<IEmployeeManager>().GetEmployeesBySalonId(salon.SalonId).SingleOrDefault<EmployeeDB>(item => item.EmployeeId == id);
            employee.Deleted = true;
            employee = IoC.Resolve<IEmployeeManager>().UpdateEmployee(employee);
            IoC.Resolve<IEmployeeManager>().DeleteEmployeeServiceMappingByEmployee(employee);
            IoC.Resolve<ISchedulingManager>().DeleteTimeBlocksByEmployee(employee);
            string uRL = IFRMHelper.GetURL("employees.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                if (this.PostedEmployeeId == Guid.Empty)
                {
                    string uRL = IFRMHelper.GetURL("employees.aspx", new string[0]);
                    base.Response.Redirect(uRL, true);
                }
                Guid id = this.PostedEmployeeId;
                SalonDB salon = IFRMContext.Current.Salon;
                EmployeeDB edb = IoC.Resolve<IEmployeeManager>().GetEmployeesBySalonId(salon.SalonId).SingleOrDefault<EmployeeDB>(item => item.EmployeeId == id);
                if (edb == null)
                {
                    string url = IFRMHelper.GetURL("employees.aspx", new string[0]);
                    base.Response.Redirect(url, true);
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
    }
}

