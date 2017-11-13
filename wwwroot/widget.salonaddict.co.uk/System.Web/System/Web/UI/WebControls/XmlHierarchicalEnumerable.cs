﻿namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Web.UI;
    using System.Xml;

    internal sealed class XmlHierarchicalEnumerable : IHierarchicalEnumerable, IEnumerable
    {
        private XmlNodeList _nodeList;
        private string _path;

        internal XmlHierarchicalEnumerable(XmlNodeList nodeList)
        {
            this._nodeList = nodeList;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerator enumerator = this._nodeList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                XmlNode current = (XmlNode) enumerator.Current;
                if (current.NodeType == XmlNodeType.Element)
                {
                    yield return new XmlHierarchyData(this, current);
                }
            }
            IDisposable iteratorVariable2 = enumerator as IDisposable;
            if (iteratorVariable2 != null)
            {
                iteratorVariable2.Dispose();
            }
        }

        IHierarchyData IHierarchicalEnumerable.GetHierarchyData(object enumeratedItem) => 
            ((IHierarchyData) enumeratedItem);

        internal string Path
        {
            get => 
                this._path;
            set
            {
                this._path = value;
            }
        }

    }
}

