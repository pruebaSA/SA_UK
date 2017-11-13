namespace IFRAME.Admin.SalonManagement
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class Service_DeletePage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnDelete;
        protected CheckBox cbActive;
        protected SalonMenu cntlMenu;
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
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("services.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
            Guid postedServiceId = this.PostedServiceId;
            ServiceDB serviceById = IoC.Resolve<IServiceManager>().GetServiceById(postedServiceId);
            serviceById.Deleted = true;
            serviceById = IoC.Resolve<IServiceManager>().UpdateService(serviceById);
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("services.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                if (this.PostedServiceId == Guid.Empty)
                {
                    string uRL = IFRMHelper.GetURL("services.aspx", new string[0]);
                    base.Response.Redirect(uRL, true);
                }
                if (this.PostedSalonId == Guid.Empty)
                {
                    string url = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                if (!this.Page.IsPostBack)
                {
                    if (IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId) == null)
                    {
                        string str3 = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                        base.Response.Redirect(str3, true);
                    }
                    Guid postedServiceId = this.PostedServiceId;
                    ServiceDB serviceById = IoC.Resolve<IServiceManager>().GetServiceById(this.PostedServiceId);
                    if ((serviceById == null) || (serviceById.SalonId != this.PostedSalonId))
                    {
                        string str4 = $"{"sid"}={this.PostedSalonId}";
                        string str5 = IFRMHelper.GetURL("services.aspx", new string[] { str4 });
                        base.Response.Redirect(str5, true);
                    }
                    this.BindServiceDetails(serviceById);
                }
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

