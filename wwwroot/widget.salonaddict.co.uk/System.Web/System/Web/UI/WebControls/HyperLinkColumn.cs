namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HyperLinkColumn : DataGridColumn
    {
        private PropertyDescriptor textFieldDesc;
        private PropertyDescriptor urlFieldDesc;

        protected virtual string FormatDataNavigateUrlValue(object dataUrlValue)
        {
            string str = string.Empty;
            if (DataBinder.IsNull(dataUrlValue))
            {
                return str;
            }
            string dataNavigateUrlFormatString = this.DataNavigateUrlFormatString;
            if (dataNavigateUrlFormatString.Length == 0)
            {
                return dataUrlValue.ToString();
            }
            return string.Format(CultureInfo.CurrentCulture, dataNavigateUrlFormatString, new object[] { dataUrlValue });
        }

        protected virtual string FormatDataTextValue(object dataTextValue)
        {
            string str = string.Empty;
            if (DataBinder.IsNull(dataTextValue))
            {
                return str;
            }
            string dataTextFormatString = this.DataTextFormatString;
            if (dataTextFormatString.Length == 0)
            {
                return dataTextValue.ToString();
            }
            return string.Format(CultureInfo.CurrentCulture, dataTextFormatString, new object[] { dataTextValue });
        }

        public override void Initialize()
        {
            base.Initialize();
            this.textFieldDesc = null;
            this.urlFieldDesc = null;
        }

        public override void InitializeCell(TableCell cell, int columnIndex, ListItemType itemType)
        {
            base.InitializeCell(cell, columnIndex, itemType);
            if ((itemType != ListItemType.Header) && (itemType != ListItemType.Footer))
            {
                HyperLink child = new HyperLink {
                    Text = this.Text,
                    NavigateUrl = this.NavigateUrl,
                    Target = this.Target
                };
                if ((this.DataNavigateUrlField.Length != 0) || (this.DataTextField.Length != 0))
                {
                    child.DataBinding += new EventHandler(this.OnDataBindColumn);
                }
                cell.Controls.Add(child);
            }
        }

        private void OnDataBindColumn(object sender, EventArgs e)
        {
            HyperLink link = (HyperLink) sender;
            DataGridItem namingContainer = (DataGridItem) link.NamingContainer;
            object dataItem = namingContainer.DataItem;
            if ((this.textFieldDesc == null) && (this.urlFieldDesc == null))
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(dataItem);
                string dataTextField = this.DataTextField;
                if (dataTextField.Length != 0)
                {
                    this.textFieldDesc = properties.Find(dataTextField, true);
                    if ((this.textFieldDesc == null) && !base.DesignMode)
                    {
                        throw new HttpException(System.Web.SR.GetString("Field_Not_Found", new object[] { dataTextField }));
                    }
                }
                dataTextField = this.DataNavigateUrlField;
                if (dataTextField.Length != 0)
                {
                    this.urlFieldDesc = properties.Find(dataTextField, true);
                    if ((this.urlFieldDesc == null) && !base.DesignMode)
                    {
                        throw new HttpException(System.Web.SR.GetString("Field_Not_Found", new object[] { dataTextField }));
                    }
                }
            }
            if (this.textFieldDesc != null)
            {
                object dataTextValue = this.textFieldDesc.GetValue(dataItem);
                string str2 = this.FormatDataTextValue(dataTextValue);
                link.Text = str2;
            }
            else if (base.DesignMode && (this.DataTextField.Length != 0))
            {
                link.Text = System.Web.SR.GetString("Sample_Databound_Text");
            }
            if (this.urlFieldDesc != null)
            {
                object dataUrlValue = this.urlFieldDesc.GetValue(dataItem);
                string str3 = this.FormatDataNavigateUrlValue(dataUrlValue);
                link.NavigateUrl = str3;
            }
            else if (base.DesignMode && (this.DataNavigateUrlField.Length != 0))
            {
                link.NavigateUrl = "url";
            }
        }

        [WebCategory("Data"), WebSysDescription("HyperLinkColumn_DataNavigateUrlField"), DefaultValue("")]
        public virtual string DataNavigateUrlField
        {
            get
            {
                object obj2 = base.ViewState["DataNavigateUrlField"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                base.ViewState["DataNavigateUrlField"] = value;
                this.OnColumnChanged();
            }
        }

        [Description("The formatting applied to the value bound to the NavigateUrl property."), WebCategory("Data"), DefaultValue("")]
        public virtual string DataNavigateUrlFormatString
        {
            get
            {
                object obj2 = base.ViewState["DataNavigateUrlFormatString"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                base.ViewState["DataNavigateUrlFormatString"] = value;
                this.OnColumnChanged();
            }
        }

        [WebSysDescription("HyperLinkColumn_DataTextField"), DefaultValue(""), WebCategory("Data")]
        public virtual string DataTextField
        {
            get
            {
                object obj2 = base.ViewState["DataTextField"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                base.ViewState["DataTextField"] = value;
                this.OnColumnChanged();
            }
        }

        [DefaultValue(""), Description("The formatting applied to the value bound to the Text property."), WebCategory("Data")]
        public virtual string DataTextFormatString
        {
            get
            {
                object obj2 = base.ViewState["DataTextFormatString"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                base.ViewState["DataTextFormatString"] = value;
                this.OnColumnChanged();
            }
        }

        [WebSysDescription("HyperLinkColumn_NavigateUrl"), WebCategory("Behavior"), DefaultValue(""), UrlProperty]
        public virtual string NavigateUrl
        {
            get
            {
                object obj2 = base.ViewState["NavigateUrl"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                base.ViewState["NavigateUrl"] = value;
                this.OnColumnChanged();
            }
        }

        [WebSysDescription("HyperLink_Target"), WebCategory("Behavior"), DefaultValue(""), TypeConverter(typeof(TargetConverter))]
        public virtual string Target
        {
            get
            {
                object obj2 = base.ViewState["Target"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                base.ViewState["Target"] = value;
                this.OnColumnChanged();
            }
        }

        [Localizable(true), WebSysDescription("HyperLinkColumn_Text"), WebCategory("Appearance"), DefaultValue("")]
        public virtual string Text
        {
            get
            {
                object obj2 = base.ViewState["Text"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                base.ViewState["Text"] = value;
                this.OnColumnChanged();
            }
        }
    }
}

