namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [SupportsEventValidation, ValidationProperty("SelectedItem"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class RadioButtonList : ListControl, IRepeatInfoUser, INamingContainer, IPostBackDataHandler
    {
        private bool _cachedIsEnabled;
        private bool _cachedRegisterEnabled;
        private RadioButton _controlToRepeat;
        private int _offset = 0;

        protected override Style CreateControlStyle() => 
            new TableStyle(this.ViewState);

        protected override Control FindControl(string id, int pathOffset) => 
            this;

        protected virtual Style GetItemStyle(ListItemType itemType, int repeatIndex) => 
            null;

        protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            string eventArgument = postCollection[postDataKey];
            int selectedIndex = this.SelectedIndex;
            this.EnsureDataBound();
            int count = this.Items.Count;
            for (int i = 0; i < count; i++)
            {
                if ((eventArgument == this.Items[i].Value) && this.Items[i].Enabled)
                {
                    base.ValidateEvent(postDataKey, eventArgument);
                    if (i != selectedIndex)
                    {
                        base.SetPostDataSelection(i);
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        protected virtual void RaisePostDataChangedEvent()
        {
            if ((this.AutoPostBack && (this.Page != null)) && !this.Page.IsPostBackEventControlRegistered)
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
                this.ControlToRepeat.TabIndex = tabIndex;
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
                info.RenderRepeater(writer, this, controlStyle, this);
                if (this.Page != null)
                {
                    this.Page.ClientScript.RegisterForEventValidation(this.UniqueID);
                }
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
                this._cachedRegisterEnabled = (this.Page != null) && !base.SaveSelectedIndicesViewState;
            }
            RadioButton controlToRepeat = this.ControlToRepeat;
            int num = repeatIndex + this._offset;
            ListItem item = this.Items[num];
            controlToRepeat.Attributes.Clear();
            if (item.HasAttributes)
            {
                foreach (string str in item.Attributes.Keys)
                {
                    controlToRepeat.Attributes[str] = item.Attributes[str];
                }
            }
            controlToRepeat.ID = num.ToString(NumberFormatInfo.InvariantInfo);
            controlToRepeat.Text = item.Text;
            controlToRepeat.Attributes["value"] = item.Value;
            controlToRepeat.Checked = item.Selected;
            controlToRepeat.Enabled = this._cachedIsEnabled && item.Enabled;
            controlToRepeat.TextAlign = this.TextAlign;
            controlToRepeat.RenderControl(writer);
            if ((controlToRepeat.Enabled && this._cachedRegisterEnabled) && (this.Page != null))
            {
                this.Page.RegisterEnabledControl(controlToRepeat);
            }
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

        [WebCategory("Layout"), WebSysDescription("RadioButtonList_CellPadding"), DefaultValue(-1)]
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

        [WebCategory("Layout"), DefaultValue(-1), WebSysDescription("RadioButtonList_CellSpacing")]
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

        private RadioButton ControlToRepeat
        {
            get
            {
                if (this._controlToRepeat == null)
                {
                    this._controlToRepeat = new RadioButton();
                    this._controlToRepeat.EnableViewState = false;
                    this.Controls.Add(this._controlToRepeat);
                    this._controlToRepeat.AutoPostBack = this.AutoPostBack;
                    this._controlToRepeat.CausesValidation = this.CausesValidation;
                    this._controlToRepeat.ValidationGroup = this.ValidationGroup;
                }
                return this._controlToRepeat;
            }
        }

        protected virtual bool HasFooter =>
            false;

        protected virtual bool HasHeader =>
            false;

        protected virtual bool HasSeparators =>
            false;

        [WebSysDescription("RadioButtonList_RepeatColumns"), WebCategory("Layout"), DefaultValue(0)]
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

        [DefaultValue(0), WebSysDescription("WebControl_RepeatLayout"), WebCategory("Layout")]
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

        [WebCategory("Appearance"), DefaultValue(2), WebSysDescription("WebControl_TextAlign")]
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

