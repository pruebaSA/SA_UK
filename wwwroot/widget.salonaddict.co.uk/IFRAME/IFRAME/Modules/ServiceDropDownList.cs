namespace IFRAME.Modules
{
    using AjaxControlToolkit;
    using IFRAME.Controllers;
    using SA.BAL;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ValidationProperty("SelectedValue")]
    public class ServiceDropDownList : IFRMUserControl
    {
        protected DropDownList ddlValue;
        protected RequiredFieldValidator rfvValue;
        protected ValidatorCalloutExtender rfvValueEX;

        private void BindDropDownList()
        {
            Action<ServiceDB> action = null;
            ListItemCollection datasource = new ListItemCollection();
            if (this.DefaultText != null)
            {
                datasource.Add(new ListItem(this.DefaultText, this.DefaultValue ?? string.Empty));
            }
            if (IFRMContext.Current.Salon != null)
            {
                SalonDB salon = IFRMContext.Current.Salon;
                if (action == null)
                {
                    action = item => datasource.Add(new ListItem($"{item.Name} £{item.Price.ToString("#,#.00#")}", item.ServiceId.ToString()));
                }
                (from item in IoC.Resolve<IServiceManager>().GetServicesBySalonId(salon.SalonId)
                    where item.Active
                    select item).ToList<ServiceDB>().ForEach(action);
            }
            this.ddlValue.DataSource = datasource;
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

        public bool IsRequired
        {
            get => 
                this.rfvValue.Visible;
            set
            {
                this.rfvValue.Visible = value;
                this.rfvValue.Enabled = value;
                this.rfvValueEX.Enabled = value;
            }
        }

        [Localizable(true)]
        public string RequiredErrorMessage
        {
            get => 
                this.rfvValue.ErrorMessage;
            set
            {
                this.rfvValue.ErrorMessage = value;
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

        public Guid ServiceId
        {
            get
            {
                string selectedValue = this.SelectedValue;
                if (selectedValue == string.Empty)
                {
                    return Guid.Empty;
                }
                return new Guid(selectedValue);
            }
            set
            {
                if ((value != Guid.Empty) && (this.ddlValue.Items.FindByValue(value.ToString()) != null))
                {
                    this.SelectedValue = value.ToString();
                }
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

