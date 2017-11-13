namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.XPath;

    internal class DocumentOrderComparer : IComparer<XPathNavigator>
    {
        private List<XPathNavigator> roots;

        public int Compare(XPathNavigator navThis, XPathNavigator navThat)
        {
            switch (navThis.ComparePosition(navThat))
            {
                case XmlNodeOrder.Before:
                    return -1;

                case XmlNodeOrder.After:
                    return 1;

                case XmlNodeOrder.Same:
                    return 0;
            }
            if (this.roots == null)
            {
                this.roots = new List<XPathNavigator>();
            }
            if (this.GetDocumentIndex(navThis) >= this.GetDocumentIndex(navThat))
            {
                return 1;
            }
            return -1;
        }

        public int GetDocumentIndex(XPathNavigator nav)
        {
            if (this.roots == null)
            {
                this.roots = new List<XPathNavigator>();
            }
            XPathNavigator item = nav.Clone();
            item.MoveToRoot();
            for (int i = 0; i < this.roots.Count; i++)
            {
                if (item.IsSamePosition(this.roots[i]))
                {
                    return i;
                }
            }
            this.roots.Add(item);
            return (this.roots.Count - 1);
        }
    }
}

