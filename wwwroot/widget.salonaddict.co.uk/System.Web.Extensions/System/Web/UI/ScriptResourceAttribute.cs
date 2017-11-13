namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Resources;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;
    using System.Web.Handlers;
    using System.Web.Resources;
    using System.Web.Script.Serialization;
    using System.Web.Util;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ScriptResourceAttribute : Attribute
    {
        private string _scriptName;
        private string _scriptResourceName;
        private string _typeName;
        private static readonly Regex _webResourceRegEx = new Regex("<%\\s*=\\s*(?<resourceType>WebResource|ScriptResource)\\(\"(?<resourceName>[^\"]*)\"\\)\\s*%>", RegexOptions.Singleline | RegexOptions.Multiline);

        public ScriptResourceAttribute(string scriptName, string scriptResourceName, string typeName)
        {
            if (string.IsNullOrEmpty(scriptName))
            {
                throw new ArgumentException(AtlasWeb.Common_NullOrEmpty, "scriptName");
            }
            if (string.IsNullOrEmpty(scriptResourceName))
            {
                throw new ArgumentException(AtlasWeb.Common_NullOrEmpty, "scriptResourceName");
            }
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentException(AtlasWeb.Common_NullOrEmpty, "typeName");
            }
            this._scriptName = scriptName;
            this._scriptResourceName = scriptResourceName;
            this._typeName = typeName;
        }

        private static void CopyScriptToStringBuilderWithSubstitution(string content, Assembly assembly, bool zip, bool notifyScriptLoaded, StringBuilder output)
        {
            MatchCollection matchs = _webResourceRegEx.Matches(content);
            int startIndex = 0;
            foreach (Match match in matchs)
            {
                output.Append(content.Substring(startIndex, match.Index - startIndex));
                Group group = match.Groups["resourceName"];
                string resourceName = group.Value;
                bool flag = string.Equals(match.Groups["resourceType"].Value, "ScriptResource", StringComparison.Ordinal);
                try
                {
                    if (flag)
                    {
                        output.Append(ScriptResourceHandler.GetScriptResourceUrl(assembly, resourceName, CultureInfo.CurrentUICulture, zip, notifyScriptLoaded));
                    }
                    else
                    {
                        output.Append(ScriptResourceHandler.GetWebResourceUrl(assembly, resourceName));
                    }
                }
                catch (HttpException exception)
                {
                    throw new HttpException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ScriptResourceHandler_UnknownResource, new object[] { resourceName }), exception);
                }
                startIndex = match.Index + match.Length;
            }
            output.Append(content.Substring(startIndex, content.Length - startIndex));
        }

        internal static ResourceManager GetResourceManager(string resourceName, Assembly assembly)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                return null;
            }
            return new ResourceManager(GetResourceName(resourceName), assembly);
        }

        private static string GetResourceName(string rawResourceName)
        {
            if (rawResourceName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
            {
                return rawResourceName.Substring(0, rawResourceName.Length - 10);
            }
            return rawResourceName;
        }

        internal static string GetScriptFromWebResourceInternal(Assembly assembly, string resourceName, CultureInfo culture, bool zip, bool notifyScriptLoaded, out string contentType)
        {
            string str2;
            ScriptResourceInfo instance = ScriptResourceInfo.GetInstance(assembly, resourceName);
            ScriptResourceInfo releaseResourceInfo = null;
            if (resourceName.EndsWith(".debug.js", StringComparison.OrdinalIgnoreCase))
            {
                string str = resourceName.Substring(0, resourceName.Length - 9) + ".js";
                releaseResourceInfo = ScriptResourceInfo.GetInstance(assembly, str);
            }
            if ((instance == ScriptResourceInfo.Empty) && ((releaseResourceInfo == null) || (releaseResourceInfo == ScriptResourceInfo.Empty)))
            {
                throw new HttpException(AtlasWeb.ScriptResourceHandler_InvalidRequest);
            }
            ResourceManager resourceManager = null;
            ResourceSet neutralSet = null;
            ResourceManager releaseResourceManager = null;
            ResourceSet releaseNeutralSet = null;
            CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
            try
            {
                Thread.CurrentThread.CurrentUICulture = culture;
                if (!string.IsNullOrEmpty(instance.ScriptResourceName))
                {
                    resourceManager = GetResourceManager(instance.ScriptResourceName, assembly);
                    neutralSet = resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
                }
                if ((releaseResourceInfo != null) && !string.IsNullOrEmpty(releaseResourceInfo.ScriptResourceName))
                {
                    releaseResourceManager = GetResourceManager(releaseResourceInfo.ScriptResourceName, assembly);
                    releaseNeutralSet = releaseResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
                }
                if (((releaseResourceInfo != null) && !string.IsNullOrEmpty(releaseResourceInfo.ScriptResourceName)) && (!string.IsNullOrEmpty(instance.ScriptResourceName) && (releaseResourceInfo.TypeName != instance.TypeName)))
                {
                    throw new HttpException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ScriptResourceHandler_TypeNameMismatch, new object[] { releaseResourceInfo.ScriptResourceName }));
                }
                StringBuilder output = new StringBuilder();
                WriteScript(assembly, instance, releaseResourceInfo, resourceManager, neutralSet, releaseResourceManager, releaseNeutralSet, zip, notifyScriptLoaded, output);
                contentType = instance.ContentType;
                str2 = output.ToString();
            }
            finally
            {
                Thread.CurrentThread.CurrentUICulture = currentUICulture;
                if (releaseNeutralSet != null)
                {
                    releaseNeutralSet.Dispose();
                }
                if (neutralSet != null)
                {
                    neutralSet.Dispose();
                }
            }
            return str2;
        }

        private static void RegisterNamespace(StringBuilder builder, string typeName, bool isDebug)
        {
            int length = typeName.LastIndexOf('.');
            if (length != -1)
            {
                builder.Append("Type.registerNamespace('");
                builder.Append(typeName.Substring(0, length));
                builder.Append("');");
                if (isDebug)
                {
                    builder.AppendLine();
                }
            }
        }

        internal static void WriteNotificationToStringBuilder(bool notifyScriptLoaded, StringBuilder builder)
        {
            if (notifyScriptLoaded)
            {
                builder.AppendLine();
                builder.Append("if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();");
            }
        }

        private static bool WriteResource(StringBuilder builder, ResourceManager resourceManager, ResourceSet neutralSet, bool first, bool isDebug)
        {
            foreach (DictionaryEntry entry in neutralSet)
            {
                string key = (string) entry.Key;
                string str2 = resourceManager.GetObject(key) as string;
                if (str2 != null)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        builder.Append(',');
                    }
                    if (isDebug)
                    {
                        builder.AppendLine();
                    }
                    builder.Append('"');
                    builder.Append(JavaScriptString.QuoteString(key));
                    builder.Append("\":\"");
                    builder.Append(JavaScriptString.QuoteString(str2));
                    builder.Append('"');
                }
            }
            return first;
        }

        private static void WriteResources(StringBuilder builder, string typeName, ResourceManager resourceManager, ResourceSet neutralSet, ResourceManager releaseResourceManager, ResourceSet releaseNeutralSet, bool isDebug)
        {
            builder.AppendLine();
            RegisterNamespace(builder, typeName, isDebug);
            builder.Append(typeName);
            builder.Append("={");
            bool first = true;
            if (resourceManager != null)
            {
                first = WriteResource(builder, resourceManager, neutralSet, first, isDebug);
            }
            if (releaseResourceManager != null)
            {
                WriteResource(builder, releaseResourceManager, releaseNeutralSet, first, isDebug);
            }
            if (isDebug)
            {
                builder.AppendLine();
                builder.AppendLine("};");
            }
            else
            {
                builder.Append("};");
            }
        }

        private static void WriteResourceToStringBuilder(ScriptResourceInfo resourceInfo, ScriptResourceInfo releaseResourceInfo, ResourceManager resourceManager, ResourceSet neutralSet, ResourceManager releaseResourceManager, ResourceSet releaseNeutralSet, StringBuilder builder)
        {
            if ((resourceManager != null) || (releaseResourceManager != null))
            {
                string typeName = resourceInfo.TypeName;
                if (string.IsNullOrEmpty(typeName))
                {
                    typeName = releaseResourceInfo.TypeName;
                }
                WriteResources(builder, typeName, resourceManager, neutralSet, releaseResourceManager, releaseNeutralSet, resourceInfo.IsDebug);
            }
        }

        private static void WriteScript(Assembly assembly, ScriptResourceInfo resourceInfo, ScriptResourceInfo releaseResourceInfo, ResourceManager resourceManager, ResourceSet neutralSet, ResourceManager releaseResourceManager, ResourceSet releaseNeutralSet, bool zip, bool notifyScriptLoaded, StringBuilder output)
        {
            using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(resourceInfo.ScriptName), true))
            {
                if (resourceInfo.IsDebug)
                {
                    AssemblyName name = assembly.GetName();
                    output.AppendLine("// Name:        " + resourceInfo.ScriptName);
                    output.AppendLine("// Assembly:    " + name.Name);
                    output.AppendLine("// Version:     " + name.Version.ToString());
                    output.AppendLine("// FileVersion: " + AssemblyUtil.GetAssemblyFileVersion(assembly));
                }
                if (resourceInfo.PerformSubstitution)
                {
                    CopyScriptToStringBuilderWithSubstitution(reader.ReadToEnd(), assembly, zip, notifyScriptLoaded, output);
                }
                else
                {
                    output.Append(reader.ReadToEnd());
                }
                WriteResourceToStringBuilder(resourceInfo, releaseResourceInfo, resourceManager, neutralSet, releaseResourceManager, releaseNeutralSet, output);
                WriteNotificationToStringBuilder(notifyScriptLoaded, output);
            }
        }

        public string ScriptName =>
            this._scriptName;

        public string ScriptResourceName =>
            this._scriptResourceName;

        public string TypeName =>
            this._typeName;
    }
}

