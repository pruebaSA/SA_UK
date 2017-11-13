namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using IFRAME.Controllers.Controls;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.UI.WebControls;

    public class SalonsPage : IFRMAdminPage
    {
        protected Button btnAdd;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected IFRMPager cntrlPager;
        protected GridView gv;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gv.Columns[0].HeaderText = base.GetLocaleResourceString("gv.Columns[0].HeaderText");
            this.gv.Columns[1].HeaderText = base.GetLocaleResourceString("gv.Columns[1].HeaderText");
            this.gv.Columns[2].HeaderText = base.GetLocaleResourceString("gv.Columns[2].HeaderText");
            this.gv.Columns[3].HeaderText = base.GetLocaleResourceString("gv.Columns[3].HeaderText");
            this.gv.Columns[4].HeaderText = base.GetLocaleResourceString("gv.Columns[4].HeaderText");
        }

        private void BindSalons()
        {
            List<SalonDB> salonsThatStartWith = IoC.Resolve<ISalonManager>().GetSalonsThatStartWith(this.SearchTerm, "GBP");
            this.gv.DataSource = salonsThatStartWith;
            this.gv.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string str = $"{"url"}={base.Request.Url.PathAndQuery}";
            string uRL = IFRMHelper.GetURL("saloncreate.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void gv_RowCreated(object sender, GridViewRowEventArgs e)
        {
        }

        protected void gv_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid guid = new Guid(this.gv.DataKeys[e.NewEditIndex].Value.ToString());
            string str = $"{"sid"}={guid}";
            string str2 = $"{"url"}={HttpUtility.UrlEncode(base.Request.Url.PathAndQuery)}";
            string uRL = IFRMHelper.GetURL("salondetails.aspx", new string[] { str, str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (!this.Page.IsPostBack)
            {
                this.BindSalons();
            }
        }

        public string SearchTerm
        {
            get
            {
                string str = base.Request.QueryString["s"];
                if (str == null)
                {
                    return "A";
                }
                if (str.Length != 1)
                {
                    return "A";
                }
                if ("ABCDEFGHIJKLMNOPQRSTUVWYXZ0123456789".IndexOf(str) == -1)
                {
                    return "A";
                }
                return str;
            }
        }
    }
}

