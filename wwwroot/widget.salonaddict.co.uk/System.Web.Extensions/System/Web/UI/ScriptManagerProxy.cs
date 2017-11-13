namespace System.Web.UI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;

    [Designer("System.Web.UI.Design.ScriptManagerProxyDesigner, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), DefaultProperty("Scripts"), ToolboxBitmap(typeof(EmbeddedResourceFinder), "System.Web.Resources.ScriptManagerProxy.bmp"), ToolboxItemFilter("System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", ToolboxItemFilterType.Require), ParseChildren(true), PersistChildren(false), NonVisualControl, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ScriptManagerProxy : Control, IControl, IClientUrlResolver
    {
        private AuthenticationServiceManager _authenticationServiceManager;
        private CompositeScriptReference _compositeScript;
        private static readonly object _navigateEvent = new object();
        private ProfileServiceManager _profileServiceManager;
        private RoleServiceManager _roleServiceManager;
        private IScriptManagerInternal _scriptManager;
        private ScriptReferenceCollection _scripts;
        private ServiceReferenceCollection _services;

        [ResourceDescription("ScriptManager_Navigate"), Category("Action")]
        public event EventHandler<HistoryEventArgs> Navigate
        {
            add
            {
                base.Events.AddHandler(_navigateEvent, value);
            }
            remove
            {
                base.Events.RemoveHandler(_navigateEvent, value);
            }
        }

        public ScriptManagerProxy()
        {
        }

        internal ScriptManagerProxy(IScriptManagerInternal scriptManager)
        {
            this._scriptManager = scriptManager;
        }

        internal void CollectScripts(List<ScriptReferenceBase> scripts)
        {
            if ((this._compositeScript != null) && (this._compositeScript.Scripts.Count != 0))
            {
                this._compositeScript.ClientUrlResolver = this;
                this._compositeScript.ContainingControl = this;
                this._compositeScript.IsStaticReference = true;
                scripts.Add(this._compositeScript);
            }
            if (this._scripts != null)
            {
                foreach (ScriptReference reference in this._scripts)
                {
                    reference.ClientUrlResolver = this;
                    reference.ContainingControl = this;
                    reference.IsStaticReference = true;
                    scripts.Add(reference);
                }
            }
        }

        protected internal override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!base.DesignMode)
            {
                this.ScriptManager.RegisterProxy(this);
            }
        }

        internal void RegisterServices(System.Web.UI.ScriptManager scriptManager)
        {
            if (this._services != null)
            {
                foreach (ServiceReference reference in this._services)
                {
                    reference.Register(this, this.Context, scriptManager, scriptManager.IsDebuggingEnabled);
                }
            }
        }

        string IClientUrlResolver.get_AppRelativeTemplateSourceDirectory() => 
            base.AppRelativeTemplateSourceDirectory;

        [PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false), ResourceDescription("ScriptManager_AuthenticationService"), Category("Behavior"), DefaultValue((string) null)]
        public AuthenticationServiceManager AuthenticationService
        {
            get
            {
                if (this._authenticationServiceManager == null)
                {
                    this._authenticationServiceManager = new AuthenticationServiceManager();
                }
                return this._authenticationServiceManager;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), ResourceDescription("ScriptManager_CompositeScript"), MergableProperty(false), Category("Behavior")]
        public CompositeScriptReference CompositeScript
        {
            get
            {
                if (this._compositeScript == null)
                {
                    this._compositeScript = new CompositeScriptReference();
                }
                return this._compositeScript;
            }
        }

        internal bool HasAuthenticationServiceManager =>
            (this._authenticationServiceManager != null);

        internal bool HasProfileServiceManager =>
            (this._profileServiceManager != null);

        internal bool HasRoleServiceManager =>
            (this._roleServiceManager != null);

        internal EventHandler<HistoryEventArgs> NavigateEvent =>
            ((EventHandler<HistoryEventArgs>) base.Events[_navigateEvent]);

        [Category("Behavior"), DefaultValue((string) null), MergableProperty(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), ResourceDescription("ScriptManager_ProfileService")]
        public ProfileServiceManager ProfileService
        {
            get
            {
                if (this._profileServiceManager == null)
                {
                    this._profileServiceManager = new ProfileServiceManager();
                }
                return this._profileServiceManager;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Category("Behavior"), DefaultValue((string) null), ResourceDescription("ScriptManager_RoleService")]
        public RoleServiceManager RoleService
        {
            get
            {
                if (this._roleServiceManager == null)
                {
                    this._roleServiceManager = new RoleServiceManager();
                }
                return this._roleServiceManager;
            }
        }

        private IScriptManagerInternal ScriptManager
        {
            get
            {
                if (this._scriptManager == null)
                {
                    if (this.Page == null)
                    {
                        throw new InvalidOperationException(AtlasWeb.Common_PageCannotBeNull);
                    }
                    this._scriptManager = System.Web.UI.ScriptManager.GetCurrent(this.Page);
                    if (this._scriptManager == null)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.Common_ScriptManagerRequired, new object[] { this.ID }));
                    }
                }
                return this._scriptManager;
            }
        }

        [Category("Behavior"), MergableProperty(false), ResourceDescription("ScriptManager_Scripts"), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), Editor("System.Web.UI.Design.CollectionEditorBase, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", typeof(UITypeEditor))]
        public ScriptReferenceCollection Scripts
        {
            get
            {
                if (this._scripts == null)
                {
                    this._scripts = new ScriptReferenceCollection();
                }
                return this._scripts;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), DefaultValue((string) null), ResourceDescription("ScriptManager_Services"), Category("Behavior"), Editor("System.Web.UI.Design.CollectionEditorBase, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", typeof(UITypeEditor))]
        public ServiceReferenceCollection Services
        {
            get
            {
                if (this._services == null)
                {
                    this._services = new ServiceReferenceCollection();
                }
                return this._services;
            }
        }

        HttpContextBase IControl.Context =>
            new System.Web.HttpContextWrapper(this.Context);

        bool IControl.DesignMode =>
            base.DesignMode;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Visible
        {
            get => 
                base.Visible;
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}

