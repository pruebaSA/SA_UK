namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;
    using System.Xml;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class XmlDataSourceView : DataSourceView
    {
        private XmlDataSource _owner;

        public XmlDataSourceView(XmlDataSource owner, string name) : base(owner, name)
        {
            this._owner = owner;
        }

        protected internal override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            arguments.RaiseUnsupportedCapabilitiesError(this);
            XmlNode xmlDocument = this._owner.GetXmlDocument();
            XmlNodeList nodes = null;
            if (this._owner.XPath.Length != 0)
            {
                nodes = xmlDocument.SelectNodes(this._owner.XPath);
            }
            else
            {
                nodes = xmlDocument.SelectNodes("/node()/node()");
            }
            return new XmlDataSourceNodeDescriptorEnumeration(nodes);
        }

        public IEnumerable Select(DataSourceSelectArguments arguments) => 
            this.ExecuteSelect(arguments);

        private class XmlDataSourceNodeDescriptorEnumeration : ICollection, IEnumerable
        {
            private int _count = -1;
            private XmlNodeList _nodes;

            public XmlDataSourceNodeDescriptorEnumeration(XmlNodeList nodes)
            {
                this._nodes = nodes;
            }

            void ICollection.CopyTo(Array array, int index)
            {
                IEnumerator enumerator = ((IEnumerable) this).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    array.SetValue(enumerator.Current, index++);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                IEnumerator enumerator = this._nodes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    XmlNode current = (XmlNode) enumerator.Current;
                    if (current.NodeType == XmlNodeType.Element)
                    {
                        yield return new XmlDataSourceNodeDescriptor(current);
                    }
                }
                IDisposable iteratorVariable2 = enumerator as IDisposable;
                if (iteratorVariable2 != null)
                {
                    iteratorVariable2.Dispose();
                }
            }

            int ICollection.Count
            {
                get
                {
                    if (this._count == -1)
                    {
                        this._count = 0;
                        foreach (XmlNode node in this._nodes)
                        {
                            if (node.NodeType == XmlNodeType.Element)
                            {
                                this._count++;
                            }
                        }
                    }
                    return this._count;
                }
            }

            bool ICollection.IsSynchronized =>
                false;

            object ICollection.SyncRoot =>
                null;

        }
    }
}

