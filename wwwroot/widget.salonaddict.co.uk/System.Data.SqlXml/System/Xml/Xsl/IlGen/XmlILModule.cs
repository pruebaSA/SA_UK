namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Diagnostics.SymbolStore;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using System.Xml.Xsl.Runtime;

    internal class XmlILModule
    {
        private static long AssemblyId;
        public static readonly PermissionSet CreateModulePermissionSet = new PermissionSet(PermissionState.None);
        private bool emitSymbols;
        private static readonly Guid LanguageGuid = new Guid(0x462d4a3e, 0xb257, 0x4aee, 0x97, 0xcd, 0x59, 0x18, 0xc7, 0x53, 0x17, 0x58);
        private static ModuleBuilder LREModule;
        private Hashtable methods;
        private string modFile;
        private bool persistAsm;
        private const string RuntimeName = "{urn:schemas-microsoft-com:xslt-debug}runtime";
        private TypeBuilder typeBldr;
        private Hashtable urlToSymWriter;
        private bool useLRE;
        private static readonly Guid VendorGuid = new Guid(0x994b45c4, 0xe6e9, 0x11d2, 0x90, 0x3f, 0, 0xc0, 0x4f, 0xa3, 2, 0xa1);

        static XmlILModule()
        {
            CreateModulePermissionSet.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.ReflectionEmit));
            CreateModulePermissionSet.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
            CreateModulePermissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.ControlEvidence));
            AssemblyId = 0L;
            AssemblyName name = CreateAssemblyName();
            AssemblyBuilder builder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            try
            {
                CreateModulePermissionSet.Assert();
                builder.SetCustomAttribute(new CustomAttributeBuilder(XmlILConstructors.Transparent, new object[0]));
                LREModule = builder.DefineDynamicModule("System.Xml.Xsl.CompiledQuery", false);
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
        }

        public XmlILModule(TypeBuilder typeBldr)
        {
            this.typeBldr = typeBldr;
            this.emitSymbols = ((ModuleBuilder) this.typeBldr.Module).GetSymWriter() != null;
            this.useLRE = false;
            this.persistAsm = false;
            this.methods = new Hashtable();
            if (this.emitSymbols)
            {
                this.urlToSymWriter = new Hashtable();
            }
        }

        public XmlILModule(bool useLRE, bool emitSymbols)
        {
            this.useLRE = useLRE;
            this.emitSymbols = emitSymbols;
            this.persistAsm = false;
            this.methods = new Hashtable();
            if (!useLRE)
            {
                ModuleBuilder builder2;
                AssemblyName name = CreateAssemblyName();
                if (XmlILTrace.IsEnabled)
                {
                    this.modFile = "System.Xml.Xsl.CompiledQuery";
                    this.persistAsm = true;
                }
                AssemblyBuilder builder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, this.persistAsm ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run);
                builder.SetCustomAttribute(new CustomAttributeBuilder(XmlILConstructors.Transparent, new object[0]));
                if (emitSymbols)
                {
                    this.urlToSymWriter = new Hashtable();
                    DebuggableAttribute.DebuggingModes modes = DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggableAttribute.DebuggingModes.Default;
                    builder.SetCustomAttribute(new CustomAttributeBuilder(XmlILConstructors.Debuggable, new object[] { modes }));
                }
                if (this.persistAsm)
                {
                    builder2 = builder.DefineDynamicModule("System.Xml.Xsl.CompiledQuery", this.modFile + ".dll", emitSymbols);
                }
                else
                {
                    builder2 = builder.DefineDynamicModule("System.Xml.Xsl.CompiledQuery", emitSymbols);
                }
                this.typeBldr = builder2.DefineType("System.Xml.Xsl.CompiledQuery.Query", TypeAttributes.Public);
            }
        }

        public ISymbolDocumentWriter AddSourceDocument(string fileName)
        {
            ISymbolDocumentWriter writer = this.urlToSymWriter[fileName] as ISymbolDocumentWriter;
            if (writer == null)
            {
                writer = ((ModuleBuilder) this.typeBldr.Module).DefineDocument(fileName, LanguageGuid, VendorGuid, Guid.Empty);
                this.urlToSymWriter.Add(fileName, writer);
            }
            return writer;
        }

        public void BakeMethods()
        {
            if (!this.useLRE)
            {
                Type type = this.typeBldr.CreateType();
                if (this.persistAsm)
                {
                    ((AssemblyBuilder) this.typeBldr.Module.Assembly).Save(this.modFile + ".dll");
                }
                Hashtable hashtable = new Hashtable(this.methods.Count);
                foreach (string str in this.methods.Keys)
                {
                    hashtable[str] = type.GetMethod(str, BindingFlags.NonPublic | BindingFlags.Static);
                }
                this.methods = hashtable;
                this.typeBldr = null;
                this.urlToSymWriter = null;
            }
        }

        private static AssemblyName CreateAssemblyName()
        {
            Interlocked.Increment(ref AssemblyId);
            return new AssemblyName { Name = "System.Xml.Xsl.CompiledQuery." + AssemblyId };
        }

        public Delegate CreateDelegate(string name, Type typDelegate)
        {
            if (!this.useLRE)
            {
                return Delegate.CreateDelegate(typDelegate, (MethodInfo) this.methods[name]);
            }
            return ((DynamicMethod) this.methods[name]).CreateDelegate(typDelegate);
        }

        public FieldInfo DefineField(string fieldName, Type type) => 
            this.typeBldr.DefineField(fieldName, type, FieldAttributes.Static | FieldAttributes.Private);

        public FieldInfo DefineInitializedData(string name, byte[] data) => 
            this.typeBldr.DefineInitializedData(name, data, FieldAttributes.Static | FieldAttributes.Private);

        public MethodInfo DefineMethod(string name, Type returnType, Type[] paramTypes, string[] paramNames, XmlILMethodAttributes xmlAttrs)
        {
            MethodInfo info;
            int num = 1;
            string str = name;
            bool flag = (xmlAttrs & XmlILMethodAttributes.Raw) != XmlILMethodAttributes.None;
            while (this.methods[name] != null)
            {
                num++;
                name = string.Concat(new object[] { str, " (", num, ")" });
            }
            if (!flag)
            {
                Type[] destinationArray = new Type[paramTypes.Length + 1];
                destinationArray[0] = typeof(XmlQueryRuntime);
                Array.Copy(paramTypes, 0, destinationArray, 1, paramTypes.Length);
                paramTypes = destinationArray;
            }
            if (!this.useLRE)
            {
                MethodBuilder builder = this.typeBldr.DefineMethod(name, MethodAttributes.Static | MethodAttributes.Private, returnType, paramTypes);
                if (this.emitSymbols && ((xmlAttrs & XmlILMethodAttributes.NonUser) != XmlILMethodAttributes.None))
                {
                    builder.SetCustomAttribute(new CustomAttributeBuilder(XmlILConstructors.StepThrough, new object[0]));
                    builder.SetCustomAttribute(new CustomAttributeBuilder(XmlILConstructors.NonUserCode, new object[0]));
                }
                if (!flag)
                {
                    builder.DefineParameter(1, ParameterAttributes.None, "{urn:schemas-microsoft-com:xslt-debug}runtime");
                }
                for (int i = 0; i < paramNames.Length; i++)
                {
                    if ((paramNames[i] != null) && (paramNames[i].Length != 0))
                    {
                        builder.DefineParameter(i + (flag ? 1 : 2), ParameterAttributes.None, paramNames[i]);
                    }
                }
                info = builder;
            }
            else
            {
                DynamicMethod method = new DynamicMethod(name, returnType, paramTypes, LREModule) {
                    InitLocals = true
                };
                if (!flag)
                {
                    method.DefineParameter(1, ParameterAttributes.None, "{urn:schemas-microsoft-com:xslt-debug}runtime");
                }
                for (int j = 0; j < paramNames.Length; j++)
                {
                    if ((paramNames[j] != null) && (paramNames[j].Length != 0))
                    {
                        method.DefineParameter(j + (flag ? 1 : 2), ParameterAttributes.None, paramNames[j]);
                    }
                }
                info = method;
            }
            this.methods[name] = info;
            return info;
        }

        public static ILGenerator DefineMethodBody(MethodBase methInfo)
        {
            DynamicMethod method = methInfo as DynamicMethod;
            if (method != null)
            {
                return method.GetILGenerator();
            }
            MethodBuilder builder = methInfo as MethodBuilder;
            if (builder != null)
            {
                return builder.GetILGenerator();
            }
            return ((ConstructorBuilder) methInfo).GetILGenerator();
        }

        public ConstructorInfo DefineTypeInitializer() => 
            this.typeBldr.DefineTypeInitializer();

        public MethodInfo FindMethod(string name) => 
            ((MethodInfo) this.methods[name]);
    }
}

