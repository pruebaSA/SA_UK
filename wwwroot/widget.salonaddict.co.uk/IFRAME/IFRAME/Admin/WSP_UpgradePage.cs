namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Globalization;
    using System.Web.UI.WebControls;

    public class WSP_UpgradePage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected SalonMenu cntrlMenu;
        protected DropDownList ddlDay;
        protected DropDownList ddlMonth;
        protected DropDownList ddlPlanType;
        protected DropDownList ddlYear;
        protected FilteredTextBoxExtender fltExcessFeeWT;
        protected FilteredTextBoxExtender ftrPrice;
        protected Label lblError;
        protected Literal ltrSalon;
        protected Panel pnl;
        protected TextBox txtDescription;
        protected TextBox txtExcessFeeWT;
        protected TextBox txtPrice;
        protected RequiredFieldValidator valDescription;
        protected ValidatorCalloutExtender valDescriptionEx;
        protected RequiredFieldValidator valExcessFeeWT;
        protected ValidatorCalloutExtender valExcessFeeWTEx;
        protected RangeValidator valExcessFeeWTRange;
        protected ValidatorCalloutExtender valExcessFeeWTRangeEx;
        protected RequiredFieldValidator valPrice;
        protected ValidatorCalloutExtender valPriceEx;
        protected RangeValidator valPriceRange;
        protected ValidatorCalloutExtender valPriceRangeEx;

        private void BindActiveDateDropDownLists()
        {
            ListItemCollection items = new ListItemCollection();
            for (int i = 1; i <= 0x1c; i++)
            {
                string text = i.ToString("00");
                ListItem item = new ListItem(text, i.ToString("00"));
                items.Add(item);
            }
            this.ddlDay.DataSource = items;
            this.ddlDay.DataTextField = "Text";
            this.ddlDay.DataValueField = "Value";
            this.ddlDay.DataBind();
            items = new ListItemCollection();
            DateTime today = new DateTime(DateTime.Today.Year, 1, 1);
            for (int j = 1; j <= 12; j++)
            {
                string introduced16 = today.ToString("MMM");
                ListItem item2 = new ListItem(introduced16, today.Month.ToString("00"));
                items.Add(item2);
                today = today.AddMonths(1);
            }
            this.ddlMonth.DataSource = items;
            this.ddlMonth.DataTextField = "Text";
            this.ddlMonth.DataValueField = "Value";
            this.ddlMonth.DataBind();
            today = DateTime.Today;
            items = new ListItemCollection();
            for (int k = 0; k < 2; k++)
            {
                string introduced17 = today.Year.ToString("0000");
                ListItem item3 = new ListItem(introduced17, today.Year.ToString("0000"));
                items.Add(item3);
                today = today.AddYears(1);
            }
            this.ddlYear.DataSource = items;
            this.ddlYear.DataTextField = "Text";
            this.ddlYear.DataValueField = "Value";
            this.ddlYear.DataBind();
            today = DateTime.Today;
            this.ddlDay.SelectedValue = today.Day.ToString("00");
            this.ddlMonth.SelectedValue = today.Month.ToString("00");
            this.ddlYear.SelectedValue = today.Year.ToString("0000");
        }

        private void BindSalonDetails(SalonDB value)
        {
            this.ltrSalon.Text = value.Name;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("wsp.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                string selectedValue = this.ddlPlanType.SelectedValue;
                int num = (int) (decimal.Parse(this.txtExcessFeeWT.Text.Trim()) * 100M);
                string str2 = this.txtDescription.Text.Trim();
                int num2 = (int) (decimal.Parse(this.txtPrice.Text.Trim()) * 100M);
                DateTime time = DateTime.ParseExact(this.ddlDay.SelectedValue + "/" + this.ddlMonth.SelectedValue + "/" + this.ddlYear.SelectedValue, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime time2 = time;
                if (selectedValue == "100")
                {
                    time2 = time.AddYears(1).AddDays(-1.0);
                }
                else
                {
                    time2 = time.AddMonths(1).AddDays(-1.0);
                }
                DateTime time3 = time;
                DateTime time4 = time.AddMonths(1).AddDays(-1.0);
                if (time.Day > 0x1c)
                {
                    this.lblError.Text = base.GetLocaleResourceString("Error.Date.Invalid");
                    return;
                }
                if (time < DateTime.Today)
                {
                    this.lblError.Text = base.GetLocaleResourceString("Error.Date.Future");
                    return;
                }
                WSPDB wSPCurrent = IoC.Resolve<IBillingManager>().GetWSPCurrent(this.PostedSalonId);
                if (time < IFRMHelper.FromUrlFriendlyDate(wSPCurrent.PlanStartDate))
                {
                    this.lblError.Text = base.GetLocaleResourceString("Error.Date.InvalidStartDate");
                    return;
                }
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (IoC.Resolve<IBillingManager>().GetSalonInvoiceUnpaidCount(salonById.SalonId, null, "GBP") > 0)
                {
                    this.lblError.Text = base.GetLocaleResourceString("Error.UpgradeNotElligible");
                    return;
                }
                SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
                int num4 = 0;
                int num5 = (num4 + IoC.Resolve<IBillingManager>().GetSalonInvoiceCount()) + 1;
                WSPUpgradeXML input = new WSPUpgradeXML {
                    BillEndDate = IFRMHelper.ToUrlFriendlyDate(time4),
                    BillingAddressLine1 = salonById.AddressLine1,
                    BillingAddressLine2 = salonById.AddressLine2,
                    BillingAddressLine3 = salonById.AddressLine3,
                    BillingAddressLine4 = salonById.AddressLine4,
                    BillingAddressLine5 = salonById.AddressLine5,
                    BillingCompany = salonById.Name,
                    BillingEmail = salonById.Email,
                    BillingFirstName = string.Empty,
                    BillingLastName = string.Empty,
                    BillingMobile = string.Empty,
                    BillingPhoneNumber = salonById.PhoneNumber,
                    BillingPhoneNumberType = string.Empty,
                    BillStartDate = IFRMHelper.ToUrlFriendlyDate(time3),
                    CreatedBy = workingUser.Username,
                    CreatedOn = DateTime.Now,
                    CurrencyCode = "GBP",
                    ExcessFeeWT = num,
                    ExcessLimitWT = 0,
                    InvoiceNumber = $"SLN{time4.ToString("yyyyMMdd")}-{num5 + 1}",
                    InvoiceType = "U",
                    PaymentDueDate = IFRMHelper.ToUrlFriendlyDate(DateTime.Today.AddDays(15.0)),
                    PlanDescription = str2,
                    PlanEndDate = IFRMHelper.ToUrlFriendlyDate(time2),
                    PlanFee = num2,
                    PlanParentId = new Guid?(wSPCurrent.PlanId),
                    PlanStartDate = IFRMHelper.ToUrlFriendlyDate(time),
                    PlanType = selectedValue,
                    SalonId = salonById.SalonId,
                    Status = string.Empty,
                    TotalPlan = num2
                };
                input.SubtotalExclTax = input.TotalPlan;
                input.TaxRate = "0.2000";
                input.TotalExclTax = input.SubtotalExclTax;
                input.TotalOverdue = 0;
                input.TotalTax = (int) Math.Round((double) (input.TotalExclTax * double.Parse(input.TaxRate)), 0, MidpointRounding.AwayFromZero);
                input.VATNumber = "";
                input.TotalInclTax = input.TotalExclTax + input.TotalTax;
                input.TotalAmountDue = input.TotalInclTax + input.TotalOverdue;
                IoC.Resolve<IBillingManager>().UpgradeCurrentWSP(input);
            }
            string str3 = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("wsp.aspx", new string[] { str3 });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("salons.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string url = IFRMHelper.GetURL("salons.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                WSPDB wSPCurrent = IoC.Resolve<IBillingManager>().GetWSPCurrent(salonById.SalonId);
                if (wSPCurrent.CancelDate != string.Empty)
                {
                    string str3 = $"{"sid"}={this.PostedSalonId}";
                    string str4 = IFRMHelper.GetURL("wsp.aspx", new string[] { str3 });
                    base.Response.Redirect(str4, true);
                }
                this.ddlPlanType.SelectedValue = wSPCurrent.PlanType;
                this.txtDescription.Text = wSPCurrent.Description;
                this.txtPrice.Text = (((double) wSPCurrent.PlanPrice) / 100.0).ToString();
                this.txtExcessFeeWT.Text = (((double) wSPCurrent.ExcessFeeWT) / 100.0).ToString();
                this.BindActiveDateDropDownLists();
                this.BindSalonDetails(salonById);
            }
        }

        public Guid PostedSalonId
        {
            get
            {
                string str = base.Request.QueryString["sid"];
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

