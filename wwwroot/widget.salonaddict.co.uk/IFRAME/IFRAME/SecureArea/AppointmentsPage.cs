namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class AppointmentsPage : IFRMSecurePage
    {
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected GridView gv;
        protected Panel pnl;
        protected System.Web.UI.Timer Timer;
        protected UpdatePanel up;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
            this.gv.Columns[3].HeaderText = base.GetLocaleResourceString("gv.Columns[3].HeaderText");
            this.gv.Columns[4].HeaderText = base.GetLocaleResourceString("gv.Columns[4].HeaderText");
            this.gv.Columns[5].HeaderText = base.GetLocaleResourceString("gv.Columns[5].HeaderText");
            this.gv.Columns[6].HeaderText = base.GetLocaleResourceString("gv.Columns[6].HeaderText");
        }

        private void BindReport(SalonDB salon)
        {
            List<OpenTicketDB> openTicketsBySalonId = IoC.Resolve<ITicketManager>().GetOpenTicketsBySalonId(salon.SalonId);
            this.gv.DataSource = openTicketsBySalonId;
            this.gv.DataBind();
        }

        protected void gv_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Image image = (Image) e.Row.FindControl("imgMail");
                Label label = (Label) e.Row.FindControl("lblClient");
                Label label2 = (Label) e.Row.FindControl("lblService");
                Label label3 = (Label) e.Row.FindControl("lblDate");
                Label label4 = (Label) e.Row.FindControl("lblTime");
                Label label5 = (Label) e.Row.FindControl("lblTotal");
                HyperLink link = (HyperLink) e.Row.FindControl("hlDetails");
                image.ImageUrl = "~/App_Themes/" + base.Theme + "/images/ico-mail_unread.png";
                label.Font.Bold = true;
                label2.Font.Bold = true;
                label3.Font.Bold = true;
                label4.Font.Bold = true;
                label5.Font.Bold = true;
                link.Font.Bold = true;
                OpenTicketDB dataItem = (OpenTicketDB) e.Row.DataItem;
                if (dataItem.OpenedOnUtc.HasValue)
                {
                    image.ImageUrl = "~/App_Themes/" + base.Theme + "/images/ico-mail_read.png";
                    label.Font.Bold = false;
                    label2.Font.Bold = false;
                    label3.Font.Bold = false;
                    label4.Font.Bold = false;
                    label5.Font.Bold = false;
                    link.Font.Bold = false;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            this.Timer.Interval = IFRAME.Controllers.Settings.IAPPOINTMENT_MANAGER_APPOINTMENT_REFRESH_RATE;
            if (!this.Page.IsPostBack)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                this.BindReport(salon);
            }
        }

        protected void Timer_Tick(object sender, EventArgs e)
        {
            SalonDB salon = IFRMContext.Current.Salon;
            this.BindReport(salon);
        }

        public int PostedPageIndex
        {
            get
            {
                string str = base.Request.QueryString["page_index"];
                if (!string.IsNullOrEmpty(str))
                {
                    try
                    {
                        int num = int.Parse(str);
                        if (num > 0)
                        {
                            return num;
                        }
                    }
                    catch
                    {
                    }
                }
                return 1;
            }
        }
    }
}

