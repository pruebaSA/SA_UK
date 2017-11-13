namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Web;
    using System.Web.UI.WebControls;

    public class QueuedMessage_DeletePage : IFRMAdminPage
    {
        protected Button btnCancel;
        protected Button btnDelete;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected Literal ltrCreatedOn;
        protected Literal ltrFrom;
        protected Literal ltrSendTries;
        protected Literal ltrSubject;
        protected Literal ltrTo;
        protected Panel pnl;

        private void BindMessageDetails(QueuedMessageDB message)
        {
            this.ltrFrom.Text = HttpUtility.HtmlEncode(message.Sender);
            this.ltrTo.Text = HttpUtility.HtmlEncode(message.Recipient);
            this.ltrSubject.Text = HttpUtility.HtmlEncode(message.Subject);
            this.ltrSendTries.Text = HttpUtility.HtmlEncode(message.SendTries.ToString());
            this.ltrCreatedOn.Text = HttpUtility.HtmlEncode(message.CreatedOn.ToString("dd MMM yyyy"));
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("reportnondelivery.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            QueuedMessageDB queuedMessageById = IoC.Resolve<IMessageManager>().GetQueuedMessageById(this.PostedMessageID);
            IoC.Resolve<IMessageManager>().DeleteQueuedMessage(queuedMessageById);
            string uRL = IFRMHelper.GetURL("reportnondelivery.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedMessageID == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("reportnondelivery.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                QueuedMessageDB queuedMessageById = IoC.Resolve<IMessageManager>().GetQueuedMessageById(this.PostedMessageID);
                if (queuedMessageById == null)
                {
                    string url = IFRMHelper.GetURL("reportnondelivery.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                this.BindMessageDetails(queuedMessageById);
            }
        }

        public Guid PostedMessageID
        {
            get
            {
                string str = base.Request.QueryString["mid"];
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

