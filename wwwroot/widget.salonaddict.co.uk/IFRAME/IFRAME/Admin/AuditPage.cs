namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class AuditPage : IFRMAdminPage
    {
        protected Button btnClear;
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
        }

        private void BindLogList()
        {
            List<LogDB> allErrors = IoC.Resolve<ILogManager>().GetAllErrors();
            this.gv.DataSource = allErrors;
            this.gv.DataBind();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            IoC.Resolve<ILogManager>().ClearLog();
            string uRL = IFRMHelper.GetURL("audit.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid logId = new Guid(this.gv.DataKeys[e.RowIndex].Value.ToString());
            LogDB errorById = IoC.Resolve<ILogManager>().GetErrorById(logId);
            IoC.Resolve<ILogManager>().DeleteLog(errorById);
            string uRL = IFRMHelper.GetURL("audit.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.NewEditIndex].Value.ToString());
            string str = $"{"lid"}={guid}";
            string uRL = IFRMHelper.GetURL("logdetails.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindLogList();
            }
        }
    }
}

