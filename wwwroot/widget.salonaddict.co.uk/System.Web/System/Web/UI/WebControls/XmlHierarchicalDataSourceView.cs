namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;
    using System.Xml;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class XmlHierarchicalDataSourceView : HierarchicalDataSourceView
    {
        private XmlDataSource _owner;
        private string _viewPath;

        internal XmlHierarchicalDataSourceView(XmlDataSource owner, string viewPath)
        {
            this._owner = owner;
            this._viewPath = viewPath;
        }

        public override IHierarchicalEnumerable Select()
        {
            XmlNode xmlDocument = this._owner.GetXmlDocument();
            XmlNodeList nodeList = null;
            if (!string.IsNullOrEmpty(this._viewPath))
            {
                XmlNode node2 = xmlDocument.SelectSingleNode(this._viewPath);
                if (node2 != null)
                {
                    nodeList = node2.ChildNodes;
                }
            }
            else if (this._owner.XPath.Length > 0)
            {
                nodeList = xmlDocument.SelectNodes(this._owner.XPath);
            }
            else
            {
                nodeList = xmlDocument.ChildNodes;
            }
            return new XmlHierarchicalEnumerable(nodeList);
        }
    }
}

