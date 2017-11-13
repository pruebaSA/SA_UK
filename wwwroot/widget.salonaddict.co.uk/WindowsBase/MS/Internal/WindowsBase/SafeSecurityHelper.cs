namespace MS.Internal.WindowsBase
{
    using System;
    using System.Globalization;
    using System.Reflection;

    internal static class SafeSecurityHelper
    {
        internal static readonly CultureInfo DOTNETCULTURE = TypeConverterHelper.EnglishUSCulture;
        internal const string IMAGE = "image";

        internal static Assembly GetLoadedAssembly(AssemblyName assemblyName)
        {
            Version version = assemblyName.Version;
            CultureInfo cultureInfo = assemblyName.CultureInfo;
            byte[] publicKeyToken = assemblyName.GetPublicKeyToken();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = assemblies.Length - 1; i >= 0; i--)
            {
                AssemblyName name = new AssemblyName(assemblies[i].FullName);
                Version version2 = name.Version;
                CultureInfo info2 = name.CultureInfo;
                byte[] curKeyToken = name.GetPublicKeyToken();
                if ((((string.Compare(name.Name, assemblyName.Name, true, DOTNETCULTURE) == 0) && ((version == null) || version.Equals(version2))) && ((cultureInfo == null) || cultureInfo.Equals(info2))) && ((publicKeyToken == null) || IsSameKeyToken(publicKeyToken, curKeyToken)))
                {
                    return assemblies[i];
                }
            }
            return null;
        }

        private static bool IsSameKeyToken(byte[] reqKeyToken, byte[] curKeyToken)
        {
            bool flag = false;
            if ((reqKeyToken == null) && (curKeyToken == null))
            {
                return true;
            }
            if (((reqKeyToken != null) && (curKeyToken != null)) && (reqKeyToken.Length == curKeyToken.Length))
            {
                flag = true;
                for (int i = 0; i < reqKeyToken.Length; i++)
                {
                    if (reqKeyToken[i] != curKeyToken[i])
                    {
                        return false;
                    }
                }
            }
            return flag;
        }
    }
}

