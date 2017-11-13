namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class ReportNonDeliveryPage : IFRMAdminPage
    {
        protected Button btnResend;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
            this.gv.Columns[3].HeaderText = base.GetLocaleResourceString("gv.Columns[3].HeaderText");
        }

        private void BindReport()
        {
            List<QueuedMessageDB> list = IoC.Resolve<IMessageManager>().SearchQueuedMessages(null, null, DateTime.Now.AddMonths(-1), DateTime.Now, 100, true, 10);
            this.gv.DataSource = list;
            this.gv.DataBind();
        }

        protected void btnResend_Click(object sender, EventArgs e)
        {
            foreach (QueuedMessageDB edb in IoC.Resolve<IMessageManager>().SearchQueuedMessages(null, null, DateTime.Now.AddMonths(-1), DateTime.Now, 10, true, 10))
            {
                if (!IFRMHelper.InitiateDelivery(edb, base.Request.Url.IsLoopback))
                {
                    edb.SendTries++;
                    IoC.Resolve<IMessageManager>().UpdateQueuedMessage(edb);
                    break;
                }
                IoC.Resolve<IMessageManager>().DeleteQueuedMessage(edb);
            }
            string uRL = IFRMHelper.GetURL("reportnondelivery.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.RowIndex].Value.ToString());
            string str = $"{"mid"}={guid}";
            string uRL = IFRMHelper.GetURL("queuedmessage-delete.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.NewEditIndex].Value.ToString());
            string str = $"{"mid"}={guid}";
            string uRL = IFRMHelper.GetURL("queuedmessage-edit.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindReport();
            }
        }
    }
}

