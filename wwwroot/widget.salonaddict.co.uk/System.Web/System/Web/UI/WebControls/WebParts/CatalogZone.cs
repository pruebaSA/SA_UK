namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [Designer("System.Web.UI.Design.WebControls.WebParts.CatalogZoneDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), SupportsEventValidation, AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class CatalogZone : CatalogZoneBase
    {
        private ITemplate _zoneTemplate;

        protected override CatalogPartCollection CreateCatalogParts()
        {
            CatalogPartCollection parts = new CatalogPartCollection();
            if (this._zoneTemplate != null)
            {
                Control container = new NonParentingControl();
                this._zoneTemplate.InstantiateIn(container);
                if (!container.HasControls())
                {
                    return parts;
                }
                foreach (Control control2 in container.Controls)
                {
                    CatalogPart part = control2 as CatalogPart;
                    if (part != null)
                    {
                        parts.Add(part);
                    }
                    else
                    {
                        LiteralControl control3 = control2 as LiteralControl;
                        if (((control3 == null) || (control3.Text.Trim().Length != 0)) && !base.DesignMode)
                        {
                            throw new InvalidOperationException(System.Web.SR.GetString("CatalogZone_OnlyCatalogParts", new object[] { this.ID }));
                        }
                    }
                }
            }
            return parts;
        }

        [Browsable(false), TemplateContainer(typeof(CatalogZone)), TemplateInstance(TemplateInstance.Single), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate ZoneTemplate
        {
            get => 
                this._zoneTemplate;
            set
            {
                base.InvalidateCatalogParts();
                this._zoneTemplate = value;
            }
        }
    }
}

