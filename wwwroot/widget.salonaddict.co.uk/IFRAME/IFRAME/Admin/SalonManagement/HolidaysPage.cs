namespace IFRAME.Admin.SalonManagement
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class HolidaysPage : IFRMAdminPage
    {
        protected Button btnAdd;
        protected SalonMenu cntlMenu;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
        }

        private void BindHolidayDetails(SalonDB salon)
        {
            List<ClosingDayDB> closingDaysBySalonId = IoC.Resolve<ISalonManager>().GetClosingDaysBySalonId(salon.SalonId);
            this.gv.DataSource = closingDaysBySalonId;
            this.gv.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("holiday-create.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.RowIndex].Value.ToString());
            string str = $"{"sid"}={this.PostedSalonId}";
            string str2 = $"{"hid"}={guid}";
            string uRL = IFRMHelper.GetURL("holiday-delete.aspx", new string[] { str, str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string url = IFRMHelper.GetURL("~/admin/default.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                this.BindHolidayDetails(salonById);
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

