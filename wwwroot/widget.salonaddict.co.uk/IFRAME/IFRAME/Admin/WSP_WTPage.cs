namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class WSP_WTPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected SalonMenu cntrlMenu;
        protected FilteredTextBoxExtender fltExcessFeeWT;
        protected Literal ltrHeader;
        protected Literal ltrSalon;
        protected Panel pnl;
        protected TextBox txtExcessFeeWT;
        protected RequiredFieldValidator valExcessFeeWT;
        protected ValidatorCalloutExtender valExcessFeeWTEx;
        protected RangeValidator valExcessFeeWTRange;
        protected ValidatorCalloutExtender valExcessFeeWTRangeEx;

        private void BindWSP(WSPDB value)
        {
            this.ltrHeader.Text = value.Description + " " + ((((double) value.PlanPrice) / 100.0)).ToString("C");
            this.txtExcessFeeWT.Text = (((double) value.ExcessFeeWT) / 100.0).ToString();
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
                wSPCurrent.ExcessFeeWT = (int) (decimal.Parse(this.txtExcessFeeWT.Text) * 100M);
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
                if (wSPCurrent.CancelDate != string.Empty)
                {
                    string str3 = $"{"sid"}={this.PostedSalonId}";
                    string str4 = IFRMHelper.GetURL("wsp.aspx", new string[] { str3 });
                    base.Response.Redirect(str4, true);
                }
                this.ltrSalon.Text = salonById.Name;
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

