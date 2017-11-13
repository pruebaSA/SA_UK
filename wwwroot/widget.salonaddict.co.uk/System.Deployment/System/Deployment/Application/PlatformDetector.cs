namespace System.Deployment.Application
{
    using System;
    using System.ComponentModel;
    using System.Deployment.Application.Manifest;
    using System.Deployment.Application.Win32InterOp;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class PlatformDetector
    {
        private const int MAX_PATH = 260;
        private static Product[] Products = new Product[] { new Product("workstation", 1), new Product("domainController", 2), new Product("server", 3) };
        private const uint RUNTIME_INFO_DONT_RETURN_DIRECTORY = 0x10;
        private const uint RUNTIME_INFO_DONT_RETURN_VERSION = 0x20;
        private const uint RUNTIME_INFO_DONT_SHOW_ERROR_DIALOG = 0x40;
        private const uint RUNTIME_INFO_REQUEST_AMD64 = 4;
        private const uint RUNTIME_INFO_REQUEST_IA64 = 2;
        private const uint RUNTIME_INFO_REQUEST_X86 = 8;
        private const uint RUNTIME_INFO_UPGRADE_VERSION = 1;
        private static Suite[] Suites = new Suite[] { new Suite("server", 0x80000000), new Suite("workstation", 0x40000000), new Suite("smallbusiness", 1), new Suite("enterprise", 2), new Suite("backoffice", 4), new Suite("communications", 8), new Suite("terminal", 0x10), new Suite("smallbusinessRestricted", 0x20), new Suite("embeddednt", 0x40), new Suite("datacenter", 0x80), new Suite("singleuserts", 0x100), new Suite("personal", 0x200), new Suite("blade", 0x400), new Suite("embeddedrestricted", 0x800) };
        private const byte VER_AND = 6;
        private const uint VER_BUILDNUMBER = 4;
        private const byte VER_EQUAL = 1;
        private const byte VER_GREATER = 2;
        private const byte VER_GREATER_EQUAL = 3;
        private const byte VER_LESS = 4;
        private const byte VER_LESS_EQUAL = 5;
        private const uint VER_MAJORVERSION = 2;
        private const uint VER_MINORVERSION = 1;
        private const uint VER_NT_DOMAIN_CONTROLLER = 2;
        private const uint VER_NT_SERVER = 3;
        private const uint VER_NT_WORKSTATION = 1;
        private const byte VER_OR = 7;
        private const uint VER_PLATFORMID = 8;
        private const uint VER_PRODUCT_TYPE = 0x80;
        private const uint VER_SERVER_NT = 0x80000000;
        private const uint VER_SERVICEPACKMAJOR = 0x20;
        private const uint VER_SERVICEPACKMINOR = 0x10;
        private const uint VER_SUITE_BACKOFFICE = 4;
        private const uint VER_SUITE_BLADE = 0x400;
        private const uint VER_SUITE_COMMUNICATIONS = 8;
        private const uint VER_SUITE_DATACENTER = 0x80;
        private const uint VER_SUITE_EMBEDDED_RESTRICTED = 0x800;
        private const uint VER_SUITE_EMBEDDEDNT = 0x40;
        private const uint VER_SUITE_ENTERPRISE = 2;
        private const uint VER_SUITE_PERSONAL = 0x200;
        private const uint VER_SUITE_SINGLEUSERTS = 0x100;
        private const uint VER_SUITE_SMALLBUSINESS = 1;
        private const uint VER_SUITE_SMALLBUSINESS_RESTRICTED = 0x20;
        private const uint VER_SUITE_TERMINAL = 0x10;
        private const uint VER_SUITENAME = 0x40;
        private const uint VER_WORKSTATION_NT = 0x40000000;
        private const uint Windows9XMajorVersion = 4;

        private static NetFX35SP1SKU GetPlatformNetFx35SKU(string tempDir)
        {
            ReferenceIdentity refId = new ReferenceIdentity("Sentinel.v3.5Client, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a,processorArchitecture=msil");
            ReferenceIdentity identity2 = new ReferenceIdentity("System.Data.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089,processorArchitecture=msil");
            bool flag = false;
            bool flag2 = false;
            if (VerifyGACDependency(refId, tempDir))
            {
                flag = true;
            }
            if (VerifyGACDependency(identity2, tempDir))
            {
                flag2 = true;
            }
            if (flag && !flag2)
            {
                return NetFX35SP1SKU.Client35SP1;
            }
            if (flag && flag2)
            {
                return NetFX35SP1SKU.Full35SP1;
            }
            return NetFX35SP1SKU.No35SP1;
        }

        public static bool IsCLRDependencyText(string clrTextName)
        {
            if ((string.Compare(clrTextName, "Microsoft-Windows-CLRCoreComp", StringComparison.OrdinalIgnoreCase) != 0) && (string.Compare(clrTextName, "Microsoft.Windows.CommonLanguageRuntime", StringComparison.OrdinalIgnoreCase) != 0))
            {
                return false;
            }
            return true;
        }

        private static bool IsNetFX35SP1ClientSignatureAsm(ReferenceIdentity ra)
        {
            DefinitionIdentity identity = new DefinitionIdentity("Sentinel.v3.5Client, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a,processorArchitecture=msil");
            return identity.Matches(ra, true);
        }

        private static bool IsNetFX35SP1FullSignatureAsm(ReferenceIdentity ra)
        {
            DefinitionIdentity identity = new DefinitionIdentity("System.Data.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089,processorArchitecture=msil");
            return identity.Matches(ra, true);
        }

        public static bool IsSupportedProcessorArchitecture(string arch)
        {
            if ((string.Compare(arch, "msil", StringComparison.OrdinalIgnoreCase) == 0) || (string.Compare(arch, "x86", StringComparison.OrdinalIgnoreCase) == 0))
            {
                return true;
            }
            NativeMethods.SYSTEM_INFO sysInfo = new NativeMethods.SYSTEM_INFO();
            bool flag = false;
            try
            {
                NativeMethods.GetNativeSystemInfo(ref sysInfo);
                flag = true;
            }
            catch (EntryPointNotFoundException)
            {
                flag = false;
            }
            if (!flag)
            {
                NativeMethods.GetSystemInfo(ref sysInfo);
            }
            switch (sysInfo.uProcessorInfo.wProcessorArchitecture)
            {
                case 6:
                    return (string.Compare(arch, "ia64", StringComparison.OrdinalIgnoreCase) == 0);

                case 9:
                    return (string.Compare(arch, "amd64", StringComparison.OrdinalIgnoreCase) == 0);
            }
            return false;
        }

        public static bool VerifyCLRVersionInfo(Version v, string procArch)
        {
            bool flag = true;
            NameMap[] nmArray = new NameMap[] { new NameMap("x86", 8), new Product("ia64", 2), new Product("amd64", 4) };
            uint runtimeInfoFlags = NameMap.MapNameToMask(procArch, nmArray) | 0x41;
            StringBuilder pDirectory = new StringBuilder(260);
            StringBuilder pVersion = new StringBuilder("v65535.65535.65535".Length);
            uint dwDirectoryLength = 0;
            uint dwLength = 0;
            string pwszVersion = v.ToString(3);
            pwszVersion = "v" + pwszVersion;
            try
            {
                NativeMethods.GetRequestedRuntimeInfo(null, pwszVersion, null, 0, runtimeInfoFlags, pDirectory, (uint) pDirectory.Capacity, out dwDirectoryLength, pVersion, (uint) pVersion.Capacity, out dwLength);
            }
            catch (COMException exception)
            {
                flag = false;
                if (exception.ErrorCode != -2146232576)
                {
                    throw;
                }
            }
            return flag;
        }

        public static bool VerifyGACDependency(ReferenceIdentity refId, string tempDir)
        {
            if (string.Compare(refId.ProcessorArchitecture, "msil", StringComparison.OrdinalIgnoreCase) == 0)
            {
                return VerifyGACDependencyWhidbey(refId);
            }
            if (!VerifyGACDependencyXP(refId, tempDir))
            {
                return VerifyGACDependencyWhidbey(refId);
            }
            return true;
        }

        public static bool VerifyGACDependencyWhidbey(ReferenceIdentity refId)
        {
            NativeMethods.IAssemblyName name;
            NativeMethods.IAssemblyEnum enum2;
            string assemblyName = refId.ToString();
            string text = null;
            try
            {
                text = AppDomain.CurrentDomain.ApplyPolicy(assemblyName);
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (COMException)
            {
                return false;
            }
            ReferenceIdentity identity = new ReferenceIdentity(text) {
                ProcessorArchitecture = refId.ProcessorArchitecture
            };
            string str3 = identity.ToString();
            SystemUtils.AssemblyInfo info = null;
            info = SystemUtils.QueryAssemblyInfo(SystemUtils.QueryAssemblyInfoFlags.All, str3);
            if ((info != null) || (identity.ProcessorArchitecture != null))
            {
                return (info != null);
            }
            NativeMethods.CreateAssemblyNameObject(out name, identity.ToString(), 1, IntPtr.Zero);
            NativeMethods.CreateAssemblyEnum(out enum2, null, name, 2, IntPtr.Zero);
            return (enum2.GetNextAssembly(null, out name, 0) == 0);
        }

        public static bool VerifyGACDependencyXP(ReferenceIdentity refId, string tempDir)
        {
            if (!PlatformSpecific.OnXPOrAbove)
            {
                return false;
            }
            using (TempFile file = new TempFile(tempDir, ".manifest"))
            {
                ManifestGenerator.GenerateGACDetectionManifest(refId, file.Path);
                NativeMethods.ACTCTXW actCtx = new NativeMethods.ACTCTXW(file.Path);
                IntPtr hActCtx = NativeMethods.CreateActCtxW(actCtx);
                if (hActCtx != NativeMethods.INVALID_HANDLE_VALUE)
                {
                    NativeMethods.ReleaseActCtx(hActCtx);
                    return true;
                }
                return false;
            }
        }

        public static bool VerifyOSDependency(ref OSDependency osd)
        {
            OperatingSystem oSVersion = Environment.OSVersion;
            if (oSVersion.Version.Major == 4L)
            {
                if (oSVersion.Version.Major < osd.dwMajorVersion)
                {
                    return false;
                }
                return true;
            }
            NativeMethods.OSVersionInfoEx structure = new NativeMethods.OSVersionInfoEx();
            structure.dwOSVersionInfoSize = (uint) Marshal.SizeOf(structure);
            structure.dwMajorVersion = osd.dwMajorVersion;
            structure.dwMinorVersion = osd.dwMinorVersion;
            structure.dwBuildNumber = osd.dwBuildNumber;
            structure.dwPlatformId = 0;
            structure.szCSDVersion = null;
            structure.wServicePackMajor = osd.wServicePackMajor;
            structure.wServicePackMinor = osd.wServicePackMinor;
            structure.wSuiteMask = (osd.suiteName != null) ? ((ushort) NameMap.MapNameToMask(osd.suiteName, Suites)) : ((ushort) 0);
            structure.bProductType = (osd.productName != null) ? ((byte) NameMap.MapNameToMask(osd.productName, Products)) : ((byte) 0);
            structure.bReserved = 0;
            ulong conditionMask = 0L;
            uint dwTypeMask = (uint) ((((((2 | ((osd.dwMinorVersion != 0) ? 1 : 0)) | ((osd.dwBuildNumber != 0) ? 4 : 0)) | ((osd.suiteName != null) ? 0x40 : 0)) | ((osd.productName != null) ? 0x80 : 0)) | ((osd.wServicePackMajor != 0) ? 0x20 : 0)) | ((osd.wServicePackMinor != 0) ? 0x10 : 0));
            conditionMask = NativeMethods.VerSetConditionMask(conditionMask, 2, 3);
            if (osd.dwMinorVersion != 0)
            {
                conditionMask = NativeMethods.VerSetConditionMask(conditionMask, 1, 3);
            }
            if (osd.dwBuildNumber != 0)
            {
                conditionMask = NativeMethods.VerSetConditionMask(conditionMask, 4, 3);
            }
            if (osd.suiteName != null)
            {
                conditionMask = NativeMethods.VerSetConditionMask(conditionMask, 0x40, 6);
            }
            if (osd.productName != null)
            {
                conditionMask = NativeMethods.VerSetConditionMask(conditionMask, 0x80, 1);
            }
            if (osd.wServicePackMajor != 0)
            {
                conditionMask = NativeMethods.VerSetConditionMask(conditionMask, 0x20, 3);
            }
            if (osd.wServicePackMinor != 0)
            {
                conditionMask = NativeMethods.VerSetConditionMask(conditionMask, 0x10, 3);
            }
            bool flag = NativeMethods.VerifyVersionInfo(structure, dwTypeMask, conditionMask);
            if (!flag)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0x47e)
                {
                    throw new Win32Exception(error);
                }
            }
            return flag;
        }

        public static void VerifyPlatformDependencies(AssemblyManifest appManifest, Uri deploySupportUri, string tempDir)
        {
            string description = null;
            Uri supportUrl = null;
            supportUrl = deploySupportUri;
            DependentOS dependentOS = appManifest.DependentOS;
            if (dependentOS != null)
            {
                OSDependency osd = new OSDependency(dependentOS.MajorVersion, dependentOS.MinorVersion, dependentOS.BuildNumber, dependentOS.ServicePackMajor, dependentOS.ServicePackMinor, null, null);
                if (!VerifyOSDependency(ref osd))
                {
                    StringBuilder builder = new StringBuilder();
                    string str2 = string.Concat(new object[] { dependentOS.MajorVersion, ".", dependentOS.MinorVersion, ".", dependentOS.BuildNumber, ".", dependentOS.ServicePackMajor, dependentOS.ServicePackMinor });
                    builder.AppendFormat(Resources.GetString("PlatformMicrosoftWindowsOperatingSystem"), str2);
                    description = builder.ToString();
                    if (dependentOS.SupportUrl != null)
                    {
                        supportUrl = dependentOS.SupportUrl;
                    }
                    throw new DependentPlatformMissingException(string.Format(CultureInfo.CurrentUICulture, Resources.GetString("ErrorMessage_PlatformDetectionFailed"), new object[] { description }), supportUrl);
                }
            }
            bool flag = false;
            bool flag2 = false;
            foreach (DependentAssembly assembly in appManifest.DependentAssemblies)
            {
                if (assembly.IsPreRequisite && IsCLRDependencyText(assembly.Identity.Name))
                {
                    Version v = assembly.Identity.Version;
                    string processorArchitecture = assembly.Identity.ProcessorArchitecture;
                    if (!VerifyCLRVersionInfo(v, processorArchitecture))
                    {
                        StringBuilder builder2 = new StringBuilder();
                        builder2.AppendFormat(Resources.GetString("PlatformMicrosoftCommonLanguageRuntime"), v.ToString());
                        description = builder2.ToString();
                        if (assembly.SupportUrl != null)
                        {
                            supportUrl = assembly.SupportUrl;
                        }
                        throw new DependentPlatformMissingException(string.Format(CultureInfo.CurrentUICulture, Resources.GetString("ErrorMessage_PlatformDetectionFailed"), new object[] { description }), supportUrl);
                    }
                }
                if (assembly.IsPreRequisite && IsNetFX35SP1ClientSignatureAsm(assembly.Identity))
                {
                    flag = true;
                }
                if (assembly.IsPreRequisite && IsNetFX35SP1FullSignatureAsm(assembly.Identity))
                {
                    flag2 = true;
                }
            }
            if (!PolicyKeys.SkipSKUDetection())
            {
                if (((GetPlatformNetFx35SKU(tempDir) == NetFX35SP1SKU.Client35SP1) && !flag) && !flag2)
                {
                    description = ".NET Framework 3.5 SP1";
                    throw new DependentPlatformMissingException(string.Format(CultureInfo.CurrentUICulture, Resources.GetString("ErrorMessage_PlatformDetectionFailed"), new object[] { description }));
                }
            }
            foreach (DependentAssembly assembly2 in appManifest.DependentAssemblies)
            {
                if ((assembly2.IsPreRequisite && !IsCLRDependencyText(assembly2.Identity.Name)) && !VerifyGACDependency(assembly2.Identity, tempDir))
                {
                    if (assembly2.Description != null)
                    {
                        description = assembly2.Description;
                    }
                    else
                    {
                        ReferenceIdentity identity = assembly2.Identity;
                        StringBuilder builder3 = new StringBuilder();
                        builder3.AppendFormat(Resources.GetString("PlatformDependentAssemblyVersion"), identity.Name, identity.Version);
                        description = builder3.ToString();
                    }
                    if (assembly2.SupportUrl != null)
                    {
                        supportUrl = assembly2.SupportUrl;
                    }
                    throw new DependentPlatformMissingException(string.Format(CultureInfo.CurrentUICulture, Resources.GetString("ErrorMessage_PlatformGACDetectionFailed"), new object[] { description }), supportUrl);
                }
            }
        }

        public class NameMap
        {
            public uint mask;
            public string name;

            public NameMap(string Name, uint Mask)
            {
                this.name = Name;
                this.mask = Mask;
            }

            public static string MapMaskToName(uint mask, PlatformDetector.NameMap[] nmArray)
            {
                foreach (PlatformDetector.NameMap map in nmArray)
                {
                    if (map.mask == mask)
                    {
                        return map.name;
                    }
                }
                return null;
            }

            public static uint MapNameToMask(string name, PlatformDetector.NameMap[] nmArray)
            {
                foreach (PlatformDetector.NameMap map in nmArray)
                {
                    if (map.name == name)
                    {
                        return map.mask;
                    }
                }
                return 0;
            }
        }

        private enum NetFX35SP1SKU
        {
            No35SP1,
            Client35SP1,
            Full35SP1
        }

        public class OSDependency
        {
            public uint dwBuildNumber;
            public uint dwMajorVersion;
            public uint dwMinorVersion;
            public string productName;
            public string suiteName;
            public ushort wServicePackMajor;
            public ushort wServicePackMinor;

            public OSDependency()
            {
            }

            public OSDependency(NativeMethods.OSVersionInfoEx osvi)
            {
                this.dwMajorVersion = osvi.dwMajorVersion;
                this.dwMinorVersion = osvi.dwMinorVersion;
                this.dwMajorVersion = osvi.dwBuildNumber;
                this.dwMajorVersion = osvi.wServicePackMajor;
                this.dwMajorVersion = osvi.wServicePackMinor;
                this.suiteName = PlatformDetector.NameMap.MapMaskToName(osvi.wSuiteMask, PlatformDetector.Suites);
                this.productName = PlatformDetector.NameMap.MapMaskToName(osvi.bProductType, PlatformDetector.Products);
            }

            public OSDependency(uint dwMajorVersion, uint dwMinorVersion, uint dwBuildNumber, ushort wServicePackMajor, ushort wServicePackMinor, string suiteName, string productName)
            {
                this.dwMajorVersion = dwMajorVersion;
                this.dwMinorVersion = dwMinorVersion;
                this.dwBuildNumber = dwBuildNumber;
                this.wServicePackMajor = wServicePackMajor;
                this.wServicePackMinor = wServicePackMinor;
                this.suiteName = suiteName;
                this.productName = productName;
            }
        }

        public class Product : PlatformDetector.NameMap
        {
            public Product(string Name, uint Mask) : base(Name, Mask)
            {
            }
        }

        public class Suite : PlatformDetector.NameMap
        {
            public Suite(string Name, uint Mask) : base(Name, Mask)
            {
            }
        }
    }
}

