namespace IFRAME.SecureArea
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using IFRAME.SecureArea.Modules;
    using SA.BAL;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.UI.WebControls;

    public class Service_CreatePage : IFRMSecurePage
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

        private void BindCategories()
        {
            List<CategoryDB> categories = IoC.Resolve<IServiceManager>().GetCategories();
            List<ListItem> items = this.GetCategoryNavigation(categories, null, 0);
            this.BindCategory1(items);
            this.BindCategory2(items);
            this.BindCategory3(items);
        }

        private void BindCategory1(List<ListItem> items)
        {
            List<ListItem> list = new List<ListItem> {
                new ListItem("Select a Category", string.Empty)
            };
            list.AddRange(items);
            this.ddlCategory1.DataSource = list;
            this.ddlCategory1.DataTextField = "Text";
            this.ddlCategory1.DataValueField = "Value";
            this.ddlCategory1.DataBind();
        }

        private void BindCategory2(List<ListItem> items)
        {
            List<ListItem> list = new List<ListItem> {
                new ListItem("Optional", string.Empty)
            };
            list.AddRange(items);
            this.ddlCategory2.DataSource = list;
            this.ddlCategory2.DataTextField = "Text";
            this.ddlCategory2.DataValueField = "Value";
            this.ddlCategory2.DataBind();
        }

        private void BindCategory3(List<ListItem> items)
        {
            List<ListItem> list = new List<ListItem> {
                new ListItem("Optional", string.Empty)
            };
            list.AddRange(items);
            this.ddlCategory3.DataSource = list;
            this.ddlCategory3.DataTextField = "Text";
            this.ddlCategory3.DataValueField = "Value";
            this.ddlCategory3.DataBind();
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
                ServiceDB service = new ServiceDB {
                    Active = this.cbActive.Checked,
                    BookOnWeb = true,
                    BookOnMobile = true,
                    BookOnWidget = true,
                    CategoryId1 = new Guid(this.ddlCategory1.SelectedValue)
                };
                if (this.ddlCategory2.SelectedValue != string.Empty)
                {
                    service.CategoryId2 = new Guid(this.ddlCategory2.SelectedValue);
                }
                if (this.ddlCategory3.SelectedValue != string.Empty)
                {
                    service.CategoryId3 = new Guid(this.ddlCategory3.SelectedValue);
                }
                service.CreatedOnUtc = DateTime.UtcNow;
                service.CurrencyCode = "GBP";
                service.Deleted = false;
                service.DisplayOrder = 1;
                service.Name = this.txtName.Text.Trim();
                service.Duration = int.Parse(this.txtLength.Text);
                service.IsTaxExempt = false;
                service.OldPrice = decimal.Parse(this.txtPrice.Text.Trim());
                service.Price = decimal.Parse(this.txtPrice.Text.Trim());
                service.SalonId = salon.SalonId;
                service.SEName = Guid.NewGuid().ToString().ToLowerInvariant();
                service.ShortDescription = this.txtDescription.Text.Trim();
                service.ShowOnWeb = true;
                service.ShowOnMobile = true;
                service.ShowOnWidget = true;
                service = IoC.Resolve<IServiceManager>().InsertService(service);
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
                this.BindCategories();
            }
        }
    }
}

