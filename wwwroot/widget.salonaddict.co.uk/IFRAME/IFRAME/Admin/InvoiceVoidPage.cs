namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class InvoiceVoidPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected Label lblError;
        protected Literal ltrInvoiceNumber;
        protected Panel pnl;
        protected TextBox txtAdminComment;
        protected TextBox txtPassword;
        protected RequiredFieldValidator valAdminComment;
        protected ValidatorCalloutExtender valAdminCommentEx;
        protected RequiredFieldValidator valPassword;
        protected ValidatorCalloutExtender valPasswordEx;

        private void BindSalonInvoice(SalonInvoiceDB value)
        {
            this.ltrInvoiceNumber.Text = value.InvoiceNumber;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"inv"}={this.PostedSalonInvoiceId}";
            string uRL = IFRMHelper.GetURL("invoiceedit.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
                string str = this.txtPassword.Text.Trim();
                string str2 = IoC.Resolve<ISecurityManager>().DecryptUserPassword(workingUser.Password, IFRAME.Controllers.Settings.Security_Key_3DES);
                if (str != str2)
                {
                    this.lblError.Text = base.GetLocaleResourceString("lblError.Password.Text");
                    return;
                }
                SalonInvoiceDB salonInvoiceById = IoC.Resolve<IBillingManager>().GetSalonInvoiceById(this.PostedSalonInvoiceId);
                salonInvoiceById.Deleted = true;
                salonInvoiceById.AdminComment = this.txtAdminComment.Text.Trim();
                salonInvoiceById.UpdatedOn = DateTime.Now;
                salonInvoiceById.UpdatedBy = workingUser.Username;
                salonInvoiceById = IoC.Resolve<IBillingManager>().UpdateSalonInvoice(salonInvoiceById);
            }
            string str3 = $"{"inv"}={this.PostedSalonInvoiceId}";
            string uRL = IFRMHelper.GetURL("invoiceedit.aspx", new string[] { str3 });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("invoicesearch.aspx", new string[0]);
            if (this.PostedSalonInvoiceId == Guid.Empty)
            {
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonInvoiceDB salonInvoiceById = IoC.Resolve<IBillingManager>().GetSalonInvoiceById(this.PostedSalonInvoiceId);
                if (salonInvoiceById == null)
                {
                    base.Response.Redirect(uRL, true);
                }
                if (salonInvoiceById.Deleted)
                {
                    base.Response.Redirect(uRL, true);
                }
                this.BindSalonInvoice(salonInvoiceById);
            }
        }

        private Guid PostedSalonInvoiceId
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

