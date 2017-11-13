namespace IFRAME.Modules
{
    using IFRAME.Controllers;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ValidationProperty("SelectedValue")]
    public class PhoneDropDownList : IFRMUserControl
    {
        protected DropDownList ddlValue;

        private void BindDropDownList()
        {
            ListItemCollection items = new ListItemCollection();
            if (this.DefaultText != null)
            {
                items.Add(new ListItem(this.DefaultText, this.DefaultValue ?? string.Empty));
            }
            items.Add(new ListItem(base.GetLocaleResourceString("ddlValue.Options[0].Text"), base.GetLocaleResourceString("ddlValue.Options[0].Value")));
            items.Add(new ListItem(base.GetLocaleResourceString("ddlValue.Options[1].Text"), base.GetLocaleResourceString("ddlValue.Options[1].Value")));
            this.ddlValue.DataSource = items;
            this.ddlValue.DataTextField = "Text";
            this.ddlValue.DataValueField = "Value";
            this.ddlValue.DataBind();
        }

        public override void DataBind()
        {
            this.BindDropDownList();
            base.DataBind();
        }

        public override string ClientID =>
            this.ddlValue.ClientID;

        [Localizable(true)]
        public string DefaultText { get; set; }

        [Localizable(true)]
        public string DefaultValue { get; set; }

        public PhoneNumberType SelectedItem
        {
            get
            {
                if (this.SelectedValue == "MOBILE")
                {
                    return PhoneNumberType.Mobile;
                }
                return PhoneNumberType.Phone;
            }
            set
            {
                if (value == PhoneNumberType.Mobile)
                {
                    this.SelectedValue = "MOBILE";
                }
                else
                {
                    this.SelectedValue = "PHONE";
                }
            }
        }

        public string SelectedValue
        {
            get => 
                this.ddlValue.SelectedValue;
            set
            {
                this.ddlValue.SelectedValue = value;
            }
        }

        public Unit Width
        {
            get => 
                this.ddlValue.Width;
            set
            {
                this.ddlValue.Width = value;
            }
        }
    }
}

