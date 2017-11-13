namespace IFRAME.Admin
{
    using AjaxControlToolkit;
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web;
    using System.Web.UI.WebControls;

    public class LogDetailsPage : IFRMAdminPage
    {
        protected Button btnDelete;
        protected IFRAME.Admin.Modules.Menu cntlMenu;
        protected Literal ltrCreatedOn;
        protected Literal ltrException;
        protected Literal ltrLogType;
        protected Literal ltrMessage;
        protected Literal ltrPageURL;
        protected Literal ltrParameters;
        protected Literal ltrReferrerURL;
        protected Literal ltrUser;
        protected Literal ltrUserHostAddress;
        protected Panel pnl;
        protected Panel pnlParamsContent;
        protected Panel pnlParamsHeader;
        protected BalloonPopupExtender pnlPopout;

        private void BindLogDetails(LogDB value)
        {
            this.ltrCreatedOn.Text = value.CreatedOn.ToString();
            this.ltrException.Text = HttpUtility.HtmlEncode(value.Exception);
            this.ltrLogType.Text = value.LogType;
            this.ltrMessage.Text = HttpUtility.HtmlEncode(value.Message);
            this.ltrPageURL.Text = HttpUtility.HtmlEncode(value.PageURL);
            this.ltrReferrerURL.Text = HttpUtility.HtmlEncode(value.ReferrerURL);
            if (value.UserId.HasValue)
            {
                SalonUserDB salonUserById = IoC.Resolve<IUserManager>().GetSalonUserById(value.UserId.Value);
                if (salonUserById != null)
                {
                    this.ltrUser.Text = $"{(salonUserById.FirstName + " " + salonUserById.LastName).Trim()} ({salonUserById.Username})";
                }
            }
            this.ltrUserHostAddress.Text = value.UserHostAddress;
            this.ltrParameters.Text = value.Params;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            LogDB errorById = IoC.Resolve<ILogManager>().GetErrorById(this.PostedLogId);
            IoC.Resolve<ILogManager>().DeleteLog(errorById);
            string uRL = IFRMHelper.GetURL("audit.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedLogId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("audit.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                LogDB errorById = IoC.Resolve<ILogManager>().GetErrorById(this.PostedLogId);
                if (errorById == null)
                {
                    string url = IFRMHelper.GetURL("audit.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                this.BindLogDetails(errorById);
            }
        }

        public Guid PostedLogId
        {
            get
            {
                string str = base.Request.QueryString["lid"];
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

