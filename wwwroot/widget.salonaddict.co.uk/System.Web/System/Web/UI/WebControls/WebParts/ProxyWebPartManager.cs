namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [ParseChildren(true), Bindable(false), NonVisualControl, PersistChildren(false), Designer("System.Web.UI.Design.WebControls.WebParts.ProxyWebPartManagerDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ProxyWebPartManager : Control
    {
        private ProxyWebPartConnectionCollection _staticConnections;

        protected override ControlCollection CreateControlCollection() => 
            new EmptyControlCollection(this);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Focus()
        {
            throw new NotSupportedException(System.Web.SR.GetString("NoFocusSupport", new object[] { base.GetType().Name }));
        }

        protected internal override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Page page = this.Page;
            if ((page != null) && !base.DesignMode)
            {
                WebPartManager currentWebPartManager = WebPartManager.GetCurrentWebPartManager(page);
                if (currentWebPartManager == null)
                {
                    throw new InvalidOperationException(System.Web.SR.GetString("WebPartManagerRequired"));
                }
                this.StaticConnections.SetWebPartManager(currentWebPartManager);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ClientID =>
            base.ClientID;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ControlCollection Controls =>
            base.Controls;

        [DefaultValue(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool EnableTheming
        {
            get => 
                false;
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("NoThemingSupport", new object[] { base.GetType().Name }));
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), DefaultValue("")]
        public override string SkinID
        {
            get => 
                string.Empty;
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("NoThemingSupport", new object[] { base.GetType().Name }));
            }
        }

        [WebSysDescription("WebPartManager_StaticConnections"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), EditorBrowsable(EditorBrowsableState.Never), MergableProperty(false), PersistenceMode(PersistenceMode.InnerProperty), WebCategory("Behavior")]
        public ProxyWebPartConnectionCollection StaticConnections
        {
            get
            {
                if (this._staticConnections == null)
                {
                    this._staticConnections = new ProxyWebPartConnectionCollection();
                }
                return this._staticConnections;
            }
        }

        [Browsable(false), DefaultValue(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Visible
        {
            get => 
                false;
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("ControlNonVisual", new object[] { base.GetType().Name }));
            }
        }
    }
}

