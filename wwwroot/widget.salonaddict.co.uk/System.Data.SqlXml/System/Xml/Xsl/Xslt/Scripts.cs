namespace System.Xml.Xsl.Xslt
{
    using Microsoft.VisualBasic;
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Xsl;
    using System.Xml.Xsl.IlGen;
    using System.Xml.Xsl.Runtime;

    internal class Scripts
    {
        private int assemblyCounter;
        private Compiler compiler;
        private static readonly string[] defaultNamespaces = new string[] { "System", "System.Collections", "System.Text", "System.Text.RegularExpressions", "System.Xml", "System.Xml.Xsl", "System.Xml.XPath" };
        private XmlExtensionFunctionTable extFuncs = new XmlExtensionFunctionTable();
        private static readonly StringComparison fileNameComparison = StringComparison.OrdinalIgnoreCase;
        private Dictionary<string, Type> nsToType = new Dictionary<string, Type>();
        private List<ScriptClass> scriptClasses = new List<ScriptClass>();
        private const string ScriptClassesNamespace = "System.Xml.Xsl.CompiledQuery";

        public Scripts(Compiler compiler)
        {
            this.compiler = compiler;
        }

        [PermissionSet(SecurityAction.Demand, Name="FullTrust")]
        private Assembly CompileAssembly(List<ScriptClass> scriptsForLang)
        {
            CodeDomProvider provider;
            CompilerResults results;
            TempFileCollection tempFiles = this.compiler.CompilerResults.TempFiles;
            CompilerErrorCollection errors = this.compiler.CompilerResults.Errors;
            ScriptClass class2 = scriptsForLang[scriptsForLang.Count - 1];
            bool flag = false;
            try
            {
                provider = class2.compilerInfo.CreateProvider();
            }
            catch (ConfigurationException exception)
            {
                errors.Add(class2.CreateCompileExceptionError(exception));
                return null;
            }
            flag = provider is VBCodeProvider;
            CodeCompileUnit[] compilationUnits = new CodeCompileUnit[scriptsForLang.Count];
            CompilerParameters options = class2.compilerInfo.CreateDefaultCompilerParameters();
            options.ReferencedAssemblies.Add(typeof(Res).Assembly.Location);
            options.ReferencedAssemblies.Add("System.dll");
            if (flag)
            {
                options.ReferencedAssemblies.Add("Microsoft.VisualBasic.dll");
            }
            bool flag2 = false;
            for (int i = 0; i < scriptsForLang.Count; i++)
            {
                ScriptClass class3 = scriptsForLang[i];
                CodeNamespace namespace2 = new CodeNamespace("System.Xml.Xsl.CompiledQuery");
                foreach (string str in defaultNamespaces)
                {
                    namespace2.Imports.Add(new CodeNamespaceImport(str));
                }
                if (flag)
                {
                    namespace2.Imports.Add(new CodeNamespaceImport("Microsoft.VisualBasic"));
                }
                foreach (string str2 in class3.nsImports)
                {
                    namespace2.Imports.Add(new CodeNamespaceImport(str2));
                }
                namespace2.Types.Add(class3.typeDecl);
                CodeCompileUnit unit = new CodeCompileUnit();
                unit.Namespaces.Add(namespace2);
                if (flag)
                {
                    unit.UserData["AllowLateBound"] = true;
                    unit.UserData["RequireVariableDeclaration"] = false;
                }
                if (i == 0)
                {
                    unit.AssemblyCustomAttributes.Add(new CodeAttributeDeclaration("System.Security.SecurityTransparentAttribute"));
                }
                compilationUnits[i] = unit;
                foreach (string str3 in class3.refAssemblies)
                {
                    options.ReferencedAssemblies.Add(str3);
                }
                flag2 |= class3.refAssembliesByHref;
            }
            XsltSettings settings = this.compiler.Settings;
            options.WarningLevel = (settings.WarningLevel >= 0) ? settings.WarningLevel : options.WarningLevel;
            options.TreatWarningsAsErrors = settings.TreatWarningsAsErrors;
            options.IncludeDebugInformation = this.compiler.IsDebug;
            string scriptAssemblyPath = this.compiler.ScriptAssemblyPath;
            if ((scriptAssemblyPath != null) && (scriptsForLang.Count < this.scriptClasses.Count))
            {
                scriptAssemblyPath = Path.ChangeExtension(scriptAssemblyPath, "." + this.GetLanguageName(class2.compilerInfo) + Path.GetExtension(scriptAssemblyPath));
            }
            options.OutputAssembly = scriptAssemblyPath;
            string tempDir = settings.TempFiles?.TempDir;
            options.TempFiles = new TempFileCollection(tempDir);
            bool flag3 = ((this.compiler.IsDebug && (scriptAssemblyPath == null)) || XmlILTrace.IsEnabled) && !settings.CheckOnly;
            options.TempFiles.KeepFiles = flag3;
            options.GenerateInMemory = (((scriptAssemblyPath == null) && !this.compiler.IsDebug) && !flag2) || settings.CheckOnly;
            try
            {
                results = provider.CompileAssemblyFromDom(options, compilationUnits);
            }
            catch (ExternalException exception2)
            {
                results = new CompilerResults(options.TempFiles);
                results.Errors.Add(class2.CreateCompileExceptionError(exception2));
            }
            if (!settings.CheckOnly)
            {
                foreach (string str6 in results.TempFiles)
                {
                    tempFiles.AddFile(str6, tempFiles.KeepFiles);
                }
            }
            foreach (CompilerError error in results.Errors)
            {
                FixErrorPosition(error, scriptsForLang);
            }
            errors.AddRange(results.Errors);
            if (!results.Errors.HasErrors)
            {
                return results.CompiledAssembly;
            }
            return null;
        }

        public void CompileScripts()
        {
            List<ScriptClass> scriptsForLang = new List<ScriptClass>();
            for (int i = 0; i < this.scriptClasses.Count; i++)
            {
                if (this.scriptClasses[i] != null)
                {
                    CompilerInfo compilerInfo = this.scriptClasses[i].compilerInfo;
                    scriptsForLang.Clear();
                    for (int j = i; j < this.scriptClasses.Count; j++)
                    {
                        if ((this.scriptClasses[j] != null) && (this.scriptClasses[j].compilerInfo == compilerInfo))
                        {
                            scriptsForLang.Add(this.scriptClasses[j]);
                            this.scriptClasses[j] = null;
                        }
                    }
                    Assembly assembly = this.CompileAssembly(scriptsForLang);
                    if (assembly != null)
                    {
                        foreach (ScriptClass class2 in scriptsForLang)
                        {
                            Type type = assembly.GetType("System.Xml.Xsl.CompiledQuery" + Type.Delimiter + class2.typeDecl.Name);
                            if (type != null)
                            {
                                this.nsToType.Add(class2.ns, type);
                            }
                        }
                    }
                }
            }
        }

        private static void FixErrorPosition(CompilerError error, List<ScriptClass> scriptsForLang)
        {
            int num;
            int num2;
            string fileName = error.FileName;
            foreach (ScriptClass class2 in scriptsForLang)
            {
                foreach (string str2 in class2.scriptFiles)
                {
                    if (fileName.Equals(str2, fileNameComparison))
                    {
                        error.FileName = str2;
                        return;
                    }
                }
            }
            ScriptClass class3 = scriptsForLang[scriptsForLang.Count - 1];
            fileName = Path.GetFileNameWithoutExtension(fileName);
            if ((((num = fileName.LastIndexOf('.')) >= 0) && int.TryParse(fileName.Substring(num + 1), NumberStyles.None, NumberFormatInfo.InvariantInfo, out num2)) && (((ulong) num2) < scriptsForLang.Count))
            {
                class3 = scriptsForLang[num2];
            }
            error.FileName = class3.endFileName;
            error.Line = class3.endLine;
            error.Column = class3.endPos;
        }

        private string GetLanguageName(CompilerInfo compilerInfo)
        {
            Regex regex = new Regex("^[0-9a-zA-Z]+$");
            foreach (string str in compilerInfo.GetLanguages())
            {
                if (regex.IsMatch(str))
                {
                    return str;
                }
            }
            int num3 = ++this.assemblyCounter;
            return ("script" + num3.ToString(CultureInfo.InvariantCulture));
        }

        public ScriptClass GetScriptClass(string ns, string language, IErrorHelper errorHelper)
        {
            CompilerInfo compilerInfo;
            try
            {
                compilerInfo = CodeDomProvider.GetCompilerInfo(language);
            }
            catch (ConfigurationException)
            {
                errorHelper.ReportError("Xslt_ScriptInvalidLanguage", new string[] { language });
                return null;
            }
            foreach (ScriptClass class2 in this.scriptClasses)
            {
                if (ns == class2.ns)
                {
                    if (compilerInfo != class2.compilerInfo)
                    {
                        errorHelper.ReportError("Xslt_ScriptMixedLanguages", new string[] { ns });
                        return null;
                    }
                    return class2;
                }
            }
            ScriptClass item = new ScriptClass(ns, compilerInfo) {
                typeDecl = { TypeAttributes = TypeAttributes.Public }
            };
            this.scriptClasses.Add(item);
            return item;
        }

        public XmlExtensionFunction ResolveFunction(string name, string ns, int numArgs, IErrorHelper errorHelper)
        {
            Type type;
            if (this.nsToType.TryGetValue(ns, out type))
            {
                try
                {
                    return this.extFuncs.Bind(name, ns, numArgs, type, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                }
                catch (XslTransformException exception)
                {
                    errorHelper.ReportError(exception.Message, new string[0]);
                }
            }
            return null;
        }

        public Dictionary<string, Type> ScriptClasses =>
            this.nsToType;
    }
}

