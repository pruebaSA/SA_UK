namespace IFRAME.SecureArea
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;

    public class Service_EditPage : IFRMSecurePage
    {
        protected Button btnCancel;
        protected Button btnSave;
        protected CheckBox cbActive;
        protected IFRAME.SecureArea.Modules.Menu cntlMenu;
        protected DropDownList ddlCategory1;
        protected DropDownList ddlCategory2;
        protected DropDownList ddlCategory3;
        protected Panel pnl;
        protected TabPanel pnlInfo;
        protected TabPanel pnlOther;
        protected TabContainer pnlTabs;
        protected TextBox txtDescription;
        protected TextBox txtLength;
        protected TextBox txtName;
        protected TextBox txtPrice;
        protected RequiredFieldValidator valCategory1;
        protected ValidatorCalloutExtender valCategory1Ex;
        protected RequiredFieldValidator valLength;
        protected ValidatorCalloutExtender valLengthEx;
        protected RegularExpressionValidator valLengthRegex;
        protected ValidatorCalloutExtender valLengthRegexEx;
        protected RequiredFieldValidator valName;
        protected ValidatorCalloutExtender valNameEx;
        protected RequiredFieldValidator valPrice;
        protected ValidatorCalloutExtender valPriceEx;
        protected RegularExpressionValidator valPriceRegEx;
        protected ValidatorCalloutExtender valPriceRegExEx;

        private void BindCategories(ServiceDB service)
        {
            List<CategoryDB> categories = IoC.Resolve<IServiceManager>().GetCategories();
            List<ListItem> items = this.GetCategoryNavigation(categories, null, 0);
            this.BindCategory1(items, service.CategoryId1);
            this.BindCategory2(items, service.CategoryId2);
            this.BindCategory3(items, service.CategoryId3);
        }

        private void BindCategory1(List<ListItem> items, Guid? selectedID)
        {
            List<ListItem> list = new List<ListItem> {
                new ListItem("Select a Category", string.Empty)
            };
            list.AddRange(items);
            this.ddlCategory1.DataSource = list;
            this.ddlCategory1.DataTextField = "Text";
            this.ddlCategory1.DataValueField = "Value";
            this.ddlCategory1.DataBind();
            if (selectedID.HasValue && (this.ddlCategory1.Items.FindByValue(selectedID.ToString()) != null))
            {
                this.ddlCategory1.SelectedValue = selectedID.ToString();
            }
        }

        private void BindCategory2(List<ListItem> items, Guid? selectedID)
        {
            List<ListItem> list = new List<ListItem> {
                new ListItem("Optional", string.Empty)
            };
            list.AddRange(items);
            this.ddlCategory2.DataSource = list;
            this.ddlCategory2.DataTextField = "Text";
            this.ddlCategory2.DataValueField = "Value";
            this.ddlCategory2.DataBind();
            if (selectedID.HasValue && (this.ddlCategory2.Items.FindByValue(selectedID.ToString()) != null))
            {
                this.ddlCategory2.SelectedValue = selectedID.ToString();
            }
        }

        private void BindCategory3(List<ListItem> items, Guid? selectedID)
        {
            List<ListItem> list = new List<ListItem> {
                new ListItem("Optional", string.Empty)
            };
            list.AddRange(items);
            this.ddlCategory3.DataSource = list;
            this.ddlCategory3.DataTextField = "Text";
            this.ddlCategory3.DataValueField = "Value";
            this.ddlCategory3.DataBind();
            if (selectedID.HasValue && (this.ddlCategory3.Items.FindByValue(selectedID.ToString()) != null))
            {
                this.ddlCategory3.SelectedValue = selectedID.ToString();
            }
        }

        private void BindServiceDetails(ServiceDB value)
        {
            this.txtName.Text = value.Name;
            this.txtDescription.Text = value.ShortDescription;
            this.txtPrice.Text = value.Price.ToString("#,#.00#");
            this.txtLength.Text = value.Duration.ToString();
            this.cbActive.Checked = value.Active;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            string uRL = IFRMHelper.GetURL("services.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (this.Page.IsValid)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                ServiceDB serviceById = IoC.Resolve<IServiceManager>().GetServiceById(this.PostedServiceId);
                if (serviceById.SalonId != salon.SalonId)
                {
                    throw new SecurityException();
                }
                serviceById.Active = this.cbActive.Checked;
                serviceById.CategoryId1 = new Guid(this.ddlCategory1.SelectedValue);
                if (this.ddlCategory2.SelectedValue != string.Empty)
                {
                    serviceById.CategoryId2 = new Guid(this.ddlCategory2.SelectedValue);
                }
                else
                {
                    serviceById.CategoryId2 = null;
                }
                if (this.ddlCategory3.SelectedValue != string.Empty)
                {
                    serviceById.CategoryId3 = new Guid(this.ddlCategory3.SelectedValue);
                }
                else
                {
                    serviceById.CategoryId3 = null;
                }
                serviceById.ShortDescription = this.txtDescription.Text.Trim();
                serviceById.Duration = int.Parse(this.txtLength.Text.Trim());
                serviceById.Name = this.txtName.Text.Trim();
                serviceById.Price = decimal.Parse(this.txtPrice.Text.Trim());
                serviceById = IoC.Resolve<IServiceManager>().UpdateService(serviceById);
            }
            string uRL = IFRMHelper.GetURL("services.aspx", new string[0]);
            base.Response.Redirect(uRL, true);
        }

        private List<ListItem> GetCategoryNavigation(List<CategoryDB> source, Guid? parentID, int level)
        {
            Func<CategoryDB, bool> predicate = null;
            List<ListItem> list = new List<ListItem>();
            foreach (CategoryDB ydb in from c in source.FindAll(delegate (CategoryDB c) {
                Guid? parentCategoryId = c.ParentCategoryIdGuid? nullable1 = parentIDif (parentCategoryId.HasValue != nullable1.HasValue)
                {
                    return false;
                }
                if (parentCategoryId.HasValue)
                {
                    return parentCategoryId.GetValueOrDefault() == nullable1.GetValueOrDefault();
                }
                return true;
            })
                orderby c.DisplayOrder, c.Name
                select c)
            {
                StringBuilder builder = new StringBuilder();
                if (parentID.HasValue)
                {
                    if (predicate == null)
                    {
                        predicate = item => item.CategoryId == parentID;
                    }
                    CategoryDB ydb2 = source.FirstOrDefault<CategoryDB>(predicate);
                    if (ydb2 != null)
                    {
                        builder.Append(ydb2.Name);
                        builder.Append(HttpUtility.HtmlDecode("&nbsp;"));
                        builder.Append(HttpUtility.HtmlDecode("&#187;"));
                        builder.Append(HttpUtility.HtmlDecode("&nbsp;"));
                    }
                    builder.Append(ydb.Name);
                    list.Add(new ListItem(builder.ToString(), ydb.CategoryId.ToString()));
                }
                list.AddRange(this.GetCategoryNavigation(source, new Guid?(ydb.CategoryId), level + 1).ToArray());
            }
            return list;
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
                Guid postedServiceId = this.PostedServiceId;
                SalonDB salon = IFRMContext.Current.Salon;
                ServiceDB serviceById = IoC.Resolve<IServiceManager>().GetServiceById(this.PostedServiceId);
                if ((serviceById == null) || (serviceById.SalonId != salon.SalonId))
                {
                    string url = IFRMHelper.GetURL("services.aspx", new string[0]);
                    base.Response.Redirect(url, true);
                }
                this.BindCategories(serviceById);
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

