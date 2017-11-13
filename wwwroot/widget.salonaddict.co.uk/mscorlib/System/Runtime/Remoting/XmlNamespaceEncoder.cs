namespace System.Runtime.Remoting
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class XmlNamespaceEncoder
    {
        internal static string GetTypeNameForSoapActionNamespace(string uri, out bool assemblyIncluded)
        {
            assemblyIncluded = false;
            string fullNS = SoapServices.fullNS;
            string namespaceNS = SoapServices.namespaceNS;
            if (uri.StartsWith(fullNS, StringComparison.Ordinal))
            {
                uri = uri.Substring(fullNS.Length);
                char[] separator = new char[] { '/' };
                string[] strArray = uri.Split(separator);
                if (strArray.Length != 2)
                {
                    return null;
                }
                assemblyIncluded = true;
                return (strArray[0] + ", " + strArray[1]);
            }
            if (uri.StartsWith(namespaceNS, StringComparison.Ordinal))
            {
                string str3 = typeof(string).Module.Assembly.nGetSimpleName();
                assemblyIncluded = true;
                return (uri.Substring(namespaceNS.Length) + ", " + str3);
            }
            return null;
        }

        internal static string GetXmlNamespaceForType(Type type, string dynamicUrl)
        {
            string fullName = type.FullName;
            Assembly assembly = type.Module.Assembly;
            StringBuilder builder = new StringBuilder(0x100);
            Assembly assembly2 = typeof(string).Module.Assembly;
            if (assembly == assembly2)
            {
                builder.Append(SoapServices.namespaceNS);
                builder.Append(fullName);
            }
            else
            {
                builder.Append(SoapServices.fullNS);
                builder.Append(fullName);
                builder.Append('/');
                builder.Append(assembly.nGetSimpleName());
            }
            return builder.ToString();
        }

        internal static string GetXmlNamespaceForTypeNamespace(Type type, string dynamicUrl)
        {
            string str = type.Namespace;
            Assembly assembly = type.Module.Assembly;
            StringBuilder builder = new StringBuilder(0x100);
            Assembly assembly2 = typeof(string).Module.Assembly;
            if (assembly == assembly2)
            {
                builder.Append(SoapServices.namespaceNS);
                builder.Append(str);
            }
            else
            {
                builder.Append(SoapServices.fullNS);
                builder.Append(str);
                builder.Append('/');
                builder.Append(assembly.nGetSimpleName());
            }
            return builder.ToString();
        }
    }
}

