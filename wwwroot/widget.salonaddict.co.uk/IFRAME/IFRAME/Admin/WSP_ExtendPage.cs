namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Globalization;
    using System.Web.UI.WebControls;

    public class WSP_ExtendPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected SalonMenu cntrlMenu;
        protected DropDownList ddlDay;
        protected DropDownList ddlMonth;
        protected DropDownList ddlYear;
        protected Label lblError;
        protected Literal ltrSalon;
        protected Panel pnl;

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
                string introduced13 = today.ToString("MMM");
                ListItem item2 = new ListItem(introduced13, today.Month.ToString("00"));
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
                string introduced14 = today.Year.ToString("0000");
                ListItem item3 = new ListItem(introduced14, today.Year.ToString("0000"));
                items.Add(item3);
                today = today.AddYears(1);
            }
            this.ddlYear.DataSource = items;
            this.ddlYear.DataTextField = "Text";
            this.ddlYear.DataValueField = "Value";
            this.ddlYear.DataBind();
        }

        private void BindWSP(WSPDB value)
        {
            DateTime time = IFRMHelper.FromUrlFriendlyDate(value.PlanEndDate);
            this.ddlDay.SelectedValue = time.Day.ToString("00");
            this.ddlMonth.SelectedValue = time.Month.ToString("00");
            this.ddlYear.SelectedValue = time.Year.ToString("0000");
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
                SalonUserDB workingUser = IFRMContext.Current.WorkingUser;
                WSPDB wSPCurrent = IoC.Resolve<IBillingManager>().GetWSPCurrent(this.PostedSalonId);
                DateTime time = DateTime.ParseExact(this.ddlDay.SelectedValue + "/" + this.ddlMonth.SelectedValue + "/" + this.ddlYear.SelectedValue, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (time <= DateTime.Today)
                {
                    this.lblError.Text = base.GetLocaleResourceString("Error.Date.InvalidEndDate");
                    return;
                }
                wSPCurrent.PlanEndDate = IFRMHelper.ToUrlFriendlyDate(time);
                wSPCurrent.UpdatedBy = workingUser.Username;
                wSPCurrent.UpdatedOn = DateTime.Now;
                wSPCurrent = IoC.Resolve<IBillingManager>().UpdateWSP(wSPCurrent);
            }
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("wsp.aspx", new string[] { str });
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
                if ((wSPCurrent.PlanType != "10") || (wSPCurrent.CancelDate != string.Empty))
                {
                    string str3 = $"{"sid"}={this.PostedSalonId}";
                    string str4 = IFRMHelper.GetURL("wsp.aspx", new string[] { str3 });
                    base.Response.Redirect(str4, true);
                }
                this.ltrSalon.Text = salonById.Name;
                this.BindActiveDateDropDownLists();
                this.BindWSP(wSPCurrent);
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

