namespace System.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class XmlWellFormedWriter : XmlWriter
    {
        private int attrCount;
        private Dictionary<string, int> attrHashTable;
        private const int AttributeArrayInitialSize = 8;
        private AttrName[] attrStack;
        private StringBuilder attrValue;
        private bool checkCharacters;
        private ConformanceLevel conformanceLevel;
        private string curDeclPrefix;
        private State currentState;
        private bool dtdWritten;
        private const int ElementStackInitialSize = 8;
        private ElementScope[] elemScopeStack;
        private int elemTop;
        private SecureStringHasher hasher;
        private const int MaxAttrDuplWalkCount = 14;
        private const int MaxNamespacesWalkCount = 0x10;
        private const int NamespaceStackInitialSize = 8;
        private Dictionary<string, int> nsHashtable;
        private Namespace[] nsStack;
        private int nsTop;
        private IXmlNamespaceResolver predefinedNamespaces;
        private XmlRawWriter rawWriter;
        private SpecialAttribute specAttr;
        private static System.Xml.WriteState[] state2WriteState;
        internal static readonly string[] stateName = new string[] { 
            "Start", "TopLevel", "Document", "Element Start Tag", "Element Content", "Element Content", "Attribute", "EndRootElement", "Attribute", "Special Attribute", "End Document", "Root Level Attribute Value", "Root Level Special Attribute Value", "Root Level Base64 Attribute Value", "After Root Level Attribute", "Closed",
            "Error"
        };
        private State[] stateTable;
        private static readonly State[] StateTableAuto;
        private static readonly State[] StateTableDocument;
        internal static readonly string[] tokenName = new string[] { "StartDocument", "EndDocument", "PI", "Comment", "DTD", "StartElement", "EndElement", "StartAttribute", "EndAttribute", "Text", "CDATA", "Atomic value", "Base64", "RawData", "Whitespace" };
        private bool useNsHashtable;
        private XmlWriter writer;
        private XmlCharType xmlCharType = XmlCharType.Instance;
        private bool xmlDeclFollows;

        static XmlWellFormedWriter()
        {
            System.Xml.WriteState[] stateArray = new System.Xml.WriteState[0x11];
            stateArray[1] = System.Xml.WriteState.Prolog;
            stateArray[2] = System.Xml.WriteState.Prolog;
            stateArray[3] = System.Xml.WriteState.Element;
            stateArray[4] = System.Xml.WriteState.Content;
            stateArray[5] = System.Xml.WriteState.Content;
            stateArray[6] = System.Xml.WriteState.Attribute;
            stateArray[7] = System.Xml.WriteState.Content;
            stateArray[8] = System.Xml.WriteState.Attribute;
            stateArray[9] = System.Xml.WriteState.Attribute;
            stateArray[10] = System.Xml.WriteState.Content;
            stateArray[11] = System.Xml.WriteState.Attribute;
            stateArray[12] = System.Xml.WriteState.Attribute;
            stateArray[13] = System.Xml.WriteState.Attribute;
            stateArray[14] = System.Xml.WriteState.Attribute;
            stateArray[15] = System.Xml.WriteState.Closed;
            stateArray[0x10] = System.Xml.WriteState.Error;
            state2WriteState = stateArray;
            StateTableDocument = new State[] { 
                State.Document, State.Error, State.Error, State.Error, State.Error, State.PostB64Cont, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.Error, State.Error, State.Error, State.Error, State.Error, State.PostB64Cont, State.Error, State.EndDocument, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.StartDoc, State.TopLevel, State.Document, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.AfterRootEle, State.EndAttrSCont, State.EndAttrSCont, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.StartDoc, State.TopLevel, State.Document, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.AfterRootEle, State.EndAttrSCont, State.EndAttrSCont, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.StartDoc, State.TopLevel, State.Document, State.Error, State.Error, State.PostB64Cont, State.PostB64Attr, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.StartDocEle, State.Element, State.Element, State.StartContentEle, State.Element, State.PostB64Cont, State.PostB64Attr, State.Error, State.EndAttrSEle, State.EndAttrSEle, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.Error, State.Error, State.Error, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.Error, State.EndAttrEEle, State.EndAttrEEle, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.Error, State.Error, State.Error, State.Attribute, State.Error, State.PostB64Cont, State.PostB64Attr, State.Error, State.EndAttrSAttr, State.EndAttrSAttr, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.Error, State.Error, State.Error, State.Error, State.Error, State.PostB64Cont, State.PostB64Attr, State.Error, State.Element, State.Element, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.Error, State.Error, State.Error, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.Error, State.Attribute, State.SpecialAttr, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.Error, State.Error, State.Error, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.Error, State.EndAttrSCont, State.EndAttrSCont, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.Error, State.Error, State.Error, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.Error, State.Attribute, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.Error, State.Error, State.Error, State.StartContentB64, State.B64Content, State.B64Content, State.B64Attribute, State.Error, State.B64Attribute, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.StartDoc, State.Error, State.Document, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.AfterRootEle, State.Attribute, State.SpecialAttr, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.StartDoc, State.TopLevel, State.Document, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.AfterRootEle, State.Attribute, State.SpecialAttr, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error
            };
            StateTableAuto = new State[] { 
                State.Document, State.Error, State.Error, State.Error, State.Error, State.PostB64Cont, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.Error, State.Error, State.Error, State.Error, State.Error, State.PostB64Cont, State.Error, State.EndDocument, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.TopLevel, State.TopLevel, State.Error, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.AfterRootEle, State.EndAttrSCont, State.EndAttrSCont, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.TopLevel, State.TopLevel, State.Error, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.AfterRootEle, State.EndAttrSCont, State.EndAttrSCont, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.StartDoc, State.TopLevel, State.Error, State.Error, State.Error, State.PostB64Cont, State.PostB64Attr, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.StartFragEle, State.Element, State.Error, State.StartContentEle, State.Element, State.PostB64Cont, State.PostB64Attr, State.Element, State.EndAttrSEle, State.EndAttrSEle, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.Error, State.Error, State.Error, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.Error, State.EndAttrEEle, State.EndAttrEEle, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.RootLevelAttr, State.Error, State.Error, State.Attribute, State.Error, State.PostB64Cont, State.PostB64Attr, State.Error, State.EndAttrSAttr, State.EndAttrSAttr, State.Error, State.StartRootLevelAttr, State.StartRootLevelAttr, State.PostB64RootAttr, State.RootLevelAttr, State.Error,
                State.Error, State.Error, State.Error, State.Error, State.Error, State.PostB64Cont, State.PostB64Attr, State.Error, State.Element, State.Element, State.Error, State.AfterRootLevelAttr, State.AfterRootLevelAttr, State.PostB64RootAttr, State.Error, State.Error,
                State.StartFragCont, State.StartFragCont, State.Error, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.Content, State.Attribute, State.SpecialAttr, State.Error, State.RootLevelAttr, State.RootLevelSpecAttr, State.PostB64RootAttr, State.Error, State.Error,
                State.StartFragCont, State.StartFragCont, State.Error, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.Content, State.EndAttrSCont, State.EndAttrSCont, State.Error, State.Error, State.Error, State.Error, State.Error, State.Error,
                State.StartFragCont, State.StartFragCont, State.Error, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.Content, State.Attribute, State.Error, State.Error, State.RootLevelAttr, State.Error, State.PostB64RootAttr, State.Error, State.Error,
                State.StartFragB64, State.StartFragB64, State.Error, State.StartContentB64, State.B64Content, State.B64Content, State.B64Attribute, State.B64Content, State.B64Attribute, State.Error, State.Error, State.RootLevelB64Attr, State.Error, State.RootLevelB64Attr, State.Error, State.Error,
                State.StartFragCont, State.TopLevel, State.Error, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.Content, State.Attribute, State.SpecialAttr, State.Error, State.RootLevelAttr, State.RootLevelSpecAttr, State.PostB64RootAttr, State.AfterRootLevelAttr, State.Error,
                State.TopLevel, State.TopLevel, State.Error, State.StartContent, State.Content, State.PostB64Cont, State.PostB64Attr, State.AfterRootEle, State.Attribute, State.SpecialAttr, State.Error, State.RootLevelAttr, State.RootLevelSpecAttr, State.PostB64RootAttr, State.AfterRootLevelAttr, State.Error
            };
        }

        internal XmlWellFormedWriter(XmlWriter writer, XmlWriterSettings settings)
        {
            this.writer = writer;
            this.rawWriter = writer as XmlRawWriter;
            this.predefinedNamespaces = writer as IXmlNamespaceResolver;
            if (this.rawWriter != null)
            {
                this.rawWriter.NamespaceResolver = new NamespaceResolverProxy(this);
            }
            this.checkCharacters = settings.CheckCharacters;
            this.conformanceLevel = settings.ConformanceLevel;
            this.stateTable = (this.conformanceLevel == ConformanceLevel.Document) ? StateTableDocument : StateTableAuto;
            this.currentState = State.Start;
            this.nsStack = new Namespace[8];
            this.nsStack[0].Set("xmlns", "http://www.w3.org/2000/xmlns/", NamespaceKind.Special);
            this.nsStack[1].Set("xml", "http://www.w3.org/XML/1998/namespace", NamespaceKind.Special);
            if (this.predefinedNamespaces == null)
            {
                this.nsStack[2].Set(string.Empty, string.Empty, NamespaceKind.Implied);
            }
            else
            {
                string str = this.predefinedNamespaces.LookupNamespace(string.Empty);
                this.nsStack[2].Set(string.Empty, (str == null) ? string.Empty : str, NamespaceKind.Implied);
            }
            this.nsTop = 2;
            this.elemScopeStack = new ElementScope[8];
            this.elemScopeStack[0].Set(string.Empty, string.Empty, string.Empty, this.nsTop);
            this.elemScopeStack[0].xmlSpace = System.Xml.XmlSpace.None;
            this.elemScopeStack[0].xmlLang = null;
            this.elemTop = 0;
            this.attrStack = new AttrName[8];
            this.attrValue = new StringBuilder();
            this.hasher = new SecureStringHasher();
        }

        private void AddAttribute(string prefix, string localName, string namespaceName)
        {
            int length = this.attrCount++;
            if (length == this.attrStack.Length)
            {
                AttrName[] destinationArray = new AttrName[length * 2];
                Array.Copy(this.attrStack, destinationArray, length);
                this.attrStack = destinationArray;
            }
            this.attrStack[length].Set(prefix, localName, namespaceName);
            if (this.attrCount < 14)
            {
                for (int i = 0; i < length; i++)
                {
                    if (this.attrStack[i].IsDuplicate(prefix, localName, namespaceName))
                    {
                        throw DupAttrException(prefix, localName);
                    }
                }
            }
            else
            {
                if (this.attrCount == 14)
                {
                    if (this.attrHashTable == null)
                    {
                        this.attrHashTable = new Dictionary<string, int>(this.hasher);
                    }
                    for (int k = 0; k < length; k++)
                    {
                        this.AddToAttrHashTable(k);
                    }
                }
                this.AddToAttrHashTable(length);
                for (int j = this.attrStack[length].prev; j > 0; j = this.attrStack[j].prev)
                {
                    j--;
                    if (this.attrStack[j].IsDuplicate(prefix, localName, namespaceName))
                    {
                        throw DupAttrException(prefix, localName);
                    }
                }
            }
        }

        private void AddToAttrHashTable(int attributeIndex)
        {
            string localName = this.attrStack[attributeIndex].localName;
            int count = this.attrHashTable.Count;
            this.attrHashTable[localName] = 0;
            if (count == this.attrHashTable.Count)
            {
                int index = attributeIndex - 1;
                while (index >= 0)
                {
                    if (this.attrStack[index].localName == localName)
                    {
                        break;
                    }
                    index--;
                }
                this.attrStack[attributeIndex].prev = index + 1;
            }
        }

        private void AddToNamespaceHashtable(int namespaceIndex)
        {
            int num;
            string prefix = this.nsStack[namespaceIndex].prefix;
            if (this.nsHashtable.TryGetValue(prefix, out num))
            {
                this.nsStack[namespaceIndex].prevNsIndex = num;
            }
            this.nsHashtable[prefix] = namespaceIndex;
        }

        private void AdvanceState(Token token)
        {
            State content;
            if (this.currentState >= State.Closed)
            {
                if ((this.currentState == State.Closed) || (this.currentState == State.Error))
                {
                    throw new InvalidOperationException(Res.GetString("Xml_ClosedOrError"));
                }
                throw new InvalidOperationException(Res.GetString("Xml_WrongToken", new object[] { tokenName[(int) token], GetStateName(this.currentState) }));
            }
        Label_005E:
            content = this.stateTable[(((int) token) << 4) + this.currentState];
            if (content >= State.Error)
            {
                switch (content)
                {
                    case State.StartContent:
                        this.StartElementContent();
                        content = State.Content;
                        break;

                    case State.StartContentEle:
                        this.StartElementContent();
                        content = State.Element;
                        break;

                    case State.StartContentB64:
                        this.StartElementContent();
                        content = State.B64Content;
                        break;

                    case State.StartDoc:
                        this.WriteStartDocument();
                        content = State.Document;
                        break;

                    case State.StartDocEle:
                        this.WriteStartDocument();
                        content = State.Element;
                        break;

                    case State.EndAttrSEle:
                        this.WriteEndAttribute();
                        this.StartElementContent();
                        content = State.Element;
                        break;

                    case State.EndAttrEEle:
                        this.WriteEndAttribute();
                        this.StartElementContent();
                        content = State.Content;
                        break;

                    case State.EndAttrSCont:
                        this.WriteEndAttribute();
                        this.StartElementContent();
                        content = State.Content;
                        break;

                    case State.EndAttrSAttr:
                        this.WriteEndAttribute();
                        content = State.Attribute;
                        break;

                    case State.PostB64Cont:
                        if (this.rawWriter != null)
                        {
                            this.rawWriter.WriteEndBase64();
                        }
                        this.currentState = State.Content;
                        goto Label_005E;

                    case State.PostB64Attr:
                        if (this.rawWriter != null)
                        {
                            this.rawWriter.WriteEndBase64();
                        }
                        this.currentState = State.Attribute;
                        goto Label_005E;

                    case State.PostB64RootAttr:
                        if (this.rawWriter != null)
                        {
                            this.rawWriter.WriteEndBase64();
                        }
                        this.currentState = State.RootLevelAttr;
                        goto Label_005E;

                    case State.StartFragEle:
                        this.StartFragment();
                        content = State.Element;
                        break;

                    case State.StartFragCont:
                        this.StartFragment();
                        content = State.Content;
                        break;

                    case State.StartFragB64:
                        this.StartFragment();
                        content = State.B64Content;
                        break;

                    case State.StartRootLevelAttr:
                        this.WriteEndAttribute();
                        content = State.RootLevelAttr;
                        break;

                    case State.Error:
                        this.ThrowInvalidStateTransition(token, this.currentState);
                        break;
                }
            }
            this.currentState = content;
        }

        private unsafe void CheckNCName(string ncname)
        {
            if ((this.xmlCharType.charProperties[ncname[0]] & 4) == 0)
            {
                throw InvalidCharsException(ncname, ncname[0]);
            }
            int num = 1;
            int length = ncname.Length;
            while (num < length)
            {
                if ((this.xmlCharType.charProperties[ncname[num]] & 8) == 0)
                {
                    throw InvalidCharsException(ncname, ncname[num]);
                }
                num++;
            }
        }

        public override void Close()
        {
            if (this.currentState != State.Closed)
            {
                while ((this.currentState != State.Error) && (this.elemTop > 0))
                {
                    this.WriteEndElement();
                }
                this.writer.Flush();
                if (this.rawWriter != null)
                {
                    this.rawWriter.Close(this.WriteState);
                }
                else
                {
                    this.writer.Close();
                }
                this.currentState = State.Closed;
            }
        }

        private static XmlException DupAttrException(string prefix, string localName)
        {
            StringBuilder builder = new StringBuilder();
            if (prefix.Length > 0)
            {
                builder.Append(prefix);
                builder.Append(':');
            }
            builder.Append(localName);
            return new XmlException("Xml_DupAttributeName", builder.ToString());
        }

        public override void Flush()
        {
            try
            {
                this.writer.Flush();
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        private string GeneratePrefix()
        {
            string str2;
            string prefix = "p" + ((this.nsTop - 2)).ToString("d", CultureInfo.InvariantCulture);
            if (this.LookupNamespace(prefix) == null)
            {
                return prefix;
            }
            int num = 0;
            do
            {
                str2 = prefix + num.ToString(CultureInfo.InvariantCulture);
                num++;
            }
            while (this.LookupNamespace(str2) != null);
            return str2;
        }

        private static string GetStateName(State state)
        {
            if (state >= State.Error)
            {
                return "Error";
            }
            return stateName[(int) state];
        }

        private static Exception InvalidCharsException(string name, char badChar)
        {
            string[] args = new string[] { name, badChar.ToString(CultureInfo.InvariantCulture), ((int) badChar).ToString("X2", CultureInfo.InvariantCulture) };
            return new ArgumentException(Res.GetString("Xml_InvalidNameCharsDetail", args));
        }

        private string LookupLocalNamespace(string prefix)
        {
            for (int i = this.nsTop; i > this.elemScopeStack[this.elemTop].prevNSTop; i--)
            {
                if (this.nsStack[i].prefix == prefix)
                {
                    return this.nsStack[i].namespaceUri;
                }
            }
            return null;
        }

        internal string LookupNamespace(string prefix)
        {
            for (int i = this.nsTop; i >= 0; i--)
            {
                if (this.nsStack[i].prefix == prefix)
                {
                    return this.nsStack[i].namespaceUri;
                }
            }
            return this.predefinedNamespaces?.LookupNamespace(prefix);
        }

        private int LookupNamespaceIndex(string prefix)
        {
            if (this.useNsHashtable)
            {
                int num;
                if (this.nsHashtable.TryGetValue(prefix, out num))
                {
                    return num;
                }
            }
            else
            {
                for (int i = this.nsTop; i >= 0; i--)
                {
                    if (this.nsStack[i].prefix == prefix)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public override string LookupPrefix(string ns)
        {
            string prefix;
            try
            {
                if (ns == null)
                {
                    throw new ArgumentNullException("ns");
                }
                for (int i = this.nsTop; i >= 0; i--)
                {
                    if (this.nsStack[i].namespaceUri == ns)
                    {
                        string str = this.nsStack[i].prefix;
                        i++;
                        while (i <= this.nsTop)
                        {
                            if (this.nsStack[i].prefix == str)
                            {
                                return null;
                            }
                            i++;
                        }
                        return str;
                    }
                }
                prefix = this.predefinedNamespaces?.LookupPrefix(ns);
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
            return prefix;
        }

        private void PopNamespaces(int indexFrom, int indexTo)
        {
            for (int i = indexTo; i >= indexFrom; i--)
            {
                if (this.nsStack[i].prevNsIndex == -1)
                {
                    this.nsHashtable.Remove(this.nsStack[i].prefix);
                }
                else
                {
                    this.nsHashtable[this.nsStack[i].prefix] = this.nsStack[i].prevNsIndex;
                }
            }
        }

        private void PushNamespace(string prefix, string ns, bool explicitlyDefined)
        {
            NamespaceKind implied;
            int num2;
            int namespaceIndex = this.LookupNamespaceIndex(prefix);
            if (namespaceIndex != -1)
            {
                if (namespaceIndex > this.elemScopeStack[this.elemTop].prevNSTop)
                {
                    if (this.nsStack[namespaceIndex].namespaceUri != ns)
                    {
                        throw new XmlException("Xml_RedefinePrefix", new string[] { prefix, this.nsStack[namespaceIndex].namespaceUri, ns });
                    }
                    if (explicitlyDefined)
                    {
                        if (this.nsStack[namespaceIndex].kind == NamespaceKind.Written)
                        {
                            throw DupAttrException((prefix.Length == 0) ? string.Empty : "xmlns", (prefix.Length == 0) ? "xmlns" : prefix);
                        }
                        this.nsStack[namespaceIndex].kind = NamespaceKind.Written;
                    }
                    return;
                }
                if (!explicitlyDefined)
                {
                    if (this.nsStack[namespaceIndex].kind == NamespaceKind.Special)
                    {
                        if (prefix != "xml")
                        {
                            throw new ArgumentException(Res.GetString("Xml_XmlnsPrefix"));
                        }
                        if (ns != this.nsStack[namespaceIndex].namespaceUri)
                        {
                            throw new ArgumentException(Res.GetString("Xml_XmlPrefix"));
                        }
                        implied = NamespaceKind.Implied;
                    }
                    else
                    {
                        implied = (this.nsStack[namespaceIndex].namespaceUri == ns) ? NamespaceKind.Implied : NamespaceKind.NeedToWrite;
                    }
                    goto Label_0231;
                }
            }
            if (((ns == "http://www.w3.org/XML/1998/namespace") && (prefix != "xml")) || ((ns == "http://www.w3.org/2000/xmlns/") && (prefix != "xmlns")))
            {
                throw new ArgumentException(Res.GetString("Xml_NamespaceDeclXmlXmlns", new object[] { prefix }));
            }
            if (!explicitlyDefined)
            {
                if (this.predefinedNamespaces == null)
                {
                    implied = NamespaceKind.NeedToWrite;
                }
                else
                {
                    implied = (this.predefinedNamespaces.LookupNamespace(prefix) == ns) ? NamespaceKind.Implied : NamespaceKind.NeedToWrite;
                }
            }
            else
            {
                if ((prefix.Length > 0) && (prefix[0] == 'x'))
                {
                    if (prefix == "xml")
                    {
                        if (ns != "http://www.w3.org/XML/1998/namespace")
                        {
                            throw new ArgumentException(Res.GetString("Xml_XmlPrefix"));
                        }
                    }
                    else if (prefix == "xmlns")
                    {
                        throw new ArgumentException(Res.GetString("Xml_XmlnsPrefix"));
                    }
                }
                implied = NamespaceKind.Written;
            }
        Label_0231:
            num2 = ++this.nsTop;
            if (num2 == this.nsStack.Length)
            {
                Namespace[] destinationArray = new Namespace[num2 * 2];
                Array.Copy(this.nsStack, destinationArray, num2);
                this.nsStack = destinationArray;
            }
            this.nsStack[num2].Set(prefix, ns, implied);
            if (this.useNsHashtable)
            {
                this.AddToNamespaceHashtable(this.nsTop);
            }
            else if (this.nsTop == 0x10)
            {
                this.nsHashtable = new Dictionary<string, int>(this.hasher);
                for (int i = 0; i <= this.nsTop; i++)
                {
                    this.AddToNamespaceHashtable(i);
                }
                this.useNsHashtable = true;
            }
        }

        private void SetSpecialAttribute(SpecialAttribute special)
        {
            this.specAttr = special;
            if (State.Attribute == this.currentState)
            {
                this.currentState = State.SpecialAttr;
            }
            else if (State.RootLevelAttr == this.currentState)
            {
                this.currentState = State.RootLevelSpecAttr;
            }
        }

        private void StartElementContent()
        {
            int prevNSTop = this.elemScopeStack[this.elemTop].prevNSTop;
            for (int i = this.nsTop; i > prevNSTop; i--)
            {
                if (this.nsStack[i].kind == NamespaceKind.NeedToWrite)
                {
                    this.nsStack[i].WriteDecl(this.writer, this.rawWriter);
                }
            }
            if (this.rawWriter != null)
            {
                this.rawWriter.StartElementContent();
            }
        }

        private void StartFragment()
        {
            this.conformanceLevel = ConformanceLevel.Fragment;
        }

        private void ThrowInvalidStateTransition(Token token, State currentState)
        {
            string message = Res.GetString("Xml_WrongToken", new object[] { tokenName[(int) token], GetStateName(currentState) });
            State state = currentState;
            if (((state == State.Start) || (state == State.AfterRootEle)) && (this.conformanceLevel == ConformanceLevel.Document))
            {
                throw new InvalidOperationException(message + ' ' + Res.GetString("Xml_ConformanceLevelFragment"));
            }
            throw new InvalidOperationException(message);
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            try
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                if (count < 0)
                {
                    throw new ArgumentOutOfRangeException("count");
                }
                if (count > (buffer.Length - index))
                {
                    throw new ArgumentOutOfRangeException("count");
                }
                this.AdvanceState(Token.Base64);
                this.writer.WriteBase64(buffer, index, count);
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteBinHex(byte[] buffer, int index, int count)
        {
            if (this.IsClosedOrErrorState)
            {
                throw new InvalidOperationException(Res.GetString("Xml_ClosedOrError"));
            }
            try
            {
                this.AdvanceState(Token.Text);
                base.WriteBinHex(buffer, index, count);
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteCData(string text)
        {
            try
            {
                if (text == null)
                {
                    text = string.Empty;
                }
                this.AdvanceState(Token.CData);
                this.writer.WriteCData(text);
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteCharEntity(char ch)
        {
            try
            {
                if (char.IsSurrogate(ch))
                {
                    throw new ArgumentException(Res.GetString("Xml_InvalidSurrogateMissingLowChar"));
                }
                this.AdvanceState(Token.Text);
                if (this.SaveAttrValue)
                {
                    this.attrValue.Append(ch);
                }
                else
                {
                    this.writer.WriteCharEntity(ch);
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            try
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                if (count < 0)
                {
                    throw new ArgumentOutOfRangeException("count");
                }
                if (count > (buffer.Length - index))
                {
                    throw new ArgumentOutOfRangeException("count");
                }
                this.AdvanceState(Token.Text);
                if (this.SaveAttrValue)
                {
                    this.attrValue.Append(buffer, index, count);
                }
                else
                {
                    this.writer.WriteChars(buffer, index, count);
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteComment(string text)
        {
            try
            {
                if (text == null)
                {
                    text = string.Empty;
                }
                this.AdvanceState(Token.Comment);
                this.writer.WriteComment(text);
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            try
            {
                if ((name == null) || (name.Length == 0))
                {
                    throw new ArgumentException(Res.GetString("Xml_EmptyName"));
                }
                XmlConvert.VerifyQName(name);
                if (this.conformanceLevel == ConformanceLevel.Fragment)
                {
                    throw new InvalidOperationException(Res.GetString("Xml_DtdNotAllowedInFragment"));
                }
                this.AdvanceState(Token.Dtd);
                if (this.dtdWritten)
                {
                    this.currentState = State.Error;
                    throw new InvalidOperationException(Res.GetString("Xml_DtdAlreadyWritten"));
                }
                if (this.conformanceLevel == ConformanceLevel.Auto)
                {
                    this.conformanceLevel = ConformanceLevel.Document;
                    this.stateTable = StateTableDocument;
                }
                if (this.checkCharacters)
                {
                    int num;
                    if ((pubid != null) && ((num = this.xmlCharType.IsPublicId(pubid)) >= 0))
                    {
                        throw new ArgumentException(Res.GetString("Xml_InvalidCharacter", XmlException.BuildCharExceptionStr(pubid[num])), "pubid");
                    }
                    if ((sysid != null) && ((num = this.xmlCharType.IsOnlyCharData(sysid)) >= 0))
                    {
                        throw new ArgumentException(Res.GetString("Xml_InvalidCharacter", XmlException.BuildCharExceptionStr(sysid[num])), "sysid");
                    }
                    if ((subset != null) && ((num = this.xmlCharType.IsOnlyCharData(subset)) >= 0))
                    {
                        throw new ArgumentException(Res.GetString("Xml_InvalidCharacter", XmlException.BuildCharExceptionStr(subset[num])), "subset");
                    }
                }
                this.writer.WriteDocType(name, pubid, sysid, subset);
                this.dtdWritten = true;
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteEndAttribute()
        {
            try
            {
                string str;
                this.AdvanceState(Token.EndAttribute);
                if (this.specAttr == SpecialAttribute.No)
                {
                    goto Label_021A;
                }
                if (this.attrValue != null)
                {
                    str = this.attrValue.ToString();
                    this.attrValue.Length = 0;
                }
                else
                {
                    str = string.Empty;
                }
                switch (this.specAttr)
                {
                    case SpecialAttribute.DefaultXmlns:
                        this.PushNamespace(string.Empty, str, true);
                        if (this.rawWriter == null)
                        {
                            break;
                        }
                        this.rawWriter.WriteNamespaceDeclaration(string.Empty, str);
                        goto Label_00A1;

                    case SpecialAttribute.PrefixedXmlns:
                        if (str.Length == 0)
                        {
                            throw new ArgumentException(Res.GetString("Xml_PrefixForEmptyNs"));
                        }
                        goto Label_00C5;

                    case SpecialAttribute.XmlSpace:
                        str = XmlConvert.TrimString(str);
                        if (str != "default")
                        {
                            goto Label_0180;
                        }
                        this.elemScopeStack[this.elemTop].xmlSpace = System.Xml.XmlSpace.Default;
                        goto Label_01C2;

                    case SpecialAttribute.XmlLang:
                        this.elemScopeStack[this.elemTop].xmlLang = str;
                        this.writer.WriteAttributeString("xml", "lang", "http://www.w3.org/XML/1998/namespace", str);
                        goto Label_0211;

                    default:
                        goto Label_0211;
                }
                this.writer.WriteAttributeString(string.Empty, "xmlns", "http://www.w3.org/2000/xmlns/", str);
            Label_00A1:
                this.curDeclPrefix = null;
                goto Label_0211;
            Label_00C5:
                if ((str == "http://www.w3.org/2000/xmlns/") || ((str == "http://www.w3.org/XML/1998/namespace") && (this.curDeclPrefix != "xml")))
                {
                    throw new ArgumentException(Res.GetString("Xml_CanNotBindToReservedNamespace"));
                }
                this.PushNamespace(this.curDeclPrefix, str, true);
                if (this.rawWriter != null)
                {
                    this.rawWriter.WriteNamespaceDeclaration(this.curDeclPrefix, str);
                }
                else
                {
                    this.writer.WriteAttributeString("xmlns", this.curDeclPrefix, "http://www.w3.org/2000/xmlns/", str);
                }
                this.curDeclPrefix = null;
                goto Label_0211;
            Label_0180:
                if (str == "preserve")
                {
                    this.elemScopeStack[this.elemTop].xmlSpace = System.Xml.XmlSpace.Preserve;
                }
                else
                {
                    throw new ArgumentException(Res.GetString("Xml_InvalidXmlSpace", new object[] { str }));
                }
            Label_01C2:
                this.writer.WriteAttributeString("xml", "space", "http://www.w3.org/XML/1998/namespace", str);
            Label_0211:
                this.specAttr = SpecialAttribute.No;
                return;
            Label_021A:
                this.writer.WriteEndAttribute();
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteEndDocument()
        {
            try
            {
                while (this.elemTop > 0)
                {
                    this.WriteEndElement();
                }
                State currentState = this.currentState;
                this.AdvanceState(Token.EndDocument);
                if (currentState != State.AfterRootEle)
                {
                    throw new ArgumentException(Res.GetString("Xml_NoRoot"));
                }
                if (this.rawWriter == null)
                {
                    this.writer.WriteEndDocument();
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteEndElement()
        {
            try
            {
                this.AdvanceState(Token.EndElement);
                int elemTop = this.elemTop;
                if (elemTop == 0)
                {
                    throw new XmlException("Xml_NoStartTag", string.Empty);
                }
                if (this.rawWriter != null)
                {
                    this.elemScopeStack[elemTop].WriteEndElement(this.rawWriter);
                }
                else
                {
                    this.writer.WriteEndElement();
                }
                int prevNSTop = this.elemScopeStack[elemTop].prevNSTop;
                if (this.useNsHashtable && (prevNSTop < this.nsTop))
                {
                    this.PopNamespaces(prevNSTop + 1, this.nsTop);
                }
                this.nsTop = prevNSTop;
                this.elemTop = elemTop - 1;
                if (this.elemTop == 0)
                {
                    if (this.conformanceLevel == ConformanceLevel.Document)
                    {
                        this.currentState = State.AfterRootEle;
                    }
                    else
                    {
                        this.currentState = State.TopLevel;
                    }
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteEntityRef(string name)
        {
            try
            {
                if ((name == null) || (name.Length == 0))
                {
                    throw new ArgumentException(Res.GetString("Xml_EmptyName"));
                }
                this.CheckNCName(name);
                this.AdvanceState(Token.Text);
                if (this.SaveAttrValue)
                {
                    this.attrValue.Append('&');
                    this.attrValue.Append(name);
                    this.attrValue.Append(';');
                }
                else
                {
                    this.writer.WriteEntityRef(name);
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteFullEndElement()
        {
            try
            {
                this.AdvanceState(Token.EndElement);
                int elemTop = this.elemTop;
                if (elemTop == 0)
                {
                    throw new XmlException("Xml_NoStartTag", string.Empty);
                }
                if (this.rawWriter != null)
                {
                    this.elemScopeStack[elemTop].WriteFullEndElement(this.rawWriter);
                }
                else
                {
                    this.writer.WriteFullEndElement();
                }
                int prevNSTop = this.elemScopeStack[elemTop].prevNSTop;
                if (this.useNsHashtable && (prevNSTop < this.nsTop))
                {
                    this.PopNamespaces(prevNSTop + 1, this.nsTop);
                }
                this.nsTop = prevNSTop;
                this.elemTop = elemTop - 1;
                if (this.elemTop == 0)
                {
                    if (this.conformanceLevel == ConformanceLevel.Document)
                    {
                        this.currentState = State.AfterRootEle;
                    }
                    else
                    {
                        this.currentState = State.TopLevel;
                    }
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            try
            {
                if ((name == null) || (name.Length == 0))
                {
                    throw new ArgumentException(Res.GetString("Xml_EmptyName"));
                }
                this.CheckNCName(name);
                if (text == null)
                {
                    text = string.Empty;
                }
                if ((name.Length == 3) && (string.Compare(name, "xml", StringComparison.OrdinalIgnoreCase) == 0))
                {
                    if (this.currentState != State.Start)
                    {
                        throw new ArgumentException(Res.GetString((this.conformanceLevel == ConformanceLevel.Document) ? "Xml_DupXmlDecl" : "Xml_CannotWriteXmlDecl"));
                    }
                    this.xmlDeclFollows = true;
                    this.AdvanceState(Token.PI);
                    if (this.rawWriter != null)
                    {
                        this.rawWriter.WriteXmlDeclaration(text);
                    }
                    else
                    {
                        this.writer.WriteProcessingInstruction(name, text);
                    }
                }
                else
                {
                    this.AdvanceState(Token.PI);
                    this.writer.WriteProcessingInstruction(name, text);
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteQualifiedName(string localName, string ns)
        {
            try
            {
                if ((localName == null) || (localName.Length == 0))
                {
                    throw new ArgumentException(Res.GetString("Xml_EmptyLocalName"));
                }
                this.CheckNCName(localName);
                this.AdvanceState(Token.Text);
                string prefix = string.Empty;
                if ((ns != null) && (ns.Length != 0))
                {
                    prefix = this.LookupPrefix(ns);
                    if (prefix == null)
                    {
                        if (this.currentState != State.Attribute)
                        {
                            throw new ArgumentException(Res.GetString("Xml_UndefNamespace", new object[] { ns }));
                        }
                        prefix = this.GeneratePrefix();
                        this.PushNamespace(prefix, ns, false);
                    }
                }
                if (this.SaveAttrValue || (this.rawWriter == null))
                {
                    if (prefix.Length != 0)
                    {
                        this.WriteString(prefix);
                        this.WriteString(":");
                    }
                    this.WriteString(localName);
                }
                else
                {
                    this.rawWriter.WriteQualifiedName(prefix, localName, ns);
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteRaw(string data)
        {
            try
            {
                if (data != null)
                {
                    this.AdvanceState(Token.RawData);
                    if (this.SaveAttrValue)
                    {
                        this.attrValue.Append(data);
                    }
                    else
                    {
                        this.writer.WriteRaw(data);
                    }
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            try
            {
                if (buffer == null)
                {
                    throw new ArgumentNullException("buffer");
                }
                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                if (count < 0)
                {
                    throw new ArgumentOutOfRangeException("count");
                }
                if (count > (buffer.Length - index))
                {
                    throw new ArgumentOutOfRangeException("count");
                }
                this.AdvanceState(Token.RawData);
                if (this.SaveAttrValue)
                {
                    this.attrValue.Append(buffer, index, count);
                }
                else
                {
                    this.writer.WriteRaw(buffer, index, count);
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteStartAttribute(string prefix, string localName, string namespaceName)
        {
            try
            {
                if ((localName == null) || (localName.Length == 0))
                {
                    if (prefix != "xmlns")
                    {
                        throw new ArgumentException(Res.GetString("Xml_EmptyLocalName"));
                    }
                    localName = "xmlns";
                    prefix = string.Empty;
                }
                this.CheckNCName(localName);
                this.AdvanceState(Token.StartAttribute);
                if (prefix == null)
                {
                    if ((namespaceName != null) && ((localName != "xmlns") || (namespaceName != "http://www.w3.org/2000/xmlns/")))
                    {
                        prefix = this.LookupPrefix(namespaceName);
                    }
                    if (prefix == null)
                    {
                        prefix = string.Empty;
                    }
                }
                if (namespaceName == null)
                {
                    if ((prefix != null) && (prefix.Length > 0))
                    {
                        namespaceName = this.LookupNamespace(prefix);
                    }
                    if (namespaceName == null)
                    {
                        namespaceName = string.Empty;
                    }
                }
                if (prefix.Length == 0)
                {
                    if ((localName[0] == 'x') && (localName == "xmlns"))
                    {
                        if ((namespaceName.Length > 0) && (namespaceName != "http://www.w3.org/2000/xmlns/"))
                        {
                            throw new ArgumentException(Res.GetString("Xml_XmlnsPrefix"));
                        }
                        this.curDeclPrefix = string.Empty;
                        this.SetSpecialAttribute(SpecialAttribute.DefaultXmlns);
                        goto Label_0238;
                    }
                    if (namespaceName.Length > 0)
                    {
                        prefix = this.LookupPrefix(namespaceName);
                        if ((prefix == null) || (prefix.Length == 0))
                        {
                            prefix = this.GeneratePrefix();
                        }
                    }
                }
                else
                {
                    if (prefix[0] == 'x')
                    {
                        if (prefix == "xmlns")
                        {
                            if ((namespaceName.Length > 0) && (namespaceName != "http://www.w3.org/2000/xmlns/"))
                            {
                                throw new ArgumentException(Res.GetString("Xml_XmlnsPrefix"));
                            }
                            this.curDeclPrefix = localName;
                            this.SetSpecialAttribute(SpecialAttribute.PrefixedXmlns);
                            goto Label_0238;
                        }
                        if (prefix == "xml")
                        {
                            if ((namespaceName.Length > 0) && (namespaceName != "http://www.w3.org/XML/1998/namespace"))
                            {
                                throw new ArgumentException(Res.GetString("Xml_XmlPrefix"));
                            }
                            switch (localName)
                            {
                                case "space":
                                    this.SetSpecialAttribute(SpecialAttribute.XmlSpace);
                                    goto Label_0238;

                                case "lang":
                                    this.SetSpecialAttribute(SpecialAttribute.XmlLang);
                                    goto Label_0238;
                            }
                        }
                    }
                    this.CheckNCName(prefix);
                    if (namespaceName.Length == 0)
                    {
                        prefix = string.Empty;
                    }
                    else
                    {
                        string localNamespace = this.LookupLocalNamespace(prefix);
                        if ((localNamespace != null) && (localNamespace != namespaceName))
                        {
                            prefix = this.GeneratePrefix();
                        }
                    }
                }
                if (prefix.Length != 0)
                {
                    this.PushNamespace(prefix, namespaceName, false);
                }
                this.writer.WriteStartAttribute(prefix, localName, namespaceName);
            Label_0238:
                this.AddAttribute(prefix, localName, namespaceName);
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteStartDocument()
        {
            this.WriteStartDocumentImpl(XmlStandalone.Omit);
        }

        public override void WriteStartDocument(bool standalone)
        {
            this.WriteStartDocumentImpl(standalone ? XmlStandalone.Yes : XmlStandalone.No);
        }

        private void WriteStartDocumentImpl(XmlStandalone standalone)
        {
            try
            {
                this.AdvanceState(Token.StartDocument);
                if (this.conformanceLevel == ConformanceLevel.Auto)
                {
                    this.conformanceLevel = ConformanceLevel.Document;
                    this.stateTable = StateTableDocument;
                }
                else if (this.conformanceLevel == ConformanceLevel.Fragment)
                {
                    throw new InvalidOperationException(Res.GetString("Xml_CannotStartDocumentOnFragment"));
                }
                if (this.rawWriter != null)
                {
                    if (!this.xmlDeclFollows)
                    {
                        this.rawWriter.WriteXmlDeclaration(standalone);
                    }
                }
                else
                {
                    this.writer.WriteStartDocument();
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            try
            {
                if ((localName == null) || (localName.Length == 0))
                {
                    throw new ArgumentException(Res.GetString("Xml_EmptyLocalName"));
                }
                this.CheckNCName(localName);
                this.AdvanceState(Token.StartElement);
                if (prefix == null)
                {
                    if (ns != null)
                    {
                        prefix = this.LookupPrefix(ns);
                    }
                    if (prefix == null)
                    {
                        prefix = string.Empty;
                    }
                }
                else if (prefix.Length > 0)
                {
                    this.CheckNCName(prefix);
                    if (ns == null)
                    {
                        ns = this.LookupNamespace(prefix);
                    }
                    if ((ns == null) || ((ns != null) && (ns.Length == 0)))
                    {
                        throw new ArgumentException(Res.GetString("Xml_PrefixForEmptyNs"));
                    }
                }
                if (ns == null)
                {
                    ns = this.LookupNamespace(prefix);
                    if (ns == null)
                    {
                        ns = string.Empty;
                    }
                }
                if ((this.elemTop == 0) && (this.rawWriter != null))
                {
                    this.rawWriter.OnRootElement(this.conformanceLevel);
                }
                this.writer.WriteStartElement(prefix, localName, ns);
                int length = ++this.elemTop;
                if (length == this.elemScopeStack.Length)
                {
                    ElementScope[] destinationArray = new ElementScope[length * 2];
                    Array.Copy(this.elemScopeStack, destinationArray, length);
                    this.elemScopeStack = destinationArray;
                }
                this.elemScopeStack[length].Set(prefix, localName, ns, this.nsTop);
                this.PushNamespace(prefix, ns, false);
                if (this.attrCount >= 14)
                {
                    this.attrHashTable.Clear();
                }
                this.attrCount = 0;
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteString(string text)
        {
            try
            {
                if (text != null)
                {
                    this.AdvanceState(Token.Text);
                    if (this.SaveAttrValue)
                    {
                        this.attrValue.Append(text);
                    }
                    else
                    {
                        this.writer.WriteString(text);
                    }
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            try
            {
                if (!char.IsSurrogatePair(highChar, lowChar))
                {
                    throw XmlConvert.CreateInvalidSurrogatePairException(lowChar, highChar);
                }
                this.AdvanceState(Token.Text);
                if (this.SaveAttrValue)
                {
                    this.attrValue.Append(highChar);
                    this.attrValue.Append(lowChar);
                }
                else
                {
                    this.writer.WriteSurrogateCharEntity(lowChar, highChar);
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteValue(bool value)
        {
            try
            {
                this.AdvanceState(Token.AtomicValue);
                this.writer.WriteValue(value);
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteValue(DateTime value)
        {
            try
            {
                this.AdvanceState(Token.AtomicValue);
                this.writer.WriteValue(value);
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteValue(decimal value)
        {
            try
            {
                this.AdvanceState(Token.AtomicValue);
                this.writer.WriteValue(value);
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteValue(double value)
        {
            try
            {
                this.AdvanceState(Token.AtomicValue);
                this.writer.WriteValue(value);
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteValue(int value)
        {
            try
            {
                this.AdvanceState(Token.AtomicValue);
                this.writer.WriteValue(value);
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteValue(long value)
        {
            try
            {
                this.AdvanceState(Token.AtomicValue);
                this.writer.WriteValue(value);
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteValue(object value)
        {
            try
            {
                if (this.SaveAttrValue && (value is string))
                {
                    this.AdvanceState(Token.Text);
                    this.attrValue.Append(value);
                }
                else
                {
                    this.AdvanceState(Token.AtomicValue);
                    this.writer.WriteValue(value);
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteValue(float value)
        {
            try
            {
                this.AdvanceState(Token.AtomicValue);
                this.writer.WriteValue(value);
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteValue(string value)
        {
            try
            {
                if (this.SaveAttrValue)
                {
                    this.AdvanceState(Token.Text);
                    this.attrValue.Append(value);
                }
                else
                {
                    this.AdvanceState(Token.AtomicValue);
                    this.writer.WriteValue(value);
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        public override void WriteWhitespace(string ws)
        {
            try
            {
                if (ws == null)
                {
                    ws = string.Empty;
                }
                if (!XmlCharType.Instance.IsOnlyWhitespace(ws))
                {
                    throw new ArgumentException(Res.GetString("Xml_NonWhitespace"));
                }
                this.AdvanceState(Token.Whitespace);
                if (this.SaveAttrValue)
                {
                    this.attrValue.Append(ws);
                }
                else
                {
                    this.writer.WriteWhitespace(ws);
                }
            }
            catch
            {
                this.currentState = State.Error;
                throw;
            }
        }

        internal XmlWriter InnerWriter =>
            this.writer;

        private bool IsClosedOrErrorState =>
            (this.currentState >= State.Closed);

        internal XmlRawWriter RawWriter =>
            this.rawWriter;

        private bool SaveAttrValue =>
            (this.specAttr != SpecialAttribute.No);

        public override XmlWriterSettings Settings
        {
            get
            {
                XmlWriterSettings settings = this.writer.Settings;
                settings.ReadOnly = false;
                settings.ConformanceLevel = this.conformanceLevel;
                settings.ReadOnly = true;
                return settings;
            }
        }

        public override System.Xml.WriteState WriteState
        {
            get
            {
                if (this.currentState <= State.Error)
                {
                    return state2WriteState[(int) this.currentState];
                }
                return System.Xml.WriteState.Error;
            }
        }

        public override string XmlLang
        {
            get
            {
                int elemTop = this.elemTop;
                while ((elemTop > 0) && (this.elemScopeStack[elemTop].xmlLang == null))
                {
                    elemTop--;
                }
                return this.elemScopeStack[elemTop].xmlLang;
            }
        }

        public override System.Xml.XmlSpace XmlSpace
        {
            get
            {
                int elemTop = this.elemTop;
                while ((elemTop >= 0) && (this.elemScopeStack[elemTop].xmlSpace == ~System.Xml.XmlSpace.None))
                {
                    elemTop--;
                }
                return this.elemScopeStack[elemTop].xmlSpace;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct AttrName
        {
            internal string prefix;
            internal string namespaceUri;
            internal string localName;
            internal int prev;
            internal void Set(string prefix, string localName, string namespaceUri)
            {
                this.prefix = prefix;
                this.namespaceUri = namespaceUri;
                this.localName = localName;
                this.prev = 0;
            }

            internal bool IsDuplicate(string prefix, string localName, string namespaceUri)
            {
                if (this.localName != localName)
                {
                    return false;
                }
                if (this.prefix != prefix)
                {
                    return (this.namespaceUri == namespaceUri);
                }
                return true;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ElementScope
        {
            internal int prevNSTop;
            internal string prefix;
            internal string localName;
            internal string namespaceUri;
            internal XmlSpace xmlSpace;
            internal string xmlLang;
            internal void Set(string prefix, string localName, string namespaceUri, int prevNSTop)
            {
                this.prevNSTop = prevNSTop;
                this.prefix = prefix;
                this.namespaceUri = namespaceUri;
                this.localName = localName;
                this.xmlSpace = ~XmlSpace.None;
                this.xmlLang = null;
            }

            internal void WriteEndElement(XmlRawWriter rawWriter)
            {
                rawWriter.WriteEndElement(this.prefix, this.localName, this.namespaceUri);
            }

            internal void WriteFullEndElement(XmlRawWriter rawWriter)
            {
                rawWriter.WriteFullEndElement(this.prefix, this.localName, this.namespaceUri);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Namespace
        {
            internal string prefix;
            internal string namespaceUri;
            internal XmlWellFormedWriter.NamespaceKind kind;
            internal int prevNsIndex;
            internal void Set(string prefix, string namespaceUri, XmlWellFormedWriter.NamespaceKind kind)
            {
                this.prefix = prefix;
                this.namespaceUri = namespaceUri;
                this.kind = kind;
                this.prevNsIndex = -1;
            }

            internal void WriteDecl(XmlWriter writer, XmlRawWriter rawWriter)
            {
                if (rawWriter != null)
                {
                    rawWriter.WriteNamespaceDeclaration(this.prefix, this.namespaceUri);
                }
                else
                {
                    if (this.prefix.Length == 0)
                    {
                        writer.WriteStartAttribute(string.Empty, "xmlns", "http://www.w3.org/2000/xmlns/");
                    }
                    else
                    {
                        writer.WriteStartAttribute("xmlns", this.prefix, "http://www.w3.org/2000/xmlns/");
                    }
                    writer.WriteString(this.namespaceUri);
                    writer.WriteEndAttribute();
                }
            }
        }

        private enum NamespaceKind
        {
            Written,
            NeedToWrite,
            Implied,
            Special
        }

        private class NamespaceResolverProxy : IXmlNamespaceResolver
        {
            private XmlWellFormedWriter wfWriter;

            internal NamespaceResolverProxy(XmlWellFormedWriter wfWriter)
            {
                this.wfWriter = wfWriter;
            }

            IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
            {
                throw new NotImplementedException();
            }

            string IXmlNamespaceResolver.LookupNamespace(string prefix) => 
                this.wfWriter.LookupNamespace(prefix);

            string IXmlNamespaceResolver.LookupPrefix(string namespaceName) => 
                this.wfWriter.LookupPrefix(namespaceName);
        }

        private enum SpecialAttribute
        {
            No,
            DefaultXmlns,
            PrefixedXmlns,
            XmlSpace,
            XmlLang
        }

        private enum State
        {
            AfterRootEle = 7,
            AfterRootLevelAttr = 14,
            Attribute = 8,
            B64Attribute = 6,
            B64Content = 5,
            Closed = 15,
            Content = 4,
            Document = 2,
            Element = 3,
            EndAttrEEle = 0x6c,
            EndAttrSAttr = 0x6f,
            EndAttrSCont = 0x6d,
            EndAttrSEle = 0x6b,
            EndDocument = 10,
            Error = 0x10,
            PostB64Attr = 0x71,
            PostB64Cont = 0x70,
            PostB64RootAttr = 0x72,
            RootLevelAttr = 11,
            RootLevelB64Attr = 13,
            RootLevelSpecAttr = 12,
            SpecialAttr = 9,
            Start = 0,
            StartContent = 0x65,
            StartContentB64 = 0x67,
            StartContentEle = 0x66,
            StartDoc = 0x68,
            StartDocEle = 0x6a,
            StartFragB64 = 0x75,
            StartFragCont = 0x74,
            StartFragEle = 0x73,
            StartRootLevelAttr = 0x76,
            TopLevel = 1
        }

        private enum Token
        {
            StartDocument,
            EndDocument,
            PI,
            Comment,
            Dtd,
            StartElement,
            EndElement,
            StartAttribute,
            EndAttribute,
            Text,
            CData,
            AtomicValue,
            Base64,
            RawData,
            Whitespace
        }
    }
}

