namespace IFRAME
{
    using IFRAME.Controllers;
    using IFRAME.Modules;
    using SA.BAL;
    using System;
    using System.Web.UI.WebControls;

    public class DefaultPage : IFRMPage
    {
        protected Button btnSubmit;
        protected HomepageSalonHours cntrlHours;
        protected ServiceDropDownList ddlService;
        protected Literal ltrAddress;
        protected Literal ltrHeader;
        protected Panel pnl;
        protected DateTextBox txtDate;

        private void BindSalonDetails()
        {
            SalonDB salon = IFRMContext.Current.Salon;
            this.ltrHeader.Text = string.Format(base.GetLocaleResourceString("ltrHeader.Text"), salon.Name);
            this.ltrAddress.Text = string.Format(base.GetLocaleResourceString("ltrAddress.Text"), IFRMHelper.GetAddressFriendlyString(salon));
        }

        private void BindServiceList()
        {
            this.ddlService.DataBind();
            this.ddlService.ServiceId = this.PostedServiceId;
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                Guid serviceId = this.ddlService.ServiceId;
                DateTime date = this.txtDate.Date;
                string str = $"{"svid"}={serviceId}";
                string str2 = $"{"date"}={IFRMHelper.ToUrlFriendlyDate(date)}";
                string uRL = IFRMHelper.GetURL("search.aspx", new string[] { str, str2 });
                base.Response.Redirect(uRL, true);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        protected override void OnInitComplete(EventArgs e)
        {
            base.OnInitComplete(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.BindSalonDetails();
                this.BindServiceList();
            }
        }

        public Guid PostedServiceId
        {
            get
            {
                string str = base.Request.QueryString["svid"];
                if (string.IsNullOrEmpty(str))
                {
                    return Guid.Empty;
                }
                str = str.Trim();
                try
                {
                    return new Guid(str);
                }
                catch (Exception)
                {
                    return Guid.Empty;
                }
            }
        }
    }
}

