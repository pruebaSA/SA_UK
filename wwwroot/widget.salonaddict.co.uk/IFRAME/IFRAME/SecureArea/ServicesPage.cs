namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class ServicesPage : IFRMSecurePage
    {
        protected Button btnAdd;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
            this.gv.Columns[3].HeaderText = base.GetLocaleResourceString("gv.Columns[3].HeaderText");
            this.gv.Columns[4].HeaderText = base.GetLocaleResourceString("gv.Columns[4].HeaderText");
            this.gv.Columns[5].HeaderText = base.GetLocaleResourceString("gv.Columns[5].HeaderText");
        }

        private void BindServices()
        {
            SalonDB salon = IFRMContext.Current.Salon;
            List<EmployeeDB> employeesBySalonId = IoC.Resolve<IEmployeeManager>().GetEmployeesBySalonId(salon.SalonId);
            List<ServiceDB> servicesBySalonId = IoC.Resolve<IServiceManager>().GetServicesBySalonId(salon.SalonId);
            List<Employee_Service_MappingDB> employeeServiceMappingBySalonId = IoC.Resolve<IEmployeeManager>().GetEmployeeServiceMappingBySalonId(salon.SalonId);
            List<object> list4 = new List<object>();
            using (List<ServiceDB>.Enumerator enumerator = servicesBySalonId.GetEnumerator())
            {
                Func<Employee_Service_MappingDB, bool> predicate = null;
                ServiceDB service;
                while (enumerator.MoveNext())
                {
                    service = enumerator.Current;
                    List<object> list5 = new List<object>();
                    if (predicate == null)
                    {
                        predicate = item => item.ServiceId == service.ServiceId;
                    }
                    using (List<Employee_Service_MappingDB>.Enumerator enumerator2 = employeeServiceMappingBySalonId.Where<Employee_Service_MappingDB>(predicate).ToList<Employee_Service_MappingDB>().GetEnumerator())
                    {
                        Func<EmployeeDB, bool> func = null;
                        Employee_Service_MappingDB mapping;
                        while (enumerator2.MoveNext())
                        {
                            mapping = enumerator2.Current;
                            if (func == null)
                            {
                                func = item => item.EmployeeId == mapping.EmployeeId;
                            }
                            EmployeeDB edb = employeesBySalonId.SingleOrDefault<EmployeeDB>(func);
                            if (edb != null)
                            {
                                list5.Add(new { DisplayText = edb.DisplayText });
                            }
                        }
                    }
                    list4.Add(new { 
                        ServiceId = service.ServiceId,
                        Name = service.Name,
                        ShortDescription = service.ShortDescription,
                        Price = service.Price,
                        Published = service.Active,
                        Employees = list5
                    });
                }
            }
            this.gv.DataSource = list4;
            this.gv.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("service-create.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void cb_CheckedChanged(object sender, EventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            List<ServiceDB> servicesBySalonId = IoC.Resolve<IServiceManager>().GetServicesBySalonId(salon.SalonId);
            foreach (GridViewRow row in this.gv.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox box = row.FindControl("cb") as CheckBox;
                    if (box != null)
                    {
                        Guid id = new Guid(this.gv.DataKeys[row.RowIndex].Value.ToString());
                        ServiceDB service = servicesBySalonId.SingleOrDefault<ServiceDB>(item => item.ServiceId == id);
                        if ((service != null) && (service.Active != box.Checked))
                        {
                            service.Active = box.Checked;
                            service = IoC.Resolve<IServiceManager>().UpdateService(service);
                            break;
                        }
                    }
                }
            }
            string uRL = IFRMHelper.GetURL("services.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.RowIndex].Value.ToString());
            string str = $"{"svid"}={guid}";
            string uRL = IFRMHelper.GetURL("service-delete.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.NewEditIndex].Value.ToString());
            string str = $"{"svid"}={guid}";
            string uRL = IFRMHelper.GetURL("service-edit.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindServices();
            }
        }
    }
}

