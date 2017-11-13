namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SiteMapHierarchicalDataSourceView : HierarchicalDataSourceView
    {
        private SiteMapNodeCollection _collection;

        public SiteMapHierarchicalDataSourceView(SiteMapNode node)
        {
            this._collection = new SiteMapNodeCollection(node);
        }

        public SiteMapHierarchicalDataSourceView(SiteMapNodeCollection collection)
        {
            this._collection = collection;
        }

        public override IHierarchicalEnumerable Select() => 
            this._collection;
    }
}

