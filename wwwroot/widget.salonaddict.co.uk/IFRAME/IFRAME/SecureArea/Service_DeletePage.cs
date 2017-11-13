namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class Service_DeletePage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnDelete;
        protected CheckBox cbActive;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected Literal ltrCategory;
        protected Literal ltrDescription;
        protected Literal ltrDuration;
        protected Literal ltrName;
        protected Literal ltrPrice;
        protected Panel pnl;

        private void BindServiceDetails(ServiceDB value)
        {
            Func<CategoryDB, bool> predicate = null;
            this.ltrName.Text = value.Name;
            if (value.CategoryId1.HasValue)
            {
                if (predicate == null)
                {
                    predicate = item => item.CategoryId == value.CategoryId1;
                }
                CategoryDB ydb = IoC.Resolve<IServiceManager>().GetCategories().FirstOrDefault<CategoryDB>(predicate);
                if (ydb != null)
                {
                    this.ltrCategory.Text = ydb.Name;
                }
            }
            this.ltrPrice.Text = value.Price.ToString("C");
            this.ltrDescription.Text = value.ShortDescription;
            this.ltrDuration.Text = value.Duration.ToString();
            this.cbActive.Checked = value.Active;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("services.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            Guid postedServiceId = this.PostedServiceId;
            ServiceDB serviceById = IoC.Resolve<IServiceManager>().GetServiceById(postedServiceId);
            serviceById.Deleted = true;
            serviceById = IoC.Resolve<IServiceManager>().UpdateService(serviceById);
            string uRL = IFRMHelper.GetURL("services.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected override void OnPreInit(EventArgs e)
        {
            if (this.PostedServiceId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("services.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            base.OnPreInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                Guid postedServiceId = this.PostedServiceId;
                SalonDB salon = IFRMContext.Current.Salon;
                ServiceDB serviceById = IoC.Resolve<IServiceManager>().GetServiceById(this.PostedServiceId);
                if ((serviceById == null) || (serviceById.SalonId != salon.SalonId))
                {
                    string uRL = IFRMHelper.GetURL("services.aspx", new string[0]);
                    base.Response.Redirect(uRL, true);
                }
                this.BindServiceDetails(serviceById);
            }
        }

        public Guid PostedServiceId
        {
            get
            {
                string str = base.Request.QueryString["svid"];
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

