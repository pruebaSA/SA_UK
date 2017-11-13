namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class CheckBoxList : ListControl, IRepeatInfoUser, INamingContainer, IPostBackDataHandler
    {
        private bool _cachedIsEnabled;
        private bool _cachedRegisterEnabled;
        private CheckBox _controlToRepeat = new CheckBox();
        private bool _hasNotifiedOfChange;
        private string _oldAccessKey;

        public CheckBoxList()
        {
            this._controlToRepeat.EnableViewState = false;
            this._controlToRepeat.ID = "0";
            this.Controls.Add(this._controlToRepeat);
        }

        protected override Style CreateControlStyle() => 
            new TableStyle(this.ViewState);

        protected override Control FindControl(string id, int pathOffset) => 
            this;

        protected virtual Style GetItemStyle(ListItemType itemType, int repeatIndex) => 
            null;

        protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            if (base.IsEnabled)
            {
                int num = int.Parse(postDataKey.Substring(this.UniqueID.Length + 1), CultureInfo.InvariantCulture);
                this.EnsureDataBound();
                if ((num >= 0) && (num < this.Items.Count))
                {
                    ListItem item = this.Items[num];
                    if (!item.Enabled)
                    {
                        return false;
                    }
                    bool flag = postCollection[postDataKey] != null;
                    if (item.Selected != flag)
                    {
                        item.Selected = flag;
                        if (!this._hasNotifiedOfChange)
                        {
                            this._hasNotifiedOfChange = true;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        protected internal override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this._controlToRepeat.AutoPostBack = this.AutoPostBack;
            this._controlToRepeat.CausesValidation = this.CausesValidation;
            this._controlToRepeat.ValidationGroup = this.ValidationGroup;
            if (this.Page != null)
            {
                for (int i = 0; i < this.Items.Count; i++)
                {
                    this._controlToRepeat.ID = i.ToString(NumberFormatInfo.InvariantInfo);
                    this.Page.RegisterRequiresPostBack(this._controlToRepeat);
                }
            }
        }

        protected virtual void RaisePostDataChangedEvent()
        {
            if (this.AutoPostBack && !this.Page.IsPostBackEventControlRegistered)
            {
                this.Page.AutoPostBackControl = this;
                if (this.CausesValidation)
                {
                    this.Page.Validate(this.ValidationGroup);
                }
            }
            this.OnSelectedIndexChanged(EventArgs.Empty);
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            if ((this.Items.Count != 0) || base.EnableLegacyRendering)
            {
                RepeatInfo info = new RepeatInfo();
                Style controlStyle = base.ControlStyleCreated ? base.ControlStyle : null;
                short tabIndex = this.TabIndex;
                bool flag = false;
                this._controlToRepeat.TextAlign = this.TextAlign;
                this._controlToRepeat.TabIndex = tabIndex;
                if (tabIndex != 0)
                {
                    if (!this.ViewState.IsItemDirty("TabIndex"))
                    {
                        flag = true;
                    }
                    this.TabIndex = 0;
                }
                info.RepeatColumns = this.RepeatColumns;
                info.RepeatDirection = this.RepeatDirection;
                if (!base.DesignMode && !this.Context.Request.Browser.Tables)
                {
                    info.RepeatLayout = System.Web.UI.WebControls.RepeatLayout.Flow;
                }
                else
                {
                    info.RepeatLayout = this.RepeatLayout;
                }
                if (info.RepeatLayout == System.Web.UI.WebControls.RepeatLayout.Flow)
                {
                    info.EnableLegacyRendering = base.EnableLegacyRendering;
                }
                this._oldAccessKey = this.AccessKey;
                this.AccessKey = string.Empty;
                info.RenderRepeater(writer, this, controlStyle, this);
                this.AccessKey = this._oldAccessKey;
                if (tabIndex != 0)
                {
                    this.TabIndex = tabIndex;
                }
                if (flag)
                {
                    this.ViewState.SetItemDirty("TabIndex", false);
                }
            }
        }

        protected virtual void RenderItem(ListItemType itemType, int repeatIndex, RepeatInfo repeatInfo, HtmlTextWriter writer)
        {
            if (repeatIndex == 0)
            {
                this._cachedIsEnabled = base.IsEnabled;
                this._cachedRegisterEnabled = ((this.Page != null) && base.IsEnabled) && !base.SaveSelectedIndicesViewState;
            }
            int num = repeatIndex;
            ListItem item = this.Items[num];
            this._controlToRepeat.Attributes.Clear();
            if (item.HasAttributes)
            {
                foreach (string str in item.Attributes.Keys)
                {
                    this._controlToRepeat.Attributes[str] = item.Attributes[str];
                }
            }
            this._controlToRepeat.ID = num.ToString(NumberFormatInfo.InvariantInfo);
            this._controlToRepeat.Text = item.Text;
            this._controlToRepeat.Checked = item.Selected;
            this._controlToRepeat.Enabled = this._cachedIsEnabled && item.Enabled;
            this._controlToRepeat.AccessKey = this._oldAccessKey;
            if (this._cachedRegisterEnabled && this._controlToRepeat.Enabled)
            {
                this.Page.RegisterEnabledControl(this._controlToRepeat);
            }
            this._controlToRepeat.RenderControl(writer);
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) => 
            this.LoadPostData(postDataKey, postCollection);

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            this.RaisePostDataChangedEvent();
        }

        Style IRepeatInfoUser.GetItemStyle(ListItemType itemType, int repeatIndex) => 
            this.GetItemStyle(itemType, repeatIndex);

        void IRepeatInfoUser.RenderItem(ListItemType itemType, int repeatIndex, RepeatInfo repeatInfo, HtmlTextWriter writer)
        {
            this.RenderItem(itemType, repeatIndex, repeatInfo, writer);
        }

        [DefaultValue(-1), WebCategory("Layout"), WebSysDescription("CheckBoxList_CellPadding")]
        public virtual int CellPadding
        {
            get
            {
                if (!base.ControlStyleCreated)
                {
                    return -1;
                }
                return ((TableStyle) base.ControlStyle).CellPadding;
            }
            set
            {
                ((TableStyle) base.ControlStyle).CellPadding = value;
            }
        }

        [DefaultValue(-1), WebCategory("Layout"), WebSysDescription("CheckBoxList_CellSpacing")]
        public virtual int CellSpacing
        {
            get
            {
                if (!base.ControlStyleCreated)
                {
                    return -1;
                }
                return ((TableStyle) base.ControlStyle).CellSpacing;
            }
            set
            {
                ((TableStyle) base.ControlStyle).CellSpacing = value;
            }
        }

        protected virtual bool HasFooter =>
            false;

        protected virtual bool HasHeader =>
            false;

        protected virtual bool HasSeparators =>
            false;

        internal override bool IsMultiSelectInternal =>
            true;

        [WebCategory("Layout"), DefaultValue(0), WebSysDescription("CheckBoxList_RepeatColumns")]
        public virtual int RepeatColumns
        {
            get
            {
                object obj2 = this.ViewState["RepeatColumns"];
                if (obj2 != null)
                {
                    return (int) obj2;
                }
                return 0;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["RepeatColumns"] = value;
            }
        }

        [WebSysDescription("Item_RepeatDirection"), DefaultValue(1), WebCategory("Layout")]
        public virtual System.Web.UI.WebControls.RepeatDirection RepeatDirection
        {
            get
            {
                object obj2 = this.ViewState["RepeatDirection"];
                if (obj2 != null)
                {
                    return (System.Web.UI.WebControls.RepeatDirection) obj2;
                }
                return System.Web.UI.WebControls.RepeatDirection.Vertical;
            }
            set
            {
                if ((value < System.Web.UI.WebControls.RepeatDirection.Horizontal) || (value > System.Web.UI.WebControls.RepeatDirection.Vertical))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["RepeatDirection"] = value;
            }
        }

        protected virtual int RepeatedItemCount =>
            this.Items?.Count;

        [WebSysDescription("WebControl_RepeatLayout"), WebCategory("Layout"), DefaultValue(0)]
        public virtual System.Web.UI.WebControls.RepeatLayout RepeatLayout
        {
            get
            {
                object obj2 = this.ViewState["RepeatLayout"];
                if (obj2 != null)
                {
                    return (System.Web.UI.WebControls.RepeatLayout) obj2;
                }
                return System.Web.UI.WebControls.RepeatLayout.Table;
            }
            set
            {
                if ((value < System.Web.UI.WebControls.RepeatLayout.Table) || (value > System.Web.UI.WebControls.RepeatLayout.Flow))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["RepeatLayout"] = value;
            }
        }

        bool IRepeatInfoUser.HasFooter =>
            this.HasFooter;

        bool IRepeatInfoUser.HasHeader =>
            this.HasHeader;

        bool IRepeatInfoUser.HasSeparators =>
            this.HasSeparators;

        int IRepeatInfoUser.RepeatedItemCount =>
            this.RepeatedItemCount;

        [DefaultValue(2), WebSysDescription("WebControl_TextAlign"), WebCategory("Appearance")]
        public virtual System.Web.UI.WebControls.TextAlign TextAlign
        {
            get
            {
                object obj2 = this.ViewState["TextAlign"];
                if (obj2 != null)
                {
                    return (System.Web.UI.WebControls.TextAlign) obj2;
                }
                return System.Web.UI.WebControls.TextAlign.Right;
            }
            set
            {
                if ((value < System.Web.UI.WebControls.TextAlign.Left) || (value > System.Web.UI.WebControls.TextAlign.Right))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["TextAlign"] = value;
            }
        }
    }
}

