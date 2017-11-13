namespace IFRAME.Modules
{
    using IFRAME.Controllers;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ValidationProperty("SelectedValue")]
    public class YesNoOptions : IFRMUserControl
    {
        protected RadioButtonList rbl;

        private void BindRadioButtonList()
        {
            ListItemCollection items = new ListItemCollection();
            if (this.DefaultText != null)
            {
                items.Add(new ListItem(this.DefaultText, this.DefaultValue ?? string.Empty));
            }
            items.Add(new ListItem(base.GetLocaleResourceString("OptionYes.Text"), "YES"));
            items.Add(new ListItem(base.GetLocaleResourceString("OptionNo.Text"), "NO"));
            this.rbl.DataSource = items;
            this.rbl.DataTextField = "Text";
            this.rbl.DataValueField = "Value";
            this.rbl.DataBind();
        }

        public override void DataBind()
        {
            this.BindRadioButtonList();
            base.DataBind();
        }

        public override string ClientID =>
            this.rbl.ClientID;

        [Localizable(true)]
        public string DefaultText { get; set; }

        [Localizable(true)]
        public string DefaultValue { get; set; }

        public YesNoOption SelectedItem
        {
            get
            {
                if (this.SelectedValue == "YES")
                {
                    return YesNoOption.Yes;
                }
                return YesNoOption.No;
            }
            set
            {
                if (value == YesNoOption.No)
                {
                    this.SelectedValue = "NO";
                }
                else
                {
                    this.SelectedValue = "YES";
                }
            }
        }

        public string SelectedValue
        {
            get => 
                this.rbl.SelectedValue;
            set
            {
                this.rbl.SelectedValue = value;
            }
        }

        public Unit Width
        {
            get => 
                this.rbl.Width;
            set
            {
                this.rbl.Width = value;
            }
        }
    }
}

