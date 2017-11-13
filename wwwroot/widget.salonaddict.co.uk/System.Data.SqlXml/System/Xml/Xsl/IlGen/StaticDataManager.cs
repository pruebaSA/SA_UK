namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.Runtime;

    internal class StaticDataManager
    {
        private UniqueList<EarlyBoundInfo> earlyInfo;
        private List<string> globalNames;
        private List<StringPair[]> prefixMappingsList;
        private UniqueList<XmlCollation> uniqueCollations;
        private UniqueList<Int32Pair> uniqueFilters;
        private UniqueList<string> uniqueNames;
        private UniqueList<XmlQueryType> uniqueXmlTypes;

        public int DeclareCollation(string collation) => 
            this.uniqueCollations?.Add(XmlCollation.Create(collation));

        public int DeclareEarlyBound(string namespaceUri, Type ebType) => 
            this.earlyInfo?.Add(new EarlyBoundInfo(namespaceUri, ebType));

        public int DeclareGlobalValue(string name)
        {
            if (this.globalNames == null)
            {
                this.globalNames = new List<string>();
            }
            int count = this.globalNames.Count;
            this.globalNames.Add(name);
            return count;
        }

        public int DeclareName(string name) => 
            this.uniqueNames?.Add(name);

        public int DeclareNameFilter(string locName, string nsUri) => 
            this.uniqueFilters?.Add(new Int32Pair(this.DeclareName(locName), this.DeclareName(nsUri)));

        public int DeclarePrefixMappings(IList<QilNode> list)
        {
            StringPair[] item = new StringPair[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                QilBinary binary = (QilBinary) list[i];
                item[i] = new StringPair((string) ((QilLiteral) binary.Left), (string) ((QilLiteral) binary.Right));
            }
            if (this.prefixMappingsList == null)
            {
                this.prefixMappingsList = new List<StringPair[]>();
            }
            this.prefixMappingsList.Add(item);
            return (this.prefixMappingsList.Count - 1);
        }

        public int DeclareXmlType(XmlQueryType type) => 
            this.uniqueXmlTypes?.Add(type);

        public XmlCollation[] Collations =>
            this.uniqueCollations?.ToArray();

        public EarlyBoundInfo[] EarlyBound
        {
            get
            {
                if (this.earlyInfo != null)
                {
                    return this.earlyInfo.ToArray();
                }
                return null;
            }
        }

        public string[] GlobalNames =>
            this.globalNames?.ToArray();

        public Int32Pair[] NameFilters =>
            this.uniqueFilters?.ToArray();

        public string[] Names =>
            this.uniqueNames?.ToArray();

        public StringPair[][] PrefixMappingsList =>
            this.prefixMappingsList?.ToArray();

        public XmlQueryType[] XmlTypes =>
            this.uniqueXmlTypes?.ToArray();
    }
}

