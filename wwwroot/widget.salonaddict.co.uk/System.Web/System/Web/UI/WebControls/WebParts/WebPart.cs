namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [Designer("System.Web.UI.Design.WebControls.WebParts.WebPartDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class WebPart : Part, IWebPart, IWebActionable, IWebEditable
    {
        private bool _allowClose = true;
        private bool _allowConnect = true;
        private bool _allowEdit = true;
        private bool _allowHide = true;
        private bool _allowMinimize = true;
        private bool _allowZoneChange = true;
        private string _authorizationFilter;
        private string _catalogIconImageUrl;
        private PartChromeState _chromeState = PartChromeState.Normal;
        private string _connectErrorMessage;
        private WebPartExportMode _exportMode = WebPartExportMode.None;
        private bool _hasSharedData;
        private bool _hasUserData;
        private WebPartHelpMode _helpMode = WebPartHelpMode.Navigate;
        private string _helpUrl;
        private bool _hidden;
        private string _importErrorMessage;
        private bool _isClosed;
        private bool _isShared;
        private bool _isStandalone = true;
        private bool _isStatic = true;
        private string _titleIconImageUrl;
        private string _titleUrl;
        private Dictionary<ProviderConnectionPoint, int> _trackerCounter;
        private System.Web.UI.WebControls.WebParts.WebPartManager _webPartManager;
        private WebPartZoneBase _zone;
        private string _zoneID;
        private int _zoneIndex;
        private const string titleBarIDPrefix = "WebPartTitle_";
        internal const string WholePartIDPrefix = "WebPart_";

        protected WebPart()
        {
        }

        public virtual EditorPartCollection CreateEditorParts() => 
            EditorPartCollection.Empty;

        protected internal virtual void OnClosing(EventArgs e)
        {
        }

        protected internal virtual void OnConnectModeChanged(EventArgs e)
        {
        }

        protected internal virtual void OnDeleting(EventArgs e)
        {
        }

        protected internal virtual void OnEditModeChanged(EventArgs e)
        {
        }

        internal override void PreRenderRecursiveInternal()
        {
            if (this.IsStandalone)
            {
                if (this.Hidden)
                {
                    throw new InvalidOperationException(System.Web.SR.GetString("WebPart_NotStandalone", new object[] { "Hidden", this.ID }));
                }
            }
            else if (!this.Visible)
            {
                throw new InvalidOperationException(System.Web.SR.GetString("WebPart_OnlyStandalone", new object[] { "Visible", this.ID }));
            }
            base.PreRenderRecursiveInternal();
        }

        internal void SetConnectErrorMessage(string connectErrorMessage)
        {
            if (string.IsNullOrEmpty(this._connectErrorMessage))
            {
                this._connectErrorMessage = connectErrorMessage;
            }
        }

        internal void SetHasSharedData(bool hasSharedData)
        {
            this._hasSharedData = hasSharedData;
        }

        internal void SetHasUserData(bool hasUserData)
        {
            this._hasUserData = hasUserData;
        }

        internal void SetIsClosed(bool isClosed)
        {
            this._isClosed = isClosed;
        }

        internal void SetIsShared(bool isShared)
        {
            this._isShared = isShared;
        }

        internal void SetIsStandalone(bool isStandalone)
        {
            this._isStandalone = isStandalone;
        }

        internal void SetIsStatic(bool isStatic)
        {
            this._isStatic = isStatic;
        }

        protected void SetPersonalizationDirty()
        {
            if (this.WebPartManager == null)
            {
                throw new InvalidOperationException(System.Web.SR.GetString("WebPartManagerRequired"));
            }
            this.WebPartManager.Personalization.SetDirty(this);
        }

        public static void SetPersonalizationDirty(Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (control.Page == null)
            {
                throw new ArgumentException(System.Web.SR.GetString("PropertyCannotBeNull", new object[] { "Page" }), "control");
            }
            System.Web.UI.WebControls.WebParts.WebPartManager currentWebPartManager = System.Web.UI.WebControls.WebParts.WebPartManager.GetCurrentWebPartManager(control.Page);
            if (currentWebPartManager == null)
            {
                throw new InvalidOperationException(System.Web.SR.GetString("WebPartManagerRequired"));
            }
            WebPart genericWebPart = currentWebPartManager.GetGenericWebPart(control);
            if (genericWebPart == null)
            {
                throw new ArgumentException(System.Web.SR.GetString("WebPart_NonWebPart"), "control");
            }
            genericWebPart.SetPersonalizationDirty();
        }

        internal void SetWebPartManager(System.Web.UI.WebControls.WebParts.WebPartManager webPartManager)
        {
            this._webPartManager = webPartManager;
        }

        internal void SetZoneIndex(int zoneIndex)
        {
            if (zoneIndex < 0)
            {
                throw new ArgumentOutOfRangeException("zoneIndex");
            }
            this._zoneIndex = zoneIndex;
        }

        internal Control ToControl()
        {
            GenericWebPart part = this as GenericWebPart;
            if (part == null)
            {
                return this;
            }
            Control childControl = part.ChildControl;
            if (childControl == null)
            {
                throw new InvalidOperationException(System.Web.SR.GetString("GenericWebPart_ChildControlIsNull"));
            }
            return childControl;
        }

        protected override void TrackViewState()
        {
            if (this.WebPartManager != null)
            {
                this.WebPartManager.Personalization.ApplyPersonalizationState(this);
            }
            base.TrackViewState();
        }

        [Personalizable(PersonalizationScope.Shared), DefaultValue(true), Themeable(false), WebCategory("WebPartBehavior"), WebSysDescription("WebPart_AllowClose")]
        public virtual bool AllowClose
        {
            get => 
                this._allowClose;
            set
            {
                this._allowClose = value;
            }
        }

        [Personalizable(PersonalizationScope.Shared), DefaultValue(true), WebSysDescription("WebPart_AllowConnect"), Themeable(false), WebCategory("WebPartBehavior")]
        public virtual bool AllowConnect
        {
            get => 
                this._allowConnect;
            set
            {
                this._allowConnect = value;
            }
        }

        [DefaultValue(true), WebSysDescription("WebPart_AllowEdit"), Personalizable(PersonalizationScope.Shared), Themeable(false), WebCategory("WebPartBehavior")]
        public virtual bool AllowEdit
        {
            get => 
                this._allowEdit;
            set
            {
                this._allowEdit = value;
            }
        }

        [Themeable(false), DefaultValue(true), Personalizable(PersonalizationScope.Shared), WebCategory("WebPartBehavior"), WebSysDescription("WebPart_AllowHide")]
        public virtual bool AllowHide
        {
            get => 
                this._allowHide;
            set
            {
                this._allowHide = value;
            }
        }

        [Themeable(false), Personalizable(PersonalizationScope.Shared), DefaultValue(true), WebCategory("WebPartBehavior"), WebSysDescription("WebPart_AllowMinimize")]
        public virtual bool AllowMinimize
        {
            get => 
                this._allowMinimize;
            set
            {
                this._allowMinimize = value;
            }
        }

        [DefaultValue(true), Personalizable(PersonalizationScope.Shared), Themeable(false), WebCategory("WebPartBehavior"), WebSysDescription("WebPart_AllowZoneChange")]
        public virtual bool AllowZoneChange
        {
            get => 
                this._allowZoneChange;
            set
            {
                this._allowZoneChange = value;
            }
        }

        [DefaultValue(""), WebSysDescription("WebPart_AuthorizationFilter"), Personalizable(PersonalizationScope.Shared), Themeable(false), WebCategory("WebPartBehavior")]
        public virtual string AuthorizationFilter
        {
            get
            {
                if (this._authorizationFilter == null)
                {
                    return string.Empty;
                }
                return this._authorizationFilter;
            }
            set
            {
                this._authorizationFilter = value;
            }
        }

        [UrlProperty, DefaultValue(""), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), WebSysDescription("WebPart_CatalogIconImageUrl"), WebCategory("WebPartAppearance"), Personalizable(PersonalizationScope.Shared)]
        public virtual string CatalogIconImageUrl
        {
            get
            {
                if (this._catalogIconImageUrl == null)
                {
                    return string.Empty;
                }
                return this._catalogIconImageUrl;
            }
            set
            {
                if (CrossSiteScriptingValidation.IsDangerousUrl(value))
                {
                    throw new ArgumentException(System.Web.SR.GetString("WebPart_BadUrl", new object[] { value }), "value");
                }
                this._catalogIconImageUrl = value;
            }
        }

        [Personalizable]
        public override PartChromeState ChromeState
        {
            get => 
                this._chromeState;
            set
            {
                if ((value < PartChromeState.Normal) || (value > PartChromeState.Minimized))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._chromeState = value;
            }
        }

        [Personalizable]
        public override PartChromeType ChromeType
        {
            get => 
                base.ChromeType;
            set
            {
                base.ChromeType = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ConnectErrorMessage
        {
            get
            {
                if (this._connectErrorMessage == null)
                {
                    return string.Empty;
                }
                return this._connectErrorMessage;
            }
        }

        [Personalizable(PersonalizationScope.Shared)]
        public override string Description
        {
            get => 
                base.Description;
            set
            {
                base.Description = value;
            }
        }

        [Personalizable]
        public override ContentDirection Direction
        {
            get => 
                base.Direction;
            set
            {
                base.Direction = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public string DisplayTitle
        {
            get
            {
                if (this._webPartManager != null)
                {
                    return this._webPartManager.GetDisplayTitle(this);
                }
                string title = this.Title;
                if (string.IsNullOrEmpty(title))
                {
                    title = System.Web.SR.GetString("Part_Untitled");
                }
                return title;
            }
        }

        [Themeable(false), Personalizable(PersonalizationScope.Shared), DefaultValue(0), WebCategory("WebPartBehavior"), WebSysDescription("WebPart_ExportMode")]
        public virtual WebPartExportMode ExportMode
        {
            get => 
                this._exportMode;
            set
            {
                if ((base.ControlState >= ControlState.Loaded) && ((this.WebPartManager == null) || ((this.WebPartManager.Personalization.Scope == PersonalizationScope.User) && this.IsShared)))
                {
                    throw new InvalidOperationException(System.Web.SR.GetString("WebPart_CantSetExportMode"));
                }
                if ((value < WebPartExportMode.None) || (value > WebPartExportMode.NonSensitiveData))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._exportMode = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool HasSharedData =>
            this._hasSharedData;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool HasUserData =>
            this._hasUserData;

        [Personalizable]
        public override Unit Height
        {
            get => 
                base.Height;
            set
            {
                base.Height = value;
            }
        }

        [Themeable(false), WebSysDescription("WebPart_HelpMode"), WebCategory("WebPartBehavior"), DefaultValue(2), Personalizable(PersonalizationScope.Shared)]
        public virtual WebPartHelpMode HelpMode
        {
            get => 
                this._helpMode;
            set
            {
                if ((value < WebPartHelpMode.Modal) || (value > WebPartHelpMode.Navigate))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._helpMode = value;
            }
        }

        [UrlProperty, Themeable(false), WebCategory("WebPartBehavior"), WebSysDescription("WebPart_HelpUrl"), DefaultValue(""), Personalizable(PersonalizationScope.Shared), Editor("System.Web.UI.Design.UrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public virtual string HelpUrl
        {
            get
            {
                if (this._helpUrl == null)
                {
                    return string.Empty;
                }
                return this._helpUrl;
            }
            set
            {
                if (CrossSiteScriptingValidation.IsDangerousUrl(value))
                {
                    throw new ArgumentException(System.Web.SR.GetString("WebPart_BadUrl", new object[] { value }), "value");
                }
                this._helpUrl = value;
            }
        }

        [WebCategory("WebPartAppearance"), Themeable(false), Personalizable, DefaultValue(false), WebSysDescription("WebPart_Hidden")]
        public virtual bool Hidden
        {
            get => 
                this._hidden;
            set
            {
                this._hidden = value;
            }
        }

        [Personalizable(PersonalizationScope.Shared), WebSysDefaultValue("WebPart_DefaultImportErrorMessage"), WebSysDescription("WebPart_ImportErrorMessage"), Localizable(true), WebCategory("WebPartAppearance")]
        public virtual string ImportErrorMessage
        {
            get
            {
                if (this._importErrorMessage == null)
                {
                    return System.Web.SR.GetString("WebPart_DefaultImportErrorMessage");
                }
                return this._importErrorMessage;
            }
            set
            {
                this._importErrorMessage = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsClosed =>
            this._isClosed;

        internal bool IsOrphaned =>
            ((this.Zone == null) && !this.IsClosed);

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsShared =>
            this._isShared;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsStandalone =>
            this._isStandalone;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool IsStatic =>
            this._isStatic;

        [Localizable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual string Subtitle =>
            string.Empty;

        [Personalizable]
        public override string Title
        {
            get => 
                base.Title;
            set
            {
                base.Title = value;
            }
        }

        internal string TitleBarID =>
            ("WebPartTitle_" + this.ID);

        [UrlProperty, Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), WebCategory("WebPartAppearance"), Personalizable(PersonalizationScope.Shared), DefaultValue(""), WebSysDescription("WebPart_TitleIconImageUrl")]
        public virtual string TitleIconImageUrl
        {
            get
            {
                if (this._titleIconImageUrl == null)
                {
                    return string.Empty;
                }
                return this._titleIconImageUrl;
            }
            set
            {
                if (CrossSiteScriptingValidation.IsDangerousUrl(value))
                {
                    throw new ArgumentException(System.Web.SR.GetString("WebPart_BadUrl", new object[] { value }), "value");
                }
                this._titleIconImageUrl = value;
            }
        }

        [Themeable(false), UrlProperty, Editor("System.Web.UI.Design.UrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), WebCategory("WebPartBehavior"), Personalizable(PersonalizationScope.Shared), WebSysDescription("WebPart_TitleUrl"), DefaultValue("")]
        public virtual string TitleUrl
        {
            get
            {
                if (this._titleUrl == null)
                {
                    return string.Empty;
                }
                return this._titleUrl;
            }
            set
            {
                if (CrossSiteScriptingValidation.IsDangerousUrl(value))
                {
                    throw new ArgumentException(System.Web.SR.GetString("WebPart_BadUrl", new object[] { value }), "value");
                }
                this._titleUrl = value;
            }
        }

        internal Dictionary<ProviderConnectionPoint, int> TrackerCounter
        {
            get
            {
                if (this._trackerCounter == null)
                {
                    this._trackerCounter = new Dictionary<ProviderConnectionPoint, int>();
                }
                return this._trackerCounter;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual WebPartVerbCollection Verbs =>
            WebPartVerbCollection.Empty;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual object WebBrowsableObject =>
            this;

        protected System.Web.UI.WebControls.WebParts.WebPartManager WebPartManager =>
            this._webPartManager;

        internal string WholePartID =>
            ("WebPart_" + this.ID);

        [Personalizable]
        public override Unit Width
        {
            get => 
                base.Width;
            set
            {
                base.Width = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WebPartZoneBase Zone
        {
            get
            {
                if (this._zone == null)
                {
                    string zoneID = this.ZoneID;
                    if (!string.IsNullOrEmpty(zoneID) && (this.WebPartManager != null))
                    {
                        WebPartZoneCollection zones = this.WebPartManager.Zones;
                        if (zones != null)
                        {
                            this._zone = zones[zoneID];
                        }
                    }
                }
                return this._zone;
            }
        }

        internal string ZoneID
        {
            get => 
                this._zoneID;
            set
            {
                if (this.ZoneID != value)
                {
                    this._zoneID = value;
                    this._zone = null;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public int ZoneIndex =>
            this._zoneIndex;

        internal sealed class ZoneIndexComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                WebPart part = (WebPart) x;
                WebPart part2 = (WebPart) y;
                int num = part.ZoneIndex - part2.ZoneIndex;
                if (num == 0)
                {
                    num = part.ID.CompareTo(part2.ID);
                }
                return num;
            }
        }
    }
}

