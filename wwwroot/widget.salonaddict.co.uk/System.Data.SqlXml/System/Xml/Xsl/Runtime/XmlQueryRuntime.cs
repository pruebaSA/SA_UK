namespace System.Xml.Xsl.Runtime
{
    using MS.Internal.Xml.XPath;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class XmlQueryRuntime
    {
        private string[] atomizedNames;
        private XmlCollation[] collations;
        private XmlQueryContext ctxt;
        private DocumentOrderComparer docOrderCmp;
        internal const BindingFlags EarlyBoundFlags = (BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        private EarlyBoundInfo[] earlyInfo;
        private object[] earlyObjects;
        private XmlNavigatorFilter[] filters;
        private string[] globalNames;
        private object[] globalValues;
        private ArrayList[] indexes;
        internal const BindingFlags LateBoundFlags = (BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        private XmlNameTable nameTableQuery;
        private XmlQueryOutput output;
        private StringPair[][] prefixMappingsList;
        private Stack<XmlQueryOutput> stkOutput;
        private XmlQueryType[] types;
        private XsltLibrary xsltLib;

        internal XmlQueryRuntime(XmlQueryStaticData data, object defaultDataSource, XmlResolver dataSources, XsltArgumentList argList, XmlSequenceWriter seqWrt)
        {
            int num;
            string[] names = data.Names;
            Int32Pair[] filters = data.Filters;
            WhitespaceRuleLookup wsRules = ((data.WhitespaceRules != null) && (data.WhitespaceRules.Count != 0)) ? new WhitespaceRuleLookup(data.WhitespaceRules) : null;
            this.ctxt = new XmlQueryContext(this, defaultDataSource, dataSources, argList, wsRules);
            this.xsltLib = null;
            this.earlyInfo = data.EarlyBound;
            this.earlyObjects = (this.earlyInfo != null) ? new object[this.earlyInfo.Length] : null;
            this.globalNames = data.GlobalNames;
            this.globalValues = (this.globalNames != null) ? new object[this.globalNames.Length] : null;
            this.nameTableQuery = this.ctxt.QueryNameTable;
            this.atomizedNames = null;
            if (names != null)
            {
                XmlNameTable defaultNameTable = this.ctxt.DefaultNameTable;
                this.atomizedNames = new string[names.Length];
                if ((defaultNameTable != this.nameTableQuery) && (defaultNameTable != null))
                {
                    for (num = 0; num < names.Length; num++)
                    {
                        string str = defaultNameTable.Get(names[num]);
                        this.atomizedNames[num] = this.nameTableQuery.Add(str ?? names[num]);
                    }
                }
                else
                {
                    num = 0;
                    while (num < names.Length)
                    {
                        this.atomizedNames[num] = this.nameTableQuery.Add(names[num]);
                        num++;
                    }
                }
            }
            this.filters = null;
            if (filters != null)
            {
                this.filters = new XmlNavigatorFilter[filters.Length];
                for (num = 0; num < filters.Length; num++)
                {
                    this.filters[num] = XmlNavNameFilter.Create(this.atomizedNames[filters[num].Left], this.atomizedNames[filters[num].Right]);
                }
            }
            this.prefixMappingsList = data.PrefixMappingsList;
            this.types = data.Types;
            this.collations = data.Collations;
            this.docOrderCmp = new DocumentOrderComparer();
            this.indexes = null;
            this.stkOutput = new Stack<XmlQueryOutput>(0x10);
            this.output = new XmlQueryOutput(this, seqWrt);
        }

        public void AddNewIndex(XPathNavigator context, int indexId, XmlILIndex index)
        {
            XPathNavigator navigator = context.Clone();
            navigator.MoveToRoot();
            if (this.indexes == null)
            {
                this.indexes = new ArrayList[indexId + 4];
            }
            else if (indexId >= this.indexes.Length)
            {
                ArrayList[] destinationArray = new ArrayList[indexId + 4];
                Array.Copy(this.indexes, 0, destinationArray, 0, this.indexes.Length);
                this.indexes = destinationArray;
            }
            ArrayList list = this.indexes[indexId];
            if (list == null)
            {
                list = new ArrayList();
                this.indexes[indexId] = list;
            }
            list.Add(navigator);
            list.Add(index);
        }

        public object ChangeTypeXsltArgument(int indexType, object value, Type destinationType) => 
            this.ChangeTypeXsltArgument(this.GetXmlType(indexType), value, destinationType);

        internal object ChangeTypeXsltArgument(XmlQueryType xmlType, object value, Type destinationType)
        {
            switch (xmlType.TypeCode)
            {
                case XmlTypeCode.Item:
                {
                    if (destinationType != XsltConvert.ObjectType)
                    {
                        throw new XslTransformException("Xslt_UnsupportedClrType", new string[] { destinationType.Name });
                    }
                    IList<XPathItem> list2 = (IList<XPathItem>) value;
                    if (list2.Count == 1)
                    {
                        XPathItem item = list2[0];
                        if (item.IsNode)
                        {
                            RtfNavigator navigator = item as RtfNavigator;
                            if (navigator != null)
                            {
                                value = navigator.ToNavigator();
                                return value;
                            }
                            value = new XPathArrayIterator((IList) value);
                            return value;
                        }
                        value = item.TypedValue;
                        return value;
                    }
                    value = new XPathArrayIterator((IList) value);
                    return value;
                }
                case XmlTypeCode.Node:
                    if (destinationType != XsltConvert.XPathNodeIteratorType)
                    {
                        if (destinationType == XsltConvert.XPathNavigatorArrayType)
                        {
                            IList<XPathNavigator> list = (IList<XPathNavigator>) value;
                            XPathNavigator[] navigatorArray = new XPathNavigator[list.Count];
                            for (int i = 0; i < list.Count; i++)
                            {
                                navigatorArray[i] = list[i];
                            }
                            value = navigatorArray;
                        }
                        return value;
                    }
                    value = new XPathArrayIterator((IList) value);
                    return value;

                case XmlTypeCode.String:
                    if (destinationType == XsltConvert.DateTimeType)
                    {
                        value = XsltConvert.ToDateTime((string) value);
                    }
                    return value;

                case XmlTypeCode.Double:
                    if (destinationType != XsltConvert.DoubleType)
                    {
                        value = Convert.ChangeType(value, destinationType, CultureInfo.InvariantCulture);
                    }
                    return value;
            }
            return value;
        }

        public object ChangeTypeXsltResult(int indexType, object value) => 
            this.ChangeTypeXsltResult(this.GetXmlType(indexType), value);

        internal object ChangeTypeXsltResult(XmlQueryType xmlType, object value)
        {
            if (value == null)
            {
                throw new XslTransformException("Xslt_ItemNull", new string[] { string.Empty });
            }
            switch (xmlType.TypeCode)
            {
                case XmlTypeCode.Item:
                {
                    Type clrType = value.GetType();
                    switch (XsltConvert.InferXsltType(clrType).TypeCode)
                    {
                        case XmlTypeCode.Item:
                        {
                            if (value is XPathNodeIterator)
                            {
                                value = this.ChangeTypeXsltResult(XmlQueryTypeFactory.NodeS, value);
                                return value;
                            }
                            IXPathNavigable navigable = value as IXPathNavigable;
                            if (navigable == null)
                            {
                                throw new XslTransformException("Xslt_UnsupportedClrType", new string[] { clrType.Name });
                            }
                            if (value is XPathNavigator)
                            {
                                value = new XmlQueryNodeSequence((XPathNavigator) value);
                                return value;
                            }
                            value = new XmlQueryNodeSequence(navigable.CreateNavigator());
                            return value;
                        }
                        case XmlTypeCode.Node:
                            value = this.ChangeTypeXsltResult(XmlQueryTypeFactory.NodeS, value);
                            return value;

                        case XmlTypeCode.String:
                            if (clrType == XsltConvert.DateTimeType)
                            {
                                value = new XmlQueryItemSequence(new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), XsltConvert.ToString((DateTime) value)));
                                return value;
                            }
                            value = new XmlQueryItemSequence(new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.String), value));
                            return value;

                        case XmlTypeCode.Boolean:
                            value = new XmlQueryItemSequence(new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Boolean), value));
                            return value;

                        case XmlTypeCode.Decimal:
                        case XmlTypeCode.Float:
                            return value;

                        case XmlTypeCode.Double:
                            value = new XmlQueryItemSequence(new XmlAtomicValue(XmlSchemaType.GetBuiltInSimpleType(XmlTypeCode.Double), ((IConvertible) value).ToDouble(null)));
                            return value;
                    }
                    return value;
                }
                case XmlTypeCode.Node:
                {
                    if (xmlType.IsSingleton)
                    {
                        return value;
                    }
                    XPathArrayIterator iterator = value as XPathArrayIterator;
                    if ((iterator == null) || !(iterator.AsList is XmlQueryNodeSequence))
                    {
                        XmlQueryNodeSequence sequence = new XmlQueryNodeSequence();
                        IList list = value as IList;
                        if (list != null)
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                sequence.Add(EnsureNavigator(list[i]));
                            }
                        }
                        else
                        {
                            foreach (object obj2 in (IEnumerable) value)
                            {
                                sequence.Add(EnsureNavigator(obj2));
                            }
                        }
                        value = sequence;
                        break;
                    }
                    value = iterator.AsList as XmlQueryNodeSequence;
                    break;
                }
                case XmlTypeCode.String:
                    if (value.GetType() == XsltConvert.DateTimeType)
                    {
                        value = XsltConvert.ToString((DateTime) value);
                    }
                    return value;

                case XmlTypeCode.Double:
                    if (value.GetType() != XsltConvert.DoubleType)
                    {
                        value = ((IConvertible) value).ToDouble(null);
                    }
                    return value;

                default:
                    return value;
            }
            value = ((XmlQueryNodeSequence) value).DocOrderDistinct(this.docOrderCmp);
            return value;
        }

        public int ComparePosition(XPathNavigator navigatorThis, XPathNavigator navigatorThat) => 
            this.docOrderCmp.Compare(navigatorThis, navigatorThat);

        public XmlCollation CreateCollation(string collation) => 
            XmlCollation.Create(collation);

        private XmlQueryType CreateXmlType(XPathItem item)
        {
            if (!item.IsNode)
            {
                return XmlQueryTypeFactory.Type((XmlSchemaSimpleType) item.XmlType, true);
            }
            if (item is RtfNavigator)
            {
                return XmlQueryTypeFactory.Node;
            }
            XPathNavigator navigator2 = (XPathNavigator) item;
            switch (navigator2.NodeType)
            {
                case XPathNodeType.Root:
                case XPathNodeType.Element:
                    if (navigator2.XmlType != null)
                    {
                        return XmlQueryTypeFactory.Type(navigator2.NodeType, XmlQualifiedNameTest.New(navigator2.LocalName, navigator2.NamespaceURI), navigator2.XmlType, navigator2.SchemaInfo.SchemaElement.IsNillable);
                    }
                    return XmlQueryTypeFactory.Type(navigator2.NodeType, XmlQualifiedNameTest.New(navigator2.LocalName, navigator2.NamespaceURI), XmlSchemaComplexType.UntypedAnyType, false);

                case XPathNodeType.Attribute:
                    if (navigator2.XmlType != null)
                    {
                        return XmlQueryTypeFactory.Type(navigator2.NodeType, XmlQualifiedNameTest.New(navigator2.LocalName, navigator2.NamespaceURI), navigator2.XmlType, false);
                    }
                    return XmlQueryTypeFactory.Type(navigator2.NodeType, XmlQualifiedNameTest.New(navigator2.LocalName, navigator2.NamespaceURI), DatatypeImplementation.UntypedAtomicType, false);
            }
            return XmlQueryTypeFactory.Type(navigator2.NodeType, XmlQualifiedNameTest.Wildcard, XmlSchemaComplexType.AnyType, false);
        }

        public string[] DebugGetGlobalNames() => 
            this.globalNames;

        public IList DebugGetGlobalValue(string name)
        {
            for (int i = 0; i < this.globalNames.Length; i++)
            {
                if (this.globalNames[i] == name)
                {
                    return (IList) this.globalValues[i];
                }
            }
            return null;
        }

        public object DebugGetXsltValue(IList seq)
        {
            if ((seq != null) && (seq.Count == 1))
            {
                XPathItem item = seq[0] as XPathItem;
                if ((item != null) && !item.IsNode)
                {
                    return item.TypedValue;
                }
                if (item is RtfNavigator)
                {
                    return ((RtfNavigator) item).ToNavigator();
                }
            }
            return seq;
        }

        public void DebugSetGlobalValue(string name, object value)
        {
            for (int i = 0; i < this.globalNames.Length; i++)
            {
                if (this.globalNames[i] == name)
                {
                    this.globalValues[i] = (IList<XPathItem>) XmlAnyListConverter.ItemList.ChangeType(value, typeof(XPathItem[]), null);
                    return;
                }
            }
        }

        public IList<XPathNavigator> DocOrderDistinct(IList<XPathNavigator> seq)
        {
            if (seq.Count <= 1)
            {
                return seq;
            }
            XmlQueryNodeSequence sequence = (XmlQueryNodeSequence) seq;
            return sequence?.DocOrderDistinct(this.docOrderCmp);
        }

        public bool EarlyBoundFunctionExists(string name, string namespaceUri)
        {
            if (this.earlyInfo != null)
            {
                for (int i = 0; i < this.earlyInfo.Length; i++)
                {
                    if (namespaceUri == this.earlyInfo[i].NamespaceUri)
                    {
                        return new XmlExtensionFunction(name, namespaceUri, -1, this.earlyInfo[i].EarlyBoundType, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).CanBind();
                    }
                }
            }
            return false;
        }

        public XPathNavigator EndRtfConstruction(out XmlQueryOutput output)
        {
            XmlEventCache writer = (XmlEventCache) this.output.Writer;
            output = this.output = this.stkOutput.Pop();
            writer.EndEvents();
            return new RtfTreeNavigator(writer, this.nameTableQuery);
        }

        public IList<XPathItem> EndSequenceConstruction(out XmlQueryOutput output)
        {
            IList<XPathItem> resultSequence = ((XmlCachedSequenceWriter) this.output.SequenceWriter).ResultSequence;
            output = this.output = this.stkOutput.Pop();
            return resultSequence;
        }

        private static XPathNavigator EnsureNavigator(object value)
        {
            XPathNavigator navigator = value as XPathNavigator;
            if (navigator == null)
            {
                throw new XslTransformException("Xslt_ItemNull", new string[] { string.Empty });
            }
            return navigator;
        }

        public bool FindIndex(XPathNavigator context, int indexId, out XmlILIndex index)
        {
            XPathNavigator other = context.Clone();
            other.MoveToRoot();
            if ((this.indexes != null) && (indexId < this.indexes.Length))
            {
                ArrayList list = this.indexes[indexId];
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i += 2)
                    {
                        if (((XPathNavigator) list[i]).IsSamePosition(other))
                        {
                            index = (XmlILIndex) list[i + 1];
                            return true;
                        }
                    }
                }
            }
            index = new XmlILIndex();
            return false;
        }

        public string GenerateId(XPathNavigator navigator) => 
            ("ID" + this.docOrderCmp.GetDocumentIndex(navigator).ToString(CultureInfo.InvariantCulture) + navigator.UniqueId);

        public string GetAtomizedName(int index) => 
            this.atomizedNames[index];

        public XmlCollation GetCollation(int index) => 
            this.collations[index];

        public object GetEarlyBoundObject(int index)
        {
            object obj2 = this.earlyObjects[index];
            if (obj2 == null)
            {
                obj2 = this.earlyInfo[index].CreateObject();
                this.earlyObjects[index] = obj2;
            }
            return obj2;
        }

        public object GetGlobalValue(int index) => 
            this.globalValues[index];

        public XmlNavigatorFilter GetNameFilter(int index) => 
            this.filters[index];

        public XmlNavigatorFilter GetTypeFilter(XPathNodeType nodeType)
        {
            if (nodeType == XPathNodeType.All)
            {
                return XmlNavNeverFilter.Create();
            }
            if (nodeType == XPathNodeType.Attribute)
            {
                return XmlNavAttrFilter.Create();
            }
            return XmlNavTypeFilter.Create(nodeType);
        }

        internal XmlQueryType GetXmlType(int idxType) => 
            this.types[idxType];

        public bool IsGlobalComputed(int index) => 
            (this.globalValues[index] != null);

        private static bool IsInheritedNamespace(XPathNavigator node)
        {
            XPathNavigator navigator = node.Clone();
            if (navigator.MoveToParent() && navigator.MoveToFirstNamespace(XPathNamespaceScope.Local))
            {
                do
                {
                    if (navigator.LocalName == node.LocalName)
                    {
                        return false;
                    }
                }
                while (navigator.MoveToNextNamespace(XPathNamespaceScope.Local));
            }
            return true;
        }

        public bool IsQNameEqual(XPathNavigator n1, XPathNavigator n2)
        {
            if (n1.NameTable == n2.NameTable)
            {
                return ((n1.LocalName == n2.LocalName) && (n1.NamespaceURI == n2.NamespaceURI));
            }
            return ((n1.LocalName == n2.LocalName) && (n1.NamespaceURI == n2.NamespaceURI));
        }

        public bool IsQNameEqual(XPathNavigator navigator, int indexLocalName, int indexNamespaceUri)
        {
            if (navigator.NameTable == this.nameTableQuery)
            {
                return ((this.GetAtomizedName(indexLocalName) == navigator.LocalName) && (this.GetAtomizedName(indexNamespaceUri) == navigator.NamespaceURI));
            }
            return ((this.GetAtomizedName(indexLocalName) == navigator.LocalName) && (this.GetAtomizedName(indexNamespaceUri) == navigator.NamespaceURI));
        }

        public bool MatchesXmlType(IList<XPathItem> seq, int indexType)
        {
            XmlQueryCardinality zero;
            XmlQueryType xmlType = this.GetXmlType(indexType);
            switch (seq.Count)
            {
                case 0:
                    zero = XmlQueryCardinality.Zero;
                    break;

                case 1:
                    zero = XmlQueryCardinality.One;
                    break;

                default:
                    zero = XmlQueryCardinality.More;
                    break;
            }
            if (zero > xmlType.Cardinality)
            {
                return false;
            }
            xmlType = xmlType.Prime;
            for (int i = 0; i < seq.Count; i++)
            {
                if (!this.CreateXmlType(seq[0]).IsSubtypeOf(xmlType))
                {
                    return false;
                }
            }
            return true;
        }

        public bool MatchesXmlType(IList<XPathItem> seq, XmlTypeCode code)
        {
            if (seq.Count != 1)
            {
                return false;
            }
            return this.MatchesXmlType(seq[0], code);
        }

        public bool MatchesXmlType(XPathItem item, int indexType) => 
            this.CreateXmlType(item).IsSubtypeOf(this.GetXmlType(indexType));

        public bool MatchesXmlType(XPathItem item, XmlTypeCode code)
        {
            if (code > XmlTypeCode.AnyAtomicType)
            {
                return (!item.IsNode && (item.XmlType.TypeCode == code));
            }
            switch (code)
            {
                case XmlTypeCode.Item:
                    return true;

                case XmlTypeCode.Node:
                    return item.IsNode;

                case XmlTypeCode.AnyAtomicType:
                    return !item.IsNode;
            }
            if (item.IsNode)
            {
                switch (((XPathNavigator) item).NodeType)
                {
                    case XPathNodeType.Root:
                        return (code == XmlTypeCode.Document);

                    case XPathNodeType.Element:
                        return (code == XmlTypeCode.Element);

                    case XPathNodeType.Attribute:
                        return (code == XmlTypeCode.Attribute);

                    case XPathNodeType.Namespace:
                        return (code == XmlTypeCode.Namespace);

                    case XPathNodeType.Text:
                        return (code == XmlTypeCode.Text);

                    case XPathNodeType.SignificantWhitespace:
                        return (code == XmlTypeCode.Text);

                    case XPathNodeType.Whitespace:
                        return (code == XmlTypeCode.Text);

                    case XPathNodeType.ProcessingInstruction:
                        return (code == XmlTypeCode.ProcessingInstruction);

                    case XPathNodeType.Comment:
                        return (code == XmlTypeCode.Comment);
                }
            }
            return false;
        }

        public static int OnCurrentNodeChanged(XPathNavigator currentNode)
        {
            IXmlLineInfo info = currentNode as IXmlLineInfo;
            if ((info != null) && ((currentNode.NodeType != XPathNodeType.Namespace) || !IsInheritedNamespace(currentNode)))
            {
                OnCurrentNodeChanged2(currentNode.BaseURI, info.LineNumber, info.LinePosition);
            }
            return 0;
        }

        private static void OnCurrentNodeChanged2(string baseUri, int lineNumber, int linePosition)
        {
        }

        public XmlQualifiedName ParseTagName(string tagName, int indexPrefixMappings)
        {
            string str;
            string str2;
            string str3;
            this.ParseTagName(tagName, indexPrefixMappings, out str, out str2, out str3);
            return new XmlQualifiedName(str2, str3);
        }

        public XmlQualifiedName ParseTagName(string tagName, string ns)
        {
            string str;
            string str2;
            ValidateNames.ParseQNameThrow(tagName, out str, out str2);
            return new XmlQualifiedName(str2, ns);
        }

        internal void ParseTagName(string tagName, int idxPrefixMappings, out string prefix, out string localName, out string ns)
        {
            ValidateNames.ParseQNameThrow(tagName, out prefix, out localName);
            ns = null;
            foreach (StringPair pair in this.prefixMappingsList[idxPrefixMappings])
            {
                if (prefix == pair.Left)
                {
                    ns = pair.Right;
                    break;
                }
            }
            if (ns == null)
            {
                if (prefix.Length == 0)
                {
                    ns = "";
                }
                else if (prefix.Equals("xml"))
                {
                    ns = "http://www.w3.org/XML/1998/namespace";
                }
                else
                {
                    if (!prefix.Equals("xmlns"))
                    {
                        throw new XslTransformException("Xslt_InvalidPrefix", new string[] { prefix });
                    }
                    ns = "http://www.w3.org/2000/xmlns/";
                }
            }
        }

        public void SendMessage(string message)
        {
            this.ctxt.OnXsltMessageEncountered(message);
        }

        public void SetGlobalValue(int index, object value)
        {
            this.globalValues[index] = value;
        }

        public void StartRtfConstruction(string baseUri, out XmlQueryOutput output)
        {
            this.stkOutput.Push(this.output);
            output = this.output = new XmlQueryOutput(this, new XmlEventCache(baseUri, true));
        }

        public void StartSequenceConstruction(out XmlQueryOutput output)
        {
            this.stkOutput.Push(this.output);
            output = this.output = new XmlQueryOutput(this, new XmlCachedSequenceWriter());
        }

        internal static XPathNavigator SyncToNavigator(XPathNavigator navigatorThis, XPathNavigator navigatorThat)
        {
            if ((navigatorThis != null) && navigatorThis.MoveTo(navigatorThat))
            {
                return navigatorThis;
            }
            return navigatorThat.Clone();
        }

        public XPathNavigator TextRtfConstruction(string text, string baseUri) => 
            new RtfTextNavigator(text, baseUri);

        public void ThrowException(string text)
        {
            throw new XslTransformException(text);
        }

        public XmlQueryContext ExternalContext =>
            this.ctxt;

        public XmlNameTable NameTable =>
            this.nameTableQuery;

        public XmlQueryOutput Output =>
            this.output;

        internal XmlQueryType[] XmlTypes =>
            this.types;

        public XsltLibrary XsltFunctions
        {
            get
            {
                if (this.xsltLib == null)
                {
                    this.xsltLib = new XsltLibrary(this);
                }
                return this.xsltLib;
            }
        }
    }
}

