namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.IlGen;
    using System.Xml.Xsl.Qil;

    internal class Compiler
    {
        public Dictionary<QilName, VarPar> AllGlobalVarPars = new Dictionary<QilName, VarPar>();
        public List<ProtoTemplate> AllTemplates = new List<ProtoTemplate>();
        public Dictionary<QilName, List<QilFunction>> ApplyTemplatesFunctions = new Dictionary<QilName, List<QilFunction>>();
        public Dictionary<QilName, AttributeSet> AttributeSets = new Dictionary<QilName, AttributeSet>();
        public System.CodeDom.Compiler.CompilerResults CompilerResults;
        public int CurrentPrecedence;
        public System.Xml.Xsl.Xslt.DecimalFormats DecimalFormats = new System.Xml.Xsl.Xslt.DecimalFormats();
        public List<VarPar> ExternalPars = new List<VarPar>();
        public List<VarPar> GlobalVars = new List<VarPar>();
        public bool IsDebug;
        public System.Xml.Xsl.Xslt.Keys Keys = new System.Xml.Xsl.Xslt.Keys();
        public Dictionary<QilName, XslFlags> ModeFlags = new Dictionary<QilName, XslFlags>();
        public Dictionary<QilName, Template> NamedTemplates = new Dictionary<QilName, Template>();
        public Dictionary<string, NsAlias> NsAliases = new Dictionary<string, NsAlias>();
        public System.Xml.Xsl.Xslt.Output Output = new System.Xml.Xsl.Xslt.Output();
        public readonly string PhantomNCName = "error";
        private int phantomNsCounter;
        public Stylesheet PrincipalStylesheet;
        private int savedErrorCount = -1;
        public string ScriptAssemblyPath;
        public System.Xml.Xsl.Xslt.Scripts Scripts;
        public XsltSettings Settings;
        public XslNode StartApplyTemplates;
        public List<WhitespaceRule> WhitespaceRules = new List<WhitespaceRule>();

        public Compiler(XsltSettings settings, bool debug, string scriptAssemblyPath)
        {
            TempFileCollection tempFiles = settings.TempFiles ?? new TempFileCollection();
            if (XmlILTrace.IsEnabled)
            {
                tempFiles.KeepFiles = true;
            }
            this.Settings = settings;
            this.IsDebug = settings.IncludeDebugInformation | debug;
            this.ScriptAssemblyPath = scriptAssemblyPath;
            this.CompilerResults = new System.CodeDom.Compiler.CompilerResults(tempFiles);
            this.Scripts = new System.Xml.Xsl.Xslt.Scripts(this);
        }

        public void ApplyNsAliases(ref string prefix, ref string nsUri)
        {
            NsAlias alias;
            if (this.NsAliases.TryGetValue(nsUri, out alias))
            {
                nsUri = alias.ResultNsUri;
                prefix = alias.ResultPrefix;
            }
        }

        public System.CodeDom.Compiler.CompilerResults Compile(object stylesheet, XmlResolver xmlResolver, out QilExpression qil)
        {
            new XsltLoader().Load(this, stylesheet, xmlResolver);
            qil = QilGenerator.CompileStylesheet(this);
            return this.CompilerResults;
        }

        public static string ConstructQName(string prefix, string localName)
        {
            if (prefix.Length == 0)
            {
                return localName;
            }
            return (prefix + ':' + localName);
        }

        public CompilerError CreateError(ISourceLineInfo lineInfo, string res, params string[] args) => 
            new CompilerError(lineInfo.Uri, lineInfo.StartLine, lineInfo.StartPos, string.Empty, XslTransformException.CreateMessage(res, args));

        public string CreatePhantomNamespace() => 
            ("\0namespace" + this.phantomNsCounter++);

        public Stylesheet CreateStylesheet()
        {
            Stylesheet stylesheet = new Stylesheet(this, this.CurrentPrecedence);
            if (this.CurrentPrecedence-- == 0)
            {
                this.PrincipalStylesheet = stylesheet;
            }
            return stylesheet;
        }

        public void EnterForwardsCompatible()
        {
            this.savedErrorCount = this.ErrorCount;
        }

        public bool ExitForwardsCompatible(bool fwdCompat)
        {
            if (fwdCompat && (this.ErrorCount > this.savedErrorCount))
            {
                this.ErrorCount = this.savedErrorCount;
                return false;
            }
            return true;
        }

        public bool IsPhantomName(QilName qname)
        {
            string namespaceUri = qname.NamespaceUri;
            return ((namespaceUri.Length > 0) && (namespaceUri[0] == '\0'));
        }

        public bool IsPhantomNamespace(string namespaceName) => 
            ((namespaceName.Length > 0) && (namespaceName[0] == '\0'));

        private void MergeAttributeSets(Stylesheet sheet)
        {
            foreach (QilName name in sheet.AttributeSets.Keys)
            {
                AttributeSet set;
                if (!this.AttributeSets.TryGetValue(name, out set))
                {
                    this.AttributeSets[name] = sheet.AttributeSets[name];
                }
                else
                {
                    set.MergeContent(sheet.AttributeSets[name]);
                }
            }
            sheet.AttributeSets = null;
        }

        private void MergeGlobalVarPars(Stylesheet sheet)
        {
            foreach (VarPar par in sheet.GlobalVarPars)
            {
                if (!this.AllGlobalVarPars.ContainsKey(par.Name))
                {
                    if (par.NodeType == XslNodeType.Variable)
                    {
                        this.GlobalVars.Add(par);
                    }
                    else
                    {
                        this.ExternalPars.Add(par);
                    }
                    this.AllGlobalVarPars[par.Name] = par;
                }
            }
            sheet.GlobalVarPars = null;
        }

        private void MergeWhitespaceRules(Stylesheet sheet)
        {
            for (int i = 0; i <= 2; i++)
            {
                sheet.WhitespaceRules[i].Reverse();
                this.WhitespaceRules.AddRange(sheet.WhitespaceRules[i]);
            }
            sheet.WhitespaceRules = null;
        }

        public void MergeWithStylesheet(Stylesheet sheet)
        {
            this.MergeWhitespaceRules(sheet);
            this.MergeAttributeSets(sheet);
            this.MergeGlobalVarPars(sheet);
        }

        public bool ParseNameTest(string nameTest, out string prefix, out string localName, IErrorHelper errorHelper)
        {
            try
            {
                ValidateNames.ParseNameTestThrow(nameTest, out prefix, out localName);
                return true;
            }
            catch (XmlException exception)
            {
                errorHelper.ReportError(exception.Message, null);
                prefix = this.PhantomNCName;
                localName = this.PhantomNCName;
                return false;
            }
        }

        public bool ParseQName(string qname, out string prefix, out string localName, IErrorHelper errorHelper)
        {
            try
            {
                ValidateNames.ParseQNameThrow(qname, out prefix, out localName);
                return true;
            }
            catch (XmlException exception)
            {
                errorHelper.ReportError(exception.Message, null);
                prefix = this.PhantomNCName;
                localName = this.PhantomNCName;
                return false;
            }
        }

        public void ReportError(ISourceLineInfo lineInfo, string res, params string[] args)
        {
            CompilerError error = this.CreateError(lineInfo, res, args);
            this.CompilerResults.Errors.Add(error);
        }

        public void ReportWarning(ISourceLineInfo lineInfo, string res, params string[] args)
        {
            int num = 1;
            if ((0 > this.Settings.WarningLevel) || (this.Settings.WarningLevel >= num))
            {
                CompilerError error = this.CreateError(lineInfo, res, args);
                if (this.Settings.TreatWarningsAsErrors)
                {
                    error.ErrorText = XslTransformException.CreateMessage("Xslt_WarningAsError", new string[] { error.ErrorText });
                    this.CompilerResults.Errors.Add(error);
                }
                else
                {
                    error.IsWarning = true;
                    this.CompilerResults.Errors.Add(error);
                }
            }
        }

        public bool SetNsAlias(string ssheetNsUri, string resultNsUri, string resultPrefix, int importPrecedence)
        {
            NsAlias alias;
            if (this.NsAliases.TryGetValue(ssheetNsUri, out alias) && ((importPrecedence < alias.ImportPrecedence) || (resultNsUri == alias.ResultNsUri)))
            {
                return false;
            }
            this.NsAliases[ssheetNsUri] = new NsAlias(resultNsUri, resultPrefix, importPrecedence);
            return (alias != null);
        }

        public void ValidatePiName(string name, IErrorHelper errorHelper)
        {
            try
            {
                ValidateNames.ValidateNameThrow(string.Empty, name, string.Empty, XPathNodeType.ProcessingInstruction, ValidateNames.Flags.AllExceptPrefixMapping);
            }
            catch (XmlException exception)
            {
                errorHelper.ReportError(exception.Message, null);
            }
        }

        private int ErrorCount
        {
            get => 
                this.CompilerResults.Errors.Count;
            set
            {
                for (int i = this.ErrorCount - 1; i >= value; i--)
                {
                    this.CompilerResults.Errors.RemoveAt(i);
                }
            }
        }
    }
}

