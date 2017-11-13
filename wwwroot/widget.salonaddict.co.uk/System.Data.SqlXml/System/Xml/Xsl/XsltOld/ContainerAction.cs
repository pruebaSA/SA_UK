namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Runtime;

    internal class ContainerAction : CompiledAction
    {
        internal ArrayList containedActions;
        internal CopyCodeAction lastCopyCodeAction;
        private int maxid;
        protected const int ProcessingChildren = 1;

        internal void AddAction(Action action)
        {
            if (this.containedActions == null)
            {
                this.containedActions = new ArrayList();
            }
            this.containedActions.Add(action);
            this.lastCopyCodeAction = null;
        }

        protected void AddEvent(Event copyEvent)
        {
            this.EnsureCopyCodeAction();
            this.lastCopyCodeAction.AddEvent(copyEvent);
        }

        protected void AddEvents(ArrayList copyEvents)
        {
            this.EnsureCopyCodeAction();
            this.lastCopyCodeAction.AddEvents(copyEvents);
        }

        private void AddScript(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            ScriptingLanguage jScript = ScriptingLanguage.JScript;
            string ns = null;
            if (input.MoveToFirstAttribute())
            {
                do
                {
                    if (input.LocalName == input.Atoms.Language)
                    {
                        string strA = input.Value;
                        if ((string.Compare(strA, "jscript", StringComparison.OrdinalIgnoreCase) != 0) && (string.Compare(strA, "javascript", StringComparison.OrdinalIgnoreCase) != 0))
                        {
                            if ((string.Compare(strA, "c#", StringComparison.OrdinalIgnoreCase) != 0) && (string.Compare(strA, "csharp", StringComparison.OrdinalIgnoreCase) != 0))
                            {
                                if ((string.Compare(strA, "vb", StringComparison.OrdinalIgnoreCase) != 0) && (string.Compare(strA, "visualbasic", StringComparison.OrdinalIgnoreCase) != 0))
                                {
                                    throw XsltException.Create("Xslt_ScriptInvalidLanguage", new string[] { strA });
                                }
                                jScript = ScriptingLanguage.VisualBasic;
                            }
                            else
                            {
                                jScript = ScriptingLanguage.CSharp;
                            }
                        }
                        else
                        {
                            jScript = ScriptingLanguage.JScript;
                        }
                    }
                    else if (input.LocalName == input.Atoms.ImplementsPrefix)
                    {
                        if (!PrefixQName.ValidatePrefix(input.Value))
                        {
                            throw XsltException.Create("Xslt_InvalidAttrValue", new string[] { input.LocalName, input.Value });
                        }
                        ns = compiler.ResolveXmlNamespace(input.Value);
                    }
                }
                while (input.MoveToNextAttribute());
                input.ToParent();
            }
            if (ns == null)
            {
                throw XsltException.Create("Xslt_MissingAttribute", new string[] { input.Atoms.ImplementsPrefix });
            }
            if (!input.Recurse() || (input.NodeType != XPathNodeType.Text))
            {
                throw XsltException.Create("Xslt_ScriptEmpty", new string[0]);
            }
            compiler.AddScript(input.Value, jScript, ns, input.BaseURI, input.LineNumber);
            input.ToParent();
        }

        internal bool CheckAttribute(bool valid, Compiler compiler)
        {
            if (valid)
            {
                return true;
            }
            if (!compiler.ForwardCompatibility)
            {
                throw XsltException.Create("Xslt_InvalidAttrValue", new string[] { compiler.Input.LocalName, compiler.Input.Value });
            }
            return false;
        }

        internal void CheckDuplicateParams(XmlQualifiedName name)
        {
            if (this.containedActions != null)
            {
                foreach (CompiledAction action in this.containedActions)
                {
                    WithParamAction action2 = action as WithParamAction;
                    if ((action2 != null) && (action2.Name == name))
                    {
                        throw XsltException.Create("Xslt_DuplicateWithParam", new string[] { name.ToString() });
                    }
                }
            }
        }

        internal override void Compile(Compiler compiler)
        {
            throw new NotImplementedException();
        }

        protected void CompileDecimalFormat(Compiler compiler)
        {
            NumberFormatInfo info = new NumberFormatInfo();
            DecimalFormat formatinfo = new DecimalFormat(info, '#', '0', ';');
            XmlQualifiedName name = null;
            NavigatorInput input = compiler.Input;
            if (input.MoveToFirstAttribute())
            {
                do
                {
                    if (Keywords.Equals(input.Prefix, input.Atoms.Empty))
                    {
                        string localName = input.LocalName;
                        string qname = input.Value;
                        if (Keywords.Equals(localName, input.Atoms.Name))
                        {
                            name = compiler.CreateXPathQName(qname);
                        }
                        else if (Keywords.Equals(localName, input.Atoms.DecimalSeparator))
                        {
                            info.NumberDecimalSeparator = qname;
                        }
                        else if (Keywords.Equals(localName, input.Atoms.GroupingSeparator))
                        {
                            info.NumberGroupSeparator = qname;
                        }
                        else if (Keywords.Equals(localName, input.Atoms.Infinity))
                        {
                            info.PositiveInfinitySymbol = qname;
                        }
                        else if (Keywords.Equals(localName, input.Atoms.MinusSign))
                        {
                            info.NegativeSign = qname;
                        }
                        else if (Keywords.Equals(localName, input.Atoms.NaN))
                        {
                            info.NaNSymbol = qname;
                        }
                        else if (Keywords.Equals(localName, input.Atoms.Percent))
                        {
                            info.PercentSymbol = qname;
                        }
                        else if (Keywords.Equals(localName, input.Atoms.PerMille))
                        {
                            info.PerMilleSymbol = qname;
                        }
                        else if (Keywords.Equals(localName, input.Atoms.Digit))
                        {
                            if (this.CheckAttribute(qname.Length == 1, compiler))
                            {
                                formatinfo.digit = qname[0];
                            }
                        }
                        else if (Keywords.Equals(localName, input.Atoms.ZeroDigit))
                        {
                            if (this.CheckAttribute(qname.Length == 1, compiler))
                            {
                                formatinfo.zeroDigit = qname[0];
                            }
                        }
                        else if (Keywords.Equals(localName, input.Atoms.PatternSeparator) && this.CheckAttribute(qname.Length == 1, compiler))
                        {
                            formatinfo.patternSeparator = qname[0];
                        }
                    }
                }
                while (input.MoveToNextAttribute());
                input.ToParent();
            }
            info.NegativeInfinitySymbol = info.NegativeSign + info.PositiveInfinitySymbol;
            if (name == null)
            {
                name = new XmlQualifiedName();
            }
            compiler.AddDecimalFormat(name, formatinfo);
            base.CheckEmpty(compiler);
        }

        protected void CompileDocument(Compiler compiler, bool inInclude)
        {
            NavigatorInput input = compiler.Input;
            while (input.NodeType != XPathNodeType.Element)
            {
                if (!compiler.Advance())
                {
                    throw XsltException.Create("Xslt_WrongStylesheetElement", new string[0]);
                }
            }
            if (Keywords.Equals(input.NamespaceURI, input.Atoms.XsltNamespace))
            {
                if (!Keywords.Equals(input.LocalName, input.Atoms.Stylesheet) && !Keywords.Equals(input.LocalName, input.Atoms.Transform))
                {
                    throw XsltException.Create("Xslt_WrongStylesheetElement", new string[0]);
                }
                compiler.PushNamespaceScope();
                this.CompileStylesheetAttributes(compiler);
                this.CompileTopLevelElements(compiler);
                if (!inInclude)
                {
                    this.CompileImports(compiler);
                }
            }
            else
            {
                compiler.PushLiteralScope();
                this.CompileSingleTemplate(compiler);
            }
            compiler.PopScope();
        }

        internal Stylesheet CompileImport(Compiler compiler, Uri uri, int id)
        {
            NavigatorInput newInput = compiler.ResolveDocument(uri);
            compiler.PushInputDocument(newInput);
            try
            {
                compiler.PushStylesheet(new Stylesheet());
                compiler.Stylesheetid = id;
                this.CompileDocument(compiler, false);
            }
            catch (XsltCompileException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new XsltCompileException(exception, newInput.BaseURI, newInput.LineNumber, newInput.LinePosition);
            }
            finally
            {
                compiler.PopInputDocument();
            }
            return compiler.PopStylesheet();
        }

        private void CompileImports(Compiler compiler)
        {
            ArrayList imports = compiler.CompiledStylesheet.Imports;
            int stylesheetid = compiler.Stylesheetid;
            for (int i = imports.Count - 1; 0 <= i; i--)
            {
                Uri uri = imports[i] as Uri;
                imports[i] = this.CompileImport(compiler, uri, ++this.maxid);
            }
            compiler.Stylesheetid = stylesheetid;
        }

        private void CompileInclude(Compiler compiler)
        {
            Uri absoluteUri = compiler.ResolveUri(compiler.GetSingleAttribute(compiler.Input.Atoms.Href));
            string href = absoluteUri.ToString();
            if (compiler.IsCircularReference(href))
            {
                throw XsltException.Create("Xslt_CircularInclude", new string[] { href });
            }
            NavigatorInput newInput = compiler.ResolveDocument(absoluteUri);
            compiler.PushInputDocument(newInput);
            try
            {
                this.CompileDocument(compiler, true);
            }
            catch (XsltCompileException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new XsltCompileException(exception, newInput.BaseURI, newInput.LineNumber, newInput.LinePosition);
            }
            finally
            {
                compiler.PopInputDocument();
            }
            base.CheckEmpty(compiler);
        }

        private void CompileInstruction(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            CompiledAction action = null;
            string localName = input.LocalName;
            if (Keywords.Equals(localName, input.Atoms.ApplyImports))
            {
                action = compiler.CreateApplyImportsAction();
            }
            else if (Keywords.Equals(localName, input.Atoms.ApplyTemplates))
            {
                action = compiler.CreateApplyTemplatesAction();
            }
            else if (Keywords.Equals(localName, input.Atoms.Attribute))
            {
                action = compiler.CreateAttributeAction();
            }
            else if (Keywords.Equals(localName, input.Atoms.CallTemplate))
            {
                action = compiler.CreateCallTemplateAction();
            }
            else if (Keywords.Equals(localName, input.Atoms.Choose))
            {
                action = compiler.CreateChooseAction();
            }
            else if (Keywords.Equals(localName, input.Atoms.Comment))
            {
                action = compiler.CreateCommentAction();
            }
            else if (Keywords.Equals(localName, input.Atoms.Copy))
            {
                action = compiler.CreateCopyAction();
            }
            else if (Keywords.Equals(localName, input.Atoms.CopyOf))
            {
                action = compiler.CreateCopyOfAction();
            }
            else if (Keywords.Equals(localName, input.Atoms.Element))
            {
                action = compiler.CreateElementAction();
            }
            else
            {
                if (Keywords.Equals(localName, input.Atoms.Fallback))
                {
                    return;
                }
                if (Keywords.Equals(localName, input.Atoms.ForEach))
                {
                    action = compiler.CreateForEachAction();
                }
                else if (Keywords.Equals(localName, input.Atoms.If))
                {
                    action = compiler.CreateIfAction(IfAction.ConditionType.ConditionIf);
                }
                else if (Keywords.Equals(localName, input.Atoms.Message))
                {
                    action = compiler.CreateMessageAction();
                }
                else if (Keywords.Equals(localName, input.Atoms.Number))
                {
                    action = compiler.CreateNumberAction();
                }
                else if (Keywords.Equals(localName, input.Atoms.ProcessingInstruction))
                {
                    action = compiler.CreateProcessingInstructionAction();
                }
                else if (Keywords.Equals(localName, input.Atoms.Text))
                {
                    action = compiler.CreateTextAction();
                }
                else if (Keywords.Equals(localName, input.Atoms.ValueOf))
                {
                    action = compiler.CreateValueOfAction();
                }
                else if (Keywords.Equals(localName, input.Atoms.Variable))
                {
                    action = compiler.CreateVariableAction(VariableType.LocalVariable);
                }
                else
                {
                    if (!compiler.ForwardCompatibility)
                    {
                        throw compiler.UnexpectedKeyword();
                    }
                    action = compiler.CreateNewInstructionAction();
                }
            }
            this.AddAction(action);
        }

        internal void CompileKey(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            string localName = input.LocalName;
            int matchKey = -1;
            int useKey = -1;
            XmlQualifiedName name = null;
            if (input.MoveToFirstAttribute())
            {
                do
                {
                    string namespaceURI = input.NamespaceURI;
                    string strA = input.LocalName;
                    string qname = input.Value;
                    if (Keywords.Equals(namespaceURI, input.Atoms.Empty))
                    {
                        if (Keywords.Equals(strA, input.Atoms.Name))
                        {
                            name = compiler.CreateXPathQName(qname);
                        }
                        else if (Keywords.Equals(strA, input.Atoms.Match))
                        {
                            matchKey = compiler.AddQuery(qname, false, false, true);
                        }
                        else if (Keywords.Equals(strA, input.Atoms.Use))
                        {
                            useKey = compiler.AddQuery(qname, false, false, false);
                        }
                        else if (!compiler.ForwardCompatibility)
                        {
                            throw XsltException.Create("Xslt_InvalidAttribute", new string[] { strA, localName });
                        }
                    }
                }
                while (input.MoveToNextAttribute());
                input.ToParent();
            }
            base.CheckRequiredAttribute(compiler, matchKey != -1, "match");
            base.CheckRequiredAttribute(compiler, useKey != -1, "use");
            base.CheckRequiredAttribute(compiler, name != null, "name");
            compiler.InsertKey(name, matchKey, useKey);
        }

        private void CompileLiteral(Compiler compiler)
        {
            switch (compiler.Input.NodeType)
            {
                case XPathNodeType.Element:
                    this.AddEvent(compiler.CreateBeginEvent());
                    this.CompileLiteralAttributesAndNamespaces(compiler);
                    if (compiler.Recurse())
                    {
                        this.CompileTemplate(compiler);
                        compiler.ToParent();
                    }
                    this.AddEvent(new EndEvent(XPathNodeType.Element));
                    return;

                case XPathNodeType.Attribute:
                case XPathNodeType.Namespace:
                case XPathNodeType.Whitespace:
                case XPathNodeType.ProcessingInstruction:
                case XPathNodeType.Comment:
                    break;

                case XPathNodeType.Text:
                case XPathNodeType.SignificantWhitespace:
                    this.AddEvent(compiler.CreateTextEvent());
                    break;

                default:
                    return;
            }
        }

        private void CompileLiteralAttributesAndNamespaces(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            if (input.Navigator.MoveToAttribute("use-attribute-sets", input.Atoms.XsltNamespace))
            {
                this.AddAction(compiler.CreateUseAttributeSetsAction());
                input.Navigator.MoveToParent();
            }
            compiler.InsertExcludedNamespace();
            if (input.MoveToFirstNamespace())
            {
                do
                {
                    string strA = input.Value;
                    if ((!Keywords.Compare(strA, input.Atoms.XsltNamespace) && !compiler.IsExcludedNamespace(strA)) && (!compiler.IsExtensionNamespace(strA) && !compiler.IsNamespaceAlias(strA)))
                    {
                        this.AddEvent(new NamespaceEvent(input));
                    }
                }
                while (input.MoveToNextNamespace());
                input.ToParent();
            }
            if (input.MoveToFirstAttribute())
            {
                do
                {
                    if (!Keywords.Equals(input.NamespaceURI, input.Atoms.XsltNamespace))
                    {
                        this.AddEvent(compiler.CreateBeginEvent());
                        this.AddEvents(compiler.CompileAvt(input.Value));
                        this.AddEvent(new EndEvent(XPathNodeType.Attribute));
                    }
                }
                while (input.MoveToNextAttribute());
                input.ToParent();
            }
        }

        internal void CompileNamespaceAlias(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            string localName = input.LocalName;
            string attrValue = null;
            string nsAlias = null;
            string prefix = null;
            string str5 = null;
            if (input.MoveToFirstAttribute())
            {
                do
                {
                    string namespaceURI = input.NamespaceURI;
                    string strA = input.LocalName;
                    if (Keywords.Equals(namespaceURI, input.Atoms.Empty))
                    {
                        if (Keywords.Equals(strA, input.Atoms.StylesheetPrefix))
                        {
                            prefix = input.Value;
                            attrValue = compiler.GetNsAlias(ref prefix);
                        }
                        else if (Keywords.Equals(strA, input.Atoms.ResultPrefix))
                        {
                            str5 = input.Value;
                            nsAlias = compiler.GetNsAlias(ref str5);
                        }
                        else if (!compiler.ForwardCompatibility)
                        {
                            throw XsltException.Create("Xslt_InvalidAttribute", new string[] { strA, localName });
                        }
                    }
                }
                while (input.MoveToNextAttribute());
                input.ToParent();
            }
            base.CheckRequiredAttribute(compiler, attrValue, "stylesheet-prefix");
            base.CheckRequiredAttribute(compiler, nsAlias, "result-prefix");
            base.CheckEmpty(compiler);
            compiler.AddNamespaceAlias(attrValue, new NamespaceInfo(str5, nsAlias, compiler.Stylesheetid));
        }

        protected void CompileOnceTemplate(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            if (input.NodeType == XPathNodeType.Element)
            {
                string namespaceURI = input.NamespaceURI;
                if (Keywords.Equals(namespaceURI, input.Atoms.XsltNamespace))
                {
                    compiler.PushNamespaceScope();
                    this.CompileInstruction(compiler);
                    compiler.PopScope();
                }
                else
                {
                    compiler.PushLiteralScope();
                    compiler.InsertExtensionNamespace();
                    if (compiler.IsExtensionNamespace(namespaceURI))
                    {
                        this.AddAction(compiler.CreateNewInstructionAction());
                    }
                    else
                    {
                        this.CompileLiteral(compiler);
                    }
                    compiler.PopScope();
                }
            }
            else
            {
                this.CompileLiteral(compiler);
            }
        }

        private void CompileOutput(Compiler compiler)
        {
            compiler.RootAction.Output.Compile(compiler);
        }

        internal void CompileSingleTemplate(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            string str = null;
            if (input.MoveToFirstAttribute())
            {
                do
                {
                    string namespaceURI = input.NamespaceURI;
                    string localName = input.LocalName;
                    if (Keywords.Equals(namespaceURI, input.Atoms.XsltNamespace) && Keywords.Equals(localName, input.Atoms.Version))
                    {
                        str = input.Value;
                    }
                }
                while (input.MoveToNextAttribute());
                input.ToParent();
            }
            if (str == null)
            {
                if (Keywords.Equals(input.LocalName, input.Atoms.Stylesheet) && (input.NamespaceURI == "http://www.w3.org/TR/WD-xsl"))
                {
                    throw XsltException.Create("Xslt_WdXslNamespace", new string[0]);
                }
                throw XsltException.Create("Xslt_WrongStylesheetElement", new string[0]);
            }
            compiler.AddTemplate(compiler.CreateSingleTemplateAction());
        }

        protected void CompileSpace(Compiler compiler, bool preserve)
        {
            string[] strArray = XmlConvert.SplitString(compiler.GetSingleAttribute(compiler.Input.Atoms.Elements));
            for (int i = 0; i < strArray.Length; i++)
            {
                double priority = this.NameTest(strArray[i]);
                compiler.CompiledStylesheet.AddSpace(compiler, strArray[i], priority, preserve);
            }
            base.CheckEmpty(compiler);
        }

        internal void CompileStylesheetAttributes(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            string localName = input.LocalName;
            string str2 = null;
            string o = null;
            if (input.MoveToFirstAttribute())
            {
                do
                {
                    string namespaceURI = input.NamespaceURI;
                    string strA = input.LocalName;
                    if (Keywords.Equals(namespaceURI, input.Atoms.Empty))
                    {
                        if (Keywords.Equals(strA, input.Atoms.Version))
                        {
                            o = input.Value;
                            if (1.0 > XmlConvert.ToXPathDouble(o))
                            {
                                if (!compiler.ForwardCompatibility)
                                {
                                    throw XsltException.Create("Xslt_InvalidAttrValue", new string[] { "version", o });
                                }
                            }
                            else
                            {
                                compiler.ForwardCompatibility = o != "1.0";
                            }
                        }
                        else if (Keywords.Equals(strA, input.Atoms.ExtensionElementPrefixes))
                        {
                            compiler.InsertExtensionNamespace(input.Value);
                        }
                        else if (Keywords.Equals(strA, input.Atoms.ExcludeResultPrefixes))
                        {
                            compiler.InsertExcludedNamespace(input.Value);
                        }
                        else if (!Keywords.Equals(strA, input.Atoms.Id))
                        {
                            str2 = strA;
                        }
                    }
                }
                while (input.MoveToNextAttribute());
                input.ToParent();
            }
            if (o == null)
            {
                throw XsltException.Create("Xslt_MissingAttribute", new string[] { "version" });
            }
            if ((str2 != null) && !compiler.ForwardCompatibility)
            {
                throw XsltException.Create("Xslt_InvalidAttribute", new string[] { str2, localName });
            }
        }

        protected void CompileTemplate(Compiler compiler)
        {
            do
            {
                this.CompileOnceTemplate(compiler);
            }
            while (compiler.Advance());
        }

        protected void CompileTopLevelElements(Compiler compiler)
        {
            if (compiler.Recurse())
            {
                NavigatorInput input = compiler.Input;
                bool flag = false;
                do
                {
                    switch (input.NodeType)
                    {
                        case XPathNodeType.Element:
                        {
                            string localName = input.LocalName;
                            string namespaceURI = input.NamespaceURI;
                            if (!Keywords.Equals(namespaceURI, input.Atoms.XsltNamespace))
                            {
                                if ((namespaceURI != input.Atoms.MsXsltNamespace) || (localName != input.Atoms.Script))
                                {
                                    if (Keywords.Equals(namespaceURI, input.Atoms.Empty))
                                    {
                                        throw XsltException.Create("Xslt_NullNsAtTopLevel", new string[] { input.Name });
                                    }
                                }
                                else
                                {
                                    this.AddScript(compiler);
                                }
                            }
                            else if (!Keywords.Equals(localName, input.Atoms.Import))
                            {
                                if (Keywords.Equals(localName, input.Atoms.Include))
                                {
                                    flag = true;
                                    this.CompileInclude(compiler);
                                }
                                else
                                {
                                    flag = true;
                                    compiler.PushNamespaceScope();
                                    if (Keywords.Equals(localName, input.Atoms.StripSpace))
                                    {
                                        this.CompileSpace(compiler, false);
                                    }
                                    else if (Keywords.Equals(localName, input.Atoms.PreserveSpace))
                                    {
                                        this.CompileSpace(compiler, true);
                                    }
                                    else if (Keywords.Equals(localName, input.Atoms.Output))
                                    {
                                        this.CompileOutput(compiler);
                                    }
                                    else if (Keywords.Equals(localName, input.Atoms.Key))
                                    {
                                        this.CompileKey(compiler);
                                    }
                                    else if (Keywords.Equals(localName, input.Atoms.DecimalFormat))
                                    {
                                        this.CompileDecimalFormat(compiler);
                                    }
                                    else if (Keywords.Equals(localName, input.Atoms.NamespaceAlias))
                                    {
                                        this.CompileNamespaceAlias(compiler);
                                    }
                                    else if (Keywords.Equals(localName, input.Atoms.AttributeSet))
                                    {
                                        compiler.AddAttributeSet(compiler.CreateAttributeSetAction());
                                    }
                                    else if (Keywords.Equals(localName, input.Atoms.Variable))
                                    {
                                        VariableAction action = compiler.CreateVariableAction(VariableType.GlobalVariable);
                                        if (action != null)
                                        {
                                            this.AddAction(action);
                                        }
                                    }
                                    else if (Keywords.Equals(localName, input.Atoms.Param))
                                    {
                                        VariableAction action2 = compiler.CreateVariableAction(VariableType.GlobalParameter);
                                        if (action2 != null)
                                        {
                                            this.AddAction(action2);
                                        }
                                    }
                                    else if (Keywords.Equals(localName, input.Atoms.Template))
                                    {
                                        compiler.AddTemplate(compiler.CreateTemplateAction());
                                    }
                                    else if (!compiler.ForwardCompatibility)
                                    {
                                        throw compiler.UnexpectedKeyword();
                                    }
                                    compiler.PopScope();
                                }
                            }
                            else
                            {
                                if (flag)
                                {
                                    throw XsltException.Create("Xslt_NotFirstImport", new string[0]);
                                }
                                Uri uri = compiler.ResolveUri(compiler.GetSingleAttribute(compiler.Input.Atoms.Href));
                                string href = uri.ToString();
                                if (compiler.IsCircularReference(href))
                                {
                                    throw XsltException.Create("Xslt_CircularInclude", new string[] { href });
                                }
                                compiler.CompiledStylesheet.Imports.Add(uri);
                                base.CheckEmpty(compiler);
                            }
                            break;
                        }
                        case XPathNodeType.SignificantWhitespace:
                        case XPathNodeType.Whitespace:
                        case XPathNodeType.ProcessingInstruction:
                        case XPathNodeType.Comment:
                            break;

                        default:
                            throw XsltException.Create("Xslt_InvalidContents", new string[] { "stylesheet" });
                    }
                }
                while (compiler.Advance());
                compiler.ToParent();
            }
        }

        private void EnsureCopyCodeAction()
        {
            if (this.lastCopyCodeAction == null)
            {
                CopyCodeAction action = new CopyCodeAction();
                this.AddAction(action);
                this.lastCopyCodeAction = action;
            }
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                    if ((this.containedActions == null) || (this.containedActions.Count <= 0))
                    {
                        frame.Finished();
                        return;
                    }
                    processor.PushActionFrame(frame);
                    frame.State = 1;
                    return;

                case 1:
                    frame.Finished();
                    return;
            }
        }

        internal Action GetAction(int actionIndex)
        {
            if ((this.containedActions != null) && (actionIndex < this.containedActions.Count))
            {
                return (Action) this.containedActions[actionIndex];
            }
            return null;
        }

        private double NameTest(string name)
        {
            string str;
            string str2;
            if (name == "*")
            {
                return -0.5;
            }
            int length = name.Length - 2;
            if (((0 <= length) && (name[length] == ':')) && (name[length + 1] == '*'))
            {
                if (!PrefixQName.ValidatePrefix(name.Substring(0, length)))
                {
                    throw XsltException.Create("Xslt_InvalidAttrValue", new string[] { "elements", name });
                }
                return -0.25;
            }
            PrefixQName.ParseQualifiedName(name, out str, out str2);
            return 0.0;
        }

        internal override void ReplaceNamespaceAlias(Compiler compiler)
        {
            if (this.containedActions != null)
            {
                int count = this.containedActions.Count;
                for (int i = 0; i < this.containedActions.Count; i++)
                {
                    ((Action) this.containedActions[i]).ReplaceNamespaceAlias(compiler);
                }
            }
        }
    }
}

