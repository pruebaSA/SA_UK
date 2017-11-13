namespace IFRAME.SecureArea
{
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public class WSP_HistoryPage : IFRMSecurePage
    {
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
        }

        private void BindWSPList(SalonDB salon)
        {
            List<WSPDB> wSPBySalonId = IoC.Resolve<IBillingManager>().GetWSPBySalonId(salon.SalonId);
            List<object> list2 = new List<object>();
            foreach (WSPDB wspdb in wSPBySalonId)
            {
                string str = "INACTIVE";
                if ((wspdb.CancelDate != string.Empty) && (DateTime.Today >= IFRMHelper.FromUrlFriendlyDate(wspdb.CancelDate)))
                {
                    str = "Cancelled";
                }
                else if (wspdb.Active)
                {
                    str = "Active";
                }
                list2.Add(new { 
                    PlanId = wspdb.PlanId,
                    Description = wspdb.Description,
                    PlanStartDate = wspdb.PlanStartDate,
                    PlanEndDate = wspdb.PlanEndDate,
                    Status = str
                });
            }
            this.gv.DataSource = list2;
            this.gv.DataBind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("account.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Guid planId = new Guid(this.gv.DataKeys[e.Row.RowIndex].Value.ToString());
                MultiView view = (MultiView) e.Row.FindControl("mv");
                List<SalonInvoiceDB> list = IoC.Resolve<IBillingManager>().GetSalonInvoicesByPlanId(planId, "U", "GBP");
                if (list.Count > 1)
                {
                    throw new Exception($"plan {planId} cannot be associated with more than one invoice of type U.");
                }
                if (list.Count == 1)
                {
                    SalonInvoiceDB edb = list[0];
                    if (!edb.PaidOn.HasValue)
                    {
                        view.ActiveViewIndex = 1;
                    }
                }
            }
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.NewEditIndex].Value.ToString());
            string str = $"{"wspid"}={guid}";
            string uRL = IFRMHelper.GetURL("wsp-payment.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                this.BindWSPList(salon);
            }
        }
    }
}

