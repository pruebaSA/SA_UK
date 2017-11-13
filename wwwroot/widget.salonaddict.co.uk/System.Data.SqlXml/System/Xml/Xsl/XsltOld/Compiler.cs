namespace System.Xml.Xsl.XsltOld
{
    using Microsoft.CSharp;
    using Microsoft.VisualBasic;
    using MS.Internal.Xml.XPath;
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Text;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Runtime;
    using System.Xml.Xsl.XsltOld.Debugger;

    internal class Compiler
    {
        private static string[] _defaultNamespaces = new string[] { "System", "System.Collections", "System.Text", "System.Text.RegularExpressions", "System.Xml", "System.Xml.Xsl", "System.Xml.XPath" };
        private Hashtable[] _typeDeclsByLang = new Hashtable[] { new Hashtable(), new Hashtable(), new Hashtable() };
        public bool AllowBuiltInMode;
        private Keywords atoms;
        internal StringBuilder AvtStringBuilder = new StringBuilder();
        public static XmlQualifiedName BuiltInMode = new XmlQualifiedName("*", string.Empty);
        private XmlQualifiedName currentMode;
        private TemplateBaseAction currentTemplate;
        private HybridDictionary documentURIs = new HybridDictionary();
        private Hashtable globalNamespaceAliasTable;
        private NavigatorInput input;
        internal const int InvalidQueryKey = -1;
        private QueryBuilder queryBuilder = new QueryBuilder();
        private List<TheQuery> queryStore;
        private System.Xml.Xsl.XsltOld.RootAction rootAction;
        internal const double RootPriority = 0.5;
        private InputScope rootScope;
        internal Stylesheet rootStylesheet;
        private int rtfCount;
        private InputScopeManager scopeManager;
        private static int scriptClassCounter = 0;
        private ArrayList scriptFiles = new ArrayList();
        internal Stylesheet stylesheet;
        private int stylesheetid;
        private Stack stylesheets;
        private XmlResolver xmlResolver;

        internal void AddAttributeSet(AttributeSetAction attributeSet)
        {
            this.stylesheet.AddAttributeSet(attributeSet);
        }

        internal int AddBooleanQuery(string xpathQuery)
        {
            string str = XmlCharType.Instance.IsOnlyWhitespace(xpathQuery) ? xpathQuery : ("boolean(" + xpathQuery + ")");
            return this.AddQuery(str);
        }

        internal void AddDecimalFormat(XmlQualifiedName name, DecimalFormat formatinfo)
        {
            this.rootAction.AddDecimalFormat(name, formatinfo);
        }

        private void AddDocumentURI(string href)
        {
            this.documentURIs.Add(href, null);
        }

        internal void AddNamespaceAlias(string StylesheetURI, NamespaceInfo AliasInfo)
        {
            if (this.globalNamespaceAliasTable == null)
            {
                this.globalNamespaceAliasTable = new Hashtable();
            }
            NamespaceInfo info = this.globalNamespaceAliasTable[StylesheetURI] as NamespaceInfo;
            if ((info == null) || (AliasInfo.stylesheetId <= info.stylesheetId))
            {
                this.globalNamespaceAliasTable[StylesheetURI] = AliasInfo;
            }
        }

        internal int AddQuery(string xpathQuery) => 
            this.AddQuery(xpathQuery, true, true, false);

        internal int AddQuery(string xpathQuery, bool allowVar, bool allowKey, bool isPattern)
        {
            CompiledXpathExpr expr;
            try
            {
                expr = new CompiledXpathExpr(isPattern ? this.queryBuilder.BuildPatternQuery(xpathQuery, allowVar, allowKey) : this.queryBuilder.Build(xpathQuery, allowVar, allowKey), xpathQuery, false);
            }
            catch (XPathException exception)
            {
                if (!this.ForwardCompatibility)
                {
                    throw XsltException.Create("Xslt_InvalidXPath", new string[] { xpathQuery }, exception);
                }
                expr = new ErrorXPathExpression(xpathQuery, this.Input.BaseURI, this.Input.LineNumber, this.Input.LinePosition);
            }
            this.queryStore.Add(new TheQuery(expr, this.scopeManager));
            return (this.queryStore.Count - 1);
        }

        internal void AddScript(string source, ScriptingLanguage lang, string ns, string fileName, int lineNumber)
        {
            ValidateExtensionNamespace(ns);
            for (ScriptingLanguage language = ScriptingLanguage.JScript; language <= ScriptingLanguage.CSharp; language += 1)
            {
                Hashtable hashtable = this._typeDeclsByLang[(int) language];
                if (lang == language)
                {
                    CodeTypeDeclaration declaration = (CodeTypeDeclaration) hashtable[ns];
                    if (declaration == null)
                    {
                        declaration = new CodeTypeDeclaration(GenerateUniqueClassName()) {
                            TypeAttributes = TypeAttributes.Public
                        };
                        hashtable.Add(ns, declaration);
                    }
                    CodeSnippetTypeMember member = new CodeSnippetTypeMember(source);
                    if (lineNumber > 0)
                    {
                        member.LinePragma = new CodeLinePragma(fileName, lineNumber);
                        this.scriptFiles.Add(fileName);
                    }
                    declaration.Members.Add(member);
                }
                else if (hashtable.Contains(ns))
                {
                    throw XsltException.Create("Xslt_ScriptMixedLanguages", new string[] { ns });
                }
            }
        }

        internal int AddStringQuery(string xpathQuery)
        {
            string str = XmlCharType.Instance.IsOnlyWhitespace(xpathQuery) ? xpathQuery : ("string(" + xpathQuery + ")");
            return this.AddQuery(str);
        }

        internal void AddTemplate(TemplateAction template)
        {
            this.stylesheet.AddTemplate(template);
        }

        internal bool Advance() => 
            this.Document.Advance();

        internal void BeginTemplate(TemplateAction template)
        {
            this.currentTemplate = template;
            this.currentMode = template.Mode;
            this.CanHaveApplyImports = template.MatchKey != -1;
        }

        private CodeDomProvider ChooseCodeDomProvider(ScriptingLanguage lang)
        {
            if (lang == ScriptingLanguage.JScript)
            {
                return (CodeDomProvider) Activator.CreateInstance(Type.GetType("Microsoft.JScript.JScriptCodeProvider, Microsoft.JScript, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null);
            }
            if (lang != ScriptingLanguage.VisualBasic)
            {
                return new CSharpCodeProvider();
            }
            return new VBCodeProvider();
        }

        internal InputScopeManager CloneScopeManager() => 
            this.scopeManager.Clone();

        internal void Compile(NavigatorInput input, XmlResolver xmlResolver, Evidence evidence)
        {
            this.xmlResolver = xmlResolver;
            this.PushInputDocument(input);
            this.rootScope = this.scopeManager.PushScope();
            this.queryStore = new List<TheQuery>();
            try
            {
                this.rootStylesheet = new Stylesheet();
                this.PushStylesheet(this.rootStylesheet);
                try
                {
                    this.CreateRootAction();
                }
                catch (XsltCompileException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    throw new XsltCompileException(exception, this.Input.BaseURI, this.Input.LineNumber, this.Input.LinePosition);
                }
                this.stylesheet.ProcessTemplates();
                this.rootAction.PorcessAttributeSets(this.rootStylesheet);
                this.stylesheet.SortWhiteSpace();
                this.CompileScript(evidence);
                if (evidence != null)
                {
                    this.rootAction.permissions = SecurityManager.ResolvePolicy(evidence);
                }
                if (this.globalNamespaceAliasTable != null)
                {
                    this.stylesheet.ReplaceNamespaceAlias(this);
                    this.rootAction.ReplaceNamespaceAlias(this);
                }
            }
            finally
            {
                this.PopInputDocument();
            }
        }

        [PermissionSet(SecurityAction.Demand, Name="FullTrust")]
        private void CompileAssembly(ScriptingLanguage lang, Hashtable typeDecls, string nsName, Evidence evidence)
        {
            nsName = "Microsoft.Xslt.CompiledScripts." + nsName;
            CodeNamespace namespace2 = new CodeNamespace(nsName);
            foreach (string str in _defaultNamespaces)
            {
                namespace2.Imports.Add(new CodeNamespaceImport(str));
            }
            if (lang == ScriptingLanguage.VisualBasic)
            {
                namespace2.Imports.Add(new CodeNamespaceImport("Microsoft.VisualBasic"));
            }
            foreach (CodeTypeDeclaration declaration in typeDecls.Values)
            {
                namespace2.Types.Add(declaration);
            }
            CodeCompileUnit unit = new CodeCompileUnit();
            unit.Namespaces.Add(namespace2);
            unit.UserData["AllowLateBound"] = true;
            unit.UserData["RequireVariableDeclaration"] = false;
            CompilerParameters options = new CompilerParameters();
            try
            {
                new SecurityPermission(SecurityPermissionFlag.ControlEvidence).Assert();
                try
                {
                    options.GenerateInMemory = true;
                    options.Evidence = evidence;
                    options.ReferencedAssemblies.Add(typeof(XPathNavigator).Module.FullyQualifiedName);
                    options.ReferencedAssemblies.Add("system.dll");
                    if (lang == ScriptingLanguage.VisualBasic)
                    {
                        options.ReferencedAssemblies.Add("microsoft.visualbasic.dll");
                    }
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
            }
            catch
            {
                throw;
            }
            CompilerResults results = this.ChooseCodeDomProvider(lang).CompileAssemblyFromDom(options, new CodeCompileUnit[] { unit });
            if (results.Errors.HasErrors)
            {
                StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
                foreach (CompilerError error in results.Errors)
                {
                    this.FixCompilerError(error);
                    writer.WriteLine(error.ToString());
                }
                throw XsltException.Create("Xslt_ScriptCompileErrors", new string[] { writer.ToString() });
            }
            Assembly compiledAssembly = results.CompiledAssembly;
            foreach (DictionaryEntry entry in typeDecls)
            {
                string key = (string) entry.Key;
                CodeTypeDeclaration declaration2 = (CodeTypeDeclaration) entry.Value;
                this.stylesheet.ScriptObjectTypes.Add(key, compiledAssembly.GetType(nsName + "." + declaration2.Name));
            }
        }

        internal ArrayList CompileAvt(string avtText)
        {
            bool flag;
            return this.CompileAvt(avtText, out flag);
        }

        internal ArrayList CompileAvt(string avtText, out bool constant)
        {
            bool flag;
            ArrayList list = new ArrayList();
            constant = true;
            int start = 0;
            while (GetNextAvtLex(avtText, ref start, this.AvtStringBuilder, out flag))
            {
                string xpathQuery = this.AvtStringBuilder.ToString();
                if (flag)
                {
                    list.Add(new AvtEvent(this.AddStringQuery(xpathQuery)));
                    constant = false;
                }
                else
                {
                    list.Add(new TextEvent(xpathQuery));
                }
            }
            return list;
        }

        private void CompileScript(Evidence evidence)
        {
            for (ScriptingLanguage language = ScriptingLanguage.JScript; language <= ScriptingLanguage.CSharp; language += 1)
            {
                int index = (int) language;
                if (this._typeDeclsByLang[index].Count > 0)
                {
                    this.CompileAssembly(language, this._typeDeclsByLang[index], language.ToString(), evidence);
                }
            }
        }

        public virtual ApplyImportsAction CreateApplyImportsAction()
        {
            ApplyImportsAction action = new ApplyImportsAction();
            action.Compile(this);
            return action;
        }

        public virtual ApplyTemplatesAction CreateApplyTemplatesAction()
        {
            ApplyTemplatesAction action = new ApplyTemplatesAction();
            action.Compile(this);
            return action;
        }

        public virtual AttributeAction CreateAttributeAction()
        {
            AttributeAction action = new AttributeAction();
            action.Compile(this);
            return action;
        }

        public virtual AttributeSetAction CreateAttributeSetAction()
        {
            AttributeSetAction action = new AttributeSetAction();
            action.Compile(this);
            return action;
        }

        public virtual BeginEvent CreateBeginEvent() => 
            new BeginEvent(this);

        public virtual CallTemplateAction CreateCallTemplateAction()
        {
            CallTemplateAction action = new CallTemplateAction();
            action.Compile(this);
            return action;
        }

        public virtual ChooseAction CreateChooseAction()
        {
            ChooseAction action = new ChooseAction();
            action.Compile(this);
            return action;
        }

        public virtual CommentAction CreateCommentAction()
        {
            CommentAction action = new CommentAction();
            action.Compile(this);
            return action;
        }

        public virtual CopyAction CreateCopyAction()
        {
            CopyAction action = new CopyAction();
            action.Compile(this);
            return action;
        }

        public virtual CopyOfAction CreateCopyOfAction()
        {
            CopyOfAction action = new CopyOfAction();
            action.Compile(this);
            return action;
        }

        public virtual ElementAction CreateElementAction()
        {
            ElementAction action = new ElementAction();
            action.Compile(this);
            return action;
        }

        public virtual ForEachAction CreateForEachAction()
        {
            ForEachAction action = new ForEachAction();
            action.Compile(this);
            return action;
        }

        public virtual IfAction CreateIfAction(IfAction.ConditionType type)
        {
            IfAction action = new IfAction(type);
            action.Compile(this);
            return action;
        }

        public virtual MessageAction CreateMessageAction()
        {
            MessageAction action = new MessageAction();
            action.Compile(this);
            return action;
        }

        public virtual NewInstructionAction CreateNewInstructionAction()
        {
            NewInstructionAction action = new NewInstructionAction();
            action.Compile(this);
            return action;
        }

        public virtual NumberAction CreateNumberAction()
        {
            NumberAction action = new NumberAction();
            action.Compile(this);
            return action;
        }

        public virtual ProcessingInstructionAction CreateProcessingInstructionAction()
        {
            ProcessingInstructionAction action = new ProcessingInstructionAction();
            action.Compile(this);
            return action;
        }

        public virtual void CreateRootAction()
        {
            this.RootAction = new System.Xml.Xsl.XsltOld.RootAction();
            this.RootAction.Compile(this);
        }

        public virtual TemplateAction CreateSingleTemplateAction()
        {
            TemplateAction action = new TemplateAction();
            action.CompileSingle(this);
            return action;
        }

        public virtual SortAction CreateSortAction()
        {
            SortAction action = new SortAction();
            action.Compile(this);
            return action;
        }

        public virtual TemplateAction CreateTemplateAction()
        {
            TemplateAction action = new TemplateAction();
            action.Compile(this);
            return action;
        }

        public virtual TextAction CreateTextAction()
        {
            TextAction action = new TextAction();
            action.Compile(this);
            return action;
        }

        public virtual TextEvent CreateTextEvent() => 
            new TextEvent(this);

        public virtual UseAttributeSetsAction CreateUseAttributeSetsAction()
        {
            UseAttributeSetsAction action = new UseAttributeSetsAction();
            action.Compile(this);
            return action;
        }

        public virtual ValueOfAction CreateValueOfAction()
        {
            ValueOfAction action = new ValueOfAction();
            action.Compile(this);
            return action;
        }

        public virtual VariableAction CreateVariableAction(VariableType type)
        {
            VariableAction action = new VariableAction(type);
            action.Compile(this);
            if (action.VarKey != -1)
            {
                return action;
            }
            return null;
        }

        public virtual WithParamAction CreateWithParamAction()
        {
            WithParamAction action = new WithParamAction();
            action.Compile(this);
            return action;
        }

        internal XmlQualifiedName CreateXmlQName(string qname)
        {
            string str;
            string str2;
            PrefixQName.ParseQualifiedName(qname, out str, out str2);
            return new XmlQualifiedName(str2, this.scopeManager.ResolveXmlNamespace(str));
        }

        internal XmlQualifiedName CreateXPathQName(string qname)
        {
            string str;
            string str2;
            PrefixQName.ParseQualifiedName(qname, out str, out str2);
            return new XmlQualifiedName(str2, this.scopeManager.ResolveXPathNamespace(str));
        }

        internal void EndTemplate()
        {
            this.currentTemplate = this.rootAction;
        }

        internal NamespaceInfo FindNamespaceAlias(string StylesheetURI)
        {
            if (this.globalNamespaceAliasTable != null)
            {
                return (NamespaceInfo) this.globalNamespaceAliasTable[StylesheetURI];
            }
            return null;
        }

        private void FixCompilerError(CompilerError e)
        {
            foreach (string str in this.scriptFiles)
            {
                if (e.FileName == str)
                {
                    return;
                }
            }
            e.FileName = string.Empty;
        }

        private static string GenerateUniqueClassName() => 
            ("ScriptClass_" + ++scriptClassCounter);

        private static bool GetNextAvtLex(string avt, ref int start, StringBuilder lex, out bool isAvt)
        {
            isAvt = false;
            if (start == avt.Length)
            {
                return false;
            }
            lex.Length = 0;
            getTextLex(avt, ref start, lex);
            if (lex.Length == 0)
            {
                isAvt = true;
                getXPathLex(avt, ref start, lex);
            }
            return true;
        }

        public string GetNsAlias(ref string prefix)
        {
            if (Keywords.Compare(this.input.Atoms.HashDefault, prefix))
            {
                prefix = string.Empty;
                return this.DefaultNamespace;
            }
            if (!PrefixQName.ValidatePrefix(prefix))
            {
                throw XsltException.Create("Xslt_InvalidAttrValue", new string[] { this.input.LocalName, prefix });
            }
            return this.ResolveXPathNamespace(prefix);
        }

        internal string GetSingleAttribute(string attributeAtom)
        {
            NavigatorInput input = this.Input;
            string localName = input.LocalName;
            string str2 = null;
            if (input.MoveToFirstAttribute())
            {
                do
                {
                    string namespaceURI = input.NamespaceURI;
                    string strA = input.LocalName;
                    if (Keywords.Equals(namespaceURI, this.Atoms.Empty))
                    {
                        if (Keywords.Equals(strA, attributeAtom))
                        {
                            str2 = input.Value;
                        }
                        else if (!this.ForwardCompatibility)
                        {
                            throw XsltException.Create("Xslt_InvalidAttribute", new string[] { strA, localName });
                        }
                    }
                }
                while (input.MoveToNextAttribute());
                input.ToParent();
            }
            if (str2 == null)
            {
                throw XsltException.Create("Xslt_MissingAttribute", new string[] { attributeAtom });
            }
            return str2;
        }

        private static void getTextLex(string avt, ref int start, StringBuilder lex)
        {
            int length = avt.Length;
            int num2 = start;
            while (num2 < length)
            {
                char ch = avt[num2];
                if (ch == '{')
                {
                    if (((num2 + 1) >= length) || (avt[num2 + 1] != '{'))
                    {
                        break;
                    }
                    num2++;
                }
                else if (ch == '}')
                {
                    if (((num2 + 1) >= length) || (avt[num2 + 1] != '}'))
                    {
                        throw XsltException.Create("Xslt_SingleRightAvt", new string[] { avt });
                    }
                    num2++;
                }
                lex.Append(ch);
                num2++;
            }
            start = num2;
        }

        internal string GetUnicRtfId()
        {
            this.rtfCount++;
            return this.rtfCount.ToString(CultureInfo.InvariantCulture);
        }

        private static void getXPathLex(string avt, ref int start, StringBuilder lex)
        {
            int length = avt.Length;
            int num2 = 0;
            for (int i = start + 1; i < length; i++)
            {
                char ch = avt[i];
                switch (num2)
                {
                    case 0:
                    {
                        switch (ch)
                        {
                            case '}':
                                goto Label_0077;

                            case '\'':
                                goto Label_00B2;

                            case '"':
                                goto Label_00B6;
                        }
                        continue;
                    }
                    case 1:
                    {
                        if (ch == '\'')
                        {
                            num2 = 0;
                        }
                        continue;
                    }
                    case 2:
                    {
                        if (ch == '"')
                        {
                            num2 = 0;
                        }
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                throw XsltException.Create("Xslt_NestedAvt", new string[] { avt });
            Label_0077:
                i++;
                if (i == (start + 2))
                {
                    throw XsltException.Create("Xslt_EmptyAvtExpr", new string[] { avt });
                }
                lex.Append(avt, start + 1, (i - start) - 2);
                start = i;
                return;
            Label_00B2:
                num2 = 1;
                continue;
            Label_00B6:
                num2 = 2;
            }
            throw XsltException.Create((num2 == 0) ? "Xslt_OpenBracesAvt" : "Xslt_OpenLiteralAvt", new string[] { avt });
        }

        internal bool GetYesNo(string value)
        {
            if (value.Equals(this.Atoms.Yes))
            {
                return true;
            }
            if (!value.Equals(this.Atoms.No))
            {
                throw XsltException.Create("Xslt_InvalidAttrValue", new string[] { this.Input.LocalName, value });
            }
            return false;
        }

        internal void InsertExcludedNamespace()
        {
            this.InsertExcludedNamespace(this.Input.Navigator.GetAttribute(this.Input.Atoms.ExcludeResultPrefixes, this.Input.Atoms.XsltNamespace));
        }

        internal void InsertExcludedNamespace(string value)
        {
            string[] nsList = this.ResolvePrefixes(value);
            if (nsList != null)
            {
                this.scopeManager.InsertExcludedNamespaces(nsList);
            }
        }

        internal void InsertExtensionNamespace()
        {
            this.InsertExtensionNamespace(this.Input.Navigator.GetAttribute(this.Input.Atoms.ExtensionElementPrefixes, this.Input.Atoms.XsltNamespace));
        }

        internal void InsertExtensionNamespace(string value)
        {
            string[] nsList = this.ResolvePrefixes(value);
            if (nsList != null)
            {
                this.scopeManager.InsertExtensionNamespaces(nsList);
            }
        }

        internal void InsertKey(XmlQualifiedName name, int MatchKey, int UseKey)
        {
            this.rootAction.InsertKey(name, MatchKey, UseKey);
        }

        internal int InsertVariable(VariableAction variable)
        {
            InputScope rootScope;
            if (variable.IsGlobal)
            {
                rootScope = this.rootScope;
            }
            else
            {
                rootScope = this.scopeManager.VariableScope;
            }
            VariableAction action = rootScope.ResolveVariable(variable.Name);
            if (action != null)
            {
                if (!action.IsGlobal)
                {
                    throw XsltException.Create("Xslt_DupVarName", new string[] { variable.NameStr });
                }
                if (variable.IsGlobal)
                {
                    if (variable.Stylesheetid == action.Stylesheetid)
                    {
                        throw XsltException.Create("Xslt_DupVarName", new string[] { variable.NameStr });
                    }
                    if (variable.Stylesheetid >= action.Stylesheetid)
                    {
                        return -1;
                    }
                    rootScope.InsertVariable(variable);
                    return action.VarKey;
                }
            }
            rootScope.InsertVariable(variable);
            return this.currentTemplate.AllocateVariableSlot();
        }

        internal bool IsCircularReference(string href) => 
            this.documentURIs.Contains(href);

        internal bool IsExcludedNamespace(string nspace) => 
            this.scopeManager.IsExcludedNamespace(nspace);

        internal bool IsExtensionNamespace(string nspace) => 
            this.scopeManager.IsExtensionNamespace(nspace);

        internal bool IsNamespaceAlias(string StylesheetURI) => 
            this.globalNamespaceAliasTable?.Contains(StylesheetURI);

        internal static XPathDocument LoadDocument(XmlTextReaderImpl reader)
        {
            XPathDocument document;
            reader.EntityHandling = EntityHandling.ExpandEntities;
            reader.XmlValidatingReaderCompatibilityMode = true;
            try
            {
                document = new XPathDocument(reader, XmlSpace.Preserve);
            }
            finally
            {
                reader.Close();
            }
            return document;
        }

        internal void PopInputDocument()
        {
            NavigatorInput input = this.input;
            this.input = input.Next;
            input.Next = null;
            if (this.input != null)
            {
                this.atoms = this.input.Atoms;
                this.scopeManager = this.input.InputScopeManager;
            }
            else
            {
                this.atoms = null;
                this.scopeManager = null;
            }
            this.RemoveDocumentURI(input.Href);
            input.Close();
        }

        internal virtual void PopScope()
        {
            this.currentTemplate.ReleaseVariableSlots(this.scopeManager.CurrentScope.GetVeriablesCount());
            this.scopeManager.PopScope();
        }

        internal Stylesheet PopStylesheet()
        {
            Stylesheet stylesheet = (Stylesheet) this.stylesheets.Pop();
            this.stylesheet = (Stylesheet) this.stylesheets.Peek();
            return stylesheet;
        }

        internal void PushInputDocument(NavigatorInput newInput)
        {
            string href = newInput.Href;
            this.AddDocumentURI(href);
            newInput.Next = this.input;
            this.input = newInput;
            this.atoms = this.input.Atoms;
            this.scopeManager = this.input.InputScopeManager;
        }

        internal void PushLiteralScope()
        {
            this.PushNamespaceScope();
            string attribute = this.Input.Navigator.GetAttribute(this.Atoms.Version, this.Atoms.XsltNamespace);
            if (attribute.Length != 0)
            {
                this.ForwardCompatibility = attribute != "1.0";
            }
        }

        internal void PushNamespaceScope()
        {
            this.scopeManager.PushScope();
            NavigatorInput input = this.Input;
            if (input.MoveToFirstNamespace())
            {
                do
                {
                    this.scopeManager.PushNamespace(input.LocalName, input.Value);
                }
                while (input.MoveToNextNamespace());
                input.ToParent();
            }
        }

        internal void PushStylesheet(Stylesheet stylesheet)
        {
            if (this.stylesheets == null)
            {
                this.stylesheets = new Stack();
            }
            this.stylesheets.Push(stylesheet);
            this.stylesheet = stylesheet;
        }

        internal bool Recurse() => 
            this.Document.Recurse();

        private void RemoveDocumentURI(string href)
        {
            this.documentURIs.Remove(href);
        }

        internal NavigatorInput ResolveDocument(Uri absoluteUri)
        {
            object obj2 = this.xmlResolver.GetEntity(absoluteUri, null, null);
            string url = absoluteUri.ToString();
            if (obj2 is Stream)
            {
                XmlTextReaderImpl reader = new XmlTextReaderImpl(url, (Stream) obj2) {
                    XmlResolver = this.xmlResolver
                };
                return new NavigatorInput(LoadDocument(reader).CreateNavigator(), url, this.rootScope);
            }
            if (!(obj2 is XPathNavigator))
            {
                throw XsltException.Create("Xslt_CantResolve", new string[] { url });
            }
            return new NavigatorInput((XPathNavigator) obj2, url, this.rootScope);
        }

        private string[] ResolvePrefixes(string tokens)
        {
            if ((tokens == null) || (tokens.Length == 0))
            {
                return null;
            }
            string[] strArray = XmlConvert.SplitString(tokens);
            try
            {
                for (int i = 0; i < strArray.Length; i++)
                {
                    string str = strArray[i];
                    strArray[i] = this.scopeManager.ResolveXmlNamespace((str == "#default") ? string.Empty : str);
                }
            }
            catch (XsltException)
            {
                if (!this.ForwardCompatibility)
                {
                    throw;
                }
                return null;
            }
            return strArray;
        }

        internal Uri ResolveUri(string relativeUri)
        {
            string baseURI = this.Input.BaseURI;
            Uri uri = this.xmlResolver.ResolveUri((baseURI.Length != 0) ? this.xmlResolver.ResolveUri(null, baseURI) : null, relativeUri);
            if (uri == null)
            {
                throw XsltException.Create("Xslt_CantResolve", new string[] { relativeUri });
            }
            return uri;
        }

        internal string ResolveXmlNamespace(string prefix) => 
            this.scopeManager.ResolveXmlNamespace(prefix);

        internal string ResolveXPathNamespace(string prefix) => 
            this.scopeManager.ResolveXPathNamespace(prefix);

        internal bool ToParent() => 
            this.Document.ToParent();

        public XsltException UnexpectedKeyword()
        {
            XPathNavigator navigator = this.Input.Navigator.Clone();
            string name = navigator.Name;
            navigator.MoveToParent();
            string str2 = navigator.Name;
            return XsltException.Create("Xslt_UnexpectedKeyword", new string[] { name, str2 });
        }

        private static void ValidateExtensionNamespace(string nsUri)
        {
            if ((nsUri.Length == 0) || (nsUri == "http://www.w3.org/1999/XSL/Transform"))
            {
                throw XsltException.Create("Xslt_InvalidExtensionNamespace", new string[0]);
            }
            XmlConvert.ToUri(nsUri);
        }

        internal Keywords Atoms =>
            this.atoms;

        internal bool CanHaveApplyImports
        {
            get => 
                this.scopeManager.CurrentScope.CanHaveApplyImports;
            set
            {
                this.scopeManager.CurrentScope.CanHaveApplyImports = value;
            }
        }

        internal Stylesheet CompiledStylesheet =>
            this.stylesheet;

        internal XmlQualifiedName CurrentMode =>
            this.currentMode;

        public virtual IXsltDebugger Debugger =>
            null;

        internal string DefaultNamespace =>
            this.scopeManager.DefaultNamespace;

        internal NavigatorInput Document =>
            this.input;

        internal bool ForwardCompatibility
        {
            get => 
                this.scopeManager.CurrentScope.ForwardCompatibility;
            set
            {
                this.scopeManager.CurrentScope.ForwardCompatibility = value;
            }
        }

        internal NavigatorInput Input =>
            this.input;

        internal List<TheQuery> QueryStore =>
            this.queryStore;

        internal System.Xml.Xsl.XsltOld.RootAction RootAction
        {
            get => 
                this.rootAction;
            set
            {
                this.rootAction = value;
                this.currentTemplate = this.rootAction;
            }
        }

        protected InputScopeManager ScopeManager =>
            this.scopeManager;

        internal int Stylesheetid
        {
            get => 
                this.stylesheetid;
            set
            {
                this.stylesheetid = value;
            }
        }

        internal class ErrorXPathExpression : CompiledXpathExpr
        {
            private string baseUri;
            private int lineNumber;
            private int linePosition;

            public ErrorXPathExpression(string expression, string baseUri, int lineNumber, int linePosition) : base(null, expression, false)
            {
                this.baseUri = baseUri;
                this.lineNumber = lineNumber;
                this.linePosition = linePosition;
            }

            public override void CheckErrors()
            {
                throw new XsltException("Xslt_InvalidXPath", new string[] { this.Expression }, this.baseUri, this.linePosition, this.lineNumber, null);
            }

            public override XPathExpression Clone() => 
                this;
        }
    }
}

