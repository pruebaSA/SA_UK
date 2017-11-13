namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class InvoiceAdjustment_CreatePage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected FilteredTextBoxExtender ftbeValue;
        protected Label lblError;
        protected Panel pnl;
        protected RequiredFieldValidator rfvValue;
        protected ValidatorCalloutExtender rfvValueE;
        protected RangeValidator rvValue;
        protected ValidatorCalloutExtender rvValueE;
        protected TextBox txtDescription;
        protected TextBox txtPassword;
        protected TextBox txtValue;
        protected RequiredFieldValidator valDescription;
        protected ValidatorCalloutExtender valDescriptionEx;
        protected RequiredFieldValidator valPassword;
        protected ValidatorCalloutExtender valPasswordEx;

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"inv"}={this.PostedSalonInvoiceId}";
            string uRL = IFRMHelper.GetURL("invoicedetails.aspx", new string[] { str });
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
                int num = (int) (decimal.Parse(this.txtValue.Text) * 100M);
                string str3 = this.txtDescription.Text.Trim();
                SalonInvoiceDB salonInvoiceById = IoC.Resolve<IBillingManager>().GetSalonInvoiceById(this.PostedSalonInvoiceId);
                SalonInvoiceADDB eaddb = new SalonInvoiceADDB {
                    Amount = num,
                    CreatedBy = workingUser.Username,
                    CreatedOn = DateTime.Now,
                    Description = str3,
                    InvoiceId = this.PostedSalonInvoiceId
                };
                eaddb = IoC.Resolve<IBillingManager>().InsertSalonInvoiceAD(eaddb);
                salonInvoiceById.TotalAdjustment += eaddb.Amount;
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
                    IoC.Resolve<IBillingManager>().DeleteSalonInvoiceAD(eaddb);
                    throw exception;
                }
            }
            string str4 = $"{"inv"}={this.PostedSalonInvoiceId}";
            string uRL = IFRMHelper.GetURL("invoicedetails.aspx", new string[] { str4 });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("invoicesissued.aspx", new string[0]);
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
                if (salonInvoiceById.PaidOn.HasValue || salonInvoiceById.Deleted)
                {
                    base.Response.Redirect(uRL, true);
                }
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

