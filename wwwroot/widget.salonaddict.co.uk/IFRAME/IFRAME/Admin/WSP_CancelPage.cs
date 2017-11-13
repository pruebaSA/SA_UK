namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class WSP_CancelPage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnDelete;
        protected SalonMenu cntrlMenu;
        protected Label lblError;
        protected Literal ltrHeader;
        protected Literal ltrPeriod;
        protected Literal ltrPlanPrice;
        protected Literal ltrSalon;
        protected Panel pnl;
        protected TextBox txtPassword;
        protected RequiredFieldValidator valPassword;
        protected ValidatorCalloutExtender valPasswordEx;

        private void BindWSP(WSPDB value)
        {
            this.ltrHeader.Text = value.Description + " " + ((((double) value.PlanPrice) / 100.0)).ToString("C");
            this.ltrPeriod.Text = IFRMHelper.FromUrlFriendlyDate(value.PlanStartDate).ToString("dd MMM yyyy") + " - " + IFRMHelper.FromUrlFriendlyDate(value.PlanEndDate).ToString("dd MMM yyyy");
            this.ltrPlanPrice.Text = (((double) value.PlanPrice) / 100.0).ToString("C");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("wsp.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
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
                WSPDB wSPCurrent = IoC.Resolve<IBillingManager>().GetWSPCurrent(this.PostedSalonId);
                wSPCurrent.CancelDate = IFRMHelper.ToUrlFriendlyDate(DateTime.Today);
                wSPCurrent.UpdatedBy = workingUser.Username;
                wSPCurrent.UpdatedOn = DateTime.Now;
                wSPCurrent = IoC.Resolve<IBillingManager>().UpdateWSP(wSPCurrent);
                wSPCurrent = new WSPDB {
                    Active = true,
                    BillStartDate = IFRMHelper.ToUrlFriendlyDate(DateTime.Now),
                    BillEndDate = IFRMHelper.ToUrlFriendlyDate(DateTime.Now),
                    CreatedBy = "SYSTEM",
                    CreatedOn = DateTime.Now,
                    Description = "Trial Plan",
                    ExcessFeeWT = 0,
                    ExcessLimitWT = 0,
                    ParentId = null,
                    PlanEndDate = IFRMHelper.ToUrlFriendlyDate(DateTime.Now),
                    PlanPrice = 0,
                    PlanStartDate = IFRMHelper.ToUrlFriendlyDate(DateTime.Now),
                    PlanType = "10",
                    Prorate = false,
                    SalonId = this.PostedSalonId,
                    UpdatedBy = "SYSTEM",
                    UpdatedOn = DateTime.Now
                };
                wSPCurrent = IoC.Resolve<IBillingManager>().InsertWSP(wSPCurrent);
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

