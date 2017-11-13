namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [Designer("System.Web.UI.Design.WebControls.WebParts.CatalogPartDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), Bindable(false), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class CatalogPart : Part
    {
        private System.Web.UI.WebControls.WebParts.WebPartManager _webPartManager;
        private CatalogZoneBase _zone;

        protected CatalogPart()
        {
        }

        public abstract WebPartDescriptionCollection GetAvailableWebPartDescriptions();
        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        protected override IDictionary GetDesignModeState() => 
            new HybridDictionary(1) { ["Zone"] = this.Zone };

        public abstract WebPart GetWebPart(WebPartDescription description);
        protected internal override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.Zone == null)
            {
                throw new InvalidOperationException(System.Web.SR.GetString("CatalogPart_MustBeInZone", new object[] { this.ID }));
            }
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        protected override void SetDesignModeState(IDictionary data)
        {
            if (data != null)
            {
                object obj2 = data["Zone"];
                if (obj2 != null)
                {
                    this.SetZone((CatalogZoneBase) obj2);
                }
            }
        }

        internal void SetWebPartManager(System.Web.UI.WebControls.WebParts.WebPartManager webPartManager)
        {
            this._webPartManager = webPartManager;
        }

        internal void SetZone(CatalogZoneBase zone)
        {
            this._zone = zone;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string DisplayTitle
        {
            get
            {
                string title = this.Title;
                if (string.IsNullOrEmpty(title))
                {
                    title = System.Web.SR.GetString("Part_Untitled");
                }
                return title;
            }
        }

        protected System.Web.UI.WebControls.WebParts.WebPartManager WebPartManager =>
            this._webPartManager;

        protected CatalogZoneBase Zone =>
            this._zone;
    }
}

