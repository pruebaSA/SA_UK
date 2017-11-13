namespace IFRAME.Admin
{
    using IFRAME.Admin.Modules;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.UI.WebControls;

    public class SalonUsersPage : IFRMAdminPage
    {
        protected Button btnAddAPIKey;
        protected Button btnAddUser;
        protected IFRAME.Admin.Modules.Menu cntrlMenu;
        protected GridView gvAPIKeys;
        protected GridView gvUsers;
        protected Literal ltrHeader;
        protected Panel pnl;

        private void ApplyLocalization()
        {
            this.gvAPIKeys.Columns[0].HeaderText = base.GetLocaleResourceString("gvAPIKeys.Columns[0].HeaderText");
            this.gvAPIKeys.Columns[1].HeaderText = base.GetLocaleResourceString("gvAPIKeys.Columns[1].HeaderText");
            this.gvUsers.Columns[0].HeaderText = base.GetLocaleResourceString("gvUsers.Columns[0].HeaderText");
            this.gvUsers.Columns[1].HeaderText = base.GetLocaleResourceString("gvUsers.Columns[1].HeaderText");
            this.gvUsers.Columns[2].HeaderText = base.GetLocaleResourceString("gvUsers.Columns[2].HeaderText");
            this.gvUsers.Columns[3].HeaderText = base.GetLocaleResourceString("gvUsers.Columns[3].HeaderText");
        }

        private void BindSalonAPIKeys(SalonDB salon)
        {
            List<WidgetApiKeyDB> widgetApiKeyBySalonId = IoC.Resolve<IUserManager>().GetWidgetApiKeyBySalonId(salon.SalonId);
            this.gvAPIKeys.DataSource = widgetApiKeyBySalonId;
            this.gvAPIKeys.DataBind();
        }

        private void BindSalonDetails(SalonDB value)
        {
            this.ltrHeader.Text = string.Format(base.GetLocaleResourceString("ltrHeader.Text"), value.Name);
        }

        private void BindSalonUsers(SalonDB salon)
        {
            List<SalonUserDB> list2 = (from item in IoC.Resolve<IUserManager>().GetSalonUsersBySalonId(salon.SalonId)
                where !item.IsAdmin
                select item).ToList<SalonUserDB>();
            this.gvUsers.DataSource = list2;
            this.gvUsers.DataBind();
        }

        protected void btnAddAPIKey_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("apikey-create.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            string str = $"{"sid"}={this.PostedSalonId}";
            string uRL = IFRMHelper.GetURL("salonuser-create.aspx", new string[] { str });
            base.Response.Redirect(uRL, true);
        }

        protected void gvAPIKeys_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid guid = new Guid(this.gvAPIKeys.DataKeys[e.RowIndex].Value.ToString());
            string str = $"{"sid"}={this.PostedSalonId}";
            string str2 = $"{"wapi"}={guid}";
            string uRL = IFRMHelper.GetURL("apikey-delete.aspx", new string[] { str, str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void gvAPIKeys_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid guid = new Guid(this.gvAPIKeys.DataKeys[e.NewEditIndex].Value.ToString());
            string str = $"{"sid"}={this.PostedSalonId}";
            string str2 = $"{"wapi"}={guid}";
            string uRL = IFRMHelper.GetURL("apikey-edit.aspx", new string[] { str, str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void gvUsers_RowCreated(object sender, GridViewRowEventArgs e)
        {
        }

        protected void gvUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            Guid guid = new Guid(this.gvUsers.DataKeys[e.RowIndex].Value.ToString());
            string str = $"{"sid"}={this.PostedSalonId}";
            string str2 = $"{"uid"}={guid}";
            string uRL = IFRMHelper.GetURL("salonuser-delete.aspx", new string[] { str, str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void gvUsers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Guid guid = new Guid(this.gvUsers.DataKeys[e.NewEditIndex].Value.ToString());
            string str = $"{"sid"}={this.PostedSalonId}";
            string str2 = $"{"uid"}={guid}";
            string uRL = IFRMHelper.GetURL("salonuser-edit.aspx", new string[] { str, str2 });
            base.Response.Redirect(uRL, true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.ApplyLocalization();
            if (this.PostedSalonId == Guid.Empty)
            {
                string uRL = IFRMHelper.GetURL("salons.aspx", new string[0]);
                base.Response.Redirect(uRL, true);
            }
            if (!this.Page.IsPostBack)
            {
                SalonDB salonById = IoC.Resolve<ISalonManager>().GetSalonById(this.PostedSalonId);
                if (salonById == null)
                {
                    string url = IFRMHelper.GetURL("salons.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                this.BindSalonDetails(salonById);
                this.BindSalonAPIKeys(salonById);
                this.BindSalonUsers(salonById);
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

