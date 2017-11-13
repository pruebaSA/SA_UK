namespace System.Web.UI
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Globalization;
    using System.IO;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;

    [ToolboxItemFilter("System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", ToolboxItemFilterType.Require), PersistChildren(false), ToolboxBitmap(typeof(EmbeddedResourceFinder), "System.Web.Resources.UpdatePanel.bmp"), DefaultProperty("Triggers"), Designer("System.Web.UI.Design.UpdatePanelDesigner, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), ParseChildren(true), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class UpdatePanel : Control, IUpdatePanel
    {
        private bool _asyncPostBackMode;
        private bool _asyncPostBackModeInitialized;
        private bool _childrenAsTriggers;
        private ITemplate _contentTemplate;
        private Control _contentTemplateContainer;
        private bool _explicitUpdate;
        private System.Web.UI.IPage _page;
        private bool _panelRegistered;
        private bool _rendered;
        private UpdatePanelRenderMode _renderMode;
        private IScriptManagerInternal _scriptManager;
        private UpdatePanelTriggerCollection _triggers;
        private UpdatePanelUpdateMode _updateMode;
        private const string UpdatePanelToken = "updatePanel";

        public UpdatePanel()
        {
            this._childrenAsTriggers = true;
        }

        internal UpdatePanel(IScriptManagerInternal scriptManager, System.Web.UI.IPage page)
        {
            this._childrenAsTriggers = true;
            this._scriptManager = scriptManager;
            this._page = page;
        }

        private void AddContentTemplateContainer()
        {
            this.ChildControls.AddSingleChild(this._contentTemplateContainer);
        }

        internal void ClearContent()
        {
            this.ContentTemplateContainer.Controls.Clear();
            this._contentTemplateContainer = null;
            this.ChildControls.ClearInternal();
        }

        private void CreateContents()
        {
            if (base.DesignMode)
            {
                this.ClearContent();
            }
            if (this._contentTemplateContainer == null)
            {
                this._contentTemplateContainer = this.CreateContentTemplateContainer();
                if (this._contentTemplate != null)
                {
                    this._contentTemplate.InstantiateIn(this._contentTemplateContainer);
                }
                this.AddContentTemplateContainer();
            }
            else if (this._contentTemplate != null)
            {
                this._contentTemplate.InstantiateIn(this._contentTemplateContainer);
            }
        }

        protected virtual Control CreateContentTemplateContainer() => 
            new Control();

        protected sealed override ControlCollection CreateControlCollection() => 
            new SingleChildControlCollection(this);

        protected internal virtual void Initialize()
        {
            if ((this._triggers != null) && this.ScriptManager.SupportsPartialRendering)
            {
                this._triggers.Initialize();
            }
        }

        protected internal override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.RegisterPanel();
            if (this._contentTemplateContainer == null)
            {
                this._contentTemplateContainer = this.CreateContentTemplateContainer();
                this.AddContentTemplateContainer();
            }
        }

        protected internal override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!base.DesignMode && !this.ScriptManager.IsInAsyncPostBack)
            {
                this.Initialize();
            }
        }

        protected internal override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!this.ChildrenAsTriggers && (this.UpdateMode == UpdatePanelUpdateMode.Always))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.UpdatePanel_ChildrenTriggersAndUpdateAlways, new object[] { this.ID }));
            }
        }

        protected internal override void OnUnload(EventArgs e)
        {
            if (!base.DesignMode && this._panelRegistered)
            {
                this.ScriptManager.UnregisterUpdatePanel(this);
            }
            base.OnUnload(e);
        }

        private void RegisterPanel()
        {
            if (!base.DesignMode && !this._panelRegistered)
            {
                for (Control control = this.Parent; control != null; control = control.Parent)
                {
                    UpdatePanel panel = control as UpdatePanel;
                    if (panel != null)
                    {
                        panel.RegisterPanel();
                        break;
                    }
                }
                this.ScriptManager.RegisterUpdatePanel(this);
                this._panelRegistered = true;
            }
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            this.IPage.VerifyRenderingInServerForm(this);
            base.Render(writer);
        }

        protected internal override void RenderChildren(HtmlTextWriter writer)
        {
            if (this._asyncPostBackMode)
            {
                if (this._rendered)
                {
                    return;
                }
                HtmlTextWriter writer2 = new HtmlTextWriter(new StringWriter(CultureInfo.CurrentCulture));
                base.RenderChildren(writer2);
                PageRequestManager.EncodeString(writer, "updatePanel", this.ClientID, writer2.InnerWriter.ToString());
            }
            else
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
                if (this.RenderMode == UpdatePanelRenderMode.Block)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                }
                else
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                }
                base.RenderChildren(writer);
                writer.RenderEndTag();
            }
            this._rendered = true;
        }

        internal void SetAsyncPostBackMode(bool asyncPostBackMode)
        {
            if (this._asyncPostBackModeInitialized)
            {
                throw new InvalidOperationException(AtlasWeb.UpdatePanel_SetPartialRenderingModeCalledOnce);
            }
            this._asyncPostBackMode = asyncPostBackMode;
            this._asyncPostBackModeInitialized = true;
        }

        public void Update()
        {
            if (this.UpdateMode == UpdatePanelUpdateMode.Always)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.UpdatePanel_UpdateConditional, new object[] { this.ID }));
            }
            if (this._asyncPostBackModeInitialized)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.UpdatePanel_UpdateTooLate, new object[] { this.ID }));
            }
            this._explicitUpdate = true;
        }

        private SingleChildControlCollection ChildControls =>
            (this.Controls as SingleChildControlCollection);

        [Category("Behavior"), ResourceDescription("UpdatePanel_ChildrenAsTriggers"), DefaultValue(true)]
        public bool ChildrenAsTriggers
        {
            get => 
                this._childrenAsTriggers;
            set
            {
                this._childrenAsTriggers = value;
            }
        }

        [Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateInstance(TemplateInstance.Single)]
        public ITemplate ContentTemplate
        {
            get => 
                this._contentTemplate;
            set
            {
                if (!base.DesignMode && (this._contentTemplate != null))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.UpdatePanel_CannotSetContentTemplate, new object[] { this.ID }));
                }
                this._contentTemplate = value;
                if (this._contentTemplate != null)
                {
                    this.CreateContents();
                }
            }
        }

        [Browsable(false)]
        public Control ContentTemplateContainer
        {
            get
            {
                if (this._contentTemplateContainer == null)
                {
                    this._contentTemplateContainer = this.CreateContentTemplateContainer();
                    this.AddContentTemplateContainer();
                }
                return this._contentTemplateContainer;
            }
        }

        public sealed override ControlCollection Controls =>
            base.Controls;

        private System.Web.UI.IPage IPage
        {
            get
            {
                if (this._page != null)
                {
                    return this._page;
                }
                Page page = this.Page;
                if (page == null)
                {
                    throw new InvalidOperationException(AtlasWeb.Common_PageCannotBeNull);
                }
                return new PageWrapper(page);
            }
        }

        [Browsable(false)]
        public bool IsInPartialRendering =>
            this._asyncPostBackMode;

        [ResourceDescription("UpdatePanel_RenderMode"), DefaultValue(0), Category("Layout")]
        public UpdatePanelRenderMode RenderMode
        {
            get => 
                this._renderMode;
            set
            {
                if ((value < UpdatePanelRenderMode.Block) || (value > UpdatePanelRenderMode.Inline))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._renderMode = value;
            }
        }

        protected internal virtual bool RequiresUpdate =>
            ((this._explicitUpdate || (this.UpdateMode == UpdatePanelUpdateMode.Always)) || (((this._triggers != null) && (this._triggers.Count != 0)) && this._triggers.HasTriggered()));

        internal IScriptManagerInternal ScriptManager
        {
            get
            {
                if (this._scriptManager == null)
                {
                    Page page = this.Page;
                    if (page == null)
                    {
                        throw new InvalidOperationException(AtlasWeb.Common_PageCannotBeNull);
                    }
                    this._scriptManager = System.Web.UI.ScriptManager.GetCurrent(page);
                    if (this._scriptManager == null)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.Common_ScriptManagerRequired, new object[] { this.ID }));
                    }
                }
                return this._scriptManager;
            }
        }

        [MergableProperty(false), Category("Behavior"), DefaultValue((string) null), Editor("System.Web.UI.Design.UpdatePanelTriggerCollectionEditor, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", typeof(UITypeEditor)), ResourceDescription("UpdatePanel_Triggers"), PersistenceMode(PersistenceMode.InnerProperty)]
        public UpdatePanelTriggerCollection Triggers
        {
            get
            {
                if (this._triggers == null)
                {
                    this._triggers = new UpdatePanelTriggerCollection(this);
                }
                return this._triggers;
            }
        }

        [Category("Behavior"), ResourceDescription("UpdatePanel_UpdateMode"), DefaultValue(0)]
        public UpdatePanelUpdateMode UpdateMode
        {
            get => 
                this._updateMode;
            set
            {
                if ((value < UpdatePanelUpdateMode.Always) || (value > UpdatePanelUpdateMode.Conditional))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._updateMode = value;
            }
        }

        private sealed class SingleChildControlCollection : ControlCollection
        {
            private bool _allowClear;

            public SingleChildControlCollection(Control owner) : base(owner)
            {
            }

            public override void Add(Control child)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.UpdatePanel_CannotModifyControlCollection, new object[] { base.Owner.ID }));
            }

            public override void AddAt(int index, Control child)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.UpdatePanel_CannotModifyControlCollection, new object[] { base.Owner.ID }));
            }

            internal void AddSingleChild(Control child)
            {
                base.Add(child);
            }

            public override void Clear()
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.UpdatePanel_CannotModifyControlCollection, new object[] { base.Owner.ID }));
            }

            internal void ClearInternal()
            {
                try
                {
                    this._allowClear = true;
                    base.Clear();
                }
                finally
                {
                    this._allowClear = false;
                }
            }

            public override void Remove(Control value)
            {
                if (!this._allowClear)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.UpdatePanel_CannotModifyControlCollection, new object[] { base.Owner.ID }));
                }
                base.Remove(value);
            }

            public override void RemoveAt(int index)
            {
                if (!this._allowClear)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.UpdatePanel_CannotModifyControlCollection, new object[] { base.Owner.ID }));
                }
                base.RemoveAt(index);
            }
        }
    }
}

