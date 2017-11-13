namespace AjaxControlToolkit
{
    using AjaxControlToolkit.Properties;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.Design.WebControls;
    using System.Web.UI.WebControls;

    [ValidationProperty("SelectedItem"), DefaultEvent("SelectedIndexChanged"), ControlValueProperty("SelectedValue"), ToolboxBitmap(typeof(ComboBox), "ComboBox.ComboBox.ico"), SupportsEventValidation, Designer(typeof(ComboBoxDesigner)), Bindable(true, BindingDirection.TwoWay), DataBindingHandler(typeof(ListControlDataBindingHandler)), DefaultProperty("SelectedValue"), ParseChildren(true, "Items"), ToolboxData("<{0}:ComboBox runat=\"server\"></{0}:ComboBox>"), ClientCssResource("ComboBox.ComboBox.css", LoadOrder=1), ClientScriptResource("Sys.Extended.UI.ComboBox", "ComboBox.ComboBox.js"), RequiredScript(typeof(ScriptControlBase), 2), RequiredScript(typeof(PopupExtender), 3), RequiredScript(typeof(CommonToolkitScripts), 4), ToolboxItem("System.Web.UI.Design.WebControlToolboxItem, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ComboBox : ListControl, IScriptControl, IPostBackDataHandler, INamingContainer, IControlResolver
    {
        private ComboBoxButton _buttonControl;
        private Table _comboTable;
        private TableCell _comboTableButtonCell;
        private TableRow _comboTableRow;
        private TableCell _comboTableTextBoxCell;
        private HiddenField _hiddenFieldControl;
        private System.Web.UI.WebControls.BulletedList _optionListControl;
        private System.Web.UI.ScriptManager _scriptManager;
        private TextBox _textBoxControl;
        private static readonly object EventItemInserted = new object();
        private static readonly object EventItemInserting = new object();

        public event EventHandler<ComboBoxItemInsertEventArgs> ItemInserted
        {
            add
            {
                base.Events.AddHandler(EventItemInserted, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventItemInserted, value);
            }
        }

        public event EventHandler<ComboBoxItemInsertEventArgs> ItemInserting
        {
            add
            {
                base.Events.AddHandler(EventItemInserting, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventItemInserting, value);
            }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            this.AddContainerAttributesToRender(writer);
            this.AddTableAttributesToRender(writer);
            this.AddTextBoxAttributesToRender(writer);
            this.AddButtonAttributesToRender(writer);
            this.AddOptionListAttributesToRender(writer);
        }

        protected virtual void AddButtonAttributesToRender(HtmlTextWriter writer)
        {
            if (base.DesignMode)
            {
                this.ButtonControl.Width = Unit.Pixel(14);
                this.ButtonControl.Height = Unit.Pixel(14);
            }
        }

        protected virtual void AddContainerAttributesToRender(HtmlTextWriter writer)
        {
            if (this.RenderMode == ComboBoxRenderMode.Inline)
            {
                base.Style.Add(HtmlTextWriterStyle.Display, this.GetInlineDisplayStyle());
            }
            base.AddAttributesToRender(writer);
        }

        protected virtual void AddOptionListAttributesToRender(HtmlTextWriter writer)
        {
            this.OptionListControl.CssClass = "ajax__combobox_itemlist";
            this.OptionListControl.Style.Add(HtmlTextWriterStyle.Display, "none");
            this.OptionListControl.Style.Add(HtmlTextWriterStyle.Visibility, "hidden");
        }

        protected virtual void AddTableAttributesToRender(HtmlTextWriter writer)
        {
            this.ComboTable.CellPadding = 0;
            this.ComboTable.CellSpacing = 0;
            this.ComboTable.CssClass = "ajax__combobox_inputcontainer";
            this.ComboTableTextBoxCell.CssClass = "ajax__combobox_textboxcontainer";
            this.ComboTableButtonCell.CssClass = "ajax__combobox_buttoncontainer";
            this.ComboTable.BorderStyle = System.Web.UI.WebControls.BorderStyle.None;
            this.ComboTable.BorderWidth = Unit.Pixel(0);
            if (this.RenderMode == ComboBoxRenderMode.Inline)
            {
                this.ComboTable.Style.Add(HtmlTextWriterStyle.Display, this.GetInlineDisplayStyle());
                if (!base.DesignMode)
                {
                    this.ComboTable.Style.Add(HtmlTextWriterStyle.Position, "relative");
                    this.ComboTable.Style.Add(HtmlTextWriterStyle.Top, "5px");
                }
            }
        }

        protected virtual void AddTextBoxAttributesToRender(HtmlTextWriter writer)
        {
            this.TextBoxControl.AutoCompleteType = AutoCompleteType.None;
            this.TextBoxControl.Attributes.Add("autocomplete", "off");
        }

        protected override void CreateChildControls()
        {
            if ((this.Controls.Count < 1) || (this.Controls[0] != this.ComboTable))
            {
                this.Controls.Clear();
                this.ComboTableTextBoxCell.Controls.Add(this.TextBoxControl);
                this.ComboTableButtonCell.Controls.Add(this.ButtonControl);
                this.Controls.Add(this.ComboTable);
                this.Controls.Add(this.OptionListControl);
                this.Controls.Add(this.HiddenFieldControl);
            }
        }

        private string GetInlineDisplayStyle()
        {
            string str = "inline";
            if (base.DesignMode || (!this.Page.Request.Browser.Browser.ToLower().Contains("safari") && !this.Page.Request.Browser.Browser.ToLower().Contains("firefox")))
            {
                return str;
            }
            return (str + "-block");
        }

        protected virtual IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            if (this.Visible)
            {
                List<ScriptDescriptor> list = new List<ScriptDescriptor>();
                ScriptControlDescriptor descriptor = new ScriptControlDescriptor(this.ClientControlType, this.ClientID);
                ScriptObjectBuilder.DescribeComponent(this, descriptor, this, this);
                descriptor.AddElementProperty("textBoxControl", this.TextBoxControl.ClientID);
                descriptor.AddElementProperty("buttonControl", this.ButtonControl.ClientID);
                descriptor.AddElementProperty("hiddenFieldControl", this.HiddenFieldControl.ClientID);
                descriptor.AddElementProperty("optionListControl", this.OptionListControl.ClientID);
                descriptor.AddElementProperty("comboTableControl", this.ComboTable.ClientID);
                descriptor.AddProperty("autoCompleteMode", this.AutoCompleteMode);
                descriptor.AddProperty("dropDownStyle", this.DropDownStyle);
                list.Add(descriptor);
                return list;
            }
            return null;
        }

        protected virtual IEnumerable<ScriptReference> GetScriptReferences()
        {
            if (this.Visible)
            {
                List<ScriptReference> list = new List<ScriptReference>();
                list.AddRange(ScriptObjectBuilder.GetScriptReferences(base.GetType(), false));
                return list;
            }
            return null;
        }

        protected virtual void InsertItem(ComboBoxItemInsertEventArgs e)
        {
            if (!e.Cancel)
            {
                int index = -1;
                if (e.InsertLocation == ComboBoxItemInsertLocation.Prepend)
                {
                    index = 0;
                }
                else if (e.InsertLocation == ComboBoxItemInsertLocation.Append)
                {
                    index = this.Items.Count;
                }
                else if (e.InsertLocation == ComboBoxItemInsertLocation.OrdinalText)
                {
                    index = 0;
                    foreach (ListItem item in this.Items)
                    {
                        if (string.Compare(e.Item.Text, item.Text, StringComparison.Ordinal) <= 0)
                        {
                            break;
                        }
                        index++;
                    }
                }
                else if (e.InsertLocation == ComboBoxItemInsertLocation.OrdinalValue)
                {
                    index = 0;
                    foreach (ListItem item2 in this.Items)
                    {
                        if (string.Compare(e.Item.Value, item2.Value, StringComparison.Ordinal) <= 0)
                        {
                            break;
                        }
                        index++;
                    }
                }
                if (index >= this.Items.Count)
                {
                    this.Items.Add(e.Item);
                    this.SelectedIndex = this.Items.Count - 1;
                }
                else
                {
                    this.Items.Insert(index, e.Item);
                    this.SelectedIndex = index;
                }
                this.OnItemInserted(e);
            }
        }

        protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            if (this.Enabled)
            {
                int num = Convert.ToInt32(postCollection.GetValues(this.HiddenFieldControl.UniqueID)[0], CultureInfo.InvariantCulture);
                this.EnsureDataBound();
                if ((num == -2) && ((this.DropDownStyle == ComboBoxStyle.Simple) || (this.DropDownStyle == ComboBoxStyle.DropDown)))
                {
                    string text = postCollection.GetValues(this.TextBoxControl.UniqueID)[0];
                    ComboBoxItemInsertEventArgs e = new ComboBoxItemInsertEventArgs(text, this.ItemInsertLocation);
                    this.OnItemInserting(e);
                    if (!e.Cancel)
                    {
                        this.InsertItem(e);
                    }
                    else
                    {
                        this.TextBoxControl.Text = (this.SelectedIndex < 0) ? string.Empty : this.SelectedItem.Text;
                    }
                }
                else if (num != this.SelectedIndex)
                {
                    this.SelectedIndex = num;
                    return true;
                }
            }
            return false;
        }

        protected virtual void OnItemInserted(ComboBoxItemInsertEventArgs e)
        {
            EventHandler<ComboBoxItemInsertEventArgs> handler = (EventHandler<ComboBoxItemInsertEventArgs>) base.Events[EventItemInserted];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnItemInserting(ComboBoxItemInsertEventArgs e)
        {
            EventHandler<ComboBoxItemInsertEventArgs> handler = (EventHandler<ComboBoxItemInsertEventArgs>) base.Events[EventItemInserting];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ScriptObjectBuilder.RegisterCssReferences(this);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            this.ScriptManager.RegisterScriptControl<ComboBox>(this);
            this.Page.RegisterRequiresPostBack(this);
        }

        public virtual void RaisePostDataChangedEvent()
        {
            this.OnSelectedIndexChanged(EventArgs.Empty);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            if (!base.DesignMode)
            {
                this.ScriptManager.RegisterScriptDescriptors(this);
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            this.ComboTable.RenderControl(writer);
            this.OptionListControl.Items.Clear();
            ListItem[] array = new ListItem[this.Items.Count];
            this.Items.CopyTo(array, 0);
            this.OptionListControl.Items.AddRange(array);
            this.OptionListControl.RenderControl(writer);
            this.HiddenFieldControl.RenderControl(writer);
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (base.DesignMode)
            {
                this.CreateChildControls();
                this.AddAttributesToRender(writer);
                this.ComboTable.RenderControl(writer);
            }
            else
            {
                this.HiddenFieldControl.Value = this.SelectedIndex.ToString();
                base.RenderControl(writer);
            }
        }

        public Control ResolveControl(string controlId) => 
            this.FindControl(controlId);

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) => 
            this.LoadPostData(postDataKey, postCollection);

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            this.RaisePostDataChangedEvent();
        }

        IEnumerable<ScriptDescriptor> IScriptControl.GetScriptDescriptors() => 
            this.GetScriptDescriptors();

        IEnumerable<ScriptReference> IScriptControl.GetScriptReferences() => 
            this.GetScriptReferences();

        [Description("Whether the ComboBox auto-completes typing by suggesting an item in the list or appending matches as the user types."), Category("Behavior"), DefaultValue(typeof(ComboBoxAutoCompleteMode), "None")]
        public virtual ComboBoxAutoCompleteMode AutoCompleteMode
        {
            get
            {
                object obj2 = this.ViewState["AutoCompleteMode"];
                if (obj2 == null)
                {
                    return ComboBoxAutoCompleteMode.None;
                }
                return (ComboBoxAutoCompleteMode) obj2;
            }
            set
            {
                this.ViewState["AutoCompleteMode"] = value;
            }
        }

        [ClientPropertyName("autoPostBack"), ExtenderControlProperty]
        public override bool AutoPostBack
        {
            get => 
                base.AutoPostBack;
            set
            {
                base.AutoPostBack = value;
            }
        }

        public override Color BackColor
        {
            get => 
                this.TextBoxControl.BackColor;
            set
            {
                this.TextBoxControl.BackColor = value;
            }
        }

        public override Color BorderColor
        {
            get => 
                this.TextBoxControl.BorderColor;
            set
            {
                this.TextBoxControl.BorderColor = value;
                this.ButtonControl.BorderColor = value;
                this.TextBoxControl.Style.Add("border-right", "0px none");
            }
        }

        public override System.Web.UI.WebControls.BorderStyle BorderStyle
        {
            get => 
                this.TextBoxControl.BorderStyle;
            set
            {
                this.TextBoxControl.BorderStyle = value;
                this.ButtonControl.BorderStyle = value;
            }
        }

        public override Unit BorderWidth
        {
            get => 
                this.TextBoxControl.BorderWidth;
            set
            {
                this.TextBoxControl.BorderWidth = value;
                this.ButtonControl.BorderWidth = value;
            }
        }

        protected virtual ComboBoxButton ButtonControl
        {
            get
            {
                if (this._buttonControl == null)
                {
                    this._buttonControl = new ComboBoxButton();
                    this._buttonControl.ID = this.ID + "_Button";
                }
                return this._buttonControl;
            }
        }

        [DefaultValue(false), ExtenderControlProperty, ClientPropertyName("caseSensitive"), Category("Behavior"), Description("Whether the ComboBox auto-completes user typing on a case-sensitive basis.")]
        public virtual bool CaseSensitive
        {
            get
            {
                object obj2 = this.ViewState["CaseSensitive"];
                if (obj2 == null)
                {
                    return false;
                }
                return (bool) obj2;
            }
            set
            {
                this.ViewState["CaseSensitive"] = value;
            }
        }

        protected virtual string ClientControlType
        {
            get
            {
                ClientScriptResourceAttribute attribute = (ClientScriptResourceAttribute) TypeDescriptor.GetAttributes(this)[typeof(ClientScriptResourceAttribute)];
                return attribute.ComponentType;
            }
        }

        protected virtual Table ComboTable
        {
            get
            {
                if (this._comboTable == null)
                {
                    this._comboTable = new Table();
                    this._comboTable.ID = this.ID + "_Table";
                    this._comboTable.Rows.Add(this.ComboTableRow);
                }
                return this._comboTable;
            }
        }

        protected virtual TableCell ComboTableButtonCell
        {
            get
            {
                if (this._comboTableButtonCell == null)
                {
                    this._comboTableButtonCell = new TableCell();
                }
                return this._comboTableButtonCell;
            }
        }

        protected virtual TableRow ComboTableRow
        {
            get
            {
                if (this._comboTableRow == null)
                {
                    this._comboTableRow = new TableRow();
                    this._comboTableRow.Cells.Add(this.ComboTableTextBoxCell);
                    this._comboTableRow.Cells.Add(this.ComboTableButtonCell);
                }
                return this._comboTableRow;
            }
        }

        protected virtual TableCell ComboTableTextBoxCell
        {
            get
            {
                if (this._comboTableTextBoxCell == null)
                {
                    this._comboTableTextBoxCell = new TableCell();
                }
                return this._comboTableTextBoxCell;
            }
        }

        [DefaultValue(typeof(ComboBoxStyle), "DropDown"), Description("Whether the ComboBox requires typed text to match an item in the list or allows new items to be created."), Category("Behavior")]
        public virtual ComboBoxStyle DropDownStyle
        {
            get
            {
                object obj2 = this.ViewState["DropDownStyle"];
                if (obj2 == null)
                {
                    return ComboBoxStyle.DropDown;
                }
                return (ComboBoxStyle) obj2;
            }
            set
            {
                this.ViewState["DropDownStyle"] = value;
            }
        }

        public override bool Enabled
        {
            get => 
                base.Enabled;
            set
            {
                base.Enabled = value;
                this.TextBoxControl.Enabled = base.Enabled;
                this.ButtonControl.Enabled = base.Enabled;
            }
        }

        public override FontInfo Font =>
            this.TextBoxControl.Font;

        public override Color ForeColor
        {
            get => 
                this.TextBoxControl.ForeColor;
            set
            {
                this.TextBoxControl.ForeColor = value;
            }
        }

        public override Unit Height
        {
            get => 
                this.TextBoxControl.Height;
            set
            {
                this.TextBoxControl.Height = value;
            }
        }

        protected virtual HiddenField HiddenFieldControl
        {
            get
            {
                if (this._hiddenFieldControl == null)
                {
                    this._hiddenFieldControl = new HiddenField();
                    this._hiddenFieldControl.ID = this.ID + "_HiddenField";
                }
                return this._hiddenFieldControl;
            }
        }

        [Category("Behavior"), Description("Whether a new item will be appended, prepended, or inserted ordinally into the items collection."), DefaultValue(typeof(ComboBoxItemInsertLocation), "Append")]
        public virtual ComboBoxItemInsertLocation ItemInsertLocation
        {
            get
            {
                object obj2 = this.ViewState["ItemInsertLocation"];
                if (obj2 == null)
                {
                    return ComboBoxItemInsertLocation.Append;
                }
                return (ComboBoxItemInsertLocation) obj2;
            }
            set
            {
                this.ViewState["ItemInsertLocation"] = value;
            }
        }

        [ExtenderControlProperty, ClientPropertyName("listItemHoverCssClass"), DefaultValue(""), Description("The CSS class to apply to a hovered item in the list."), Category("Style")]
        public virtual string ListItemHoverCssClass
        {
            get
            {
                object obj2 = this.ViewState["ListItemHoverCssClass"];
                if (obj2 == null)
                {
                    return "";
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["ListItemHoverCssClass"] = value;
            }
        }

        public virtual int MaxLength
        {
            get => 
                this.TextBoxControl.MaxLength;
            set
            {
                this.TextBoxControl.MaxLength = value;
            }
        }

        protected virtual System.Web.UI.WebControls.BulletedList OptionListControl
        {
            get
            {
                if (this._optionListControl == null)
                {
                    this._optionListControl = new System.Web.UI.WebControls.BulletedList();
                    this._optionListControl.ID = this.ID + "_OptionList";
                }
                return this._optionListControl;
            }
        }

        [DefaultValue(typeof(ComboBoxRenderMode), "Inline"), Description("Whether the ComboBox will render as a block or inline HTML element."), Category("Layout")]
        public ComboBoxRenderMode RenderMode
        {
            get
            {
                if (this.ViewState["RenderMode"] != null)
                {
                    return (ComboBoxRenderMode) this.ViewState["RenderMode"];
                }
                return ComboBoxRenderMode.Inline;
            }
            set
            {
                this.ViewState["RenderMode"] = value;
            }
        }

        protected virtual System.Web.UI.ScriptManager ScriptManager
        {
            get
            {
                if (this._scriptManager == null)
                {
                    this._scriptManager = System.Web.UI.ScriptManager.GetCurrent(this.Page);
                    if (this._scriptManager == null)
                    {
                        throw new HttpException(Resources.E_NoScriptManager);
                    }
                }
                return this._scriptManager;
            }
            set
            {
                this._scriptManager = value;
            }
        }

        [ExtenderControlProperty, ClientPropertyName("selectedIndex")]
        public override int SelectedIndex
        {
            get => 
                base.SelectedIndex;
            set
            {
                base.SelectedIndex = value;
            }
        }

        public override short TabIndex
        {
            get => 
                this.TextBoxControl.TabIndex;
            set
            {
                this.TextBoxControl.TabIndex = value;
            }
        }

        protected override HtmlTextWriterTag TagKey =>
            HtmlTextWriterTag.Div;

        protected virtual TextBox TextBoxControl
        {
            get
            {
                if (this._textBoxControl == null)
                {
                    this._textBoxControl = new TextBox();
                    this._textBoxControl.ID = this.ID + "_TextBox";
                }
                return this._textBoxControl;
            }
        }

        public override Unit Width
        {
            get => 
                this.TextBoxControl.Width;
            set
            {
                this.TextBoxControl.Width = value;
            }
        }
    }
}

