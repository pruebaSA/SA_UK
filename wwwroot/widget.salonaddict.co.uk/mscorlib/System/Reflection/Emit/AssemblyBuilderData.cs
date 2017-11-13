﻿namespace System.Reflection.Emit
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Reflection;
    using System.Security;

    internal class AssemblyBuilderData
    {
        internal AssemblyBuilderAccess m_access;
        internal Assembly m_assembly;
        internal CustomAttributeBuilder[] m_CABuilders;
        internal byte[][] m_CABytes;
        internal ConstructorInfo[] m_CACons;
        internal MethodInfo m_entryPointMethod;
        internal ModuleBuilder m_entryPointModule;
        internal bool m_hasUnmanagedVersionInfo;
        internal int m_iCABuilder;
        internal int m_iCAs;
        internal const int m_iInitialSize = 0x10;
        private ModuleBuilder m_InMemoryAssemblyModule;
        internal int m_iPublicComTypeCount;
        internal bool m_isSaved;
        internal bool m_isSynchronized;
        internal Assembly m_ISymWrapperAssembly;
        internal ArrayList m_moduleBuilderList;
        internal NativeVersionInfo m_nativeVersion;
        private ModuleBuilder m_OnDiskAssemblyModule;
        internal PermissionSet m_OptionalPset;
        internal bool m_OverrideUnmanagedVersionInfo;
        internal PEFileKinds m_peFileKind;
        internal Type[] m_publicComTypeList;
        internal PermissionSet m_RefusedPset;
        internal PermissionSet m_RequiredPset;
        internal byte[] m_resourceBytes;
        internal ArrayList m_resWriterList;
        internal string m_strAssemblyName;
        internal string m_strDir;
        internal string m_strResourceFileName;
        internal const int m_tkAssembly = 0x20000001;

        internal AssemblyBuilderData(Assembly assembly, string strAssemblyName, AssemblyBuilderAccess access, string dir)
        {
            this.m_assembly = assembly;
            this.m_strAssemblyName = strAssemblyName;
            this.m_access = access;
            this.m_moduleBuilderList = new ArrayList();
            this.m_resWriterList = new ArrayList();
            this.m_publicComTypeList = null;
            this.m_CABuilders = null;
            this.m_CABytes = null;
            this.m_CACons = null;
            this.m_iPublicComTypeCount = 0;
            this.m_iCABuilder = 0;
            this.m_iCAs = 0;
            this.m_entryPointModule = null;
            this.m_isSaved = false;
            if ((dir == null) && (access != AssemblyBuilderAccess.Run))
            {
                this.m_strDir = Environment.CurrentDirectory;
            }
            else
            {
                this.m_strDir = dir;
            }
            this.m_RequiredPset = null;
            this.m_OptionalPset = null;
            this.m_RefusedPset = null;
            this.m_isSynchronized = true;
            this.m_hasUnmanagedVersionInfo = false;
            this.m_OverrideUnmanagedVersionInfo = false;
            this.m_InMemoryAssemblyModule = null;
            this.m_OnDiskAssemblyModule = null;
            this.m_peFileKind = PEFileKinds.Dll;
            this.m_strResourceFileName = null;
            this.m_resourceBytes = null;
            this.m_nativeVersion = null;
            this.m_entryPointMethod = null;
            this.m_ISymWrapperAssembly = null;
        }

        internal void AddCustomAttribute(CustomAttributeBuilder customBuilder)
        {
            if (this.m_CABuilders == null)
            {
                this.m_CABuilders = new CustomAttributeBuilder[0x10];
            }
            if (this.m_iCABuilder == this.m_CABuilders.Length)
            {
                CustomAttributeBuilder[] destinationArray = new CustomAttributeBuilder[this.m_iCABuilder * 2];
                Array.Copy(this.m_CABuilders, destinationArray, this.m_iCABuilder);
                this.m_CABuilders = destinationArray;
            }
            this.m_CABuilders[this.m_iCABuilder] = customBuilder;
            this.m_iCABuilder++;
        }

        internal void AddCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
        {
            if (this.m_CABytes == null)
            {
                this.m_CABytes = new byte[0x10][];
                this.m_CACons = new ConstructorInfo[0x10];
            }
            if (this.m_iCAs == this.m_CABytes.Length)
            {
                byte[][] bufferArray = new byte[this.m_iCAs * 2][];
                ConstructorInfo[] infoArray = new ConstructorInfo[this.m_iCAs * 2];
                for (int i = 0; i < this.m_iCAs; i++)
                {
                    bufferArray[i] = this.m_CABytes[i];
                    infoArray[i] = this.m_CACons[i];
                }
                this.m_CABytes = bufferArray;
                this.m_CACons = infoArray;
            }
            byte[] destinationArray = new byte[binaryAttribute.Length];
            Array.Copy(binaryAttribute, destinationArray, binaryAttribute.Length);
            this.m_CABytes[this.m_iCAs] = destinationArray;
            this.m_CACons[this.m_iCAs] = con;
            this.m_iCAs++;
        }

        internal void AddModule(ModuleBuilder dynModule)
        {
            this.m_moduleBuilderList.Add(dynModule);
            if (this.m_assembly != null)
            {
                this.m_assembly.nAddFileToInMemoryFileList(dynModule.m_moduleData.m_strFileName, dynModule);
            }
        }

        internal void AddPermissionRequests(PermissionSet required, PermissionSet optional, PermissionSet refused)
        {
            if (this.m_isSaved)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_CannotAlterAssembly"));
            }
            this.m_RequiredPset = required;
            this.m_OptionalPset = optional;
            this.m_RefusedPset = refused;
        }

        internal void AddPublicComType(Type type)
        {
            if (this.m_isSaved)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_CannotAlterAssembly"));
            }
            this.EnsurePublicComTypeCapacity();
            this.m_publicComTypeList[this.m_iPublicComTypeCount] = type;
            this.m_iPublicComTypeCount++;
        }

        internal void AddResWriter(ResWriterData resData)
        {
            this.m_resWriterList.Add(resData);
        }

        internal void CheckFileNameConflict(string strFileName)
        {
            int num2;
            int count = this.m_moduleBuilderList.Count;
            for (num2 = 0; num2 < count; num2++)
            {
                ModuleBuilder builder = (ModuleBuilder) this.m_moduleBuilderList[num2];
                if ((builder.m_moduleData.m_strFileName != null) && (string.Compare(builder.m_moduleData.m_strFileName, strFileName, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    throw new ArgumentException(Environment.GetResourceString("Argument_DuplicatedFileName"));
                }
            }
            count = this.m_resWriterList.Count;
            for (num2 = 0; num2 < count; num2++)
            {
                ResWriterData data = (ResWriterData) this.m_resWriterList[num2];
                if ((data.m_strFileName != null) && (string.Compare(data.m_strFileName, strFileName, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    throw new ArgumentException(Environment.GetResourceString("Argument_DuplicatedFileName"));
                }
            }
        }

        internal void CheckNameConflict(string strNewModuleName)
        {
            int count = this.m_moduleBuilderList.Count;
            for (int i = 0; i < count; i++)
            {
                ModuleBuilder builder = (ModuleBuilder) this.m_moduleBuilderList[i];
                if (builder.m_moduleData.m_strModuleName.Equals(strNewModuleName))
                {
                    throw new ArgumentException(Environment.GetResourceString("Argument_DuplicateModuleName"));
                }
            }
            if (!(this.m_assembly is AssemblyBuilder) && (this.m_assembly.GetModule(strNewModuleName) != null))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_DuplicateModuleName"));
            }
        }

        internal void CheckResNameConflict(string strNewResName)
        {
            int count = this.m_resWriterList.Count;
            for (int i = 0; i < count; i++)
            {
                ResWriterData data = (ResWriterData) this.m_resWriterList[i];
                if (data.m_strName.Equals(strNewResName))
                {
                    throw new ArgumentException(Environment.GetResourceString("Argument_DuplicateResourceName"));
                }
            }
        }

        internal void CheckTypeNameConflict(string strTypeName, TypeBuilder enclosingType)
        {
            for (int i = 0; i < this.m_moduleBuilderList.Count; i++)
            {
                ModuleBuilder builder = (ModuleBuilder) this.m_moduleBuilderList[i];
                for (int j = 0; j < builder.m_TypeBuilderList.Count; j++)
                {
                    Type type = (Type) builder.m_TypeBuilderList[j];
                    if (type.FullName.Equals(strTypeName) && (type.DeclaringType == enclosingType))
                    {
                        throw new ArgumentException(Environment.GetResourceString("Argument_DuplicateTypeName"));
                    }
                }
            }
            if (((enclosingType == null) && !(this.m_assembly is AssemblyBuilder)) && (this.m_assembly.GetType(strTypeName, false, false) != null))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_DuplicateTypeName"));
            }
        }

        internal void EnsurePublicComTypeCapacity()
        {
            if (this.m_publicComTypeList == null)
            {
                this.m_publicComTypeList = new Type[0x10];
            }
            if (this.m_iPublicComTypeCount == this.m_publicComTypeList.Length)
            {
                Type[] destinationArray = new Type[this.m_iPublicComTypeCount * 2];
                Array.Copy(this.m_publicComTypeList, destinationArray, this.m_iPublicComTypeCount);
                this.m_publicComTypeList = destinationArray;
            }
        }

        internal void FillUnmanagedVersionInfo()
        {
            CultureInfo locale = this.m_assembly.GetLocale();
            if (locale != null)
            {
                this.m_nativeVersion.m_lcid = locale.LCID;
            }
            for (int i = 0; i < this.m_iCABuilder; i++)
            {
                Type declaringType = this.m_CABuilders[i].m_con.DeclaringType;
                if ((this.m_CABuilders[i].m_constructorArgs.Length != 0) && (this.m_CABuilders[i].m_constructorArgs[0] != null))
                {
                    if (declaringType.Equals(typeof(AssemblyCopyrightAttribute)))
                    {
                        if (this.m_CABuilders[i].m_constructorArgs.Length != 1)
                        {
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_BadCAForUnmngRSC"), new object[] { this.m_CABuilders[i].m_con.ReflectedType.Name }));
                        }
                        if (!this.m_OverrideUnmanagedVersionInfo)
                        {
                            this.m_nativeVersion.m_strCopyright = this.m_CABuilders[i].m_constructorArgs[0].ToString();
                        }
                    }
                    else if (declaringType.Equals(typeof(AssemblyTrademarkAttribute)))
                    {
                        if (this.m_CABuilders[i].m_constructorArgs.Length != 1)
                        {
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_BadCAForUnmngRSC"), new object[] { this.m_CABuilders[i].m_con.ReflectedType.Name }));
                        }
                        if (!this.m_OverrideUnmanagedVersionInfo)
                        {
                            this.m_nativeVersion.m_strTrademark = this.m_CABuilders[i].m_constructorArgs[0].ToString();
                        }
                    }
                    else if (declaringType.Equals(typeof(AssemblyProductAttribute)))
                    {
                        if (!this.m_OverrideUnmanagedVersionInfo)
                        {
                            this.m_nativeVersion.m_strProduct = this.m_CABuilders[i].m_constructorArgs[0].ToString();
                        }
                    }
                    else if (declaringType.Equals(typeof(AssemblyCompanyAttribute)))
                    {
                        if (this.m_CABuilders[i].m_constructorArgs.Length != 1)
                        {
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_BadCAForUnmngRSC"), new object[] { this.m_CABuilders[i].m_con.ReflectedType.Name }));
                        }
                        if (!this.m_OverrideUnmanagedVersionInfo)
                        {
                            this.m_nativeVersion.m_strCompany = this.m_CABuilders[i].m_constructorArgs[0].ToString();
                        }
                    }
                    else if (declaringType.Equals(typeof(AssemblyDescriptionAttribute)))
                    {
                        if (this.m_CABuilders[i].m_constructorArgs.Length != 1)
                        {
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_BadCAForUnmngRSC"), new object[] { this.m_CABuilders[i].m_con.ReflectedType.Name }));
                        }
                        this.m_nativeVersion.m_strDescription = this.m_CABuilders[i].m_constructorArgs[0].ToString();
                    }
                    else if (declaringType.Equals(typeof(AssemblyTitleAttribute)))
                    {
                        if (this.m_CABuilders[i].m_constructorArgs.Length != 1)
                        {
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_BadCAForUnmngRSC"), new object[] { this.m_CABuilders[i].m_con.ReflectedType.Name }));
                        }
                        this.m_nativeVersion.m_strTitle = this.m_CABuilders[i].m_constructorArgs[0].ToString();
                    }
                    else if (declaringType.Equals(typeof(AssemblyInformationalVersionAttribute)))
                    {
                        if (this.m_CABuilders[i].m_constructorArgs.Length != 1)
                        {
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_BadCAForUnmngRSC"), new object[] { this.m_CABuilders[i].m_con.ReflectedType.Name }));
                        }
                        if (!this.m_OverrideUnmanagedVersionInfo)
                        {
                            this.m_nativeVersion.m_strProductVersion = this.m_CABuilders[i].m_constructorArgs[0].ToString();
                        }
                    }
                    else if (declaringType.Equals(typeof(AssemblyCultureAttribute)))
                    {
                        if (this.m_CABuilders[i].m_constructorArgs.Length != 1)
                        {
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_BadCAForUnmngRSC"), new object[] { this.m_CABuilders[i].m_con.ReflectedType.Name }));
                        }
                        CultureInfo info2 = new CultureInfo(this.m_CABuilders[i].m_constructorArgs[0].ToString());
                        this.m_nativeVersion.m_lcid = info2.LCID;
                    }
                    else if (declaringType.Equals(typeof(AssemblyFileVersionAttribute)))
                    {
                        if (this.m_CABuilders[i].m_constructorArgs.Length != 1)
                        {
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_BadCAForUnmngRSC"), new object[] { this.m_CABuilders[i].m_con.ReflectedType.Name }));
                        }
                        if (!this.m_OverrideUnmanagedVersionInfo)
                        {
                            this.m_nativeVersion.m_strFileVersion = this.m_CABuilders[i].m_constructorArgs[0].ToString();
                        }
                    }
                }
            }
        }

        internal ModuleBuilder FindModuleWithFileName(string strFileName)
        {
            int count = this.m_moduleBuilderList.Count;
            for (int i = 0; i < count; i++)
            {
                ModuleBuilder builder = (ModuleBuilder) this.m_moduleBuilderList[i];
                if ((builder.m_moduleData.m_strFileName != null) && (string.Compare(builder.m_moduleData.m_strFileName, strFileName, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    return builder;
                }
            }
            return null;
        }

        internal ModuleBuilder FindModuleWithName(string strName)
        {
            int count = this.m_moduleBuilderList.Count;
            for (int i = 0; i < count; i++)
            {
                ModuleBuilder builder = (ModuleBuilder) this.m_moduleBuilderList[i];
                if ((builder.m_moduleData.m_strModuleName != null) && (string.Compare(builder.m_moduleData.m_strModuleName, strName, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    return builder;
                }
            }
            return null;
        }

        internal ModuleBuilder GetInMemoryAssemblyModule()
        {
            if (this.m_InMemoryAssemblyModule == null)
            {
                ModuleBuilder builder = (ModuleBuilder) this.m_assembly.nGetInMemoryAssemblyModule();
                builder.Init("RefEmit_InMemoryManifestModule", null, null);
                this.m_InMemoryAssemblyModule = builder;
            }
            return this.m_InMemoryAssemblyModule;
        }

        internal ModuleBuilder GetOnDiskAssemblyModule()
        {
            if (this.m_OnDiskAssemblyModule == null)
            {
                ModuleBuilder builder = (ModuleBuilder) this.m_assembly.nGetOnDiskAssemblyModule();
                builder.Init("RefEmit_OnDiskManifestModule", null, null);
                this.m_OnDiskAssemblyModule = builder;
            }
            return this.m_OnDiskAssemblyModule;
        }

        internal void SetOnDiskAssemblyModule(ModuleBuilder modBuilder)
        {
            this.m_OnDiskAssemblyModule = modBuilder;
        }
    }
}

