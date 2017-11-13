namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [DefaultProperty("TextField"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class MenuItemBinding : IStateManager, ICloneable, IDataSourceViewSchemaAccessor
    {
        private bool _isTrackingViewState;
        private StateBag _viewState;

        internal void SetDirty()
        {
            this.ViewState.SetDirty(true);
        }

        object ICloneable.Clone() => 
            new MenuItemBinding { 
                DataMember = this.DataMember,
                Depth = this.Depth,
                Enabled = this.Enabled,
                EnabledField = this.EnabledField,
                FormatString = this.FormatString,
                ImageUrl = this.ImageUrl,
                ImageUrlField = this.ImageUrlField,
                NavigateUrl = this.NavigateUrl,
                NavigateUrlField = this.NavigateUrlField,
                PopOutImageUrl = this.PopOutImageUrl,
                PopOutImageUrlField = this.PopOutImageUrlField,
                Selectable = this.Selectable,
                SelectableField = this.SelectableField,
                SeparatorImageUrl = this.SeparatorImageUrl,
                SeparatorImageUrlField = this.SeparatorImageUrlField,
                Target = this.Target,
                TargetField = this.TargetField,
                Text = this.Text,
                TextField = this.TextField,
                ToolTip = this.ToolTip,
                ToolTipField = this.ToolTipField,
                Value = this.Value,
                ValueField = this.ValueField
            };

        void IStateManager.LoadViewState(object state)
        {
            if (state != null)
            {
                ((IStateManager) this.ViewState).LoadViewState(state);
            }
        }

        object IStateManager.SaveViewState()
        {
            if (this._viewState != null)
            {
                return ((IStateManager) this._viewState).SaveViewState();
            }
            return null;
        }

        void IStateManager.TrackViewState()
        {
            this._isTrackingViewState = true;
            if (this._viewState != null)
            {
                ((IStateManager) this._viewState).TrackViewState();
            }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.DataMember))
            {
                return this.DataMember;
            }
            return System.Web.SR.GetString("TreeNodeBinding_EmptyBindingText");
        }

        [WebCategory("Data"), DefaultValue(""), WebSysDescription("Binding_DataMember")]
        public string DataMember
        {
            get
            {
                object obj2 = this.ViewState["DataMember"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["DataMember"] = value;
            }
        }

        [WebSysDescription("MenuItemBinding_Depth"), DefaultValue(-1), TypeConverter("System.Web.UI.Design.WebControls.TreeNodeBindingDepthConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), WebCategory("Data")]
        public int Depth
        {
            get
            {
                object obj2 = this.ViewState["Depth"];
                if (obj2 == null)
                {
                    return -1;
                }
                return (int) obj2;
            }
            set
            {
                this.ViewState["Depth"] = value;
            }
        }

        [DefaultValue(true), WebSysDescription("MenuItemBinding_Enabled"), WebCategory("DefaultProperties")]
        public bool Enabled
        {
            get
            {
                object obj2 = this.ViewState["Enabled"];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
                return true;
            }
            set
            {
                this.ViewState["Enabled"] = value;
            }
        }

        [DefaultValue(""), WebSysDescription("MenuItemBinding_EnabledField"), TypeConverter("System.Web.UI.Design.DataSourceViewSchemaConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), WebCategory("Databindings")]
        public string EnabledField
        {
            get
            {
                object obj2 = this.ViewState["EnabledField"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["EnabledField"] = value;
            }
        }

        [WebSysDescription("MenuItemBinding_FormatString"), DefaultValue(""), Localizable(true), WebCategory("Databindings")]
        public string FormatString
        {
            get
            {
                object obj2 = this.ViewState["FormatString"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["FormatString"] = value;
            }
        }

        [WebSysDescription("MenuItemBinding_ImageUrl"), WebCategory("DefaultProperties"), DefaultValue(""), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty]
        public string ImageUrl
        {
            get
            {
                object obj2 = this.ViewState["ImageUrl"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["ImageUrl"] = value;
            }
        }

        [TypeConverter("System.Web.UI.Design.DataSourceViewSchemaConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), WebCategory("Databindings"), WebSysDescription("MenuItemBinding_ImageUrlField"), DefaultValue("")]
        public string ImageUrlField
        {
            get
            {
                object obj2 = this.ViewState["ImageUrlField"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["ImageUrlField"] = value;
            }
        }

        [WebSysDescription("MenuItemBinding_NavigateUrl"), WebCategory("DefaultProperties"), DefaultValue(""), Editor("System.Web.UI.Design.UrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty]
        public string NavigateUrl
        {
            get
            {
                object obj2 = this.ViewState["NavigateUrl"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["NavigateUrl"] = value;
            }
        }

        [DefaultValue(""), WebSysDescription("MenuItemBinding_NavigateUrlField"), TypeConverter("System.Web.UI.Design.DataSourceViewSchemaConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), WebCategory("Databindings")]
        public string NavigateUrlField
        {
            get
            {
                object obj2 = this.ViewState["NavigateUrlField"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["NavigateUrlField"] = value;
            }
        }

        [WebCategory("DefaultProperties"), UrlProperty, DefaultValue(""), WebSysDescription("MenuItemBinding_PopOutImageUrl"), Editor("System.Web.UI.Design.UrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string PopOutImageUrl
        {
            get
            {
                object obj2 = this.ViewState["PopOutImageUrl"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["PopOutImageUrl"] = value;
            }
        }

        [WebSysDescription("MenuItemBinding_PopOutImageUrlField"), DefaultValue(""), WebCategory("Databindings"), TypeConverter("System.Web.UI.Design.DataSourceViewSchemaConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string PopOutImageUrlField
        {
            get
            {
                object obj2 = this.ViewState["PopOutImageUrlField"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["PopOutImageUrlField"] = value;
            }
        }

        [DefaultValue(true), WebCategory("DefaultProperties"), WebSysDescription("MenuItemBinding_Selectable")]
        public bool Selectable
        {
            get
            {
                object obj2 = this.ViewState["Selectable"];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
                return true;
            }
            set
            {
                this.ViewState["Selectable"] = value;
            }
        }

        [WebSysDescription("MenuItemBinding_SelectableField"), TypeConverter("System.Web.UI.Design.DataSourceViewSchemaConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), WebCategory("Databindings"), DefaultValue("")]
        public string SelectableField
        {
            get
            {
                object obj2 = this.ViewState["SelectableField"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["SelectableField"] = value;
            }
        }

        [DefaultValue(""), Editor("System.Web.UI.Design.UrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty, WebCategory("DefaultProperties"), WebSysDescription("MenuItemBinding_SeparatorImageUrl")]
        public string SeparatorImageUrl
        {
            get
            {
                object obj2 = this.ViewState["SeparatorImageUrl"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["SeparatorImageUrl"] = value;
            }
        }

        [WebSysDescription("MenuItemBinding_SeparatorImageUrlField"), DefaultValue(""), WebCategory("Databindings"), TypeConverter("System.Web.UI.Design.DataSourceViewSchemaConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string SeparatorImageUrlField
        {
            get
            {
                object obj2 = this.ViewState["SeparatorImageUrlField"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["SeparatorImageUrlField"] = value;
            }
        }

        object IDataSourceViewSchemaAccessor.DataSourceViewSchema
        {
            get => 
                this.ViewState["IDataSourceViewSchemaAccessor.DataSourceViewSchema"];
            set
            {
                this.ViewState["IDataSourceViewSchemaAccessor.DataSourceViewSchema"] = value;
            }
        }

        bool IStateManager.IsTrackingViewState =>
            this._isTrackingViewState;

        [WebCategory("DefaultProperties"), DefaultValue(""), WebSysDescription("MenuItemBinding_Target")]
        public string Target
        {
            get
            {
                object obj2 = this.ViewState["Target"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["Target"] = value;
            }
        }

        [WebCategory("Databindings"), DefaultValue(""), WebSysDescription("MenuItemBinding_TargetField"), TypeConverter("System.Web.UI.Design.DataSourceViewSchemaConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string TargetField
        {
            get
            {
                string str = (string) this.ViewState["TargetField"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                this.ViewState["TargetField"] = value;
            }
        }

        [WebCategory("DefaultProperties"), Localizable(true), DefaultValue(""), WebSysDescription("MenuItemBinding_Text")]
        public string Text
        {
            get
            {
                object obj2 = this.ViewState["Text"];
                if (obj2 == null)
                {
                    obj2 = this.ViewState["Value"];
                    if (obj2 == null)
                    {
                        return string.Empty;
                    }
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["Text"] = value;
            }
        }

        [WebCategory("Databindings"), DefaultValue(""), WebSysDescription("MenuItemBinding_TextField"), TypeConverter("System.Web.UI.Design.DataSourceViewSchemaConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string TextField
        {
            get
            {
                object obj2 = this.ViewState["TextField"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["TextField"] = value;
            }
        }

        [WebSysDescription("MenuItemBinding_ToolTip"), WebCategory("DefaultProperties"), DefaultValue(""), Localizable(true)]
        public string ToolTip
        {
            get
            {
                object obj2 = this.ViewState["ToolTip"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["ToolTip"] = value;
            }
        }

        [DefaultValue(""), WebSysDescription("MenuItemBinding_ToolTipField"), TypeConverter("System.Web.UI.Design.DataSourceViewSchemaConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), WebCategory("Databindings")]
        public string ToolTipField
        {
            get
            {
                object obj2 = this.ViewState["ToolTipField"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["ToolTipField"] = value;
            }
        }

        [DefaultValue(""), WebCategory("DefaultProperties"), WebSysDescription("MenuItemBinding_Value"), Localizable(true)]
        public string Value
        {
            get
            {
                object obj2 = this.ViewState["Value"];
                if (obj2 == null)
                {
                    obj2 = this.ViewState["Text"];
                    if (obj2 == null)
                    {
                        return string.Empty;
                    }
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["Value"] = value;
            }
        }

        [WebCategory("Databindings"), WebSysDescription("MenuItemBinding_ValueField"), DefaultValue(""), TypeConverter("System.Web.UI.Design.DataSourceViewSchemaConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string ValueField
        {
            get
            {
                object obj2 = this.ViewState["ValueField"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["ValueField"] = value;
            }
        }

        private StateBag ViewState
        {
            get
            {
                if (this._viewState == null)
                {
                    this._viewState = new StateBag();
                    if (this._isTrackingViewState)
                    {
                        ((IStateManager) this._viewState).TrackViewState();
                    }
                }
                return this._viewState;
            }
        }
    }
}

