namespace System.Resources
{
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Text;
    using System.Threading;

    [Serializable, ComVisible(true)]
    public class ResourceManager
    {
        private Assembly _callingAssembly;
        private static bool _checkedConfigFile;
        [OptionalField]
        private UltimateResourceFallbackLocation _fallbackLoc;
        private bool _ignoreCase;
        private static Hashtable _installedSatelliteInfo;
        private Type _locationInfo;
        [OptionalField]
        private bool _lookedForSatelliteContractVersion;
        private static readonly Type _minResourceSet = typeof(ResourceSet);
        private CultureInfo _neutralResourcesCulture;
        [OptionalField]
        private Version _satelliteContractVersion;
        private Type _userResourceSet;
        protected string BaseNameField;
        internal static readonly int DEBUG = 0;
        public static readonly int HeaderVersionNumber = 1;
        public static readonly int MagicNumber = -1091581234;
        protected Assembly MainAssembly;
        private string moduleDir;
        internal static readonly string MscorlibName = typeof(ResourceReader).Assembly.FullName;
        internal const string ResFileExtension = ".resources";
        internal const int ResFileExtensionLength = 10;
        protected Hashtable ResourceSets;
        internal static readonly string ResReaderTypeName = typeof(ResourceReader).FullName;
        internal static readonly string ResSetTypeName = typeof(RuntimeResourceSet).FullName;
        private bool UseManifest;
        private bool UseSatelliteAssem;

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected ResourceManager()
        {
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            this._callingAssembly = Assembly.nGetExecutingAssembly(ref lookForMyCaller);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ResourceManager(Type resourceSource)
        {
            if (resourceSource == null)
            {
                throw new ArgumentNullException("resourceSource");
            }
            this._locationInfo = resourceSource;
            this.MainAssembly = this._locationInfo.Assembly;
            this.BaseNameField = resourceSource.Name;
            this.CommonSatelliteAssemblyInit();
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            this._callingAssembly = Assembly.nGetExecutingAssembly(ref lookForMyCaller);
            if ((this.MainAssembly == typeof(object).Assembly) && (this._callingAssembly != this.MainAssembly))
            {
                this._callingAssembly = null;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ResourceManager(string baseName, Assembly assembly)
        {
            if (baseName == null)
            {
                throw new ArgumentNullException("baseName");
            }
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            this.MainAssembly = assembly;
            this._locationInfo = null;
            this.BaseNameField = baseName;
            this.CommonSatelliteAssemblyInit();
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            this._callingAssembly = Assembly.nGetExecutingAssembly(ref lookForMyCaller);
            if ((assembly == typeof(object).Assembly) && (this._callingAssembly != assembly))
            {
                this._callingAssembly = null;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ResourceManager(string baseName, Assembly assembly, Type usingResourceSet)
        {
            if (baseName == null)
            {
                throw new ArgumentNullException("baseName");
            }
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }
            this.MainAssembly = assembly;
            this._locationInfo = null;
            this.BaseNameField = baseName;
            if (((usingResourceSet != null) && (usingResourceSet != _minResourceSet)) && !usingResourceSet.IsSubclassOf(_minResourceSet))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_ResMgrNotResSet"), "usingResourceSet");
            }
            this._userResourceSet = usingResourceSet;
            this.CommonSatelliteAssemblyInit();
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            this._callingAssembly = Assembly.nGetExecutingAssembly(ref lookForMyCaller);
            if ((assembly == typeof(object).Assembly) && (this._callingAssembly != assembly))
            {
                this._callingAssembly = null;
            }
        }

        private ResourceManager(string baseName, string resourceDir, Type usingResourceSet)
        {
            if (baseName == null)
            {
                throw new ArgumentNullException("baseName");
            }
            if (resourceDir == null)
            {
                throw new ArgumentNullException("resourceDir");
            }
            this.BaseNameField = baseName;
            this.moduleDir = resourceDir;
            this._userResourceSet = usingResourceSet;
            this.ResourceSets = new Hashtable();
            this.UseManifest = false;
        }

        private static void AddResourceSet(Hashtable localResourceSets, CultureInfo culture, ref ResourceSet rs)
        {
            lock (localResourceSets)
            {
                ResourceSet objA = (ResourceSet) localResourceSets[culture];
                if (objA != null)
                {
                    if (!object.Equals(objA, rs))
                    {
                        if (!localResourceSets.ContainsValue(rs))
                        {
                            rs.Dispose();
                        }
                        rs = objA;
                    }
                }
                else
                {
                    localResourceSets.Add(culture, rs);
                }
            }
        }

        private bool CanUseDefaultResourceClasses(string readerTypeName, string resSetTypeName)
        {
            if (this._userResourceSet != null)
            {
                return false;
            }
            AssemblyName name = new AssemblyName(MscorlibName);
            if ((readerTypeName != null) && !CompareNames(readerTypeName, ResReaderTypeName, name))
            {
                return false;
            }
            if ((resSetTypeName != null) && !CompareNames(resSetTypeName, ResSetTypeName, name))
            {
                return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private Stream CaseInsensitiveManifestResourceStreamLookup(Assembly satellite, string name)
        {
            StringBuilder builder = new StringBuilder();
            if (this._locationInfo != null)
            {
                string str = this._locationInfo.Namespace;
                if (str != null)
                {
                    builder.Append(str);
                    if (name != null)
                    {
                        builder.Append(Type.Delimiter);
                    }
                }
            }
            builder.Append(name);
            string str2 = builder.ToString();
            CompareInfo compareInfo = CultureInfo.InvariantCulture.CompareInfo;
            string str3 = null;
            foreach (string str4 in satellite.GetManifestResourceNames())
            {
                if (compareInfo.Compare(str4, str2, CompareOptions.IgnoreCase) == 0)
                {
                    if (str3 != null)
                    {
                        throw new MissingManifestResourceException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("MissingManifestResource_MultipleBlobs"), new object[] { str2, satellite.ToString() }));
                    }
                    str3 = str4;
                }
            }
            if (str3 == null)
            {
                return null;
            }
            bool skipSecurityCheck = (this.MainAssembly == satellite) && (this._callingAssembly == this.MainAssembly);
            StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
            return satellite.GetManifestResourceStream(str3, ref lookForMyCaller, skipSecurityCheck);
        }

        private void CommonSatelliteAssemblyInit()
        {
            this.UseManifest = true;
            this.UseSatelliteAssem = true;
            this.ResourceSets = new Hashtable();
            this._fallbackLoc = UltimateResourceFallbackLocation.MainAssembly;
        }

        internal static bool CompareNames(string asmTypeName1, string typeName2, AssemblyName asmName2)
        {
            int index = asmTypeName1.IndexOf(',');
            if (((index == -1) ? asmTypeName1.Length : index) != typeName2.Length)
            {
                return false;
            }
            if (string.Compare(asmTypeName1, 0, typeName2, 0, typeName2.Length, StringComparison.Ordinal) != 0)
            {
                return false;
            }
            if (index != -1)
            {
                while (char.IsWhiteSpace(asmTypeName1[++index]))
                {
                }
                AssemblyName name = new AssemblyName(asmTypeName1.Substring(index));
                if (string.Compare(name.Name, asmName2.Name, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    return false;
                }
                if (((name.CultureInfo != null) && (asmName2.CultureInfo != null)) && (name.CultureInfo.LCID != asmName2.CultureInfo.LCID))
                {
                    return false;
                }
                byte[] publicKeyToken = name.GetPublicKeyToken();
                byte[] buffer2 = asmName2.GetPublicKeyToken();
                if ((publicKeyToken != null) && (buffer2 != null))
                {
                    if (publicKeyToken.Length != buffer2.Length)
                    {
                        return false;
                    }
                    for (int i = 0; i < publicKeyToken.Length; i++)
                    {
                        if (publicKeyToken[i] != buffer2[i])
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static ResourceManager CreateFileBasedResourceManager(string baseName, string resourceDir, Type usingResourceSet) => 
            new ResourceManager(baseName, resourceDir, usingResourceSet);

        private ResourceSet CreateResourceSet(string file)
        {
            ResourceSet set;
            if (this._userResourceSet == null)
            {
                return new RuntimeResourceSet(file);
            }
            object[] args = new object[] { file };
            try
            {
                set = (ResourceSet) Activator.CreateInstance(this._userResourceSet, args);
            }
            catch (MissingMethodException exception)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("InvalidOperation_ResMgrBadResSet_Type"), new object[] { this._userResourceSet.AssemblyQualifiedName }), exception);
            }
            return set;
        }

        private ResourceSet CreateResourceSet(Stream store, Assembly assembly)
        {
            ResourceSet set4;
            if (store.CanSeek && (store.Length > 4L))
            {
                long position = store.Position;
                BinaryReader reader = new BinaryReader(store);
                if (reader.ReadInt32() == MagicNumber)
                {
                    Type type2;
                    int num3 = reader.ReadInt32();
                    string readerTypeName = null;
                    string resSetTypeName = null;
                    if (num3 == HeaderVersionNumber)
                    {
                        reader.ReadInt32();
                        readerTypeName = reader.ReadString();
                        resSetTypeName = reader.ReadString();
                    }
                    else
                    {
                        if (num3 <= HeaderVersionNumber)
                        {
                            throw new NotSupportedException(Environment.GetResourceString("NotSupported_ObsoleteResourcesFile", new object[] { this.MainAssembly.nGetSimpleName() }));
                        }
                        int num4 = reader.ReadInt32();
                        long offset = reader.BaseStream.Position + num4;
                        readerTypeName = reader.ReadString();
                        resSetTypeName = reader.ReadString();
                        reader.BaseStream.Seek(offset, SeekOrigin.Begin);
                    }
                    store.Position = position;
                    if (this.CanUseDefaultResourceClasses(readerTypeName, resSetTypeName))
                    {
                        return new RuntimeResourceSet(store);
                    }
                    Type type = Type.GetType(readerTypeName, true);
                    object[] objArray = new object[] { store };
                    IResourceReader reader2 = (IResourceReader) Activator.CreateInstance(type, objArray);
                    object[] objArray2 = new object[] { reader2 };
                    if (this._userResourceSet == null)
                    {
                        type2 = Type.GetType(resSetTypeName, true, false);
                    }
                    else
                    {
                        type2 = this._userResourceSet;
                    }
                    return (ResourceSet) Activator.CreateInstance(type2, BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, objArray2, null, null);
                }
                store.Position = position;
            }
            if (this._userResourceSet == null)
            {
                return new RuntimeResourceSet(store);
            }
            object[] args = new object[] { store, assembly };
            try
            {
                ResourceSet set3 = null;
                try
                {
                    return (ResourceSet) Activator.CreateInstance(this._userResourceSet, args);
                }
                catch (MissingMethodException)
                {
                }
                args = new object[] { store };
                set3 = (ResourceSet) Activator.CreateInstance(this._userResourceSet, args);
                set4 = set3;
            }
            catch (MissingMethodException exception)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("InvalidOperation_ResMgrBadResSet_Type"), new object[] { this._userResourceSet.AssemblyQualifiedName }), exception);
            }
            return set4;
        }

        private string FindResourceFile(CultureInfo culture)
        {
            string resourceFileName = this.GetResourceFileName(culture);
            if (this.moduleDir != null)
            {
                string path = Path.Combine(this.moduleDir, resourceFileName);
                if (File.Exists(path))
                {
                    return path;
                }
            }
            if (File.Exists(resourceFileName))
            {
                return resourceFileName;
            }
            return null;
        }

        protected static CultureInfo GetNeutralResourcesLanguage(Assembly a)
        {
            UltimateResourceFallbackLocation mainAssembly = UltimateResourceFallbackLocation.MainAssembly;
            return GetNeutralResourcesLanguage(a, ref mainAssembly);
        }

        private static CultureInfo GetNeutralResourcesLanguage(Assembly a, ref UltimateResourceFallbackLocation fallbackLocation)
        {
            IList<CustomAttributeData> customAttributes = CustomAttributeData.GetCustomAttributes(a);
            CustomAttributeData data = null;
            for (int i = 0; i < customAttributes.Count; i++)
            {
                if (customAttributes[i].Constructor.DeclaringType == typeof(NeutralResourcesLanguageAttribute))
                {
                    data = customAttributes[i];
                    break;
                }
            }
            if (data == null)
            {
                fallbackLocation = UltimateResourceFallbackLocation.MainAssembly;
                return CultureInfo.InvariantCulture;
            }
            string name = null;
            if (data.Constructor.GetParameters().Length == 2)
            {
                CustomAttributeTypedArgument argument = data.ConstructorArguments[1];
                fallbackLocation = (UltimateResourceFallbackLocation) argument.Value;
                if ((fallbackLocation < UltimateResourceFallbackLocation.MainAssembly) || (fallbackLocation > UltimateResourceFallbackLocation.Satellite))
                {
                    throw new ArgumentException(Environment.GetResourceString("Arg_InvalidNeutralResourcesLanguage_FallbackLoc", new object[] { (UltimateResourceFallbackLocation) fallbackLocation }));
                }
            }
            else
            {
                fallbackLocation = UltimateResourceFallbackLocation.MainAssembly;
            }
            CustomAttributeTypedArgument argument2 = data.ConstructorArguments[0];
            name = argument2.Value as string;
            try
            {
                return CultureInfo.GetCultureInfo(name);
            }
            catch (ArgumentException exception)
            {
                if (a != typeof(object).Assembly)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Arg_InvalidNeutralResourcesLanguage_Asm_Culture"), new object[] { a.ToString(), name }), exception);
                }
                return CultureInfo.InvariantCulture;
            }
        }

        public virtual object GetObject(string name) => 
            this.GetObject(name, null, true);

        public virtual object GetObject(string name, CultureInfo culture) => 
            this.GetObject(name, culture, true);

        private object GetObject(string name, CultureInfo culture, bool wrapUnmanagedMemStream)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }
            ResourceSet set = this.InternalGetResourceSet(culture, true, true);
            if (set != null)
            {
                object obj2 = set.GetObject(name, this._ignoreCase);
                if (obj2 != null)
                {
                    UnmanagedMemoryStream stream = obj2 as UnmanagedMemoryStream;
                    if ((stream != null) && wrapUnmanagedMemStream)
                    {
                        return new UnmanagedMemoryStreamWrapper(stream);
                    }
                    return obj2;
                }
            }
            ResourceSet set2 = null;
            while (!culture.Equals(CultureInfo.InvariantCulture) && !culture.Equals(this._neutralResourcesCulture))
            {
                culture = culture.Parent;
                set = this.InternalGetResourceSet(culture, true, true);
                if (set == null)
                {
                    break;
                }
                if (set != set2)
                {
                    object obj3 = set.GetObject(name, this._ignoreCase);
                    if (obj3 != null)
                    {
                        UnmanagedMemoryStream stream2 = obj3 as UnmanagedMemoryStream;
                        if ((stream2 != null) && wrapUnmanagedMemStream)
                        {
                            return new UnmanagedMemoryStreamWrapper(stream2);
                        }
                        return obj3;
                    }
                    set2 = set;
                }
            }
            return null;
        }

        protected virtual string GetResourceFileName(CultureInfo culture)
        {
            StringBuilder builder = new StringBuilder(0xff);
            builder.Append(this.BaseNameField);
            if (!culture.Equals(CultureInfo.InvariantCulture))
            {
                CultureInfo.VerifyCultureName(culture, true);
                builder.Append('.');
                builder.Append(culture.Name);
            }
            builder.Append(".resources");
            return builder.ToString();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public virtual ResourceSet GetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            ResourceSet set;
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }
            Hashtable resourceSets = this.ResourceSets;
            if (resourceSets != null)
            {
                set = (ResourceSet) resourceSets[culture];
                if (set != null)
                {
                    return set;
                }
            }
            if (this.UseManifest && culture.Equals(CultureInfo.InvariantCulture))
            {
                StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
                string resourceFileName = this.GetResourceFileName(culture);
                Stream store = this.MainAssembly.GetManifestResourceStream(this._locationInfo, resourceFileName, this._callingAssembly == this.MainAssembly, ref lookForMyCaller);
                if (createIfNotExists && (store != null))
                {
                    set = this.CreateResourceSet(store, this.MainAssembly);
                    lock (resourceSets)
                    {
                        resourceSets.Add(culture, set);
                    }
                    return set;
                }
            }
            return this.InternalGetResourceSet(culture, createIfNotExists, tryParents);
        }

        private Hashtable GetSatelliteAssembliesFromConfig()
        {
            string configurationFileInternal = AppDomain.CurrentDomain.FusionStore.ConfigurationFileInternal;
            if (configurationFileInternal == null)
            {
                return null;
            }
            if (((configurationFileInternal.Length >= 2) && ((configurationFileInternal[1] == Path.VolumeSeparatorChar) || ((configurationFileInternal[0] == Path.DirectorySeparatorChar) && (configurationFileInternal[1] == Path.DirectorySeparatorChar)))) && !File.InternalExists(configurationFileInternal))
            {
                return null;
            }
            ConfigTreeParser parser = new ConfigTreeParser();
            string configPath = "/configuration/satelliteassemblies";
            ConfigNode node = null;
            try
            {
                node = parser.Parse(configurationFileInternal, configPath, true);
            }
            catch (Exception)
            {
            }
            if (node == null)
            {
                return null;
            }
            Hashtable hashtable = new Hashtable(StringComparer.OrdinalIgnoreCase);
            foreach (ConfigNode node2 in node.Children)
            {
                if (!string.Equals(node2.Name, "assembly"))
                {
                    throw new ApplicationException(Environment.GetResourceString("XMLSyntax_InvalidSyntaxSatAssemTag", new object[] { Path.GetFileName(configurationFileInternal), node2.Name }));
                }
                if (node2.Attributes.Count != 1)
                {
                    throw new ApplicationException(Environment.GetResourceString("XMLSyntax_InvalidSyntaxSatAssemTagBadAttr", new object[] { Path.GetFileName(configurationFileInternal) }));
                }
                DictionaryEntry entry = (DictionaryEntry) node2.Attributes[0];
                string key = (string) entry.Value;
                if ((!object.Equals(entry.Key, "name") || (key == null)) || (key.Length == 0))
                {
                    throw new ApplicationException(Environment.GetResourceString("XMLSyntax_InvalidSyntaxSatAssemTagBadAttr", new object[] { Path.GetFileName(configurationFileInternal), entry.Key, entry.Value }));
                }
                ArrayList list = new ArrayList(5);
                foreach (ConfigNode node3 in node2.Children)
                {
                    if (node3.Value != null)
                    {
                        list.Add(node3.Value);
                    }
                }
                CultureInfo[] infoArray = new CultureInfo[list.Count];
                for (int i = 0; i < infoArray.Length; i++)
                {
                    infoArray[i] = CultureInfo.GetCultureInfo((string) list[i]);
                }
                hashtable.Add(key, infoArray);
            }
            return hashtable;
        }

        private Assembly GetSatelliteAssembly(CultureInfo lookForCulture)
        {
            if (!this._lookedForSatelliteContractVersion)
            {
                this._satelliteContractVersion = GetSatelliteContractVersion(this.MainAssembly);
                this._lookedForSatelliteContractVersion = true;
            }
            Assembly assembly = null;
            try
            {
                assembly = this.MainAssembly.InternalGetSatelliteAssembly(lookForCulture, this._satelliteContractVersion, false);
            }
            catch (FileLoadException exception)
            {
                Win32Native.MakeHRFromErrorCode(5);
                Marshal.GetHRForException(exception);
            }
            catch (BadImageFormatException)
            {
            }
            return assembly;
        }

        protected static Version GetSatelliteContractVersion(Assembly a)
        {
            string str = null;
            Version version;
            foreach (CustomAttributeData data in CustomAttributeData.GetCustomAttributes(a))
            {
                if (data.Constructor.DeclaringType == typeof(SatelliteContractVersionAttribute))
                {
                    CustomAttributeTypedArgument argument = data.ConstructorArguments[0];
                    str = (string) argument.Value;
                    break;
                }
            }
            if (str == null)
            {
                return null;
            }
            try
            {
                version = new Version(str);
            }
            catch (Exception exception)
            {
                if (a != typeof(object).Assembly)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Arg_InvalidSatelliteContract_Asm_Ver"), new object[] { a.ToString(), str }), exception);
                }
                return null;
            }
            return version;
        }

        [ComVisible(false), CLSCompliant(false)]
        public UnmanagedMemoryStream GetStream(string name) => 
            this.GetStream(name, null);

        [CLSCompliant(false), ComVisible(false)]
        public UnmanagedMemoryStream GetStream(string name, CultureInfo culture)
        {
            object obj2 = this.GetObject(name, culture, false);
            UnmanagedMemoryStream stream = obj2 as UnmanagedMemoryStream;
            if ((stream == null) && (obj2 != null))
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ResourceNotStream_Name", new object[] { name }));
            }
            return stream;
        }

        public virtual string GetString(string name) => 
            this.GetString(name, null);

        public virtual string GetString(string name, CultureInfo culture)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (culture == null)
            {
                culture = CultureInfo.CurrentUICulture;
            }
            ResourceSet set = this.InternalGetResourceSet(culture, true, true);
            if (set != null)
            {
                string str = set.GetString(name, this._ignoreCase);
                if (str != null)
                {
                    return str;
                }
            }
            ResourceSet set2 = null;
            while (!culture.Equals(CultureInfo.InvariantCulture) && !culture.Equals(this._neutralResourcesCulture))
            {
                culture = culture.Parent;
                set = this.InternalGetResourceSet(culture, true, true);
                if (set == null)
                {
                    break;
                }
                if (set != set2)
                {
                    string str2 = set.GetString(name, this._ignoreCase);
                    if (str2 != null)
                    {
                        return str2;
                    }
                    set2 = set;
                }
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected virtual ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            Hashtable resourceSets = this.ResourceSets;
            ResourceSet rs = (ResourceSet) resourceSets[culture];
            if (rs == null)
            {
                Stream store = null;
                string resourceFileName = null;
                Assembly satellite = null;
                if (this.UseManifest)
                {
                    resourceFileName = this.GetResourceFileName(culture);
                    StackCrawlMark lookForMyCaller = StackCrawlMark.LookForMyCaller;
                    if (this.UseSatelliteAssem)
                    {
                        CultureInfo invariantCulture = culture;
                        if (this._neutralResourcesCulture == null)
                        {
                            this._neutralResourcesCulture = GetNeutralResourcesLanguage(this.MainAssembly, ref this._fallbackLoc);
                        }
                        if (culture.Equals(this._neutralResourcesCulture) && (this.FallbackLocation == UltimateResourceFallbackLocation.MainAssembly))
                        {
                            invariantCulture = CultureInfo.InvariantCulture;
                            resourceFileName = this.GetResourceFileName(invariantCulture);
                        }
                        if (invariantCulture.Equals(CultureInfo.InvariantCulture))
                        {
                            if (this.FallbackLocation == UltimateResourceFallbackLocation.Satellite)
                            {
                                satellite = this.GetSatelliteAssembly(this._neutralResourcesCulture);
                                if (satellite == null)
                                {
                                    string str2 = this.MainAssembly.nGetSimpleName() + ".resources.dll";
                                    if (this._satelliteContractVersion != null)
                                    {
                                        str2 = str2 + ", Version=" + this._satelliteContractVersion.ToString();
                                    }
                                    AssemblyName name = new AssemblyName();
                                    name.SetPublicKey(this.MainAssembly.nGetPublicKey());
                                    byte[] publicKeyToken = name.GetPublicKeyToken();
                                    int length = publicKeyToken.Length;
                                    StringBuilder builder = new StringBuilder(length * 2);
                                    for (int i = 0; i < length; i++)
                                    {
                                        builder.Append(publicKeyToken[i].ToString("x", CultureInfo.InvariantCulture));
                                    }
                                    str2 = str2 + ", PublicKeyToken=" + builder;
                                    string cultureName = this._neutralResourcesCulture.Name;
                                    if (cultureName.Length == 0)
                                    {
                                        cultureName = "<invariant>";
                                    }
                                    throw new MissingSatelliteAssemblyException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("MissingSatelliteAssembly_Culture_Name"), new object[] { this._neutralResourcesCulture, str2 }), cultureName);
                                }
                                resourceFileName = this.GetResourceFileName(this._neutralResourcesCulture);
                            }
                            else
                            {
                                satellite = this.MainAssembly;
                            }
                        }
                        else if (!this.TryLookingForSatellite(invariantCulture))
                        {
                            satellite = null;
                        }
                        else
                        {
                            satellite = this.GetSatelliteAssembly(invariantCulture);
                        }
                        if (satellite != null)
                        {
                            rs = (ResourceSet) resourceSets[invariantCulture];
                            if (rs != null)
                            {
                                return rs;
                            }
                            bool skipSecurityCheck = (this.MainAssembly == satellite) && (this._callingAssembly == this.MainAssembly);
                            store = satellite.GetManifestResourceStream(this._locationInfo, resourceFileName, skipSecurityCheck, ref lookForMyCaller);
                            if (store == null)
                            {
                                store = this.CaseInsensitiveManifestResourceStreamLookup(satellite, resourceFileName);
                            }
                        }
                    }
                    else
                    {
                        satellite = this.MainAssembly;
                        store = this.MainAssembly.GetManifestResourceStream(this._locationInfo, resourceFileName, this._callingAssembly == this.MainAssembly, ref lookForMyCaller);
                    }
                    if ((store == null) && tryParents)
                    {
                        if (culture.Equals(CultureInfo.InvariantCulture))
                        {
                            if ((this.MainAssembly == typeof(object).Assembly) && this.BaseName.Equals("mscorlib"))
                            {
                                throw new ExecutionEngineException("mscorlib.resources couldn't be found!  Large parts of the BCL won't work!");
                            }
                            string str4 = string.Empty;
                            if ((this._locationInfo != null) && (this._locationInfo.Namespace != null))
                            {
                                str4 = this._locationInfo.Namespace + Type.Delimiter;
                            }
                            str4 = str4 + resourceFileName;
                            throw new MissingManifestResourceException(Environment.GetResourceString("MissingManifestResource_NoNeutralAsm", new object[] { str4, this.MainAssembly.nGetSimpleName() }));
                        }
                        CultureInfo parent = culture.Parent;
                        rs = this.InternalGetResourceSet(parent, createIfNotExists, tryParents);
                        if (rs != null)
                        {
                            AddResourceSet(resourceSets, culture, ref rs);
                        }
                        return rs;
                    }
                }
                else
                {
                    new FileIOPermission(PermissionState.Unrestricted).Assert();
                    resourceFileName = this.FindResourceFile(culture);
                    if (resourceFileName == null)
                    {
                        if (tryParents)
                        {
                            if (culture.Equals(CultureInfo.InvariantCulture))
                            {
                                throw new MissingManifestResourceException(Environment.GetResourceString("MissingManifestResource_NoNeutralDisk") + Environment.NewLine + "baseName: " + this.BaseNameField + "  locationInfo: " + ((this._locationInfo == null) ? "<null>" : this._locationInfo.FullName) + "  fileName: " + this.GetResourceFileName(culture));
                            }
                            CultureInfo info3 = culture.Parent;
                            rs = this.InternalGetResourceSet(info3, createIfNotExists, tryParents);
                            if (rs != null)
                            {
                                AddResourceSet(resourceSets, culture, ref rs);
                            }
                            return rs;
                        }
                    }
                    else
                    {
                        rs = this.CreateResourceSet(resourceFileName);
                        if (rs != null)
                        {
                            AddResourceSet(resourceSets, culture, ref rs);
                        }
                        return rs;
                    }
                }
                if ((createIfNotExists && (store != null)) && (rs == null))
                {
                    rs = this.CreateResourceSet(store, satellite);
                    AddResourceSet(resourceSets, culture, ref rs);
                }
            }
            return rs;
        }

        public virtual void ReleaseAllResources()
        {
            IDictionaryEnumerator enumerator = this.ResourceSets.GetEnumerator();
            this.ResourceSets = new Hashtable();
            while (enumerator.MoveNext())
            {
                ((ResourceSet) enumerator.Value).Close();
            }
        }

        private bool TryLookingForSatellite(CultureInfo lookForCulture)
        {
            if (!_checkedConfigFile)
            {
                lock (this)
                {
                    if (!_checkedConfigFile)
                    {
                        _checkedConfigFile = true;
                        _installedSatelliteInfo = this.GetSatelliteAssembliesFromConfig();
                    }
                }
            }
            if (_installedSatelliteInfo == null)
            {
                return true;
            }
            CultureInfo[] array = (CultureInfo[]) _installedSatelliteInfo[this.MainAssembly.FullName];
            return ((array == null) || (Array.IndexOf<CultureInfo>(array, lookForCulture) >= 0));
        }

        public virtual string BaseName =>
            this.BaseNameField;

        protected UltimateResourceFallbackLocation FallbackLocation
        {
            get => 
                this._fallbackLoc;
            set
            {
                this._fallbackLoc = value;
            }
        }

        public virtual bool IgnoreCase
        {
            get => 
                this._ignoreCase;
            set
            {
                this._ignoreCase = value;
            }
        }

        public virtual Type ResourceSetType
        {
            get
            {
                if (this._userResourceSet != null)
                {
                    return this._userResourceSet;
                }
                return typeof(RuntimeResourceSet);
            }
        }
    }
}

