namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SiteMapDataSourceView : DataSourceView
    {
        private SiteMapNodeCollection _collection;
        private SiteMapDataSource _owner;

        public SiteMapDataSourceView(SiteMapDataSource owner, string name, SiteMapNode node) : base(owner, name)
        {
            this._owner = owner;
            this._collection = new SiteMapNodeCollection(node);
        }

        public SiteMapDataSourceView(SiteMapDataSource owner, string name, SiteMapNodeCollection collection) : base(owner, name)
        {
            this._owner = owner;
            this._collection = collection;
        }

        protected internal override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            arguments.RaiseUnsupportedCapabilitiesError(this);
            return this._collection;
        }

        protected override void OnDataSourceViewChanged(EventArgs e)
        {
            this._collection = this._owner.GetPathNodeCollection(base.Name);
            base.OnDataSourceViewChanged(e);
        }

        public IEnumerable Select(DataSourceSelectArguments arguments) => 
            this.ExecuteSelect(arguments);
    }
}

