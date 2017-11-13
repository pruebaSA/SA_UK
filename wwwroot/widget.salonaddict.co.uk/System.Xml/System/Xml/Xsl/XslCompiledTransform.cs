namespace System.Xml.Xsl
{
    using System;
    using System.CodeDom.Compiler;
    using System.IO;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using System.Xml;
    using System.Xml.XmlConfiguration;
    using System.Xml.XPath;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.Runtime;
    using System.Xml.Xsl.Xslt;

    public sealed class XslCompiledTransform
    {
        private XmlILCommand command;
        private CompilerResults compilerResults;
        private bool enableDebug;
        private static ConstructorInfo GeneratedCodeCtor;
        private static readonly PermissionSet MemberAccessPermissionSet = new PermissionSet(PermissionState.None);
        private XmlWriterSettings outputSettings;
        private QilExpression qil;
        private const string Version = "2.0.0.0";

        static XslCompiledTransform()
        {
            MemberAccessPermissionSet.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
        }

        public XslCompiledTransform()
        {
        }

        public XslCompiledTransform(bool enableDebug)
        {
            this.enableDebug = enableDebug;
        }

        private void CheckCommand()
        {
            if (this.command == null)
            {
                throw new InvalidOperationException(Res.GetString("Xslt_NoStylesheetLoaded"));
            }
        }

        private void CheckInput(object input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
        }

        private void CompileQilToMsil(XsltSettings settings)
        {
            this.command = new XmlILGenerator().Generate(this.qil, null);
            this.outputSettings = this.command.StaticData.DefaultWriterSettings;
            this.qil = null;
        }

        public static CompilerErrorCollection CompileToType(XmlReader stylesheet, XsltSettings settings, XmlResolver stylesheetResolver, bool debug, TypeBuilder typeBuilder, string scriptAssemblyPath)
        {
            QilExpression expression;
            if (stylesheet == null)
            {
                throw new ArgumentNullException("stylesheet");
            }
            if (typeBuilder == null)
            {
                throw new ArgumentNullException("typeBuilder");
            }
            if (settings == null)
            {
                settings = XsltSettings.Default;
            }
            if (settings.EnableScript && (scriptAssemblyPath == null))
            {
                throw new ArgumentNullException("scriptAssemblyPath");
            }
            if (scriptAssemblyPath != null)
            {
                scriptAssemblyPath = Path.GetFullPath(scriptAssemblyPath);
            }
            CompilerErrorCollection errors = new Compiler(settings, debug, scriptAssemblyPath).Compile(stylesheet, stylesheetResolver, out expression).Errors;
            if (!errors.HasErrors)
            {
                if (GeneratedCodeCtor == null)
                {
                    GeneratedCodeCtor = typeof(GeneratedCodeAttribute).GetConstructor(new Type[] { typeof(string), typeof(string) });
                }
                typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(GeneratedCodeCtor, new object[] { typeof(XslCompiledTransform).FullName, "2.0.0.0" }));
                new XmlILGenerator().Generate(expression, typeBuilder);
            }
            return errors;
        }

        private void CompileXsltToQil(object stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
        {
            this.compilerResults = new Compiler(settings, this.enableDebug, null).Compile(stylesheet, stylesheetResolver, out this.qil);
        }

        private XmlReader CreateReader(string inputUri)
        {
            if (inputUri == null)
            {
                throw new ArgumentNullException("inputUri");
            }
            return XmlReader.Create(inputUri);
        }

        private CompilerError GetFirstError()
        {
            foreach (CompilerError error in this.compilerResults.Errors)
            {
                if (!error.IsWarning)
                {
                    return error;
                }
            }
            return null;
        }

        public void Load(string stylesheetUri)
        {
            this.Reset();
            if (stylesheetUri == null)
            {
                throw new ArgumentNullException("stylesheetUri");
            }
            this.LoadInternal(stylesheetUri, XsltSettings.Default, XsltConfigSection.CreateDefaultResolver());
        }

        public void Load(Type compiledStylesheet)
        {
            this.Reset();
            if (compiledStylesheet == null)
            {
                throw new ArgumentNullException("compiledStylesheet");
            }
            object[] customAttributes = compiledStylesheet.GetCustomAttributes(typeof(GeneratedCodeAttribute), false);
            GeneratedCodeAttribute attribute = (customAttributes.Length > 0) ? ((GeneratedCodeAttribute) customAttributes[0]) : null;
            if (((attribute != null) && (attribute.Tool == typeof(XslCompiledTransform).FullName)) && (attribute.Version == "2.0.0.0"))
            {
                FieldInfo field = compiledStylesheet.GetField("staticData", BindingFlags.NonPublic | BindingFlags.Static);
                FieldInfo info2 = compiledStylesheet.GetField("ebTypes", BindingFlags.NonPublic | BindingFlags.Static);
                if ((field != null) && (info2 != null))
                {
                    if (XsltConfigSection.EnableMemberAccessForXslCompiledTransform)
                    {
                        new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Assert();
                    }
                    object obj2 = field.GetValue(null);
                    byte[] data = obj2 as byte[];
                    if (data != null)
                    {
                        lock (data)
                        {
                            obj2 = field.GetValue(null);
                            if (obj2 == data)
                            {
                                MethodInfo method = compiledStylesheet.GetMethod("Execute", BindingFlags.NonPublic | BindingFlags.Static);
                                obj2 = new XmlILCommand((ExecuteDelegate) Delegate.CreateDelegate(typeof(ExecuteDelegate), method), new XmlQueryStaticData(data, (Type[]) info2.GetValue(null)));
                                Thread.MemoryBarrier();
                                field.SetValue(null, obj2);
                            }
                        }
                    }
                    this.command = obj2 as XmlILCommand;
                }
            }
            if (this.command == null)
            {
                throw new ArgumentException(Res.GetString("Xslt_NotCompiledStylesheet", new object[] { compiledStylesheet.FullName }), "compiledStylesheet");
            }
            this.outputSettings = this.command.StaticData.DefaultWriterSettings;
        }

        public void Load(XmlReader stylesheet)
        {
            this.Reset();
            this.LoadInternal(stylesheet, XsltSettings.Default, XsltConfigSection.CreateDefaultResolver());
        }

        public void Load(IXPathNavigable stylesheet)
        {
            this.Reset();
            this.LoadInternal(stylesheet, XsltSettings.Default, XsltConfigSection.CreateDefaultResolver());
        }

        public void Load(MethodInfo executeMethod, byte[] queryData, Type[] earlyBoundTypes)
        {
            this.Reset();
            if (executeMethod == null)
            {
                throw new ArgumentNullException("executeMethod");
            }
            if (queryData == null)
            {
                throw new ArgumentNullException("queryData");
            }
            if ((!XsltConfigSection.EnableMemberAccessForXslCompiledTransform && (executeMethod.DeclaringType != null)) && !executeMethod.DeclaringType.IsVisible)
            {
                new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Demand();
            }
            DynamicMethod method = executeMethod as DynamicMethod;
            Delegate delegate2 = (method != null) ? method.CreateDelegate(typeof(ExecuteDelegate)) : Delegate.CreateDelegate(typeof(ExecuteDelegate), executeMethod);
            this.command = new XmlILCommand((ExecuteDelegate) delegate2, new XmlQueryStaticData(queryData, earlyBoundTypes));
            this.outputSettings = this.command.StaticData.DefaultWriterSettings;
        }

        public void Load(string stylesheetUri, XsltSettings settings, XmlResolver stylesheetResolver)
        {
            this.Reset();
            if (stylesheetUri == null)
            {
                throw new ArgumentNullException("stylesheetUri");
            }
            this.LoadInternal(stylesheetUri, settings, stylesheetResolver);
        }

        public void Load(XmlReader stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
        {
            this.Reset();
            this.LoadInternal(stylesheet, settings, stylesheetResolver);
        }

        public void Load(IXPathNavigable stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
        {
            this.Reset();
            this.LoadInternal(stylesheet, settings, stylesheetResolver);
        }

        private CompilerResults LoadInternal(object stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
        {
            if (stylesheet == null)
            {
                throw new ArgumentNullException("stylesheet");
            }
            if (settings == null)
            {
                settings = XsltSettings.Default;
            }
            this.CompileXsltToQil(stylesheet, settings, stylesheetResolver);
            CompilerError firstError = this.GetFirstError();
            if (firstError != null)
            {
                throw new XslLoadException(firstError);
            }
            if (!settings.CheckOnly)
            {
                this.CompileQilToMsil(settings);
            }
            return this.compilerResults;
        }

        internal static void PrintQil(object qil, XmlWriter xw, bool printComments, bool printTypes, bool printLineInfo)
        {
            QilExpression node = (QilExpression) qil;
            QilXmlWriter.Options none = QilXmlWriter.Options.None;
            if (printComments)
            {
                none |= QilXmlWriter.Options.Annotations;
            }
            if (printTypes)
            {
                none |= QilXmlWriter.Options.TypeInfo;
            }
            if (printLineInfo)
            {
                none |= QilXmlWriter.Options.LineInfo;
            }
            new QilXmlWriter(xw, none).ToXml(node);
            xw.Flush();
        }

        private void Reset()
        {
            this.compilerResults = null;
            this.outputSettings = null;
            this.qil = null;
            this.command = null;
        }

        private QilExpression TestCompile(object stylesheet, XsltSettings settings, XmlResolver stylesheetResolver)
        {
            this.Reset();
            this.CompileXsltToQil(stylesheet, settings, stylesheetResolver);
            return this.qil;
        }

        private void TestGenerate(XsltSettings settings)
        {
            this.CompileQilToMsil(settings);
        }

        public void Transform(string inputUri, string resultsFile)
        {
            this.CheckCommand();
            using (XmlReader reader = this.CreateReader(inputUri))
            {
                if (resultsFile == null)
                {
                    throw new ArgumentNullException("resultsFile");
                }
                using (FileStream stream = new FileStream(resultsFile, FileMode.Create, FileAccess.Write))
                {
                    this.command.Execute(reader, XsltConfigSection.CreateDefaultResolver(), null, (Stream) stream);
                }
            }
        }

        public void Transform(string inputUri, XmlWriter results)
        {
            this.CheckCommand();
            using (XmlReader reader = this.CreateReader(inputUri))
            {
                this.command.Execute(reader, XsltConfigSection.CreateDefaultResolver(), null, results);
            }
        }

        public void Transform(XmlReader input, XmlWriter results)
        {
            this.CheckCommand();
            this.CheckInput(input);
            this.command.Execute(input, XsltConfigSection.CreateDefaultResolver(), null, results);
        }

        public void Transform(IXPathNavigable input, XmlWriter results)
        {
            this.CheckCommand();
            this.CheckInput(input);
            this.command.Execute(input, XsltConfigSection.CreateDefaultResolver(), null, results);
        }

        public void Transform(string inputUri, XsltArgumentList arguments, Stream results)
        {
            this.CheckCommand();
            using (XmlReader reader = this.CreateReader(inputUri))
            {
                this.command.Execute(reader, XsltConfigSection.CreateDefaultResolver(), arguments, results);
            }
        }

        public void Transform(string inputUri, XsltArgumentList arguments, TextWriter results)
        {
            this.CheckCommand();
            using (XmlReader reader = this.CreateReader(inputUri))
            {
                this.command.Execute(reader, XsltConfigSection.CreateDefaultResolver(), arguments, results);
            }
        }

        public void Transform(string inputUri, XsltArgumentList arguments, XmlWriter results)
        {
            this.CheckCommand();
            using (XmlReader reader = this.CreateReader(inputUri))
            {
                this.command.Execute(reader, XsltConfigSection.CreateDefaultResolver(), arguments, results);
            }
        }

        public void Transform(XmlReader input, XsltArgumentList arguments, Stream results)
        {
            this.CheckCommand();
            this.CheckInput(input);
            this.command.Execute(input, XsltConfigSection.CreateDefaultResolver(), arguments, results);
        }

        public void Transform(XmlReader input, XsltArgumentList arguments, TextWriter results)
        {
            this.CheckCommand();
            this.CheckInput(input);
            this.command.Execute(input, XsltConfigSection.CreateDefaultResolver(), arguments, results);
        }

        public void Transform(XmlReader input, XsltArgumentList arguments, XmlWriter results)
        {
            this.CheckCommand();
            this.CheckInput(input);
            this.command.Execute(input, XsltConfigSection.CreateDefaultResolver(), arguments, results);
        }

        public void Transform(IXPathNavigable input, XsltArgumentList arguments, Stream results)
        {
            this.CheckCommand();
            this.CheckInput(input);
            this.command.Execute(input, XsltConfigSection.CreateDefaultResolver(), arguments, results);
        }

        public void Transform(IXPathNavigable input, XsltArgumentList arguments, TextWriter results)
        {
            this.CheckCommand();
            this.CheckInput(input);
            this.command.Execute(input, XsltConfigSection.CreateDefaultResolver(), arguments, results);
        }

        public void Transform(IXPathNavigable input, XsltArgumentList arguments, XmlWriter results)
        {
            this.CheckCommand();
            this.CheckInput(input);
            this.command.Execute(input, XsltConfigSection.CreateDefaultResolver(), arguments, results);
        }

        private void Transform(string inputUri, XsltArgumentList arguments, XmlWriter results, XmlResolver documentResolver)
        {
            this.command.Execute(inputUri, documentResolver, arguments, results);
        }

        public void Transform(XmlReader input, XsltArgumentList arguments, XmlWriter results, XmlResolver documentResolver)
        {
            this.CheckCommand();
            this.CheckInput(input);
            this.command.Execute(input, documentResolver, arguments, results);
        }

        public void Transform(IXPathNavigable input, XsltArgumentList arguments, XmlWriter results, XmlResolver documentResolver)
        {
            this.CheckInput(input);
            this.CheckCommand();
            this.command.Execute(input, documentResolver, arguments, results);
        }

        internal CompilerErrorCollection Errors =>
            this.compilerResults?.Errors;

        public XmlWriterSettings OutputSettings =>
            this.outputSettings;

        public TempFileCollection TemporaryFiles =>
            this.compilerResults?.TempFiles;
    }
}

