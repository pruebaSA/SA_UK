namespace AjaxControlToolkit.HTMLEditor
{
    using AjaxControlToolkit;
    using AjaxControlToolkit.HTMLEditor.ToolbarButton;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ClientCssResource("HTMLEditor.Editor.css"), ParseChildren(true), PersistChildren(false), ToolboxBitmap(typeof(Editor), "HTMLEditor.Editor.ico"), RequiredScript(typeof(CommonToolkitScripts)), RequiredScript(typeof(Enums)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.Editor", "HTMLEditor.Editor.js"), Designer("AjaxControlToolkit.EditorDesigner, AjaxControlToolkit"), ValidationProperty("Content")]
    public class Editor : ScriptControlBase
    {
        internal Toolbar _bottomToolbar;
        private TableRow _bottomToolbarRow;
        private Toolbar _changingToolbar;
        private AjaxControlToolkit.HTMLEditor.EditPanel _editPanel;
        private TableCell _editPanelCell;
        internal Toolbar _topToolbar;
        private TableRow _topToolbarRow;
        private bool _wasPreRender;

        [Category("Behavior")]
        public event ContentChangedEventHandler ContentChanged
        {
            add
            {
                this.EditPanel.Events.AddHandler(AjaxControlToolkit.HTMLEditor.EditPanel.EventContentChanged, value);
            }
            remove
            {
                this.EditPanel.Events.RemoveHandler(AjaxControlToolkit.HTMLEditor.EditPanel.EventContentChanged, value);
            }
        }

        public Editor() : base(false, HtmlTextWriterTag.Div)
        {
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (!base.ControlStyleCreated || this.IsDesign)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, (this.IsDesign ? "ajax__htmleditor_editor_base " : "") + "ajax__htmleditor_editor_default");
            }
            base.AddAttributesToRender(writer);
        }

        protected override void CreateChildControls()
        {
            this.BottomToolbar.Buttons.Clear();
            this.FillBottomToolbar();
            if (this.BottomToolbar.Buttons.Count == 0)
            {
                if (this.EditPanel.Toolbars.Contains(this.BottomToolbar))
                {
                    this.EditPanel.Toolbars.Remove(this.BottomToolbar);
                }
                this._bottomToolbarRow.Visible = false;
                (this.EditPanel.Parent as TableCell).Style["border-bottom-width"] = "0px";
            }
            else
            {
                this.BottomToolbar.AlwaysVisible = true;
                this.BottomToolbar.ButtonImagesFolder = this.ButtonImagesFolder;
                for (int i = 0; i < this.BottomToolbar.Buttons.Count; i++)
                {
                    this.BottomToolbar.Buttons[i].IgnoreTab = this.IgnoreTab;
                }
            }
            this.TopToolbar.Buttons.Clear();
            this.FillTopToolbar();
            if (this.TopToolbar.Buttons.Count == 0)
            {
                if (this.EditPanel.Toolbars.Contains(this.TopToolbar))
                {
                    this.EditPanel.Toolbars.Remove(this.TopToolbar);
                }
                this._topToolbarRow.Visible = false;
                (this.EditPanel.Parent as TableCell).Style["border-top-width"] = "0px";
                this._changingToolbar = null;
            }
            else
            {
                this.TopToolbar.ButtonImagesFolder = this.ButtonImagesFolder;
                for (int j = 0; j < this.TopToolbar.Buttons.Count; j++)
                {
                    this.TopToolbar.Buttons[j].IgnoreTab = this.IgnoreTab;
                    this.TopToolbar.Buttons[j].PreservePlace = this.TopToolbarPreservePlace;
                }
            }
            if (!this.Height.IsEmpty)
            {
                (this.Controls[0] as Table).Style.Add(HtmlTextWriterStyle.Height, this.Height.ToString());
            }
            if (!this.Width.IsEmpty)
            {
                (this.Controls[0] as Table).Style.Add(HtmlTextWriterStyle.Width, this.Width.ToString());
            }
            if (AjaxControlToolkit.HTMLEditor.EditPanel.IE(this.Page) && !this.IsDesign)
            {
                this._editPanelCell.Style[HtmlTextWriterStyle.Height] = "expression(Sys.Extended.UI.HTMLEditor.Editor.MidleCellHeightForIE(this.parentNode.parentNode.parentNode,this.parentNode))";
            }
            this.EditPanel.IgnoreTab = this.IgnoreTab;
        }

        internal void CreateChilds(DesignerWithMapPath designer)
        {
            this.CreateChildControls();
            this.TopToolbar.CreateChilds(designer);
            this.BottomToolbar.CreateChilds(designer);
            this.EditPanel.SetDesigner(designer);
        }

        protected override Style CreateControlStyle() => 
            new EditorStyle(this.ViewState) { CssClass = "ajax__htmleditor_editor_default" };

        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            descriptor.AddComponentProperty("editPanel", this.EditPanel.ClientID);
            if (this._changingToolbar != null)
            {
                descriptor.AddComponentProperty("changingToolbar", this._changingToolbar.ClientID);
            }
        }

        protected virtual void FillBottomToolbar()
        {
            this.BottomToolbar.Buttons.Add(new DesignMode());
            this.BottomToolbar.Buttons.Add(new HtmlMode());
            this.BottomToolbar.Buttons.Add(new PreviewMode());
        }

        protected virtual void FillTopToolbar()
        {
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.Undo());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.Redo());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.Bold());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.Italic());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.Underline());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.StrikeThrough());
            this.TopToolbar.Buttons.Add(new SubScript());
            this.TopToolbar.Buttons.Add(new SuperScript());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            this.TopToolbar.Buttons.Add(new Ltr());
            this.TopToolbar.Buttons.Add(new Rtl());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            FixedForeColor item = new FixedForeColor();
            this.TopToolbar.Buttons.Add(item);
            AjaxControlToolkit.HTMLEditor.ToolbarButton.ForeColorSelector selector = new AjaxControlToolkit.HTMLEditor.ToolbarButton.ForeColorSelector {
                FixedColorButtonId = item.ID = "FixedForeColor"
            };
            this.TopToolbar.Buttons.Add(selector);
            this.TopToolbar.Buttons.Add(new ForeColorClear());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            FixedBackColor color2 = new FixedBackColor();
            this.TopToolbar.Buttons.Add(color2);
            BackColorSelector selector2 = new BackColorSelector {
                FixedColorButtonId = color2.ID = "FixedBackColor"
            };
            this.TopToolbar.Buttons.Add(selector2);
            this.TopToolbar.Buttons.Add(new BackColorClear());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            this.TopToolbar.Buttons.Add(new RemoveStyles());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            FontName name = new FontName();
            this.TopToolbar.Buttons.Add(name);
            Collection<SelectOption> options = name.Options;
            SelectOption option = new SelectOption {
                Value = "arial,helvetica,sans-serif",
                Text = "Arial"
            };
            options.Add(option);
            option = new SelectOption {
                Value = "courier new,courier,monospace",
                Text = "Courier New"
            };
            options.Add(option);
            option = new SelectOption {
                Value = "georgia,times new roman,times,serif",
                Text = "Georgia"
            };
            options.Add(option);
            option = new SelectOption {
                Value = "tahoma,arial,helvetica,sans-serif",
                Text = "Tahoma"
            };
            options.Add(option);
            option = new SelectOption {
                Value = "times new roman,times,serif",
                Text = "Times New Roman"
            };
            options.Add(option);
            option = new SelectOption {
                Value = "verdana,arial,helvetica,sans-serif",
                Text = "Verdana"
            };
            options.Add(option);
            option = new SelectOption {
                Value = "impact",
                Text = "Impact"
            };
            options.Add(option);
            option = new SelectOption {
                Value = "wingdings",
                Text = "WingDings"
            };
            options.Add(option);
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            AjaxControlToolkit.HTMLEditor.ToolbarButton.FontSize size = new AjaxControlToolkit.HTMLEditor.ToolbarButton.FontSize();
            this.TopToolbar.Buttons.Add(size);
            options = size.Options;
            option = new SelectOption {
                Value = "8pt",
                Text = "1 ( 8 pt)"
            };
            options.Add(option);
            option = new SelectOption {
                Value = "10pt",
                Text = "2 (10 pt)"
            };
            options.Add(option);
            option = new SelectOption {
                Value = "12pt",
                Text = "3 (12 pt)"
            };
            options.Add(option);
            option = new SelectOption {
                Value = "14pt",
                Text = "4 (14 pt)"
            };
            options.Add(option);
            option = new SelectOption {
                Value = "18pt",
                Text = "5 (18 pt)"
            };
            options.Add(option);
            option = new SelectOption {
                Value = "24pt",
                Text = "6 (24 pt)"
            };
            options.Add(option);
            option = new SelectOption {
                Value = "36pt",
                Text = "7 (36 pt)"
            };
            options.Add(option);
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.Cut());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.Copy());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.Paste());
            this.TopToolbar.Buttons.Add(new PasteText());
            this.TopToolbar.Buttons.Add(new PasteWord());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            this.TopToolbar.Buttons.Add(new DecreaseIndent());
            this.TopToolbar.Buttons.Add(new IncreaseIndent());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            this.TopToolbar.Buttons.Add(new Paragraph());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.JustifyLeft());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.JustifyCenter());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.JustifyRight());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.JustifyFull());
            this.TopToolbar.Buttons.Add(new RemoveAlignment());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            this.TopToolbar.Buttons.Add(new OrderedList());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.BulletedList());
            this.TopToolbar.Buttons.Add(new AjaxControlToolkit.HTMLEditor.ToolbarButton.HorizontalSeparator());
            this.TopToolbar.Buttons.Add(new InsertHR());
            this.TopToolbar.Buttons.Add(new InsertLink());
            this.TopToolbar.Buttons.Add(new RemoveLink());
        }

        protected override void OnInit(EventArgs e)
        {
            TableRow row;
            base.OnInit(e);
            this.EditPanel.Toolbars.Add(this.BottomToolbar);
            this._changingToolbar = this.TopToolbar;
            this.EditPanel.Toolbars.Add(this.TopToolbar);
            Table child = new Table {
                CellPadding = 0,
                CellSpacing = 0,
                CssClass = "ajax__htmleditor_editor_container",
                Style = { [HtmlTextWriterStyle.BorderCollapse] = "separate" }
            };
            this._topToolbarRow = row = new TableRow();
            TableCell cell = new TableCell {
                Controls = { this.TopToolbar },
                CssClass = "ajax__htmleditor_editor_toptoolbar"
            };
            row.Cells.Add(cell);
            child.Rows.Add(row);
            row = new TableRow();
            this._editPanelCell = cell = new TableCell();
            cell.CssClass = "ajax__htmleditor_editor_editpanel";
            cell.Controls.Add(this.EditPanel);
            row.Cells.Add(cell);
            child.Rows.Add(row);
            this._bottomToolbarRow = row = new TableRow();
            cell = new TableCell {
                Controls = { this.BottomToolbar },
                CssClass = "ajax__htmleditor_editor_bottomtoolbar"
            };
            row.Cells.Add(cell);
            child.Rows.Add(row);
            this.Controls.Add(child);
        }

        protected override void OnPreRender(EventArgs e)
        {
            try
            {
                base.OnPreRender(e);
            }
            catch
            {
            }
            this._wasPreRender = true;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!this._wasPreRender)
            {
                this.OnPreRender(new EventArgs());
            }
            base.Render(writer);
        }

        [DefaultValue(0), Category("Behavior")]
        public virtual ActiveModeType ActiveMode
        {
            get => 
                this.EditPanel.ActiveMode;
            set
            {
                this.EditPanel.ActiveMode = value;
            }
        }

        [Category("Behavior"), DefaultValue(true)]
        public virtual bool AutoFocus
        {
            get => 
                this.EditPanel.AutoFocus;
            set
            {
                this.EditPanel.AutoFocus = value;
            }
        }

        protected Toolbar BottomToolbar
        {
            get
            {
                if (this._bottomToolbar == null)
                {
                    this._bottomToolbar = new ToolbarInstance();
                }
                return this._bottomToolbar;
            }
        }

        [DefaultValue(""), Category("Appearance"), Description("Folder used for toolbar's buttons' images")]
        public virtual string ButtonImagesFolder
        {
            get => 
                (this.ViewState["ButtonImagesFolder"] ?? "");
            set
            {
                this.ViewState["ButtonImagesFolder"] = value;
            }
        }

        [DefaultValue(""), Category("Appearance")]
        public virtual string Content
        {
            get => 
                this.EditPanel.Content;
            set
            {
                this.EditPanel.Content = value;
            }
        }

        [Category("Appearance"), DefaultValue("ajax__htmleditor_editor_default")]
        public override string CssClass
        {
            get => 
                base.CssClass;
            set
            {
                base.CssClass = value;
            }
        }

        [DefaultValue(""), Category("Appearance")]
        public virtual string DesignPanelCssPath
        {
            get => 
                this.EditPanel.DesignPanelCssPath;
            set
            {
                this.EditPanel.DesignPanelCssPath = value;
            }
        }

        [DefaultValue(""), Category("Appearance")]
        public virtual string DocumentCssPath
        {
            get => 
                this.EditPanel.DocumentCssPath;
            set
            {
                this.EditPanel.DocumentCssPath = value;
            }
        }

        internal AjaxControlToolkit.HTMLEditor.EditPanel EditPanel
        {
            get
            {
                if (this._editPanel == null)
                {
                    this._editPanel = new EditPanelInstance();
                }
                return this._editPanel;
            }
        }

        [Category("Appearance"), DefaultValue(typeof(Unit), "")]
        public override Unit Height
        {
            get => 
                base.Height;
            set
            {
                base.Height = value;
            }
        }

        [DefaultValue("ajax__htmleditor_htmlpanel_default"), Category("Appearance")]
        public virtual string HtmlPanelCssClass
        {
            get => 
                this.EditPanel.HtmlPanelCssClass;
            set
            {
                this.EditPanel.HtmlPanelCssClass = value;
            }
        }

        [DefaultValue(false), Category("Behavior")]
        public virtual bool IgnoreTab
        {
            get => 
                (this.ViewState["IgnoreTab"] ?? false);
            set
            {
                this.ViewState["IgnoreTab"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false)]
        public virtual bool InitialCleanUp
        {
            get => 
                this.EditPanel.InitialCleanUp;
            set
            {
                this.EditPanel.InitialCleanUp = value;
            }
        }

        protected bool IsDesign
        {
            get
            {
                try
                {
                    bool designMode = false;
                    if (this.Context == null)
                    {
                        designMode = true;
                    }
                    else if (base.Site != null)
                    {
                        designMode = base.Site.DesignMode;
                    }
                    else
                    {
                        designMode = false;
                    }
                    return designMode;
                }
                catch
                {
                    return true;
                }
            }
        }

        [DefaultValue(false), Category("Behavior")]
        public virtual bool NoScript
        {
            get => 
                this.EditPanel.NoScript;
            set
            {
                this.EditPanel.NoScript = value;
            }
        }

        [Category("Behavior"), DefaultValue(false)]
        public virtual bool NoUnicode
        {
            get => 
                this.EditPanel.NoUnicode;
            set
            {
                this.EditPanel.NoUnicode = value;
            }
        }

        [DefaultValue(""), Category("Behavior")]
        public virtual string OnClientActiveModeChanged
        {
            get => 
                this.EditPanel.OnClientActiveModeChanged;
            set
            {
                this.EditPanel.OnClientActiveModeChanged = value;
            }
        }

        [Category("Behavior"), DefaultValue("")]
        public virtual string OnClientBeforeActiveModeChanged
        {
            get => 
                this.EditPanel.OnClientBeforeActiveModeChanged;
            set
            {
                this.EditPanel.OnClientBeforeActiveModeChanged = value;
            }
        }

        [Category("Behavior"), DefaultValue(false)]
        public virtual bool SuppressTabInDesignMode
        {
            get => 
                this.EditPanel.SuppressTabInDesignMode;
            set
            {
                this.EditPanel.SuppressTabInDesignMode = value;
            }
        }

        protected Toolbar TopToolbar
        {
            get
            {
                if (this._topToolbar == null)
                {
                    this._topToolbar = new ToolbarInstance();
                }
                return this._topToolbar;
            }
        }

        [DefaultValue(false)]
        public virtual bool TopToolbarPreservePlace
        {
            get => 
                (this.ViewState["TopToolbarPreservePlace"] ?? false);
            set
            {
                this.ViewState["TopToolbarPreservePlace"] = value;
            }
        }

        [DefaultValue(typeof(Unit), ""), Category("Appearance")]
        public override Unit Width
        {
            get => 
                base.Width;
            set
            {
                base.Width = value;
            }
        }

        private sealed class EditorStyle : Style
        {
            public EditorStyle(StateBag state) : base(state)
            {
            }

            protected override void FillStyleAttributes(CssStyleCollection attributes, IUrlResolutionService urlResolver)
            {
                base.FillStyleAttributes(attributes, urlResolver);
                attributes.Remove(HtmlTextWriterStyle.Height);
                attributes.Remove(HtmlTextWriterStyle.Width);
            }
        }
    }
}

