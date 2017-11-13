namespace System.CodeDom.Compiler
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal static class RedistVersionInfo
    {
        internal const string DefaultVersion = "v2.0";
        private const string dotNetFrameworkRegistryPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\3.5";
        private const string dotNetFrameworkSdkInstallKeyValueV35 = "MSBuildToolsPath";
        internal const string InPlaceVersion = "v2.0";
        internal const string NameTag = "CompilerVersion";
        internal const string RedistVersion = "v3.5";

        public static string GetCompilerPath(IDictionary<string, string> provOptions, string compilerExecutable)
        {
            string str2;
            string runtimeInstallDirectory = Executor.GetRuntimeInstallDirectory();
            if ((provOptions != null) && provOptions.TryGetValue("CompilerVersion", out str2))
            {
                switch (str2)
                {
                    case "v3.5":
                        runtimeInstallDirectory = GetOrcasPath();
                        goto Label_0043;

                    case "v2.0":
                        goto Label_0043;
                }
                runtimeInstallDirectory = null;
            }
        Label_0043:
            if (runtimeInstallDirectory == null)
            {
                throw new InvalidOperationException(SR.GetString("CompilerNotFound", new object[] { compilerExecutable }));
            }
            return runtimeInstallDirectory;
        }

        private static string GetOrcasPath()
        {
            string path = null;
            string environmentVariable = Environment.GetEnvironmentVariable("COMPLUS_InstallRoot");
            string str3 = Environment.GetEnvironmentVariable("COMPLUS_Version");
            if (!string.IsNullOrEmpty(environmentVariable) && !string.IsNullOrEmpty(str3))
            {
                path = Path.Combine(environmentVariable, str3);
                if (Directory.Exists(path))
                {
                    return path;
                }
            }
            path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSBuild\ToolsVersions\3.5", "MSBuildToolsPath", null) as string;
            if ((path != null) && Directory.Exists(path))
            {
                return path;
            }
            return null;
        }
    }
}

