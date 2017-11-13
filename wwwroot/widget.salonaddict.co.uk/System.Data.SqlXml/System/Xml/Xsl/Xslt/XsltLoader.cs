namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;
    using System.Xml.XmlConfiguration;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;

    internal class XsltLoader : IErrorHelper
    {
        private Compiler compiler;
        private Stylesheet curStylesheet;
        private Template curTemplate;
        private HybridDictionary documentUriInUse = new HybridDictionary();
        private XsltInput input;
        private int loadInstructionsDepth;
        private const int MAX_LOADINSTRUCTIONS_DEPTH = 0x400;
        private static QilName nullMode = AstFactory.QName(string.Empty);
        private QueryReaderSettings readerSettings;
        private XmlResolver xmlResolver;

        private static void AddInstruction(List<XslNode> content, XslNode instruction)
        {
            if (instruction != null)
            {
                content.Add(instruction);
            }
        }

        private void AttributeSetsDfs(AttributeSet attSet)
        {
            switch (attSet.CycleCheck)
            {
                case CycleCheck.NotStarted:
                    attSet.CycleCheck = CycleCheck.Processing;
                    foreach (QilName name in attSet.UsedAttributeSets)
                    {
                        AttributeSet set;
                        if (this.compiler.AttributeSets.TryGetValue(name, out set))
                        {
                            this.AttributeSetsDfs(set);
                        }
                    }
                    attSet.CycleCheck = CycleCheck.Completed;
                    return;

                case CycleCheck.Completed:
                    return;
            }
            this.compiler.ReportError(attSet.Content[0].SourceLine, "Xslt_CircularAttributeSet", new string[] { attSet.Name.QualifiedName });
        }

        private void CheckNoContent()
        {
            string qualifiedName = this.input.QualifiedName;
            bool flag = false;
            if (this.input.MoveToFirstChild())
            {
                do
                {
                    if (this.input.NodeType != XPathNodeType.Whitespace)
                    {
                        if (!flag)
                        {
                            this.ReportError("Xslt_NotEmptyContents", new string[] { qualifiedName });
                            flag = true;
                        }
                        this.input.SkipNode();
                    }
                }
                while (this.input.MoveToNextSibling());
                this.input.MoveToParent();
            }
        }

        private void CheckWithParam(List<XslNode> content, XslNode withParam)
        {
            foreach (XslNode node in content)
            {
                if ((node.NodeType == XslNodeType.WithParam) && node.Name.Equals(withParam.Name))
                {
                    this.ReportError("Xslt_DuplicateWithParam", new string[] { withParam.Name.QualifiedName });
                    break;
                }
            }
        }

        private XmlReader CreateReader(Uri uri, XmlResolver xmlResolver)
        {
            object obj2 = xmlResolver.GetEntity(uri, null, null);
            Stream stream = obj2 as Stream;
            if (stream != null)
            {
                return this.readerSettings.CreateReader(stream, uri.ToString());
            }
            XmlReader reader = obj2 as XmlReader;
            if (reader != null)
            {
                return reader;
            }
            IXPathNavigable navigable = obj2 as IXPathNavigable;
            if (navigable == null)
            {
                throw new XslLoadException("Xslt_CannotLoadStylesheet", new string[] { uri.ToString(), (obj2 == null) ? "null" : obj2.GetType().ToString() });
            }
            return XPathNavigatorReader.Create(navigable.CreateNavigator());
        }

        private QilName CreateXPathQName(string qname)
        {
            string str;
            string str2;
            string str3;
            this.ResolveQName(true, qname, out str2, out str3, out str);
            return AstFactory.QName(str2, str3, str);
        }

        private void InsertExNamespaces(string value, ref NsDecl nsList, bool extensions)
        {
            if ((value != null) && (value.Length != 0))
            {
                this.compiler.EnterForwardsCompatible();
                string[] strArray = XmlConvert.SplitString(value);
                for (int i = 0; i < strArray.Length; i++)
                {
                    strArray[i] = this.input.LookupXmlNamespace((strArray[i] == "#default") ? string.Empty : strArray[i]);
                }
                if (this.compiler.ExitForwardsCompatible(this.input.ForwardCompatibility))
                {
                    for (int j = 0; j < strArray.Length; j++)
                    {
                        if (strArray[j] != null)
                        {
                            nsList = new NsDecl(nsList, null, strArray[j]);
                            if (extensions)
                            {
                                this.input.AddExtensionNamespace(strArray[j]);
                            }
                        }
                    }
                }
            }
        }

        public void Load(Compiler compiler, object stylesheet, XmlResolver xmlResolver)
        {
            this.compiler = compiler;
            this.xmlResolver = xmlResolver ?? XmlNullResolver.Singleton;
            XmlReader reader = stylesheet as XmlReader;
            if (reader != null)
            {
                this.readerSettings = new QueryReaderSettings(reader);
                this.LoadStylesheet(reader, false);
            }
            else
            {
                string relativeUri = stylesheet as string;
                if (relativeUri != null)
                {
                    XmlResolver resolver = xmlResolver;
                    if ((xmlResolver == null) || (xmlResolver == XmlNullResolver.Singleton))
                    {
                        resolver = new XmlUrlResolver();
                    }
                    Uri uri = resolver.ResolveUri(null, relativeUri);
                    if (uri == null)
                    {
                        throw new XslLoadException("Xslt_CantResolve", new string[] { relativeUri });
                    }
                    this.readerSettings = new QueryReaderSettings(new NameTable());
                    using (reader = this.CreateReader(uri, resolver))
                    {
                        this.LoadStylesheet(reader, false);
                        goto Label_00EA;
                    }
                }
                IXPathNavigable navigable = stylesheet as IXPathNavigable;
                if (navigable != null)
                {
                    reader = XPathNavigatorReader.Create(navigable.CreateNavigator());
                    this.readerSettings = new QueryReaderSettings(reader.NameTable);
                    this.LoadStylesheet(reader, false);
                }
            }
        Label_00EA:
            this.Process();
        }

        private void LoadAttributeSet(NsDecl stylesheetNsList)
        {
            string str;
            string str2;
            AttributeSet set;
            XsltInput.ContextInfo info = this.input.GetAttributes(1, this.input.Atoms.Name, out str, this.input.Atoms.UseAttributeSets, out str2);
            info.nsList = MergeNamespaces(info.nsList, stylesheetNsList);
            QilName key = this.CreateXPathQName(str);
            if (!this.curStylesheet.AttributeSets.TryGetValue(key, out set))
            {
                this.curStylesheet.AttributeSets[key] = set = AstFactory.AttributeSet(key);
                if (!this.compiler.AttributeSets.ContainsKey(key))
                {
                    this.compiler.AllTemplates.Add(set);
                }
            }
            List<XslNode> content = this.ParseUseAttributeSets(str2, info.lineInfo);
            foreach (XslNode node in content)
            {
                set.UsedAttributeSets.Add(node.Name);
            }
            if (this.input.MoveToFirstChild())
            {
                do
                {
                    switch (this.input.NodeType)
                    {
                        case XPathNodeType.SignificantWhitespace:
                        case XPathNodeType.Whitespace:
                            break;

                        case XPathNodeType.Element:
                            if (this.input.IsXsltNamespace() && this.input.IsKeyword(this.input.Atoms.Attribute))
                            {
                                AddInstruction(content, this.XslAttribute());
                            }
                            else
                            {
                                this.ReportError("Xslt_UnexpectedElement", new string[] { this.input.QualifiedName, this.input.Atoms.AttributeSet });
                                this.input.SkipNode();
                            }
                            break;

                        default:
                            this.ReportError("Xslt_TextNodesNotAllowed", new string[] { this.input.Atoms.AttributeSet });
                            break;
                    }
                }
                while (this.input.MoveToNextSibling());
                this.input.MoveToParent();
            }
            set.AddContent(SetInfo(AstFactory.List(), this.LoadEndTag(content), info));
        }

        private void LoadDecimalFormat(NsDecl stylesheetNsList)
        {
            XmlQualifiedName name;
            string[] values = new string[11];
            string[] names = new string[] { this.input.Atoms.DecimalSeparator, this.input.Atoms.GroupingSeparator, this.input.Atoms.Percent, this.input.Atoms.PerMille, this.input.Atoms.ZeroDigit, this.input.Atoms.Digit, this.input.Atoms.PatternSeparator, this.input.Atoms.MinusSign, this.input.Atoms.Infinity, this.input.Atoms.NaN, this.input.Atoms.Name };
            XsltInput.ContextInfo info = this.input.GetAttributes(0, 11, names, values);
            info.nsList = MergeNamespaces(info.nsList, stylesheetNsList);
            char[] characters = DecimalFormatDecl.Default.Characters;
            char[] chArray2 = new char[8];
            int index = 0;
            while (index < 8)
            {
                chArray2[index] = this.ParseCharAttribute(values[index], characters[index], names[index]);
                index++;
            }
            string infinitySymbol = values[index++];
            string nanSymbol = values[index++];
            string qname = values[index++];
            if (infinitySymbol == null)
            {
                infinitySymbol = DecimalFormatDecl.Default.InfinitySymbol;
            }
            if (nanSymbol == null)
            {
                nanSymbol = DecimalFormatDecl.Default.NanSymbol;
            }
            for (int i = 0; i < 7; i++)
            {
                for (int j = i + 1; j < 7; j++)
                {
                    if (chArray2[i] == chArray2[j])
                    {
                        this.ReportError("Xslt_DecimalFormatSignsNotDistinct", new string[] { names[i], names[j] });
                        break;
                    }
                }
            }
            if (qname == null)
            {
                name = new XmlQualifiedName();
            }
            else
            {
                this.compiler.EnterForwardsCompatible();
                name = this.ResolveQName(true, qname);
                if (!this.compiler.ExitForwardsCompatible(this.input.ForwardCompatibility))
                {
                    name = new XmlQualifiedName();
                }
            }
            if (this.compiler.DecimalFormats.Contains(name))
            {
                DecimalFormatDecl decl = this.compiler.DecimalFormats[name];
                index = 0;
                while (index < 8)
                {
                    if (chArray2[index] != decl.Characters[index])
                    {
                        this.ReportError("Xslt_DecimalFormatRedefined", new string[] { names[index], char.ToString(chArray2[index]) });
                    }
                    index++;
                }
                if (infinitySymbol != decl.InfinitySymbol)
                {
                    this.ReportError("Xslt_DecimalFormatRedefined", new string[] { names[index], infinitySymbol });
                }
                index++;
                if (nanSymbol != decl.NanSymbol)
                {
                    this.ReportError("Xslt_DecimalFormatRedefined", new string[] { names[index], nanSymbol });
                }
                index++;
                index++;
            }
            else
            {
                DecimalFormatDecl item = new DecimalFormatDecl(name, infinitySymbol, nanSymbol, new string(chArray2));
                this.compiler.DecimalFormats.Add(item);
            }
            this.CheckNoContent();
        }

        private void LoadDocument()
        {
            if (!this.input.Start())
            {
                this.ReportError("Xslt_WrongStylesheetElement", new string[0]);
            }
            else
            {
                if (this.input.IsXsltNamespace())
                {
                    if (this.input.IsKeyword(this.input.Atoms.Stylesheet) || this.input.IsKeyword(this.input.Atoms.Transform))
                    {
                        this.LoadRealStylesheet();
                    }
                    else
                    {
                        this.ReportError("Xslt_WrongStylesheetElement", new string[0]);
                        this.input.SkipNode();
                    }
                }
                else
                {
                    this.LoadSimplifiedStylesheet();
                }
                this.input.Finish();
            }
        }

        private List<XslNode> LoadEndTag(List<XslNode> content)
        {
            if (this.compiler.IsDebug && !this.input.IsEmptyElement)
            {
                AddInstruction(content, SetLineInfo(AstFactory.Nop(), this.input.BuildLineInfo()));
            }
            return content;
        }

        private List<XslNode> LoadFallbacks(string instrName)
        {
            List<XslNode> list = new List<XslNode>();
            if (this.input.MoveToFirstChild())
            {
                do
                {
                    if (Ref.Equal(this.input.NamespaceUri, this.input.Atoms.UriXsl) && Ref.Equal(this.input.LocalName, this.input.Atoms.Fallback))
                    {
                        XsltInput.ContextInfo attributes = this.input.GetAttributes();
                        list.Add(SetInfo(AstFactory.List(), this.LoadInstructions(), attributes));
                    }
                    else
                    {
                        this.input.SkipNode();
                    }
                }
                while (this.input.MoveToNextSibling());
                this.input.MoveToParent();
            }
            if (list.Count == 0)
            {
                list.Add(AstFactory.Error(XslLoadException.CreateMessage(this.input.BuildLineInfo(), "Xslt_UnknownExtensionElement", new string[] { instrName })));
            }
            return list;
        }

        private void LoadGlobalVariableOrParameter(NsDecl stylesheetNsList, XslNodeType nodeType)
        {
            VarPar var = this.XslVarPar(nodeType);
            var.Namespaces = MergeNamespaces(var.Namespaces, stylesheetNsList);
            if (!this.curStylesheet.AddVarPar(var))
            {
                this.ReportError("Xslt_DupGlobalVariable", new string[] { var.Name.QualifiedName });
            }
        }

        private void LoadImport()
        {
            string str;
            this.input.GetAttributes(1, this.input.Atoms.Href, out str);
            this.CheckNoContent();
            if (str != null)
            {
                Uri item = this.ResolveUri(str, this.input.BaseUri);
                if (this.documentUriInUse.Contains(item.ToString()))
                {
                    this.ReportError("Xslt_CircularInclude", new string[] { str });
                }
                else
                {
                    this.curStylesheet.ImportHrefs.Add(item);
                }
            }
        }

        private void LoadInclude()
        {
            string str;
            this.input.GetAttributes(1, this.input.Atoms.Href, out str);
            this.CheckNoContent();
            if (str != null)
            {
                Uri uri = this.ResolveUri(str, this.input.BaseUri);
                if (this.documentUriInUse.Contains(uri.ToString()))
                {
                    this.ReportError("Xslt_CircularInclude", new string[] { str });
                }
                else
                {
                    this.LoadStylesheet(uri, true);
                }
            }
        }

        private List<XslNode> LoadInstructions() => 
            this.LoadInstructions(new List<XslNode>(), InstructionFlags.NoParamNoSort);

        private List<XslNode> LoadInstructions(List<XslNode> content) => 
            this.LoadInstructions(content, InstructionFlags.NoParamNoSort);

        private List<XslNode> LoadInstructions(InstructionFlags flags) => 
            this.LoadInstructions(new List<XslNode>(), flags);

        private List<XslNode> LoadInstructions(List<XslNode> content, InstructionFlags flags)
        {
            XslNode node;
            string localName;
            bool flag2;
            if ((++this.loadInstructionsDepth > 0x400) && XsltConfigSection.LimitXPathComplexity)
            {
                throw XsltException.Create("Xslt_CompileError2", new string[0]);
            }
            string qualifiedName = this.input.QualifiedName;
            if (!this.input.MoveToFirstChild())
            {
                goto Label_04FB;
            }
            bool flag = true;
        Label_0050:
            switch (this.input.NodeType)
            {
                case XPathNodeType.SignificantWhitespace:
                    goto Label_04B1;

                case XPathNodeType.Whitespace:
                    goto Label_04DF;

                case XPathNodeType.Element:
                {
                    string namespaceUri = this.input.NamespaceUri;
                    localName = this.input.LocalName;
                    if (namespaceUri != this.input.Atoms.UriXsl)
                    {
                        flag = false;
                        node = this.LoadLiteralResultElement(false);
                        goto Label_04D8;
                    }
                    flag2 = false;
                    if (!Ref.Equal(localName, this.input.Atoms.Param))
                    {
                        if (Ref.Equal(localName, this.input.Atoms.Sort))
                        {
                            if ((flags & InstructionFlags.AllowSort) == InstructionFlags.NoParamNoSort)
                            {
                                this.ReportError("Xslt_UnexpectedElementQ", new string[] { this.input.QualifiedName, qualifiedName });
                                flag2 = true;
                            }
                            else if (!flag)
                            {
                                this.ReportError("Xslt_NotAtTop", new string[] { this.input.QualifiedName, qualifiedName });
                                flag2 = true;
                            }
                        }
                        else
                        {
                            flag = false;
                        }
                        break;
                    }
                    if ((flags & InstructionFlags.AllowParam) == InstructionFlags.NoParamNoSort)
                    {
                        this.ReportError("Xslt_UnexpectedElementQ", new string[] { this.input.QualifiedName, qualifiedName });
                        flag2 = true;
                    }
                    else if (!flag)
                    {
                        this.ReportError("Xslt_NotAtTop", new string[] { this.input.QualifiedName, qualifiedName });
                        flag2 = true;
                    }
                    break;
                }
                default:
                    flag = false;
                    goto Label_04B1;
            }
            if (flag2)
            {
                flag = false;
                this.input.SkipNode();
                goto Label_04DF;
            }
            node = Ref.Equal(localName, this.input.Atoms.ApplyImports) ? this.XslApplyImports() : (Ref.Equal(localName, this.input.Atoms.ApplyTemplates) ? this.XslApplyTemplates() : (Ref.Equal(localName, this.input.Atoms.CallTemplate) ? this.XslCallTemplate() : (Ref.Equal(localName, this.input.Atoms.Copy) ? this.XslCopy() : (Ref.Equal(localName, this.input.Atoms.CopyOf) ? this.XslCopyOf() : (Ref.Equal(localName, this.input.Atoms.Fallback) ? this.XslFallback() : (Ref.Equal(localName, this.input.Atoms.If) ? this.XslIf() : (Ref.Equal(localName, this.input.Atoms.Choose) ? this.XslChoose() : (Ref.Equal(localName, this.input.Atoms.ForEach) ? this.XslForEach() : (Ref.Equal(localName, this.input.Atoms.Message) ? this.XslMessage() : (Ref.Equal(localName, this.input.Atoms.Number) ? this.XslNumber() : (Ref.Equal(localName, this.input.Atoms.ValueOf) ? this.XslValueOf() : (Ref.Equal(localName, this.input.Atoms.Comment) ? this.XslComment() : (Ref.Equal(localName, this.input.Atoms.ProcessingInstruction) ? this.XslProcessingInstruction() : (Ref.Equal(localName, this.input.Atoms.Text) ? this.XslText() : (Ref.Equal(localName, this.input.Atoms.Element) ? this.XslElement() : (Ref.Equal(localName, this.input.Atoms.Attribute) ? this.XslAttribute() : (Ref.Equal(localName, this.input.Atoms.Variable) ? this.XslVarPar(XslNodeType.Variable) : (Ref.Equal(localName, this.input.Atoms.Param) ? this.XslVarPar(XslNodeType.Param) : (Ref.Equal(localName, this.input.Atoms.Sort) ? this.XslSort() : this.LoadUnknownXsltInstruction(qualifiedName))))))))))))))))))));
            goto Label_04D8;
        Label_04B1:
            node = SetLineInfo(AstFactory.Text(this.input.Value), this.input.BuildLineInfo());
        Label_04D8:
            AddInstruction(content, node);
        Label_04DF:
            if (this.input.MoveToNextSibling())
            {
                goto Label_0050;
            }
            this.input.MoveToParent();
        Label_04FB:
            this.loadInstructionsDepth--;
            return content;
        }

        private void LoadKey(NsDecl stylesheetNsList)
        {
            string str;
            string str2;
            string str3;
            XsltInput.ContextInfo info = this.input.GetAttributes(3, this.input.Atoms.Name, out str, this.input.Atoms.Match, out str2, this.input.Atoms.Use, out str3);
            info.nsList = MergeNamespaces(info.nsList, stylesheetNsList);
            this.CheckNoContent();
            QilName name = this.CreateXPathQName(str);
            Key item = (Key) SetInfo(AstFactory.Key(name, str2, str3, this.input.XslVersion), null, info);
            if (this.compiler.Keys.Contains(name))
            {
                this.compiler.Keys[name].Add(item);
            }
            else
            {
                List<Key> list = new List<Key> {
                    item
                };
                this.compiler.Keys.Add(list);
            }
        }

        private XslNode LoadLiteralResultElement(bool asStylesheet)
        {
            XslNode node2;
            string prefix = this.input.Prefix;
            string localName = this.input.LocalName;
            string namespaceUri = this.input.NamespaceUri;
            string version = null;
            string str5 = null;
            string str6 = null;
            string useAttributeSets = null;
            string attName = null;
            List<XslNode> content = new List<XslNode>();
            XsltInput.ContextInfo info = new XsltInput.ContextInfo(this.input);
            while (this.input.MoveToNextAttOrNs())
            {
                if (this.input.NodeType == XPathNodeType.Namespace)
                {
                    info.AddNamespace(this.input);
                }
                else
                {
                    info.AddAttribute(this.input);
                    if (this.input.IsXsltNamespace())
                    {
                        if (this.input.LocalName == this.input.Atoms.Version)
                        {
                            version = this.input.Value;
                            attName = this.input.QualifiedName;
                        }
                        else if (this.input.LocalName == this.input.Atoms.ExtensionElementPrefixes)
                        {
                            str5 = this.input.Value;
                        }
                        else if (this.input.LocalName == this.input.Atoms.ExcludeResultPrefixes)
                        {
                            str6 = this.input.Value;
                        }
                        else if (this.input.LocalName == this.input.Atoms.UseAttributeSets)
                        {
                            useAttributeSets = this.input.Value;
                        }
                        continue;
                    }
                    XslNode node = AstFactory.LiteralAttribute(AstFactory.QName(this.input.LocalName, this.input.NamespaceUri, this.input.Prefix), this.input.Value, this.input.XslVersion);
                    AddInstruction(content, SetLineInfo(node, info.lineInfo));
                }
            }
            info.Finish(this.input);
            if (version != null)
            {
                this.input.SetVersion(version, attName);
            }
            else if (asStylesheet)
            {
                if (Ref.Equal(namespaceUri, this.input.Atoms.UriWdXsl) && Ref.Equal(localName, this.input.Atoms.Stylesheet))
                {
                    this.ReportError("Xslt_WdXslNamespace", new string[0]);
                }
                else
                {
                    this.ReportError("Xslt_WrongStylesheetElement", new string[0]);
                }
                this.input.SkipNode();
                return null;
            }
            this.InsertExNamespaces(str5, ref info.nsList, true);
            if (this.input.IsExtensionNamespace(namespaceUri))
            {
                content = this.LoadFallbacks(localName);
                node2 = AstFactory.List();
            }
            else
            {
                this.InsertExNamespaces(str6, ref info.nsList, false);
                content.InsertRange(0, this.ParseUseAttributeSets(useAttributeSets, info.lineInfo));
                content = this.LoadEndTag(this.LoadInstructions(content));
                node2 = AstFactory.LiteralElement(AstFactory.QName(localName, namespaceUri, prefix));
            }
            return SetInfo(node2, content, info);
        }

        private void LoadMsAssembly(ScriptClass scriptClass)
        {
            string str;
            string str2;
            this.input.GetAttributes(0, this.input.Atoms.Name, out str, this.input.Atoms.Href, out str2);
            string location = null;
            if (str != null)
            {
                if (str2 != null)
                {
                    this.ReportError("Xslt_AssemblyBothNameHrefPresent", new string[0]);
                    goto Label_00DB;
                }
                try
                {
                    location = Assembly.Load(str).Location;
                    goto Label_00DB;
                }
                catch
                {
                    AssemblyName name = new AssemblyName(str);
                    byte[] publicKeyToken = name.GetPublicKeyToken();
                    if (((publicKeyToken != null) && (publicKeyToken.Length != 0)) || (name.Version != null))
                    {
                        throw;
                    }
                    location = name.Name + ".dll";
                    goto Label_00DB;
                }
            }
            if (str2 != null)
            {
                location = Assembly.LoadFrom(this.ResolveUri(str2, this.input.BaseUri).ToString()).Location;
                scriptClass.refAssembliesByHref = true;
            }
            else
            {
                this.ReportError("Xslt_AssemblyBothNameHrefAbsent", new string[0]);
            }
        Label_00DB:
            if (location != null)
            {
                scriptClass.refAssemblies.Add(location);
            }
            this.CheckNoContent();
        }

        private void LoadMsUsing(ScriptClass scriptClass)
        {
            string str;
            this.input.GetAttributes(1, this.input.Atoms.Namespace, out str);
            if (str != null)
            {
                scriptClass.nsImports.Add(str);
            }
            this.CheckNoContent();
        }

        private void LoadNamespaceAlias(NsDecl stylesheetNsList)
        {
            string str;
            string str2;
            XsltInput.ContextInfo info = this.input.GetAttributes(2, this.input.Atoms.StylesheetPrefix, out str, this.input.Atoms.ResultPrefix, out str2);
            info.nsList = MergeNamespaces(info.nsList, stylesheetNsList);
            this.CheckNoContent();
            string ssheetNsUri = null;
            string resultNsUri = null;
            if (str != null)
            {
                if (str.Length == 0)
                {
                    this.ReportError("Xslt_EmptyNsAlias", new string[] { this.input.Atoms.StylesheetPrefix });
                }
                else
                {
                    if (str == "#default")
                    {
                        str = string.Empty;
                    }
                    ssheetNsUri = this.input.LookupXmlNamespace(str);
                }
            }
            if (str2 != null)
            {
                if (str2.Length == 0)
                {
                    this.ReportError("Xslt_EmptyNsAlias", new string[] { this.input.Atoms.ResultPrefix });
                }
                else
                {
                    if (str2 == "#default")
                    {
                        str2 = string.Empty;
                    }
                    resultNsUri = this.input.LookupXmlNamespace(str2);
                }
            }
            if (((ssheetNsUri != null) && (resultNsUri != null)) && this.compiler.SetNsAlias(ssheetNsUri, resultNsUri, str2, this.curStylesheet.ImportPrecedence))
            {
                this.ReportWarning("Xslt_DupNsAlias", new string[] { ssheetNsUri });
            }
        }

        private void LoadOutput()
        {
            string str;
            string str2;
            string str3;
            string str4;
            string str5;
            string str6;
            string str7;
            string str8;
            string str9;
            string str10;
            TriState state;
            this.input.GetAttributes(0, this.input.Atoms.Method, out str, this.input.Atoms.Version, out str2, this.input.Atoms.Encoding, out str3, this.input.Atoms.OmitXmlDeclaration, out str4, this.input.Atoms.Standalone, out str5, this.input.Atoms.DocTypePublic, out str6, this.input.Atoms.DocTypeSystem, out str7, this.input.Atoms.CDataSectionElements, out str8, this.input.Atoms.Indent, out str9, this.input.Atoms.MediaType, out str10);
            Output output = this.compiler.Output;
            XmlWriterSettings settings = output.Settings;
            int currentPrecedence = this.compiler.CurrentPrecedence;
            if ((str != null) && (currentPrecedence >= output.MethodPrec))
            {
                XmlOutputMethod method;
                this.compiler.EnterForwardsCompatible();
                XmlQualifiedName name = this.ParseOutputMethod(str, out method);
                if (this.compiler.ExitForwardsCompatible(this.input.ForwardCompatibility) && (name != null))
                {
                    if ((currentPrecedence == output.MethodPrec) && !output.Method.Equals(name))
                    {
                        this.ReportWarning("Xslt_AttributeRedefinition", new string[] { this.input.Atoms.Method });
                    }
                    settings.OutputMethod = method;
                    output.Method = name;
                    output.MethodPrec = currentPrecedence;
                }
            }
            if ((str2 != null) && (currentPrecedence >= output.VersionPrec))
            {
                if ((currentPrecedence == output.VersionPrec) && (output.Version != str2))
                {
                    this.ReportWarning("Xslt_AttributeRedefinition", new string[] { this.input.Atoms.Version });
                }
                output.Version = str2;
                output.VersionPrec = currentPrecedence;
            }
            if ((str3 != null) && (currentPrecedence >= output.EncodingPrec))
            {
                try
                {
                    Encoding encoding = Encoding.GetEncoding(str3);
                    if ((currentPrecedence == output.EncodingPrec) && (output.Encoding != str3))
                    {
                        this.ReportWarning("Xslt_AttributeRedefinition", new string[] { this.input.Atoms.Encoding });
                    }
                    settings.Encoding = encoding;
                    output.Encoding = str3;
                    output.EncodingPrec = currentPrecedence;
                }
                catch (ArgumentException)
                {
                    if (!this.input.ForwardCompatibility)
                    {
                        this.ReportWarning("Xslt_InvalidEncoding", new string[] { str3 });
                    }
                }
            }
            if ((str4 != null) && (currentPrecedence >= output.OmitXmlDeclarationPrec))
            {
                state = this.ParseYesNo(str4, this.input.Atoms.OmitXmlDeclaration);
                if (state != TriState.Unknown)
                {
                    bool flag = state == TriState.True;
                    if ((currentPrecedence == output.OmitXmlDeclarationPrec) && (settings.OmitXmlDeclaration != flag))
                    {
                        this.ReportWarning("Xslt_AttributeRedefinition", new string[] { this.input.Atoms.OmitXmlDeclaration });
                    }
                    settings.OmitXmlDeclaration = flag;
                    output.OmitXmlDeclarationPrec = currentPrecedence;
                }
            }
            if ((str5 != null) && (currentPrecedence >= output.StandalonePrec))
            {
                state = this.ParseYesNo(str5, this.input.Atoms.Standalone);
                if (state != TriState.Unknown)
                {
                    XmlStandalone standalone = (state == TriState.True) ? XmlStandalone.Yes : XmlStandalone.No;
                    if ((currentPrecedence == output.StandalonePrec) && (settings.Standalone != standalone))
                    {
                        this.ReportWarning("Xslt_AttributeRedefinition", new string[] { this.input.Atoms.Standalone });
                    }
                    settings.Standalone = standalone;
                    output.StandalonePrec = currentPrecedence;
                }
            }
            if ((str6 != null) && (currentPrecedence >= output.DocTypePublicPrec))
            {
                if ((currentPrecedence == output.DocTypePublicPrec) && (settings.DocTypePublic != str6))
                {
                    this.ReportWarning("Xslt_AttributeRedefinition", new string[] { this.input.Atoms.DocTypePublic });
                }
                settings.DocTypePublic = str6;
                output.DocTypePublicPrec = currentPrecedence;
            }
            if ((str7 != null) && (currentPrecedence >= output.DocTypeSystemPrec))
            {
                if ((currentPrecedence == output.DocTypeSystemPrec) && (settings.DocTypeSystem != str7))
                {
                    this.ReportWarning("Xslt_AttributeRedefinition", new string[] { this.input.Atoms.DocTypeSystem });
                }
                settings.DocTypeSystem = str7;
                output.DocTypeSystemPrec = currentPrecedence;
            }
            if ((str8 != null) && (str8.Length != 0))
            {
                this.compiler.EnterForwardsCompatible();
                string[] strArray = XmlConvert.SplitString(str8);
                List<XmlQualifiedName> collection = new List<XmlQualifiedName>();
                for (int i = 0; i < strArray.Length; i++)
                {
                    collection.Add(this.ResolveQName(false, strArray[i]));
                }
                if (this.compiler.ExitForwardsCompatible(this.input.ForwardCompatibility))
                {
                    settings.CDataSectionElements.AddRange(collection);
                }
            }
            if ((str9 != null) && (currentPrecedence >= output.IndentPrec))
            {
                state = this.ParseYesNo(str9, this.input.Atoms.Indent);
                if (state != TriState.Unknown)
                {
                    bool flag2 = state == TriState.True;
                    if ((currentPrecedence == output.IndentPrec) && (settings.Indent != flag2))
                    {
                        this.ReportWarning("Xslt_AttributeRedefinition", new string[] { this.input.Atoms.Indent });
                    }
                    settings.Indent = flag2;
                    output.IndentPrec = currentPrecedence;
                }
            }
            if ((str10 != null) && (currentPrecedence >= output.MediaTypePrec))
            {
                if ((currentPrecedence == output.MediaTypePrec) && (settings.MediaType != str10))
                {
                    this.ReportWarning("Xslt_AttributeRedefinition", new string[] { this.input.Atoms.MediaType });
                }
                settings.MediaType = str10;
                output.MediaTypePrec = currentPrecedence;
            }
            this.CheckNoContent();
        }

        private void LoadPreserveSpace(NsDecl stylesheetNsList)
        {
            string str;
            XsltInput.ContextInfo info = this.input.GetAttributes(1, this.input.Atoms.Elements, out str);
            info.nsList = MergeNamespaces(info.nsList, stylesheetNsList);
            this.ParseWhitespaceRules(str, true);
            this.CheckNoContent();
        }

        private void LoadRealStylesheet()
        {
            string str;
            string str2;
            string str3;
            string str4;
            bool flag2;
            XsltInput.ContextInfo info = this.input.GetAttributes(1, this.input.Atoms.Version, out str, this.input.Atoms.ExtensionElementPrefixes, out str2, this.input.Atoms.ExcludeResultPrefixes, out str3, this.input.Atoms.Id, out str4);
            if (str == null)
            {
                this.input.SetVersion("1.0", this.input.Atoms.Version);
            }
            this.InsertExNamespaces(str2, ref info.nsList, true);
            this.InsertExNamespaces(str3, ref info.nsList, false);
            string qualifiedName = this.input.QualifiedName;
            if (!this.input.MoveToFirstChild())
            {
                return;
            }
            bool flag = true;
        Label_00B9:
            flag2 = false;
            switch (this.input.NodeType)
            {
                case XPathNodeType.SignificantWhitespace:
                case XPathNodeType.Whitespace:
                    goto Label_0460;

                case XPathNodeType.Element:
                    if (!this.input.IsXsltNamespace())
                    {
                        if (this.input.IsNs(this.input.Atoms.UrnMsxsl) && this.input.IsKeyword(this.input.Atoms.Script))
                        {
                            this.LoadScript(info.nsList);
                        }
                        else if (this.input.IsNullNamespace())
                        {
                            this.ReportError("Xslt_NullNsAtTopLevel", new string[] { this.input.LocalName });
                            this.input.SkipNode();
                        }
                        else
                        {
                            this.input.SkipNode();
                        }
                        break;
                    }
                    if (!this.input.IsKeyword(this.input.Atoms.Import))
                    {
                        if (this.input.IsKeyword(this.input.Atoms.Include))
                        {
                            this.LoadInclude();
                        }
                        else if (this.input.IsKeyword(this.input.Atoms.StripSpace))
                        {
                            this.LoadStripSpace(info.nsList);
                        }
                        else if (this.input.IsKeyword(this.input.Atoms.PreserveSpace))
                        {
                            this.LoadPreserveSpace(info.nsList);
                        }
                        else if (this.input.IsKeyword(this.input.Atoms.Output))
                        {
                            this.LoadOutput();
                        }
                        else if (this.input.IsKeyword(this.input.Atoms.Key))
                        {
                            this.LoadKey(info.nsList);
                        }
                        else if (this.input.IsKeyword(this.input.Atoms.DecimalFormat))
                        {
                            this.LoadDecimalFormat(info.nsList);
                        }
                        else if (this.input.IsKeyword(this.input.Atoms.NamespaceAlias))
                        {
                            this.LoadNamespaceAlias(info.nsList);
                        }
                        else if (this.input.IsKeyword(this.input.Atoms.AttributeSet))
                        {
                            this.LoadAttributeSet(info.nsList);
                        }
                        else if (this.input.IsKeyword(this.input.Atoms.Variable))
                        {
                            this.LoadGlobalVariableOrParameter(info.nsList, XslNodeType.Variable);
                        }
                        else if (this.input.IsKeyword(this.input.Atoms.Param))
                        {
                            this.LoadGlobalVariableOrParameter(info.nsList, XslNodeType.Param);
                        }
                        else if (this.input.IsKeyword(this.input.Atoms.Template))
                        {
                            this.LoadTemplate(info.nsList);
                        }
                        else
                        {
                            if (!this.input.ForwardCompatibility)
                            {
                                this.ReportError("Xslt_UnexpectedElementQ", new string[] { this.input.QualifiedName, qualifiedName });
                            }
                            this.input.SkipNode();
                        }
                        break;
                    }
                    if (!flag)
                    {
                        this.ReportError("Xslt_NotAtTop", new string[] { this.input.QualifiedName, qualifiedName });
                        this.input.SkipNode();
                    }
                    else
                    {
                        flag2 = true;
                        this.LoadImport();
                    }
                    break;

                default:
                    this.ReportError("Xslt_TextNodesNotAllowed", new string[] { this.input.Atoms.Stylesheet });
                    goto Label_0460;
            }
            flag = flag2;
        Label_0460:
            if (this.input.MoveToNextSibling())
            {
                goto Label_00B9;
            }
            this.input.MoveToParent();
        }

        private void LoadScript(NsDecl stylesheetNsList)
        {
            string str;
            string str2;
            XsltInput.ContextInfo info = this.input.GetAttributes(1, this.input.Atoms.ImplementsPrefix, out str2, this.input.Atoms.Language, out str);
            info.nsList = MergeNamespaces(info.nsList, stylesheetNsList);
            string ns = null;
            if (str2 != null)
            {
                if (str2.Length == 0)
                {
                    this.ReportError("Xslt_EmptyAttrValue", new string[] { this.input.Atoms.ImplementsPrefix, str2 });
                }
                else
                {
                    ns = this.input.LookupXmlNamespace(str2);
                    if (ns == "http://www.w3.org/1999/XSL/Transform")
                    {
                        this.ReportError("Xslt_ScriptXsltNamespace", new string[0]);
                        ns = null;
                    }
                }
            }
            if (ns == null)
            {
                ns = this.compiler.CreatePhantomNamespace();
            }
            if (str == null)
            {
                str = "jscript";
            }
            if (!this.compiler.Settings.EnableScript)
            {
                this.compiler.Scripts.ScriptClasses[ns] = null;
                this.input.SkipNode();
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                string uri = this.input.Uri;
                int lineNumber = 0;
                int num2 = 0;
                ScriptClass scriptClass = this.compiler.Scripts.GetScriptClass(ns, str, this);
                if (scriptClass == null)
                {
                    this.input.SkipNode();
                }
                else
                {
                    if (this.input.MoveToFirstChild())
                    {
                        do
                        {
                            XPathNodeType nodeType = this.input.NodeType;
                            if (nodeType == XPathNodeType.Element)
                            {
                                if (this.input.IsNs(this.input.Atoms.UrnMsxsl) && (this.input.IsKeyword(this.input.Atoms.Assembly) || this.input.IsKeyword(this.input.Atoms.Using)))
                                {
                                    if (builder.Length != 0)
                                    {
                                        this.ReportError("Xslt_ScriptNotAtTop", new string[] { this.input.QualifiedName });
                                        this.input.SkipNode();
                                    }
                                    if (this.input.IsKeyword(this.input.Atoms.Assembly))
                                    {
                                        this.LoadMsAssembly(scriptClass);
                                    }
                                    else if (this.input.IsKeyword(this.input.Atoms.Using))
                                    {
                                        this.LoadMsUsing(scriptClass);
                                    }
                                }
                                else
                                {
                                    this.ReportError("Xslt_UnexpectedElementQ", new string[] { this.input.QualifiedName, "msxsl:script" });
                                    this.input.SkipNode();
                                }
                            }
                            else if ((nodeType == XPathNodeType.Text) || (builder.Length != 0))
                            {
                                int startLine = this.input.StartLine;
                                int endLine = this.input.EndLine;
                                if (builder.Length == 0)
                                {
                                    lineNumber = startLine;
                                }
                                else if (num2 < startLine)
                                {
                                    builder.Append('\n', startLine - num2);
                                }
                                builder.Append(this.input.Value);
                                num2 = endLine;
                            }
                        }
                        while (this.input.MoveToNextSibling());
                        this.input.MoveToParent();
                    }
                    if (builder.Length == 0)
                    {
                        lineNumber = this.input.StartLine;
                    }
                    scriptClass.AddScriptBlock(builder.ToString(), uri, lineNumber, this.input.StartLine, this.input.StartPos);
                }
            }
        }

        private void LoadSimplifiedStylesheet()
        {
            this.curTemplate = AstFactory.Template(null, "/", nullMode, double.NaN, this.input.XslVersion);
            this.input.CanHaveApplyImports = true;
            XslNode node = this.LoadLiteralResultElement(true);
            if (node != null)
            {
                SetLineInfo(this.curTemplate, node.SourceLine);
                List<XslNode> content = new List<XslNode> {
                    node
                };
                SetContent(this.curTemplate, content);
                this.curStylesheet.AddTemplate(this.curTemplate);
            }
            this.curTemplate = null;
        }

        private void LoadStripSpace(NsDecl stylesheetNsList)
        {
            string str;
            XsltInput.ContextInfo info = this.input.GetAttributes(1, this.input.Atoms.Elements, out str);
            info.nsList = MergeNamespaces(info.nsList, stylesheetNsList);
            this.ParseWhitespaceRules(str, false);
            this.CheckNoContent();
        }

        private Stylesheet LoadStylesheet(Uri uri, bool include)
        {
            using (XmlReader reader = this.CreateReader(uri, this.xmlResolver))
            {
                return this.LoadStylesheet(reader, include);
            }
        }

        private Stylesheet LoadStylesheet(XmlReader reader, bool include)
        {
            string baseURI = reader.BaseURI;
            this.documentUriInUse.Add(baseURI, null);
            Stylesheet curStylesheet = this.curStylesheet;
            XsltInput input = this.input;
            Stylesheet stylesheet2 = include ? this.curStylesheet : this.compiler.CreateStylesheet();
            this.input = new XsltInput(reader, this.compiler);
            this.curStylesheet = stylesheet2;
            try
            {
                this.LoadDocument();
                if (!include)
                {
                    this.compiler.MergeWithStylesheet(this.curStylesheet);
                    List<Uri> importHrefs = this.curStylesheet.ImportHrefs;
                    this.curStylesheet.Imports = new Stylesheet[importHrefs.Count];
                    for (int i = importHrefs.Count - 1; 0 <= i; i--)
                    {
                        this.curStylesheet.Imports[i] = this.LoadStylesheet(importHrefs[i], false);
                    }
                }
                return stylesheet2;
            }
            catch (XslLoadException)
            {
                throw;
            }
            catch (Exception exception)
            {
                if (!XmlException.IsCatchableException(exception))
                {
                    throw;
                }
                XmlException inner = exception as XmlException;
                if (inner != null)
                {
                    SourceLineInfo lineInfo = new SourceLineInfo(this.input.Uri, inner.LineNumber, inner.LinePosition, inner.LineNumber, inner.LinePosition);
                    throw new XslLoadException(inner, lineInfo);
                }
                this.input.FixLastLineInfo();
                throw new XslLoadException(exception, this.input.BuildLineInfo());
            }
            finally
            {
                this.documentUriInUse.Remove(baseURI);
                this.input = input;
                this.curStylesheet = curStylesheet;
            }
            return stylesheet2;
        }

        private void LoadTemplate(NsDecl stylesheetNsList)
        {
            string str;
            string str2;
            string str3;
            string str4;
            XsltInput.ContextInfo info = this.input.GetAttributes(0, this.input.Atoms.Match, out str, this.input.Atoms.Name, out str2, this.input.Atoms.Priority, out str3, this.input.Atoms.Mode, out str4);
            info.nsList = MergeNamespaces(info.nsList, stylesheetNsList);
            if (str == null)
            {
                if (str2 == null)
                {
                    this.ReportError("Xslt_BothMatchNameAbsent", new string[0]);
                }
                if (str4 != null)
                {
                    this.ReportError("Xslt_ModeWithoutMatch", new string[0]);
                    str4 = null;
                }
                if (str3 != null)
                {
                    this.ReportWarning("Xslt_PriorityWithoutMatch", new string[0]);
                }
            }
            QilName name = null;
            if (str2 != null)
            {
                this.compiler.EnterForwardsCompatible();
                name = this.CreateXPathQName(str2);
                if (!this.compiler.ExitForwardsCompatible(this.input.ForwardCompatibility))
                {
                    name = null;
                }
            }
            double naN = double.NaN;
            if (str3 != null)
            {
                naN = XPathConvert.StringToDouble(str3);
                if (double.IsNaN(naN) && !this.input.ForwardCompatibility)
                {
                    this.ReportError("Xslt_InvalidAttrValue", new string[] { this.input.Atoms.Priority, str3 });
                }
            }
            this.curTemplate = AstFactory.Template(name, str, this.ParseMode(str4), naN, this.input.XslVersion);
            this.input.CanHaveApplyImports = str != null;
            SetInfo(this.curTemplate, this.LoadEndTag(this.LoadInstructions(InstructionFlags.AllowParam)), info);
            if (!this.curStylesheet.AddTemplate(this.curTemplate))
            {
                this.ReportError("Xslt_DupTemplateName", new string[] { this.curTemplate.Name.QualifiedName });
            }
            this.curTemplate = null;
        }

        private XslNode LoadUnknownXsltInstruction(string parentName)
        {
            if (!this.input.ForwardCompatibility)
            {
                this.ReportError("Xslt_UnexpectedElementQ", new string[] { this.input.QualifiedName, parentName });
                this.input.SkipNode();
                return null;
            }
            XsltInput.ContextInfo attributes = this.input.GetAttributes();
            List<XslNode> content = this.LoadFallbacks(this.input.LocalName);
            return SetInfo(AstFactory.List(), content, attributes);
        }

        private static NsDecl MergeNamespaces(NsDecl thisList, NsDecl parentList)
        {
            if (parentList == null)
            {
                return thisList;
            }
            if (thisList != null)
            {
                while (parentList != null)
                {
                    bool flag = false;
                    for (NsDecl decl = thisList; decl != null; decl = decl.Prev)
                    {
                        if (Ref.Equal(decl.Prefix, parentList.Prefix) && ((decl.Prefix != null) || (decl.NsUri == parentList.NsUri)))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        thisList = new NsDecl(thisList, parentList.Prefix, parentList.NsUri);
                    }
                    parentList = parentList.Prev;
                }
                return thisList;
            }
            return parentList;
        }

        private char ParseCharAttribute(string attValue, char defaultValue, string attName)
        {
            if (attValue != null)
            {
                if (attValue.Length == 1)
                {
                    return attValue[0];
                }
                if (!this.input.ForwardCompatibility)
                {
                    this.ReportError("Xslt_CharAttribute", new string[] { attName });
                }
            }
            return defaultValue;
        }

        private QilName ParseMode(string qname)
        {
            if (qname == null)
            {
                return XsltLoader.nullMode;
            }
            this.compiler.EnterForwardsCompatible();
            QilName nullMode = this.CreateXPathQName(qname);
            if (!this.compiler.ExitForwardsCompatible(this.input.ForwardCompatibility))
            {
                nullMode = XsltLoader.nullMode;
            }
            return nullMode;
        }

        private XmlQualifiedName ParseOutputMethod(string attValue, out XmlOutputMethod method)
        {
            string str;
            string str2;
            string str3;
            this.ResolveQName(true, attValue, out str2, out str3, out str);
            method = XmlOutputMethod.AutoDetect;
            if (this.compiler.IsPhantomNamespace(str3))
            {
                return null;
            }
            if (str.Length == 0)
            {
                switch (str2)
                {
                    case "xml":
                        method = XmlOutputMethod.Xml;
                        goto Label_00BD;

                    case "html":
                        method = XmlOutputMethod.Html;
                        goto Label_00BD;

                    case "text":
                        method = XmlOutputMethod.Text;
                        goto Label_00BD;
                }
                this.ReportError("Xslt_InvalidAttrValue", new string[] { this.input.Atoms.Method, attValue });
                return null;
            }
            if (!this.input.ForwardCompatibility)
            {
                this.ReportWarning("Xslt_InvalidMethod", new string[] { attValue });
            }
        Label_00BD:
            return new XmlQualifiedName(str2, str3);
        }

        private List<XslNode> ParseUseAttributeSets(string useAttributeSets, ISourceLineInfo lineInfo)
        {
            List<XslNode> content = new List<XslNode>();
            if ((useAttributeSets != null) && (useAttributeSets.Length != 0))
            {
                this.compiler.EnterForwardsCompatible();
                string[] strArray = XmlConvert.SplitString(useAttributeSets);
                for (int i = 0; i < strArray.Length; i++)
                {
                    AddInstruction(content, SetLineInfo(AstFactory.UseAttributeSet(this.CreateXPathQName(strArray[i])), lineInfo));
                }
                if (!this.compiler.ExitForwardsCompatible(this.input.ForwardCompatibility))
                {
                    content = new List<XslNode>();
                }
            }
            return content;
        }

        private void ParseWhitespaceRules(string elements, bool preserveSpace)
        {
            if ((elements != null) && (elements.Length != 0))
            {
                string[] strArray = XmlConvert.SplitString(elements);
                for (int i = 0; i < strArray.Length; i++)
                {
                    string str;
                    string str2;
                    string xmlNamespace;
                    if (!this.compiler.ParseNameTest(strArray[i], out str, out str2, this))
                    {
                        xmlNamespace = this.compiler.CreatePhantomNamespace();
                    }
                    else if ((str == null) || (str.Length == 0))
                    {
                        xmlNamespace = str;
                    }
                    else
                    {
                        xmlNamespace = this.input.LookupXmlNamespace(str);
                        if (xmlNamespace == null)
                        {
                            xmlNamespace = this.compiler.CreatePhantomNamespace();
                        }
                    }
                    int index = ((str2 == null) ? 1 : 0) + ((xmlNamespace == null) ? 1 : 0);
                    this.curStylesheet.AddWhitespaceRule(index, new WhitespaceRule(str2, xmlNamespace, preserveSpace));
                }
            }
        }

        private TriState ParseYesNo(string val, string attName)
        {
            string str = val;
            if (str != null)
            {
                if (str == "yes")
                {
                    return TriState.True;
                }
                if (str == "no")
                {
                    return TriState.False;
                }
            }
            else
            {
                return TriState.Unknown;
            }
            if (!this.input.ForwardCompatibility)
            {
                this.ReportError("Xslt_BistateAttribute", new string[] { attName, "yes", "no" });
            }
            return TriState.Unknown;
        }

        private void Process()
        {
            this.compiler.StartApplyTemplates = AstFactory.ApplyTemplates(nullMode);
            this.ProcessOutputSettings();
            this.ProcessAttributeSets();
        }

        private void ProcessAttributeSets()
        {
            foreach (AttributeSet set in this.compiler.AttributeSets.Values)
            {
                this.AttributeSetsDfs(set);
            }
        }

        private void ProcessOutputSettings()
        {
            Output output = this.compiler.Output;
            XmlWriterSettings settings = output.Settings;
            if ((settings.OutputMethod == XmlOutputMethod.Html) && (output.IndentPrec == -2147483648))
            {
                settings.Indent = true;
            }
            if (output.MediaTypePrec == -2147483648)
            {
                settings.MediaType = (settings.OutputMethod == XmlOutputMethod.Xml) ? "text/xml" : ((settings.OutputMethod == XmlOutputMethod.Html) ? "text/html" : ((settings.OutputMethod == XmlOutputMethod.Text) ? "text/plain" : null));
            }
        }

        public void ReportError(string res, params string[] args)
        {
            this.compiler.ReportError(this.input.BuildLineInfo(), res, args);
        }

        public void ReportWarning(string res, params string[] args)
        {
            this.compiler.ReportWarning(this.input.BuildLineInfo(), res, args);
        }

        private XmlQualifiedName ResolveQName(bool ignoreDefaultNs, string qname)
        {
            string str;
            string str2;
            string str3;
            this.ResolveQName(ignoreDefaultNs, qname, out str2, out str3, out str);
            return new XmlQualifiedName(str2, str3);
        }

        private void ResolveQName(bool ignoreDefaultNs, string qname, out string localName, out string namespaceName, out string prefix)
        {
            if (qname == null)
            {
                prefix = this.compiler.PhantomNCName;
                localName = this.compiler.PhantomNCName;
                namespaceName = this.compiler.CreatePhantomNamespace();
            }
            else if (!this.compiler.ParseQName(qname, out prefix, out localName, this))
            {
                namespaceName = this.compiler.CreatePhantomNamespace();
            }
            else if (ignoreDefaultNs && (prefix.Length == 0))
            {
                namespaceName = string.Empty;
            }
            else
            {
                namespaceName = this.input.LookupXmlNamespace(prefix);
                if (namespaceName == null)
                {
                    namespaceName = this.compiler.CreatePhantomNamespace();
                }
            }
        }

        private Uri ResolveUri(string relativeUri, string baseUri)
        {
            Uri uri = (baseUri.Length != 0) ? this.xmlResolver.ResolveUri(null, baseUri) : null;
            Uri uri2 = this.xmlResolver.ResolveUri(uri, relativeUri);
            if (uri2 == null)
            {
                throw new XslLoadException("Xslt_CantResolve", new string[] { relativeUri });
            }
            return uri2;
        }

        private static void SetContent(XslNode node, List<XslNode> content)
        {
            if ((content != null) && (content.Count == 0))
            {
                content = null;
            }
            node.SetContent(content);
        }

        private static XslNode SetInfo(XslNode to, List<XslNode> content, XsltInput.ContextInfo info)
        {
            to.Namespaces = info.nsList;
            SetContent(to, content);
            SetLineInfo(to, info.lineInfo);
            return to;
        }

        private static XslNode SetLineInfo(XslNode node, ISourceLineInfo lineInfo)
        {
            node.SourceLine = lineInfo;
            return node;
        }

        private XslNode XslApplyImports()
        {
            XsltInput.ContextInfo attributes = this.input.GetAttributes();
            if (!this.input.CanHaveApplyImports)
            {
                this.ReportError("Xslt_InvalidApplyImports", new string[0]);
                this.input.SkipNode();
                return null;
            }
            this.CheckNoContent();
            return SetInfo(AstFactory.ApplyImports(this.curTemplate.Mode, this.curStylesheet, this.input.XslVersion), null, attributes);
        }

        private XslNode XslApplyTemplates()
        {
            string str;
            string str2;
            XsltInput.ContextInfo info = this.input.GetAttributes(0, this.input.Atoms.Select, out str, this.input.Atoms.Mode, out str2);
            if (str == null)
            {
                str = "node()";
            }
            QilName mode = this.ParseMode(str2);
            List<XslNode> content = new List<XslNode>();
            if (this.input.MoveToFirstChild())
            {
                do
                {
                    switch (this.input.NodeType)
                    {
                        case XPathNodeType.SignificantWhitespace:
                        case XPathNodeType.Whitespace:
                            break;

                        case XPathNodeType.Element:
                            if (this.input.IsXsltNamespace())
                            {
                                if (this.input.IsKeyword(this.input.Atoms.WithParam))
                                {
                                    XslNode withParam = this.XslVarPar(XslNodeType.WithParam);
                                    this.CheckWithParam(content, withParam);
                                    AddInstruction(content, withParam);
                                    break;
                                }
                                if (this.input.IsKeyword(this.input.Atoms.Sort))
                                {
                                    AddInstruction(content, this.XslSort());
                                    break;
                                }
                            }
                            this.ReportError("Xslt_UnexpectedElement", new string[] { this.input.QualifiedName, this.input.Atoms.ApplyTemplates });
                            this.input.SkipNode();
                            break;

                        default:
                            this.ReportError("Xslt_TextNodesNotAllowed", new string[] { this.input.Atoms.ApplyTemplates });
                            break;
                    }
                }
                while (this.input.MoveToNextSibling());
                this.input.MoveToParent();
            }
            info.SaveExtendedLineInfo(this.input);
            return SetInfo(AstFactory.ApplyTemplates(mode, str, info, this.input.XslVersion), content, info);
        }

        private XslNode XslAttribute()
        {
            string phantomNCName;
            string str2;
            XsltInput.ContextInfo info = this.input.GetAttributes(1, this.input.Atoms.Name, out phantomNCName, this.input.Atoms.Namespace, out str2);
            if (phantomNCName == null)
            {
                phantomNCName = this.compiler.PhantomNCName;
            }
            if (str2 == "http://www.w3.org/2000/xmlns/")
            {
                this.ReportError("Xslt_ReservedNS", new string[] { str2 });
            }
            return SetInfo(AstFactory.Attribute(phantomNCName, str2, this.input.XslVersion), this.LoadEndTag(this.LoadInstructions()), info);
        }

        private XslNode XslCallTemplate()
        {
            string str;
            XsltInput.ContextInfo info = this.input.GetAttributes(1, this.input.Atoms.Name, out str);
            List<XslNode> content = new List<XslNode>();
            if (this.input.MoveToFirstChild())
            {
                do
                {
                    switch (this.input.NodeType)
                    {
                        case XPathNodeType.SignificantWhitespace:
                        case XPathNodeType.Whitespace:
                            break;

                        case XPathNodeType.Element:
                            if (this.input.IsXsltNamespace() && this.input.IsKeyword(this.input.Atoms.WithParam))
                            {
                                XslNode withParam = this.XslVarPar(XslNodeType.WithParam);
                                this.CheckWithParam(content, withParam);
                                AddInstruction(content, withParam);
                            }
                            else
                            {
                                this.ReportError("Xslt_UnexpectedElement", new string[] { this.input.QualifiedName, this.input.Atoms.CallTemplate });
                                this.input.SkipNode();
                            }
                            break;

                        default:
                            this.ReportError("Xslt_TextNodesNotAllowed", new string[] { this.input.Atoms.CallTemplate });
                            break;
                    }
                }
                while (this.input.MoveToNextSibling());
                this.input.MoveToParent();
            }
            info.SaveExtendedLineInfo(this.input);
            return SetInfo(AstFactory.CallTemplate(this.CreateXPathQName(str), info), content, info);
        }

        private XslNode XslChoose()
        {
            XslNode node;
            XsltInput.ContextInfo attributes = this.input.GetAttributes();
            List<XslNode> content = new List<XslNode>();
            bool flag = false;
            bool flag2 = false;
            if (!this.input.MoveToFirstChild())
            {
                goto Label_01B0;
            }
        Label_0026:
            switch (this.input.NodeType)
            {
                case XPathNodeType.SignificantWhitespace:
                case XPathNodeType.Whitespace:
                    goto Label_0194;

                case XPathNodeType.Element:
                    node = null;
                    if (Ref.Equal(this.input.NamespaceUri, this.input.Atoms.UriXsl))
                    {
                        if (!Ref.Equal(this.input.LocalName, this.input.Atoms.When))
                        {
                            if (Ref.Equal(this.input.LocalName, this.input.Atoms.Otherwise))
                            {
                                if (flag)
                                {
                                    this.ReportError("Xslt_DupOtherwise", new string[0]);
                                    this.input.SkipNode();
                                    goto Label_0194;
                                }
                                flag = true;
                                node = this.XslOtherwise();
                            }
                            break;
                        }
                        if (flag)
                        {
                            this.ReportError("Xslt_WhenAfterOtherwise", new string[0]);
                            this.input.SkipNode();
                            goto Label_0194;
                        }
                        flag2 = true;
                        node = this.XslIf();
                    }
                    break;

                default:
                    this.ReportError("Xslt_TextNodesNotAllowed", new string[] { this.input.Atoms.Choose });
                    goto Label_0194;
            }
            if (node == null)
            {
                this.ReportError("Xslt_UnexpectedElement", new string[] { this.input.QualifiedName, this.input.Atoms.Choose });
                this.input.SkipNode();
            }
            else
            {
                AddInstruction(content, node);
            }
        Label_0194:
            if (this.input.MoveToNextSibling())
            {
                goto Label_0026;
            }
            this.input.MoveToParent();
        Label_01B0:
            if (!flag2)
            {
                this.ReportError("Xslt_NoWhen", new string[0]);
            }
            return SetInfo(AstFactory.Choose(), content, attributes);
        }

        private XslNode XslComment()
        {
            XsltInput.ContextInfo attributes = this.input.GetAttributes();
            return SetInfo(AstFactory.Comment(), this.LoadEndTag(this.LoadInstructions()), attributes);
        }

        private XslNode XslCopy()
        {
            string str;
            XsltInput.ContextInfo info = this.input.GetAttributes(0, this.input.Atoms.UseAttributeSets, out str);
            List<XslNode> content = this.ParseUseAttributeSets(str, info.lineInfo);
            return SetInfo(AstFactory.Copy(), this.LoadEndTag(this.LoadInstructions(content)), info);
        }

        private XslNode XslCopyOf()
        {
            string str;
            XsltInput.ContextInfo info = this.input.GetAttributes(1, this.input.Atoms.Select, out str);
            this.CheckNoContent();
            return SetInfo(AstFactory.CopyOf(str, this.input.XslVersion), null, info);
        }

        private XslNode XslElement()
        {
            string phantomNCName;
            string str2;
            string str3;
            XsltInput.ContextInfo info = this.input.GetAttributes(1, this.input.Atoms.Name, out phantomNCName, this.input.Atoms.Namespace, out str2, this.input.Atoms.UseAttributeSets, out str3);
            if (phantomNCName == null)
            {
                phantomNCName = this.compiler.PhantomNCName;
            }
            if (str2 == "http://www.w3.org/2000/xmlns/")
            {
                this.ReportError("Xslt_ReservedNS", new string[] { str2 });
            }
            List<XslNode> content = this.ParseUseAttributeSets(str3, info.lineInfo);
            return SetInfo(AstFactory.Element(phantomNCName, str2, this.input.XslVersion), this.LoadEndTag(this.LoadInstructions(content)), info);
        }

        private XslNode XslFallback()
        {
            this.input.GetAttributes();
            this.input.SkipNode();
            return null;
        }

        private XslNode XslForEach()
        {
            string str;
            XsltInput.ContextInfo info = this.input.GetAttributes(1, this.input.Atoms.Select, out str);
            this.input.CanHaveApplyImports = false;
            List<XslNode> content = this.LoadInstructions(InstructionFlags.AllowSort);
            info.SaveExtendedLineInfo(this.input);
            return SetInfo(AstFactory.ForEach(str, info, this.input.XslVersion), content, info);
        }

        private XslNode XslIf()
        {
            string str;
            XsltInput.ContextInfo info = this.input.GetAttributes(1, this.input.Atoms.Test, out str);
            return SetInfo(AstFactory.If(str, this.input.XslVersion), this.LoadInstructions(), info);
        }

        private XslNode XslMessage()
        {
            string str;
            XsltInput.ContextInfo info = this.input.GetAttributes(0, this.input.Atoms.Terminate, out str);
            bool term = this.ParseYesNo(str, this.input.Atoms.Terminate) == TriState.True;
            return SetInfo(AstFactory.Message(term), this.LoadEndTag(this.LoadInstructions()), info);
        }

        private XslNode XslNumber()
        {
            string str;
            string str2;
            string str3;
            string str4;
            string str5;
            string str6;
            string str7;
            string str8;
            string str9;
            NumberLevel multiple;
            XsltInput.ContextInfo info = this.input.GetAttributes(0, this.input.Atoms.Level, out str, this.input.Atoms.Count, out str2, this.input.Atoms.From, out str3, this.input.Atoms.Value, out str4, this.input.Atoms.Format, out str5, this.input.Atoms.Lang, out str6, this.input.Atoms.LetterValue, out str7, this.input.Atoms.GroupingSeparator, out str8, this.input.Atoms.GroupingSize, out str9);
            string str10 = str;
            if (str10 == null)
            {
                goto Label_00F1;
            }
            if (str10 == "single")
            {
                goto Label_00E2;
            }
            if (str10 == "multiple")
            {
                multiple = NumberLevel.Multiple;
                goto Label_0131;
            }
            if (str10 == "any")
            {
                multiple = NumberLevel.Any;
                goto Label_0131;
            }
            goto Label_00F1;
        Label_00E2:
            multiple = NumberLevel.Single;
            goto Label_0131;
        Label_00F1:
            if ((str != null) && !this.input.ForwardCompatibility)
            {
                this.ReportError("Xslt_InvalidAttrValue", new string[] { this.input.Atoms.Level, str });
            }
            goto Label_00E2;
        Label_0131:
            if (str5 == null)
            {
                str5 = "1";
            }
            this.CheckNoContent();
            return SetInfo(AstFactory.Number(multiple, str2, str3, str4, str5, str6, str7, str8, str9, this.input.XslVersion), null, info);
        }

        private XslNode XslOtherwise()
        {
            XsltInput.ContextInfo attributes = this.input.GetAttributes();
            return SetInfo(AstFactory.Otherwise(), this.LoadInstructions(), attributes);
        }

        private XslNode XslProcessingInstruction()
        {
            string phantomNCName;
            XsltInput.ContextInfo info = this.input.GetAttributes(1, this.input.Atoms.Name, out phantomNCName);
            if (phantomNCName == null)
            {
                phantomNCName = this.compiler.PhantomNCName;
            }
            return SetInfo(AstFactory.PI(phantomNCName, this.input.XslVersion), this.LoadEndTag(this.LoadInstructions()), info);
        }

        private XslNode XslSort()
        {
            string str;
            string str2;
            string str3;
            string str4;
            string str5;
            XsltInput.ContextInfo info = this.input.GetAttributes(0, this.input.Atoms.Select, out str, this.input.Atoms.Lang, out str2, this.input.Atoms.DataType, out str3, this.input.Atoms.Order, out str4, this.input.Atoms.CaseOrder, out str5);
            if (str == null)
            {
                str = ".";
            }
            this.CheckNoContent();
            return SetInfo(AstFactory.Sort(str, str2, str3, str4, str5, this.input.XslVersion), null, info);
        }

        private XslNode XslText()
        {
            string str;
            XsltInput.ContextInfo info = this.input.GetAttributes(0, this.input.Atoms.DisableOutputEscaping, out str);
            SerializationHints hints = (this.ParseYesNo(str, this.input.Atoms.DisableOutputEscaping) == TriState.True) ? SerializationHints.DisableOutputEscaping : SerializationHints.None;
            List<XslNode> content = new List<XslNode>();
            if (this.input.MoveToFirstChild())
            {
                do
                {
                    switch (this.input.NodeType)
                    {
                        case XPathNodeType.Text:
                        case XPathNodeType.SignificantWhitespace:
                        case XPathNodeType.Whitespace:
                            content.Add(AstFactory.Text(this.input.Value, hints));
                            break;

                        default:
                            this.ReportError("Xslt_UnexpectedElement", new string[] { this.input.QualifiedName, this.input.Atoms.Text });
                            this.input.SkipNode();
                            break;
                    }
                }
                while (this.input.MoveToNextSibling());
                this.input.MoveToParent();
            }
            return SetInfo(AstFactory.List(), content, info);
        }

        private XslNode XslValueOf()
        {
            string str;
            string str2;
            XsltInput.ContextInfo info = this.input.GetAttributes(1, this.input.Atoms.Select, out str, this.input.Atoms.DisableOutputEscaping, out str2);
            bool flag = this.ParseYesNo(str2, this.input.Atoms.DisableOutputEscaping) == TriState.True;
            this.CheckNoContent();
            return SetInfo(AstFactory.XslNode(flag ? XslNodeType.ValueOfDoe : XslNodeType.ValueOf, null, str, this.input.XslVersion), null, info);
        }

        private VarPar XslVarPar(XslNodeType nodeType)
        {
            string str;
            string str2;
            XsltInput.ContextInfo info = this.input.GetAttributes(1, this.input.Atoms.Name, out str, this.input.Atoms.Select, out str2);
            List<XslNode> content = this.LoadInstructions();
            if (content.Count != 0)
            {
                content = this.LoadEndTag(content);
            }
            if ((str2 != null) && (content.Count != 0))
            {
                this.ReportError("Xslt_VariableCntSel2", new string[] { str });
            }
            VarPar to = AstFactory.VarPar(nodeType, this.CreateXPathQName(str), str2, this.input.XslVersion);
            SetInfo(to, content, info);
            return to;
        }

        private enum InstructionFlags
        {
            NoParamNoSort,
            AllowParam,
            AllowSort
        }
    }
}

