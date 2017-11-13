﻿namespace System.ServiceModel.Activation
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.RegularExpressions;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal class ServiceParser
    {
        private ServiceBuildProvider buildProvider;
        private System.Web.Compilation.CompilerType compilerType;
        private const string DefaultDirectiveName = "ServiceHost";
        private const string Delimiter = "|";
        private static readonly SimpleDirectiveRegex directiveRegex = new SimpleDirectiveRegex();
        private const string FactoryAttributeName = "Factory";
        private string factoryAttributeValue;
        private bool foundMainDirective;
        private int lineNumber;
        private HybridDictionary linkedAssemblies;
        private static char[] newlineChars = new char[] { '\r', '\n' };
        private ICollection referencedAssemblies;
        private const string ServiceAttributeName = "Service";
        private string serviceAttributeValue;
        private string serviceText;
        private HybridDictionary sourceDependencies;
        private string sourceString;
        private int startColumn;
        private string virtualPath;

        private ServiceParser(string serviceText)
        {
            this.factoryAttributeValue = string.Empty;
            this.serviceAttributeValue = string.Empty;
            this.serviceText = serviceText;
            this.buildProvider = new ServiceBuildProvider();
        }

        internal ServiceParser(string virtualPath, ServiceBuildProvider buildProvider)
        {
            this.factoryAttributeValue = string.Empty;
            this.serviceAttributeValue = string.Empty;
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.WebHostCompilation, new StringTraceRecord("VirtualPath", virtualPath), this, null);
            }
            this.virtualPath = virtualPath;
            this.buildProvider = buildProvider;
        }

        private void AddAssemblyDependency(Assembly assembly)
        {
            if (this.linkedAssemblies == null)
            {
                this.linkedAssemblies = new HybridDictionary(false);
            }
            this.linkedAssemblies.Add(assembly, null);
        }

        private void AddAssemblyDependency(string assemblyName)
        {
            Assembly assembly = Assembly.Load(assemblyName);
            this.AddAssemblyDependency(assembly);
        }

        private void AddSourceDependency(string fileName)
        {
            if (this.sourceDependencies == null)
            {
                this.sourceDependencies = new HybridDictionary(true);
            }
            this.sourceDependencies.Add(fileName, fileName);
        }

        private Exception CreateParseException(Exception innerException, string sourceCode) => 
            this.CreateParseException(innerException.Message, innerException, sourceCode);

        private Exception CreateParseException(string message, string sourceCode) => 
            this.CreateParseException(message, null, sourceCode);

        private Exception CreateParseException(string message, Exception innerException, string sourceCode) => 
            new HttpParseException(message, innerException, this.virtualPath, sourceCode, this.lineNumber);

        internal string CreateParseString(Assembly compiledAssembly)
        {
            Type compiledType = this.GetCompiledType(compiledAssembly);
            string assemblyQualifiedName = string.Empty;
            if (compiledType != null)
            {
                assemblyQualifiedName = compiledType.AssemblyQualifiedName;
            }
            StringBuilder builder = new StringBuilder();
            if (compiledAssembly != null)
            {
                builder.Append("|");
                builder.Append(compiledAssembly.FullName);
            }
            if (this.referencedAssemblies != null)
            {
                foreach (Assembly assembly in this.referencedAssemblies)
                {
                    builder.Append("|");
                    builder.Append(assembly.FullName);
                }
            }
            if (this.AssemblyDependencies != null)
            {
                foreach (Assembly assembly2 in this.AssemblyDependencies)
                {
                    builder.Append("|");
                    builder.Append(assembly2.FullName);
                }
            }
            return (this.virtualPath + "|" + assemblyQualifiedName + "|" + this.serviceAttributeValue + builder.ToString());
        }

        internal CodeCompileUnit GetCodeModel()
        {
            if ((this.sourceString == null) || (this.sourceString.Length == 0))
            {
                return null;
            }
            CodeSnippetCompileUnit unit = new CodeSnippetCompileUnit(this.sourceString);
            string fileName = HostingEnvironmentWrapper.MapPath(this.virtualPath);
            unit.LinePragma = new CodeLinePragma(fileName, this.lineNumber);
            return unit;
        }

        private Type GetCompiledType(Assembly compiledAssembly)
        {
            if (string.IsNullOrEmpty(this.factoryAttributeValue))
            {
                return null;
            }
            Type type = null;
            if (this.HasInlineCode && (compiledAssembly != null))
            {
                type = compiledAssembly.GetType(this.factoryAttributeValue);
            }
            if (type == null)
            {
                type = this.GetType(this.factoryAttributeValue);
            }
            return type;
        }

        internal IDictionary GetLinePragmasTable()
        {
            LinePragmaCodeInfo info = new LinePragmaCodeInfo(this.lineNumber, this.startColumn, 1, -1, false);
            return new Hashtable { [this.lineNumber] = info };
        }

        private Type GetType(string typeName)
        {
            Type type;
            if (ServiceParserUtilities.TypeNameIncludesAssembly(typeName))
            {
                try
                {
                    type = Type.GetType(typeName, true);
                }
                catch (ArgumentException exception)
                {
                    Exception exception2 = this.CreateParseException(exception, this.sourceString);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpCompileException(exception2.Message, exception2));
                }
                catch (TargetInvocationException exception3)
                {
                    Exception exception4 = this.CreateParseException(exception3, this.sourceString);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpCompileException(exception4.Message, exception4));
                }
                catch (TypeLoadException exception5)
                {
                    Exception exception6 = this.CreateParseException(System.ServiceModel.SR.GetString("Hosting_BuildProviderCouldNotCreateType", new object[] { typeName }), exception5, this.sourceString);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpCompileException(exception6.Message, exception6));
                }
                return type;
            }
            try
            {
                type = ServiceParserUtilities.GetTypeFromAssemblies(this.referencedAssemblies, typeName, false);
                if (type != null)
                {
                    return type;
                }
                type = ServiceParserUtilities.GetTypeFromAssemblies(this.AssemblyDependencies, typeName, false);
                if (type != null)
                {
                    return type;
                }
            }
            catch (HttpException exception7)
            {
                Exception exception8 = this.CreateParseException(System.ServiceModel.SR.GetString("Hosting_BuildProviderCouldNotCreateType", new object[] { typeName }), exception7, this.sourceString);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpCompileException(exception8.Message, exception8));
            }
            Exception innerException = this.CreateParseException(System.ServiceModel.SR.GetString("Hosting_BuildProviderCouldNotCreateType", new object[] { typeName }), this.sourceString);
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpCompileException(innerException.Message, innerException));
        }

        private void ImportSourceFile(string path)
        {
            string fileName = VirtualPathUtility.Combine(VirtualPathUtility.GetDirectory(this.virtualPath), path);
            this.AddSourceDependency(fileName);
            Assembly compiledAssembly = BuildManager.GetCompiledAssembly(fileName);
            this.AddAssemblyDependency(compiledAssembly);
        }

        internal void Parse(ICollection referencedAssemblies)
        {
            if (referencedAssemblies == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("referencedAssemblies"));
            }
            this.referencedAssemblies = referencedAssemblies;
            this.AddSourceDependency(this.virtualPath);
            using (TextReader reader = this.buildProvider.OpenReaderInternal())
            {
                this.serviceText = reader.ReadToEnd();
                this.ParseString();
            }
        }

        internal static IDictionary<string, string> ParseServiceDirective(string serviceText)
        {
            ServiceParser parser = new ServiceParser(serviceText);
            parser.ParseString();
            IDictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(parser.factoryAttributeValue))
            {
                dictionary.Add("Factory", parser.factoryAttributeValue);
            }
            if (!string.IsNullOrEmpty(parser.serviceAttributeValue))
            {
                dictionary.Add("Service", parser.serviceAttributeValue);
            }
            return dictionary;
        }

        private void ParseString()
        {
            try
            {
                Match match;
                int startat = 0;
                this.lineNumber = 1;
                if (this.serviceText.IndexOf('>') == -1)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderDirectiveEndBracketMissing", new object[] { "ServiceHost" })));
                }
            Label_0046:
                match = directiveRegex.Match(this.serviceText, startat);
                if (match.Success)
                {
                    this.lineNumber += ServiceParserUtilities.LineCount(this.serviceText, startat, match.Index);
                    startat = match.Index;
                    IDictionary attribs = CollectionsUtil.CreateCaseInsensitiveSortedList();
                    string directiveName = this.ProcessAttributes(match, attribs);
                    this.ProcessDirective(directiveName, attribs);
                    this.lineNumber += ServiceParserUtilities.LineCount(this.serviceText, startat, match.Index + match.Length);
                    startat = match.Index + match.Length;
                    int num2 = this.serviceText.LastIndexOfAny(newlineChars, startat - 1);
                    this.startColumn = startat - num2;
                    goto Label_0046;
                }
                if (!this.foundMainDirective)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderDirectiveMissing", new object[] { "ServiceHost" })));
                }
                string s = this.serviceText.Substring(startat);
                if (!ServiceParserUtilities.IsWhiteSpaceString(s))
                {
                    this.sourceString = s;
                }
            }
            catch (HttpException exception)
            {
                Exception innerException = this.CreateParseException(exception, this.serviceText);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpCompileException(innerException.Message, innerException));
            }
        }

        private string ProcessAttributes(Match match, IDictionary attribs)
        {
            string str = string.Empty;
            CaptureCollection captures = match.Groups["attrname"].Captures;
            CaptureCollection captures2 = match.Groups["attrval"].Captures;
            CaptureCollection captures3 = match.Groups["equal"].Captures;
            for (int i = 0; i < captures.Count; i++)
            {
                string key = captures[i].ToString();
                string str3 = captures2[i].ToString();
                bool flag = captures3[i].ToString().Length > 0;
                if (key != null)
                {
                    if (!flag && (i == 0))
                    {
                        str = key;
                    }
                    else
                    {
                        try
                        {
                            if (attribs != null)
                            {
                                attribs.Add(key, str3);
                            }
                        }
                        catch (ArgumentException)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderDuplicateAttribute", new object[] { key })));
                        }
                    }
                }
            }
            return str;
        }

        private void ProcessCompilationParams(IDictionary directive, CompilerParameters compilParams)
        {
            bool val = false;
            if (ServiceParserUtilities.GetAndRemoveBooleanAttribute(directive, "debug", ref val))
            {
                compilParams.IncludeDebugInformation = val;
            }
            int num = 0;
            if (ServiceParserUtilities.GetAndRemoveNonNegativeIntegerAttribute(directive, "warninglevel", ref num))
            {
                compilParams.WarningLevel = num;
                if (num > 0)
                {
                    compilParams.TreatWarningsAsErrors = true;
                }
            }
            string andRemoveNonEmptyAttribute = ServiceParserUtilities.GetAndRemoveNonEmptyAttribute(directive, "compileroptions");
            if (andRemoveNonEmptyAttribute != null)
            {
                compilParams.CompilerOptions = andRemoveNonEmptyAttribute;
            }
        }

        private void ProcessDirective(string directiveName, IDictionary directive)
        {
            if (directiveName.Length == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderDirectiveNameMissing")));
            }
            if (string.Compare(directiveName, "ServiceHost", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (this.foundMainDirective)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderDuplicateDirective", new object[] { "ServiceHost" })));
                }
                this.foundMainDirective = true;
                directive.Remove("codebehind");
                string andRemoveNonEmptyAttribute = ServiceParserUtilities.GetAndRemoveNonEmptyAttribute(directive, "language");
                if (andRemoveNonEmptyAttribute != null)
                {
                    this.compilerType = this.buildProvider.GetDefaultCompilerTypeForLanguageInternal(andRemoveNonEmptyAttribute);
                }
                else
                {
                    this.compilerType = this.buildProvider.GetDefaultCompilerTypeInternal();
                }
                if (directive.Contains("Factory"))
                {
                    this.factoryAttributeValue = ServiceParserUtilities.GetAndRemoveNonEmptyAttribute(directive, "Factory");
                    this.serviceAttributeValue = ServiceParserUtilities.GetAndRemoveNonEmptyAttribute(directive, "Service");
                }
                else
                {
                    if (!directive.Contains("Service"))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderMainAttributeMissing")));
                    }
                    this.serviceAttributeValue = ServiceParserUtilities.GetAndRemoveNonEmptyAttribute(directive, "Service");
                }
                this.ProcessCompilationParams(directive, this.compilerType.CompilerParameters);
            }
            else
            {
                if (string.Compare(directiveName, "assembly", StringComparison.OrdinalIgnoreCase) != 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderUnknownDirective", new object[] { directiveName })));
                }
                if (directive.Contains("name") && directive.Contains("src"))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderMutualExclusiveAttributes", new object[] { "src", "name" })));
                }
                if (!directive.Contains("name"))
                {
                    if (!directive.Contains("src"))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderRequiredAttributesMissing", new object[] { "src", "name" })));
                    }
                    string path = ServiceParserUtilities.GetAndRemoveNonEmptyAttribute(directive, "src");
                    if (path == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderAttributeEmpty", new object[] { "src" })));
                    }
                    this.ImportSourceFile(path);
                }
                else
                {
                    string assemblyName = ServiceParserUtilities.GetAndRemoveNonEmptyAttribute(directive, "name");
                    if (assemblyName == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderAttributeEmpty", new object[] { "name" })));
                    }
                    this.AddAssemblyDependency(assemblyName);
                }
            }
            if (directive.Count > 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderUnknownAttribute", new object[] { ServiceParserUtilities.FirstDictionaryKey(directive) })));
            }
        }

        internal ICollection AssemblyDependencies =>
            this.linkedAssemblies?.Keys;

        internal System.Web.Compilation.CompilerType CompilerType =>
            this.compilerType;

        internal bool HasInlineCode =>
            (this.sourceString != null);

        internal ICollection SourceDependencies =>
            this.sourceDependencies?.Keys;

        private static class ServiceParserUtilities
        {
            internal static string FirstDictionaryKey(IDictionary dictionary)
            {
                IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
                enumerator.MoveNext();
                return (string) enumerator.Key;
            }

            private static string GetAndRemove(IDictionary dictionary, string key)
            {
                string str = (string) dictionary[key];
                if (str != null)
                {
                    dictionary.Remove(key);
                    return str.Trim();
                }
                return string.Empty;
            }

            internal static bool GetAndRemoveBooleanAttribute(IDictionary directives, string key, ref bool val)
            {
                string andRemove = GetAndRemove(directives, key);
                if (andRemove.Length == 0)
                {
                    return false;
                }
                try
                {
                    val = bool.Parse(andRemove);
                }
                catch (FormatException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderInvalidValueForBooleanAttribute", new object[] { andRemove, key })));
                }
                return true;
            }

            internal static string GetAndRemoveNonEmptyAttribute(IDictionary directives, string key) => 
                GetAndRemoveNonEmptyAttribute(directives, key, false);

            internal static string GetAndRemoveNonEmptyAttribute(IDictionary directives, string key, bool required)
            {
                string andRemove = GetAndRemove(directives, key);
                if (andRemove.Length != 0)
                {
                    return andRemove;
                }
                if (required)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderAttributeMissing", new object[] { key })));
                }
                return null;
            }

            internal static bool GetAndRemoveNonNegativeIntegerAttribute(IDictionary directives, string key, ref int val)
            {
                string andRemove = GetAndRemove(directives, key);
                if (andRemove.Length == 0)
                {
                    return false;
                }
                val = GetNonNegativeIntegerAttribute(key, andRemove);
                return true;
            }

            private static int GetNonNegativeIntegerAttribute(string name, string value)
            {
                int num;
                try
                {
                    num = int.Parse(value, CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderInvalidValueForNonNegativeIntegerAttribute", new object[] { value, name })));
                }
                if (num < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderInvalidValueForNonNegativeIntegerAttribute", new object[] { value, name })));
                }
                return num;
            }

            internal static Type GetTypeFromAssemblies(ICollection assemblies, string typeName, bool ignoreCase)
            {
                if (assemblies == null)
                {
                    return null;
                }
                Type type = null;
                foreach (Assembly assembly in assemblies)
                {
                    Type type2 = assembly.GetType(typeName, false, ignoreCase);
                    if (type2 != null)
                    {
                        if ((type != null) && (type2 != type))
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(System.ServiceModel.SR.GetString("Hosting_BuildProviderAmbiguousType", new object[] { typeName, type.Assembly.FullName, type2.Assembly.FullName })));
                        }
                        type = type2;
                    }
                }
                return type;
            }

            internal static bool IsWhiteSpaceString(string s) => 
                (s.Trim().Length == 0);

            internal static int LineCount(string text, int offset, int newoffset)
            {
                int num = 0;
                while (offset < newoffset)
                {
                    if ((text[offset] == '\r') || ((text[offset] == '\n') && ((offset == 0) || (text[offset - 1] != '\r'))))
                    {
                        num++;
                    }
                    offset++;
                }
                return num;
            }

            internal static bool TypeNameIncludesAssembly(string typeName) => 
                (typeName.IndexOf(",", StringComparison.Ordinal) >= 0);
        }
    }
}

