namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;
    using System.Xml;
    using System.Xml.XPath;

    internal class Key
    {
        private ArrayList keyNodes;
        private int matchKey;
        private XmlQualifiedName name;
        private int useKey;

        public Key(XmlQualifiedName name, int matchkey, int usekey)
        {
            this.name = name;
            this.matchKey = matchkey;
            this.useKey = usekey;
            this.keyNodes = null;
        }

        public void AddKey(XPathNavigator root, Hashtable table)
        {
            if (this.keyNodes == null)
            {
                this.keyNodes = new ArrayList();
            }
            this.keyNodes.Add(new DocumentKeyList(root, table));
        }

        public Key Clone() => 
            new Key(this.name, this.matchKey, this.useKey);

        public Hashtable GetKeys(XPathNavigator root)
        {
            if (this.keyNodes != null)
            {
                for (int i = 0; i < this.keyNodes.Count; i++)
                {
                    DocumentKeyList list = (DocumentKeyList) this.keyNodes[i];
                    if (list.RootNav.IsSamePosition(root))
                    {
                        DocumentKeyList list2 = (DocumentKeyList) this.keyNodes[i];
                        return list2.KeyTable;
                    }
                }
            }
            return null;
        }

        public int MatchKey =>
            this.matchKey;

        public XmlQualifiedName Name =>
            this.name;

        public int UseKey =>
            this.useKey;
    }
}

