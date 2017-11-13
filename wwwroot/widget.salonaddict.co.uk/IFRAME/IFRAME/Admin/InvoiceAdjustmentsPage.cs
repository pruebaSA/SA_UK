namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class InvoiceAdjustmentsPage : IFRMAdminPage
    {
        protected Button btnAdd;
        protected Button btnCancel;
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
        }

        private void BindAdjustments(SalonInvoiceDB value)
        {
            List<SalonInvoiceADDB> salonInvoiceADByInvoiceId = IoC.Resolve<IBillingManager>().GetSalonInvoiceADByInvoiceId(this.PostedSalonInvoiceId);
            this.gv.DataSource = salonInvoiceADByInvoiceId;
            this.gv.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string str = $"{"inv"}={this.PostedSalonInvoiceId}";
            string uRL = IFRMHelper.GetURL("invoiceadjustment-create.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"inv"}={this.PostedSalonInvoiceId}";
            string uRL = IFRMHelper.GetURL("invoicedetails.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.RowIndex].Value.ToString());
            string str = $"{"aid"}={guid}";
            string uRL = IFRMHelper.GetURL("invoiceadjustment-delete.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            string uRL = IFRMHelper.GetURL("invoicesissued.aspx", new string[0]);
            if (this.PostedSalonInvoiceId == Guid.Empty)
            {
                base.Response.Redirect(uRL, true);
            }
            SalonInvoiceDB salonInvoiceById = IoC.Resolve<IBillingManager>().GetSalonInvoiceById(this.PostedSalonInvoiceId);
            if (salonInvoiceById == null)
            {
                base.Response.Redirect(uRL, true);
            }
            if (salonInvoiceById.PaidOn.HasValue || salonInvoiceById.Deleted)
            {
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                this.BindAdjustments(salonInvoiceById);
            }
        }

        public Guid PostedSalonInvoiceId
        {
            get
            {
                string str = base.Request.QueryString["inv"];
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

