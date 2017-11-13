namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class InvoiceAdjustment_DeletePage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnDelete;
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected Label lblError;
        protected Literal ltrDescription;
        protected Literal ltrValue;
        protected Panel pnl;
        protected TextBox txtPassword;
        protected RequiredFieldValidator valPassword;
        protected ValidatorCalloutExtender valPasswordEx;

        private void BindAdjustment(SalonInvoiceADDB value)
        {
            this.ltrDescription.Text = value.Description;
            this.ltrValue.Text = (((double) value.Amount) / 100.0).ToString("C");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            SalonInvoiceADDB salonInvoiceADById = IoC.Resolve<IBillingManager>().GetSalonInvoiceADById(this.PostedAdjustmentId);
            SalonInvoiceDB salonInvoiceById = IoC.Resolve<IBillingManager>().GetSalonInvoiceById(salonInvoiceADById.InvoiceId);
            string str = $"{"inv"}={salonInvoiceById.InvoiceId}";
            string uRL = IFRMHelper.GetURL("invoiceadjustments.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            SalonInvoiceADDB salonInvoiceADById = IoC.Resolve<IBillingManager>().GetSalonInvoiceADById(this.PostedAdjustmentId);
            SalonInvoiceDB salonInvoiceById = IoC.Resolve<IBillingManager>().GetSalonInvoiceById(salonInvoiceADById.InvoiceId);
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
                if ((salonInvoiceById.TotalAdjustment - salonInvoiceADById.Amount) < 0)
                {
                    this.lblError.Text = base.GetLocaleResourceString("lblError.Text");
                }
                IoC.Resolve<IBillingManager>().DeleteSalonInvoiceAD(salonInvoiceADById);
                salonInvoiceById.TotalAdjustment -= salonInvoiceADById.Amount;
                salonInvoiceById.TotalExclTax = salonInvoiceById.SubtotalExclTax + salonInvoiceById.TotalAdjustment;
                salonInvoiceById.TotalTax = 0;
                if (salonInvoiceById.TotalExclTax > 0)
                {
                    salonInvoiceById.TotalTax = (int) Math.Round((double) (salonInvoiceById.TotalExclTax * double.Parse(salonInvoiceById.TaxRate)), 0, MidpointRounding.AwayFromZero);
                }
                salonInvoiceById.TotalInclTax = salonInvoiceById.TotalExclTax + salonInvoiceById.TotalTax;
                salonInvoiceById.TotalAmountDue = salonInvoiceById.TotalInclTax + salonInvoiceById.TotalOverdue;
                try
                {
                    salonInvoiceById = IoC.Resolve<IBillingManager>().UpdateSalonInvoice(salonInvoiceById);
                }
                catch (Exception exception)
                {
                    IoC.Resolve<IBillingManager>().InsertSalonInvoiceAD(salonInvoiceADById);
                    throw exception;
                }
            }
            string str3 = $"{"inv"}={salonInvoiceById.InvoiceId}";
            string uRL = IFRMHelper.GetURL("invoicedetails.aspx", new string[] { str3 });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("invoicesissued.aspx", new string[0]);
            if (this.PostedAdjustmentId == Guid.Empty)
            {
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonInvoiceADDB salonInvoiceADById = IoC.Resolve<IBillingManager>().GetSalonInvoiceADById(this.PostedAdjustmentId);
                if (salonInvoiceADById == null)
                {
                    base.Response.Redirect(uRL, true);
                }
                SalonInvoiceDB salonInvoiceById = IoC.Resolve<IBillingManager>().GetSalonInvoiceById(salonInvoiceADById.InvoiceId);
                if (salonInvoiceById == null)
                {
                    base.Response.Redirect(uRL, true);
                }
                if (salonInvoiceById.PaidOn.HasValue || salonInvoiceById.Deleted)
                {
                    base.Response.Redirect(uRL, true);
                }
                this.BindAdjustment(salonInvoiceADById);
            }
        }

        private Guid PostedAdjustmentId
        {
            get
            {
                string str = base.Request.QueryString["aid"];
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

