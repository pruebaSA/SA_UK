namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class EmployeesPage : IFRMSecurePage
    {
        protected Button btnAdd;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
        }

        private void BindEmployees()
        {
            SalonDB salon = IFRMContext.Current.Salon;
            List<EmployeeDB> employeesBySalonId = IoC.Resolve<IEmployeeManager>().GetEmployeesBySalonId(salon.SalonId);
            List<ServiceDB> servicesBySalonId = IoC.Resolve<IServiceManager>().GetServicesBySalonId(salon.SalonId);
            List<Employee_Service_MappingDB> employeeServiceMappingBySalonId = IoC.Resolve<IEmployeeManager>().GetEmployeeServiceMappingBySalonId(salon.SalonId);
            List<object> list4 = new List<object>();
            using (List<EmployeeDB>.Enumerator enumerator = employeesBySalonId.GetEnumerator())
            {
                Func<Employee_Service_MappingDB, bool> predicate = null;
                EmployeeDB employee;
                while (enumerator.MoveNext())
                {
                    employee = enumerator.Current;
                    List<object> list5 = new List<object>();
                    if (predicate == null)
                    {
                        predicate = item => item.EmployeeId == employee.EmployeeId;
                    }
                    using (List<Employee_Service_MappingDB>.Enumerator enumerator2 = employeeServiceMappingBySalonId.Where<Employee_Service_MappingDB>(predicate).ToList<Employee_Service_MappingDB>().GetEnumerator())
                    {
                        Func<ServiceDB, bool> func = null;
                        Employee_Service_MappingDB mapping;
                        while (enumerator2.MoveNext())
                        {
                            mapping = enumerator2.Current;
                            if (func == null)
                            {
                                func = item => item.ServiceId == mapping.ServiceId;
                            }
                            ServiceDB edb = servicesBySalonId.SingleOrDefault<ServiceDB>(func);
                            if (edb != null)
                            {
                                list5.Add(new { 
                                    Name = edb.Name,
                                    Price = edb.Price
                                });
                            }
                        }
                    }
                    list4.Add(new { 
                        EmployeeId = employee.EmployeeId,
                        Employee = new { DisplayText = employee.DisplayText },
                        Services = list5
                    });
                }
            }
            this.gv.DataSource = list4;
            this.gv.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("employee-create.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.RowIndex].Value.ToString());
            string str = $"{"eid"}={guid}";
            string uRL = IFRMHelper.GetURL("employee-delete.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.NewEditIndex].Value.ToString());
            string str = $"{"eid"}={guid}";
            string uRL = IFRMHelper.GetURL("employee-edit.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindEmployees();
            }
        }
    }
}

