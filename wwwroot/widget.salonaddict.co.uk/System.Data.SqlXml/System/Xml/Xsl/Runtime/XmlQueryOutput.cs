namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class XmlQueryOutput : XmlWriter
    {
        private XmlAttributeCache attrCache;
        private int cntNmsp;
        private Hashtable conflictPrefixes;
        private int depth;
        private StringConcat nodeText;
        private XmlNamespaceManager nsmgr;
        private string piTarget;
        private int prefixIndex;
        private XPathNodeType rootType;
        private XmlQueryRuntime runtime;
        private XmlSequenceWriter seqwrt;
        private Stack stkNames;
        private bool useDefNmsp;
        private XmlState xstate;
        private XmlRawWriter xwrt;

        internal XmlQueryOutput(XmlQueryRuntime runtime, XmlEventCache xwrt)
        {
            this.runtime = runtime;
            this.xwrt = xwrt;
            this.xstate = XmlState.WithinContent;
            this.depth = 1;
            this.rootType = XPathNodeType.Root;
        }

        internal XmlQueryOutput(XmlQueryRuntime runtime, XmlSequenceWriter seqwrt)
        {
            this.runtime = runtime;
            this.seqwrt = seqwrt;
            this.xstate = XmlState.WithinSequence;
        }

        private void AddNamespace(string prefix, string ns)
        {
            this.nsmgr.AddNamespace(prefix, ns);
            this.cntNmsp++;
            if (ns.Length == 0)
            {
                this.useDefNmsp = true;
            }
        }

        private string CheckAttributePrefix(string prefix, string ns)
        {
            string str;
            if (this.nsmgr == null)
            {
                this.WriteNamespaceDeclarationUnchecked(prefix, ns);
                return prefix;
            }
        Label_0012:
            str = this.nsmgr.LookupNamespace(prefix);
            if (str != ns)
            {
                if (str != null)
                {
                    prefix = this.RemapPrefix(prefix, ns, false);
                    goto Label_0012;
                }
                this.AddNamespace(prefix, ns);
            }
            return prefix;
        }

        public override void Close()
        {
        }

        private void ConstructInEnumAttrs(XPathNodeType rootType)
        {
            switch (this.xstate)
            {
                case XmlState.WithinSequence:
                    this.StartTree(rootType);
                    this.xstate = XmlState.EnumAttrs;
                    return;

                case XmlState.EnumAttrs:
                    break;

                default:
                    this.ThrowInvalidStateError(rootType);
                    break;
            }
        }

        private void ConstructWithinContent(XPathNodeType rootType)
        {
            switch (this.xstate)
            {
                case XmlState.WithinSequence:
                    this.StartTree(rootType);
                    this.xstate = XmlState.WithinContent;
                    return;

                case XmlState.EnumAttrs:
                    this.StartElementContentUnchecked();
                    return;

                case XmlState.WithinContent:
                    break;

                default:
                    this.ThrowInvalidStateError(rootType);
                    break;
            }
        }

        private void CopyNamespaces(XPathNavigator navigator, XPathNamespaceScope nsScope)
        {
            if (navigator.NamespaceURI.Length == 0)
            {
                this.WriteNamespaceDeclarationUnchecked(string.Empty, string.Empty);
            }
            if (navigator.MoveToFirstNamespace(nsScope))
            {
                this.CopyNamespacesHelper(navigator, nsScope);
                navigator.MoveToParent();
            }
        }

        private void CopyNamespacesHelper(XPathNavigator navigator, XPathNamespaceScope nsScope)
        {
            string localName = navigator.LocalName;
            string ns = navigator.Value;
            if (navigator.MoveToNextNamespace(nsScope))
            {
                this.CopyNamespacesHelper(navigator, nsScope);
            }
            this.WriteNamespaceDeclarationUnchecked(localName, ns);
        }

        private void CopyNode(XPathNavigator navigator)
        {
            int depth = this.depth;
        Label_0007:
            if (this.StartCopy(navigator, this.depth == depth))
            {
                XPathNodeType nodeType = navigator.NodeType;
                if (navigator.MoveToFirstAttribute())
                {
                    do
                    {
                        this.StartCopy(navigator, false);
                    }
                    while (navigator.MoveToNextAttribute());
                    navigator.MoveToParent();
                }
                this.CopyNamespaces(navigator, ((this.depth - 1) == depth) ? XPathNamespaceScope.ExcludeXml : XPathNamespaceScope.Local);
                this.StartElementContentUnchecked();
                if (navigator.MoveToFirstChild())
                {
                    goto Label_0007;
                }
                this.EndCopy(navigator, (this.depth - 1) == depth);
            }
            while (this.depth != depth)
            {
                if (navigator.MoveToNext())
                {
                    goto Label_0007;
                }
                navigator.MoveToParent();
                this.EndCopy(navigator, (this.depth - 1) == depth);
            }
        }

        public void EndCopy(XPathNavigator navigator)
        {
            if (navigator.NodeType == XPathNodeType.Element)
            {
                this.WriteEndElement();
            }
        }

        private void EndCopy(XPathNavigator navigator, bool callChk)
        {
            if (callChk)
            {
                this.WriteEndElement();
            }
            else
            {
                this.WriteEndElementUnchecked(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
            }
        }

        public void EndTree()
        {
            this.seqwrt.EndTree();
            this.xstate = XmlState.WithinSequence;
            this.Writer = null;
        }

        private string EnsureValidName(string prefix, string localName, string ns, XPathNodeType nodeType)
        {
            if (!ValidateNames.ValidateName(prefix, localName, ns, nodeType, ValidateNames.Flags.AllExceptNCNames))
            {
                prefix = (ns.Length != 0) ? this.RemapPrefix(string.Empty, ns, nodeType == XPathNodeType.Element) : string.Empty;
                ValidateNames.ValidateNameThrow(prefix, localName, ns, nodeType, ValidateNames.Flags.AllExceptNCNames);
            }
            return prefix;
        }

        public override void Flush()
        {
        }

        public override string LookupPrefix(string ns)
        {
            throw new NotSupportedException();
        }

        private void PopElementNames(out string prefix, out string localName, out string ns)
        {
            ns = this.stkNames.Pop() as string;
            localName = this.stkNames.Pop() as string;
            prefix = this.stkNames.Pop() as string;
        }

        private void PushElementNames(string prefix, string localName, string ns)
        {
            if (this.stkNames == null)
            {
                this.stkNames = new Stack();
            }
            this.stkNames.Push(prefix);
            this.stkNames.Push(localName);
            this.stkNames.Push(ns);
        }

        private string RemapPrefix(string prefix, string ns, bool isElemPrefix)
        {
            if (this.conflictPrefixes == null)
            {
                this.conflictPrefixes = new Hashtable(0x10);
            }
            if (this.nsmgr == null)
            {
                this.nsmgr = new XmlNamespaceManager(this.runtime.NameTable);
                this.nsmgr.PushScope();
            }
            string str = this.nsmgr.LookupPrefix(ns);
            if ((str == null) || (!isElemPrefix && (str.Length == 0)))
            {
                str = this.conflictPrefixes[ns] as string;
                if (((str == null) || (str == prefix)) || (!isElemPrefix && (str.Length == 0)))
                {
                    str = string.Format(CultureInfo.InvariantCulture, "xp_{0}", new object[] { this.prefixIndex++ });
                }
            }
            this.conflictPrefixes[ns] = str;
            return str;
        }

        private void SetWrappedWriter(XmlRawWriter writer)
        {
            if (this.Writer is XmlAttributeCache)
            {
                this.attrCache = (XmlAttributeCache) this.Writer;
            }
            this.Writer = writer;
        }

        public bool StartCopy(XPathNavigator navigator)
        {
            if (navigator.NodeType == XPathNodeType.Root)
            {
                return true;
            }
            if (this.StartCopy(navigator, true))
            {
                this.CopyNamespaces(navigator, XPathNamespaceScope.ExcludeXml);
                return true;
            }
            return false;
        }

        private bool StartCopy(XPathNavigator navigator, bool callChk)
        {
            bool flag = false;
            switch (navigator.NodeType)
            {
                case XPathNodeType.Root:
                    this.ThrowInvalidStateError(XPathNodeType.Root);
                    return flag;

                case XPathNodeType.Element:
                    if (!callChk)
                    {
                        this.WriteStartElementUnchecked(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
                        break;
                    }
                    this.WriteStartElement(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
                    break;

                case XPathNodeType.Attribute:
                    if (!callChk)
                    {
                        this.WriteStartAttributeUnchecked(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
                    }
                    else
                    {
                        this.WriteStartAttribute(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
                    }
                    this.WriteString(navigator.Value);
                    if (callChk)
                    {
                        this.WriteEndAttribute();
                        return flag;
                    }
                    this.WriteEndAttributeUnchecked();
                    return flag;

                case XPathNodeType.Namespace:
                    if (callChk)
                    {
                        XmlAttributeCache writer = this.Writer as XmlAttributeCache;
                        if ((writer != null) && (writer.Count != 0))
                        {
                            throw new XslTransformException("XmlIl_NmspAfterAttr", new string[] { string.Empty });
                        }
                        this.WriteNamespaceDeclaration(navigator.LocalName, navigator.Value);
                        return flag;
                    }
                    this.WriteNamespaceDeclarationUnchecked(navigator.LocalName, navigator.Value);
                    return flag;

                case XPathNodeType.Text:
                case XPathNodeType.SignificantWhitespace:
                case XPathNodeType.Whitespace:
                    if (!callChk)
                    {
                        this.WriteStringUnchecked(navigator.Value);
                        return flag;
                    }
                    this.WriteString(navigator.Value, false);
                    return flag;

                case XPathNodeType.ProcessingInstruction:
                    this.WriteStartProcessingInstruction(navigator.LocalName);
                    this.WriteProcessingInstructionString(navigator.Value);
                    this.WriteEndProcessingInstruction();
                    return flag;

                case XPathNodeType.Comment:
                    this.WriteStartComment();
                    this.WriteCommentString(navigator.Value);
                    this.WriteEndComment();
                    return flag;

                default:
                    return flag;
            }
            return true;
        }

        public void StartElementContentUnchecked()
        {
            if (this.cntNmsp != 0)
            {
                this.WriteCachedNamespaces();
            }
            this.Writer.StartElementContent();
            this.xstate = XmlState.WithinContent;
            this.useDefNmsp = false;
        }

        public void StartTree(XPathNodeType rootType)
        {
            this.Writer = this.seqwrt.StartTree(rootType, this.nsmgr, this.runtime.NameTable);
            this.rootType = rootType;
            this.xstate = ((rootType == XPathNodeType.Attribute) || (rootType == XPathNodeType.Namespace)) ? XmlState.EnumAttrs : XmlState.WithinContent;
        }

        private void ThrowInvalidStateError(XPathNodeType constructorType)
        {
            switch (constructorType)
            {
                case XPathNodeType.Root:
                case XPathNodeType.Element:
                case XPathNodeType.Text:
                case XPathNodeType.ProcessingInstruction:
                case XPathNodeType.Comment:
                    break;

                case XPathNodeType.Attribute:
                case XPathNodeType.Namespace:
                    if (this.depth == 1)
                    {
                        throw new XslTransformException("XmlIl_BadXmlState", new string[] { constructorType.ToString(), this.rootType.ToString() });
                    }
                    if (this.xstate != XmlState.WithinContent)
                    {
                        break;
                    }
                    throw new XslTransformException("XmlIl_BadXmlStateAttr", new string[] { string.Empty });

                default:
                    throw new XslTransformException("XmlIl_BadXmlState", new string[] { "Unknown", this.XmlStateToNodeType(this.xstate).ToString() });
            }
            throw new XslTransformException("XmlIl_BadXmlState", new string[] { constructorType.ToString(), this.XmlStateToNodeType(this.xstate).ToString() });
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            throw new NotSupportedException();
        }

        private void WriteCachedNamespaces()
        {
            while (this.cntNmsp != 0)
            {
                string str;
                string str2;
                this.cntNmsp--;
                this.nsmgr.GetNamespaceDeclaration(this.cntNmsp, out str, out str2);
                this.Writer.WriteNamespaceDeclaration(str, str2);
            }
        }

        public override void WriteCData(string text)
        {
            this.WriteString(text, false);
        }

        public override void WriteCharEntity(char ch)
        {
            throw new NotSupportedException();
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            throw new NotSupportedException();
        }

        public override void WriteComment(string text)
        {
            this.WriteStartComment();
            this.WriteCommentString(text);
            this.WriteEndComment();
        }

        public void WriteCommentString(string text)
        {
            this.nodeText.ConcatNoDelimiter(text);
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            throw new NotSupportedException();
        }

        public override void WriteEndAttribute()
        {
            if (this.xstate == XmlState.WithinNmsp)
            {
                this.WriteEndNamespace();
            }
            else
            {
                this.WriteEndAttributeUnchecked();
                if (this.depth == 0)
                {
                    this.EndTree();
                }
            }
        }

        public void WriteEndAttributeUnchecked()
        {
            this.Writer.WriteEndAttribute();
            this.xstate = XmlState.EnumAttrs;
            this.depth--;
        }

        public void WriteEndComment()
        {
            this.Writer.WriteComment(this.nodeText.GetResult());
            this.xstate = XmlState.WithinContent;
            this.depth--;
            if (this.depth == 0)
            {
                this.EndTree();
            }
        }

        public override void WriteEndDocument()
        {
            throw new NotSupportedException();
        }

        public override void WriteEndElement()
        {
            string str;
            string str2;
            string str3;
            if (this.xstate == XmlState.EnumAttrs)
            {
                this.StartElementContentUnchecked();
            }
            this.PopElementNames(out str, out str2, out str3);
            this.WriteEndElementUnchecked(str, str2, str3);
            if (this.depth == 0)
            {
                this.EndTree();
            }
        }

        public void WriteEndElementUnchecked(string localName)
        {
            this.WriteEndElementUnchecked(string.Empty, localName, string.Empty);
        }

        public void WriteEndElementUnchecked(string prefix, string localName, string ns)
        {
            this.Writer.WriteEndElement(prefix, localName, ns);
            this.xstate = XmlState.WithinContent;
            this.depth--;
            if (this.nsmgr != null)
            {
                this.nsmgr.PopScope();
            }
        }

        public void WriteEndNamespace()
        {
            this.xstate = XmlState.EnumAttrs;
            this.depth--;
            this.WriteNamespaceDeclaration(this.piTarget, this.nodeText.GetResult());
            if (this.depth == 0)
            {
                this.EndTree();
            }
        }

        public void WriteEndProcessingInstruction()
        {
            this.Writer.WriteProcessingInstruction(this.piTarget, this.nodeText.GetResult());
            this.xstate = XmlState.WithinContent;
            this.depth--;
            if (this.depth == 0)
            {
                this.EndTree();
            }
        }

        public void WriteEndRoot()
        {
            this.depth--;
            this.EndTree();
        }

        public override void WriteEntityRef(string name)
        {
            throw new NotSupportedException();
        }

        public override void WriteFullEndElement()
        {
            this.WriteEndElement();
        }

        public void WriteItem(XPathItem item)
        {
            if (item.IsNode)
            {
                XPathNavigator navigator = (XPathNavigator) item;
                if (this.xstate == XmlState.WithinSequence)
                {
                    this.seqwrt.WriteItem(navigator);
                }
                else
                {
                    this.CopyNode(navigator);
                }
            }
            else
            {
                this.seqwrt.WriteItem(item);
            }
        }

        public void WriteNamespaceDeclaration(string prefix, string ns)
        {
            this.ConstructInEnumAttrs(XPathNodeType.Namespace);
            if (this.nsmgr == null)
            {
                this.WriteNamespaceDeclarationUnchecked(prefix, ns);
            }
            else
            {
                string str = this.nsmgr.LookupNamespace(prefix);
                if (ns != str)
                {
                    if ((str != null) && (((prefix.Length != 0) || (str.Length != 0)) || this.useDefNmsp))
                    {
                        throw new XslTransformException("XmlIl_NmspConflict", new string[] { (prefix.Length == 0) ? "" : ":", prefix, ns, str });
                    }
                    this.AddNamespace(prefix, ns);
                }
            }
            if (this.depth == 0)
            {
                this.EndTree();
            }
        }

        public void WriteNamespaceDeclarationUnchecked(string prefix, string ns)
        {
            if (this.depth == 0)
            {
                this.Writer.WriteNamespaceDeclaration(prefix, ns);
            }
            else
            {
                if (this.nsmgr == null)
                {
                    if ((ns.Length == 0) && (prefix.Length == 0))
                    {
                        return;
                    }
                    this.nsmgr = new XmlNamespaceManager(this.runtime.NameTable);
                    this.nsmgr.PushScope();
                }
                if (this.nsmgr.LookupNamespace(prefix) != ns)
                {
                    this.AddNamespace(prefix, ns);
                }
            }
        }

        public void WriteNamespaceString(string text)
        {
            this.nodeText.ConcatNoDelimiter(text);
        }

        public override void WriteProcessingInstruction(string target, string text)
        {
            this.WriteStartProcessingInstruction(target);
            this.WriteProcessingInstructionString(text);
            this.WriteEndProcessingInstruction();
        }

        public void WriteProcessingInstructionString(string text)
        {
            this.nodeText.ConcatNoDelimiter(text);
        }

        public override void WriteRaw(string data)
        {
            this.WriteString(data, true);
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            throw new NotSupportedException();
        }

        public void WriteRawUnchecked(string text)
        {
            this.Writer.WriteRaw(text);
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            if ((prefix.Length == 5) && (prefix == "xmlns"))
            {
                this.WriteStartNamespace(localName);
            }
            else
            {
                this.ConstructInEnumAttrs(XPathNodeType.Attribute);
                if ((ns.Length != 0) && (this.depth != 0))
                {
                    prefix = this.CheckAttributePrefix(prefix, ns);
                }
                this.WriteStartAttributeUnchecked(prefix, localName, ns);
            }
        }

        public void WriteStartAttributeComputed(XmlQualifiedName name)
        {
            this.WriteStartComputed(XPathNodeType.Attribute, name);
        }

        public void WriteStartAttributeComputed(XPathNavigator navigator)
        {
            this.WriteStartComputed(XPathNodeType.Attribute, navigator);
        }

        public void WriteStartAttributeComputed(string tagName, int prefixMappingsIndex)
        {
            this.WriteStartComputed(XPathNodeType.Attribute, tagName, prefixMappingsIndex);
        }

        public void WriteStartAttributeComputed(string tagName, string ns)
        {
            this.WriteStartComputed(XPathNodeType.Attribute, tagName, ns);
        }

        public void WriteStartAttributeLocalName(string localName)
        {
            this.WriteStartAttribute(string.Empty, localName, string.Empty);
        }

        public void WriteStartAttributeUnchecked(string localName)
        {
            this.WriteStartAttributeUnchecked(string.Empty, localName, string.Empty);
        }

        public void WriteStartAttributeUnchecked(string prefix, string localName, string ns)
        {
            this.Writer.WriteStartAttribute(prefix, localName, ns);
            this.xstate = XmlState.WithinAttr;
            this.depth++;
        }

        public void WriteStartComment()
        {
            this.ConstructWithinContent(XPathNodeType.Comment);
            this.nodeText.Clear();
            this.xstate = XmlState.WithinComment;
            this.depth++;
        }

        private void WriteStartComputed(XPathNodeType nodeType, XmlQualifiedName name)
        {
            string prefix = (name.Namespace.Length != 0) ? this.RemapPrefix(string.Empty, name.Namespace, nodeType == XPathNodeType.Element) : string.Empty;
            prefix = this.EnsureValidName(prefix, name.Name, name.Namespace, nodeType);
            if (nodeType == XPathNodeType.Element)
            {
                this.WriteStartElement(prefix, name.Name, name.Namespace);
            }
            else
            {
                this.WriteStartAttribute(prefix, name.Name, name.Namespace);
            }
        }

        private void WriteStartComputed(XPathNodeType nodeType, XPathNavigator navigator)
        {
            string prefix = navigator.Prefix;
            string localName = navigator.LocalName;
            string namespaceURI = navigator.NamespaceURI;
            if (navigator.NodeType != nodeType)
            {
                prefix = this.EnsureValidName(prefix, localName, namespaceURI, nodeType);
            }
            if (nodeType == XPathNodeType.Element)
            {
                this.WriteStartElement(prefix, localName, namespaceURI);
            }
            else
            {
                this.WriteStartAttribute(prefix, localName, namespaceURI);
            }
        }

        private void WriteStartComputed(XPathNodeType nodeType, string tagName, int prefixMappingsIndex)
        {
            string str;
            string str2;
            string str3;
            this.runtime.ParseTagName(tagName, prefixMappingsIndex, out str, out str2, out str3);
            str = this.EnsureValidName(str, str2, str3, nodeType);
            if (nodeType == XPathNodeType.Element)
            {
                this.WriteStartElement(str, str2, str3);
            }
            else
            {
                this.WriteStartAttribute(str, str2, str3);
            }
        }

        private void WriteStartComputed(XPathNodeType nodeType, string tagName, string ns)
        {
            string str;
            string str2;
            ValidateNames.ParseQNameThrow(tagName, out str, out str2);
            str = this.EnsureValidName(str, str2, ns, nodeType);
            if (nodeType == XPathNodeType.Element)
            {
                this.WriteStartElement(str, str2, ns);
            }
            else
            {
                this.WriteStartAttribute(str, str2, ns);
            }
        }

        public override void WriteStartDocument()
        {
            throw new NotSupportedException();
        }

        public override void WriteStartDocument(bool standalone)
        {
            throw new NotSupportedException();
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            this.ConstructWithinContent(XPathNodeType.Element);
            this.WriteStartElementUnchecked(prefix, localName, ns);
            this.WriteNamespaceDeclarationUnchecked(prefix, ns);
            if (this.attrCache == null)
            {
                this.attrCache = new XmlAttributeCache();
            }
            this.attrCache.Init(this.Writer);
            this.Writer = this.attrCache;
            this.attrCache = null;
            this.PushElementNames(prefix, localName, ns);
        }

        public void WriteStartElementComputed(XmlQualifiedName name)
        {
            this.WriteStartComputed(XPathNodeType.Element, name);
        }

        public void WriteStartElementComputed(XPathNavigator navigator)
        {
            this.WriteStartComputed(XPathNodeType.Element, navigator);
        }

        public void WriteStartElementComputed(string tagName, int prefixMappingsIndex)
        {
            this.WriteStartComputed(XPathNodeType.Element, tagName, prefixMappingsIndex);
        }

        public void WriteStartElementComputed(string tagName, string ns)
        {
            this.WriteStartComputed(XPathNodeType.Element, tagName, ns);
        }

        public void WriteStartElementLocalName(string localName)
        {
            this.WriteStartElement(string.Empty, localName, string.Empty);
        }

        public void WriteStartElementUnchecked(string localName)
        {
            this.WriteStartElementUnchecked(string.Empty, localName, string.Empty);
        }

        public void WriteStartElementUnchecked(string prefix, string localName, string ns)
        {
            if (this.nsmgr != null)
            {
                this.nsmgr.PushScope();
            }
            this.Writer.WriteStartElement(prefix, localName, ns);
            this.xstate = XmlState.EnumAttrs;
            this.depth++;
            this.useDefNmsp = ns.Length == 0;
        }

        public void WriteStartNamespace(string prefix)
        {
            this.ConstructInEnumAttrs(XPathNodeType.Namespace);
            this.piTarget = prefix;
            this.nodeText.Clear();
            this.xstate = XmlState.WithinNmsp;
            this.depth++;
        }

        public void WriteStartProcessingInstruction(string target)
        {
            this.ConstructWithinContent(XPathNodeType.ProcessingInstruction);
            ValidateNames.ValidateNameThrow("", target, "", XPathNodeType.ProcessingInstruction, ValidateNames.Flags.AllExceptPrefixMapping);
            this.piTarget = target;
            this.nodeText.Clear();
            this.xstate = XmlState.WithinPI;
            this.depth++;
        }

        public void WriteStartRoot()
        {
            if (this.xstate != XmlState.WithinSequence)
            {
                this.ThrowInvalidStateError(XPathNodeType.Root);
            }
            this.StartTree(XPathNodeType.Root);
            this.depth++;
        }

        public override void WriteString(string text)
        {
            this.WriteString(text, false);
        }

        private void WriteString(string text, bool disableOutputEscaping)
        {
            switch (this.xstate)
            {
                case XmlState.WithinSequence:
                    this.StartTree(XPathNodeType.Text);
                    break;

                case XmlState.EnumAttrs:
                    this.StartElementContentUnchecked();
                    break;

                case XmlState.WithinContent:
                    break;

                case XmlState.WithinAttr:
                    this.WriteStringUnchecked(text);
                    goto Label_0071;

                case XmlState.WithinNmsp:
                    this.WriteNamespaceString(text);
                    goto Label_0071;

                case XmlState.WithinComment:
                    this.WriteCommentString(text);
                    goto Label_0071;

                case XmlState.WithinPI:
                    this.WriteProcessingInstructionString(text);
                    goto Label_0071;

                default:
                    goto Label_0071;
            }
            if (disableOutputEscaping)
            {
                this.WriteRawUnchecked(text);
            }
            else
            {
                this.WriteStringUnchecked(text);
            }
        Label_0071:
            if (this.depth == 0)
            {
                this.EndTree();
            }
        }

        public void WriteStringUnchecked(string text)
        {
            this.Writer.WriteString(text);
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            throw new NotSupportedException();
        }

        public override void WriteWhitespace(string ws)
        {
            throw new NotSupportedException();
        }

        private XPathNodeType XmlStateToNodeType(XmlState xstate)
        {
            switch (xstate)
            {
                case XmlState.EnumAttrs:
                    return XPathNodeType.Element;

                case XmlState.WithinContent:
                    return XPathNodeType.Element;

                case XmlState.WithinAttr:
                    return XPathNodeType.Attribute;

                case XmlState.WithinComment:
                    return XPathNodeType.Comment;

                case XmlState.WithinPI:
                    return XPathNodeType.ProcessingInstruction;
            }
            return XPathNodeType.Element;
        }

        public void XsltCopyOf(XPathNavigator navigator)
        {
            RtfNavigator navigator2 = navigator as RtfNavigator;
            if (navigator2 != null)
            {
                navigator2.CopyToWriter(this);
            }
            else if (navigator.NodeType == XPathNodeType.Root)
            {
                if (navigator.MoveToFirstChild())
                {
                    do
                    {
                        this.CopyNode(navigator);
                    }
                    while (navigator.MoveToNext());
                    navigator.MoveToParent();
                }
            }
            else
            {
                this.CopyNode(navigator);
            }
        }

        internal XmlSequenceWriter SequenceWriter =>
            this.seqwrt;

        internal XmlRawWriter Writer
        {
            get => 
                this.xwrt;
            set
            {
                IRemovableWriter writer = value as IRemovableWriter;
                if (writer != null)
                {
                    writer.OnRemoveWriterEvent = new OnRemoveWriter(this.SetWrappedWriter);
                }
                this.xwrt = value;
            }
        }

        public override System.Xml.WriteState WriteState
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override string XmlLang
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override System.Xml.XmlSpace XmlSpace
        {
            get
            {
                throw new NotSupportedException();
            }
        }
    }
}

