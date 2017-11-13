namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.Rendering;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web.UI.WebControls;

    public class BillPay_MyBillPage : IFRMSecurePage
    {
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected DropDownList ddlBillingPeriod;
        protected GridView gv;
        protected LinkButton lbDownLoad;
        protected Literal ltrInvoiceDate;
        protected Literal ltrInvoiceNumber;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
            this.gv.Columns[3].HeaderText = base.GetLocaleResourceString("gv.Columns[3].HeaderText");
            this.gv.Columns[4].HeaderText = base.GetLocaleResourceString("gv.Columns[4].HeaderText");
        }

        private void BindBillingPeriodsList()
        {
            SalonDB salon = IFRMContext.Current.Salon;
            List<BillingPeriodListItemDB> salonInvoiceBillingPeriods = IoC.Resolve<IBillingManager>().GetSalonInvoiceBillingPeriods(salon.SalonId, 6);
            ListItemCollection datasource = new ListItemCollection();
            salonInvoiceBillingPeriods.ForEach(delegate (BillingPeriodListItemDB item) {
                ListItem item2 = new ListItem(IFRMHelper.FromUrlFriendlyDate(item.BillEndDate).ToString("dd-MMMM-yyyy"), item.InvoiceId.ToString());
                datasource.Add(item2);
            });
            this.ddlBillingPeriod.DataSource = datasource;
            this.ddlBillingPeriod.DataTextField = "Text";
            this.ddlBillingPeriod.DataValueField = "Value";
            this.ddlBillingPeriod.DataBind();
        }

        private void BindBillSummary()
        {
            string selectedValue = this.ddlBillingPeriod.SelectedValue;
            if (string.IsNullOrEmpty(selectedValue))
            {
                this.gv.Visible = false;
                this.lbDownLoad.Enabled = false;
            }
            else
            {
                Guid guid = new Guid(selectedValue);
                SalonDB salon = IFRMContext.Current.Salon;
                SalonInvoiceDB edb = null;
                SalonInvoiceDB salonInvoiceById = IoC.Resolve<IBillingManager>().GetSalonInvoiceById(guid);
                if (salonInvoiceById.ParentId.HasValue)
                {
                    edb = IoC.Resolve<IBillingManager>().GetSalonInvoiceById(salonInvoiceById.ParentId.Value);
                }
                if (salonInvoiceById.SalonId != salon.SalonId)
                {
                    this.gv.Visible = false;
                }
                else if ((edb != null) && (edb.SalonId != salon.SalonId))
                {
                    this.gv.Visible = false;
                }
                else
                {
                    string str2 = 0.0.ToString("C");
                    string str3 = 0.0.ToString("C");
                    string str4 = 0.0.ToString("C");
                    string str5 = 0.0.ToString("C");
                    string str6 = 0.0.ToString("C");
                    this.ltrInvoiceDate.Text = salonInvoiceById.CreatedOn.ToString("dd MMM yyyy");
                    this.ltrInvoiceNumber.Text = salonInvoiceById.InvoiceNumber;
                    str4 = (((double) salonInvoiceById.TotalOverdue) / 100.0).ToString("C");
                    str5 = (((double) salonInvoiceById.TotalInclTax) / 100.0).ToString("C");
                    str6 = (((double) salonInvoiceById.TotalAmountDue) / 100.0).ToString("C");
                    if (edb != null)
                    {
                        str2 = (((double) edb.TotalAmountDue) / 100.0).ToString("C");
                        str3 = (((double) edb.TotalPaid) / 100.0).ToString("C");
                    }
                    List<object> list = new List<object> {
                        new { 
                            PreviousBalance = str2,
                            PaymentReceived = str3,
                            OverdueAmount = str4,
                            TotalCharges = str5,
                            TotalAmountDue = str6
                        }
                    };
                    this.gv.DataSource = list;
                    this.gv.DataBind();
                }
            }
        }

        protected void ddlBillingPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.BindBillSummary();
        }

        protected void lbDownload_Click(object sender, EventArgs e)
        {
            string selectedValue = this.ddlBillingPeriod.SelectedValue;
            if (!string.IsNullOrEmpty(selectedValue))
            {
                Guid guid = new Guid(selectedValue);
                SalonDB salon = IFRMContext.Current.Salon;
                SalonInvoiceDB salonInvoiceById = IoC.Resolve<IBillingManager>().GetSalonInvoiceById(guid);
                SalonInvoiceDB salonInvoiceByParentId = null;
                if (salonInvoiceById.ParentId.HasValue)
                {
                    salonInvoiceByParentId = IoC.Resolve<IBillingManager>().GetSalonInvoiceByParentId(salonInvoiceById.ParentId.Value);
                }
                int prevamountdue = 0;
                int paymentreceived = 0;
                if (salonInvoiceByParentId != null)
                {
                    prevamountdue = salonInvoiceByParentId.TotalAmountDue;
                    paymentreceived = salonInvoiceByParentId.TotalPaid;
                }
                int totalWidgetCount = salonInvoiceById.TotalWidgetCount;
                List<SalonInvoiceWTDB> transactions = IoC.Resolve<IBillingManager>().GetSalonInvoiceWTByInvoiceId(salonInvoiceById.InvoiceId, 0, salonInvoiceById.TotalWidgetCount, out totalWidgetCount);
                List<SalonInvoiceADDB> salonInvoiceADByInvoiceId = IoC.Resolve<IBillingManager>().GetSalonInvoiceADByInvoiceId(salonInvoiceById.InvoiceId);
                Document document = IFRMInvoiceGenerator.GenerateInvoice(salonInvoiceById.BillingCompany, salonInvoiceById.BillingAddressLine1, salonInvoiceById.BillingAddressLine2 + ", " + salonInvoiceById.BillingAddressLine4, salonInvoiceById.BillingAddressLine5, IFRMHelper.FromUrlFriendlyDate(salonInvoiceById.BillEndDate), IFRMHelper.FromUrlFriendlyDate(salonInvoiceById.BillStartDate), (salonInvoiceById.BillingPhoneType == "MOBILE") ? salonInvoiceById.BillingMobile : salonInvoiceById.BillingPhoneNumber, transactions, salonInvoiceADByInvoiceId, salonInvoiceById.CreatedOn, salonInvoiceById.InvoiceNumber, IFRMHelper.FromUrlFriendlyDate(salonInvoiceById.PaymentDueDate), paymentreceived, salonInvoiceById.PlanDescription, IFRMHelper.FromUrlFriendlyDate(salonInvoiceById.PlanEndDate), salonInvoiceById.PlanFee, prevamountdue, salonInvoiceById.BillingCompany, salonInvoiceById.SubtotalExclTax, decimal.Parse(salonInvoiceById.TaxRate), salonInvoiceById.TotalAmountDue, salonInvoiceById.TotalAdjustment, salonInvoiceById.TotalExclTax, salonInvoiceById.TotalInclTax, salonInvoiceById.TotalOverdue, salonInvoiceById.TotalPlan, salonInvoiceById.TotalTax, salonInvoiceById.TotalWidgetCount, salonInvoiceById.TotalWidget, salonInvoiceById.VATNumber, salonInvoiceById.Deleted);
                PdfDocumentRenderer renderer = new PdfDocumentRenderer(true) {
                    Document = document
                };
                renderer.RenderDocument();
                using (MemoryStream stream = new MemoryStream())
                {
                    renderer.Save(stream, false);
                    base.Response.Clear();
                    base.Response.ContentType = "application/pdf";
                    base.Response.AddHeader("content-length", stream.Length.ToString());
                    base.Response.AppendHeader("Content-Disposition", $"attachment; filename=INV_{IFRMHelper.FromUrlFriendlyDate(salonInvoiceById.BillEndDate).ToString("dd-MMM-yyyy")}.pdf");
                    base.Response.BinaryWrite(stream.ToArray());
                    base.Response.Flush();
                    stream.Close();
                    base.Response.End();
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindBillingPeriodsList();
                this.BindBillSummary();
            }
        }
    }
}

