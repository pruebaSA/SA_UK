namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class HolidaysPage : IFRMSecurePage
    {
        protected Button btnAdd;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
        }

        private void BindHolidayDetails()
        {
            SalonDB salon = IFRMContext.Current.Salon;
            List<ClosingDayDB> closingDaysBySalonId = IoC.Resolve<ISalonManager>().GetClosingDaysBySalonId(salon.SalonId);
            this.gv.DataSource = closingDaysBySalonId;
            this.gv.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("holiday-create.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.RowIndex].Value.ToString());
            string str = $"{"hid"}={guid}";
            string uRL = IFRMHelper.GetURL("holiday-delete.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindHolidayDetails();
            }
        }
    }
}

