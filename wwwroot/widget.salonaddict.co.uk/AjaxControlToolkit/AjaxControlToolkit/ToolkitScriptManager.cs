namespace AjaxControlToolkit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.Compression;
    using System.Reflection;
    using System.Resources;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Web;
    using System.Web.UI;

    [Themeable(true)]
    public class ToolkitScriptManager : ScriptManager
    {
        private string _combinedScriptUrl;
        private bool _combineScripts = true;
        private Uri _combineScriptsHandlerUrl;
        private List<ScriptReference> _disabledScriptReferences;
        private static readonly Dictionary<Assembly, ScriptCombineAttribute[]> _scriptCombineAttributeCache = new Dictionary<Assembly, ScriptCombineAttribute[]>();
        private List<ScriptEntry> _scriptEntries;
        private static readonly Dictionary<Assembly, ScriptResourceAttribute[]> _scriptResourceAttributeCache = new Dictionary<Assembly, ScriptResourceAttribute[]>();
        private static Dictionary<string, bool> _scripts = new Dictionary<string, bool>();
        private List<ScriptReference> _uncombinableScriptReferences;
        private static readonly Dictionary<Assembly, WebResourceAttribute[]> _webResourceAttributeCache = new Dictionary<Assembly, WebResourceAttribute[]>();
        private const string CombinedScriptsParamName = "_TSM_CombinedScripts_";
        private const string HiddenFieldParamName = "_TSM_HiddenField_";
        protected static readonly Regex WebResourceRegex = new Regex("<%\\s*=\\s*(?<resourceType>WebResource|ScriptResource)\\(\"(?<resourceName>[^\"]*)\"\\)\\s*%>", RegexOptions.Singleline | RegexOptions.Multiline);

        static ToolkitScriptManager()
        {
            _scripts.Add("MicrosoftAjax.js", true);
            _scripts.Add("MicrosoftAjaxWebForms.js", true);
            _scripts.Add("MicrosoftAjaxTimer.js", true);
            _scripts.Add("MicrosoftAjax.debug.js", true);
            _scripts.Add("MicrosoftAjaxWebForms.debug.js", true);
            _scripts.Add("MicrosoftAjaxTimer.debug.js", true);
        }

        protected static void AppendCharAsUnicode(StringBuilder builder, char c)
        {
            builder.Append(@"\u");
            builder.AppendFormat(CultureInfo.InvariantCulture, "{0:x4}", new object[] { (int) c });
        }

        private void ApplyAssembly(ScriptReference script, bool isComposite)
        {
            if ((!string.IsNullOrEmpty(script.Name) && string.IsNullOrEmpty(script.Path)) && (string.IsNullOrEmpty(script.Assembly) || (Assembly.Load(script.Assembly) == typeof(ScriptManager).Assembly)))
            {
                if (!isComposite && _scripts.ContainsKey(script.Name))
                {
                    script.Path = new RedirectScriptReference(script.Name).GetBaseUrl(ScriptManager.GetCurrent(this.Page));
                    script.ScriptMode = ScriptMode.Release;
                }
                else
                {
                    script.Assembly = typeof(ToolkitScriptManager).Assembly.FullName;
                }
            }
        }

        private static List<ScriptEntry> DeserializeScriptEntries(string serializedScriptEntries, bool loaded)
        {
            List<ScriptEntry> list = new List<ScriptEntry>();
            foreach (string str in serializedScriptEntries.Split(new char[] { ';' }))
            {
                string assembly = null;
                string culture = null;
                string str4 = null;
                Dictionary<string, string> dictionary = null;
                foreach (string str5 in str.Split(new char[] { ':' }))
                {
                    if (assembly == null)
                    {
                        assembly = str5;
                    }
                    else if (culture == null)
                    {
                        culture = str5;
                    }
                    else if (str4 == null)
                    {
                        str4 = str5;
                    }
                    else
                    {
                        string str8;
                        if (dictionary == null)
                        {
                            dictionary = new Dictionary<string, string>();
                            foreach (string str6 in new ScriptEntry(assembly, null, null).LoadAssembly().GetManifestResourceNames())
                            {
                                string key = str6.GetHashCode().ToString("x", CultureInfo.InvariantCulture);
                                if (dictionary.ContainsKey(key))
                                {
                                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Assembly \"{0}\" contains multiple scripts with hash code \"{1}\".", new object[] { assembly, key }));
                                }
                                dictionary[key] = str6;
                            }
                        }
                        if (!dictionary.TryGetValue(str5, out str8))
                        {
                            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Assembly \"{0}\" does not contain a script with hash code \"{1}\".", new object[] { assembly, str5 }));
                        }
                        ScriptEntry item = new ScriptEntry(assembly, str8, culture) {
                            Loaded = loaded
                        };
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        private static ScriptCombineAttribute[] GetScriptCombineAttributes(Assembly assembly)
        {
            ScriptCombineAttribute[] customAttributes;
            if (!_scriptCombineAttributeCache.TryGetValue(assembly, out customAttributes))
            {
                customAttributes = (ScriptCombineAttribute[]) assembly.GetCustomAttributes(typeof(ScriptCombineAttribute), false);
                _scriptCombineAttributeCache[assembly] = customAttributes;
            }
            return customAttributes;
        }

        private static ScriptResourceAttribute[] GetScriptResourceAttributes(Assembly assembly)
        {
            ScriptResourceAttribute[] customAttributes;
            if (!_scriptResourceAttributeCache.TryGetValue(assembly, out customAttributes))
            {
                customAttributes = (ScriptResourceAttribute[]) assembly.GetCustomAttributes(typeof(ScriptResourceAttribute), false);
                _scriptResourceAttributeCache[assembly] = customAttributes;
            }
            return customAttributes;
        }

        private static WebResourceAttribute[] GetWebResourceAttributes(Assembly assembly)
        {
            WebResourceAttribute[] customAttributes;
            if (!_webResourceAttributeCache.TryGetValue(assembly, out customAttributes))
            {
                customAttributes = (WebResourceAttribute[]) assembly.GetCustomAttributes(typeof(WebResourceAttribute), false);
                _webResourceAttributeCache[assembly] = customAttributes;
            }
            return customAttributes;
        }

        private static bool IsScriptCombinable(ScriptEntry scriptEntry)
        {
            bool flag = false;
            Assembly assembly = scriptEntry.LoadAssembly();
            foreach (ScriptCombineAttribute attribute in GetScriptCombineAttributes(assembly))
            {
                if (string.IsNullOrEmpty(attribute.IncludeScripts))
                {
                    flag = true;
                }
                else
                {
                    foreach (string str in attribute.IncludeScripts.Split(new char[] { ',' }))
                    {
                        if (string.Compare(scriptEntry.Name, str.Trim(), StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(attribute.ExcludeScripts))
                {
                    foreach (string str2 in attribute.ExcludeScripts.Split(new char[] { ',' }))
                    {
                        if (string.Compare(scriptEntry.Name, str2.Trim(), StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            flag = false;
                            break;
                        }
                    }
                }
            }
            if (!flag)
            {
                return flag;
            }
            bool flag2 = false;
            foreach (WebResourceAttribute attribute2 in GetWebResourceAttributes(assembly))
            {
                if (scriptEntry.Name == attribute2.WebResource)
                {
                    flag2 = true;
                    break;
                }
            }
            return (flag & flag2);
        }

        protected override void OnInit(EventArgs e)
        {
            if ((!base.DesignMode && (this.Context != null)) && OutputCombinedScriptFile(this.Context))
            {
                this.Page.Response.End();
            }
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            this._disabledScriptReferences = new List<ScriptReference>();
            this._uncombinableScriptReferences = new List<ScriptReference>();
            string hiddenFieldName = this.HiddenFieldName;
            string hiddenFieldInitialValue = "";
            if (!base.IsInAsyncPostBack || (this.Page.Request.Form[hiddenFieldName] == null))
            {
                ScriptManager.RegisterHiddenField(this.Page, hiddenFieldName, hiddenFieldInitialValue);
                if (this._combineScripts)
                {
                    ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "ClearHiddenOnLoad", string.Format(CultureInfo.InvariantCulture, "(function() {{var fn = function() {{$get(\"{0}\").value = '';Sys.Application.remove_init(fn);}};Sys.Application.add_init(fn);}})();", new object[] { QuoteString(hiddenFieldName), "" }), true);
                }
            }
            else
            {
                hiddenFieldInitialValue = this.Page.Request.Form[hiddenFieldName];
            }
            this._scriptEntries = DeserializeScriptEntries(hiddenFieldInitialValue, true);
            base.OnLoad(e);
        }

        protected override void OnResolveCompositeScriptReference(CompositeScriptReferenceEventArgs e)
        {
            foreach (ScriptReference reference in e.CompositeScript.Scripts)
            {
                this.ApplyAssembly(reference, true);
            }
            base.OnResolveCompositeScriptReference(e);
        }

        protected override void OnResolveScriptReference(ScriptReferenceEventArgs e)
        {
            this.ApplyAssembly(e.Script, false);
            base.OnResolveScriptReference(e);
            if ((this._combineScripts && !string.IsNullOrEmpty(e.Script.Assembly)) && !string.IsNullOrEmpty(e.Script.Name))
            {
                ScriptReference script = e.Script;
                ScriptEntry scriptEntry = new ScriptEntry(script);
                if (IsScriptCombinable(scriptEntry))
                {
                    if (!this._scriptEntries.Contains(scriptEntry))
                    {
                        this._scriptEntries.Add(scriptEntry);
                        this._combinedScriptUrl = null;
                    }
                    if (this._combinedScriptUrl == null)
                    {
                        this._combinedScriptUrl = string.Format(CultureInfo.InvariantCulture, "{0}?{1}={2}&{3}={4}", new object[] { (null != this._combineScriptsHandlerUrl) ? this._combineScriptsHandlerUrl.ToString() : this.Page.Request.Path.Replace(" ", "%20"), "_TSM_HiddenField_", this.HiddenFieldName, "_TSM_CombinedScripts_", HttpUtility.UrlEncode(SerializeScriptEntries(this._scriptEntries, false)) });
                    }
                    script.Name = "";
                    script.Assembly = "";
                    this._disabledScriptReferences.Add(script);
                    foreach (ScriptReference reference2 in this._disabledScriptReferences)
                    {
                        reference2.Path = this._combinedScriptUrl;
                    }
                }
                else
                {
                    bool flag = false;
                    foreach (ScriptReference reference3 in this._uncombinableScriptReferences)
                    {
                        if ((reference3.Assembly == script.Assembly) && (reference3.Name == script.Name))
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        this._uncombinableScriptReferences.Add(script);
                        this._disabledScriptReferences.Clear();
                        foreach (ScriptEntry entry2 in this._scriptEntries)
                        {
                            entry2.Loaded = true;
                        }
                    }
                }
            }
        }

        public static bool OutputCombinedScriptFile(HttpContext context)
        {
            bool flag = false;
            HttpRequest request = context.Request;
            string str = request.Params["_TSM_HiddenField_"];
            string str2 = request.Params["_TSM_CombinedScripts_"];
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(str2))
            {
                return flag;
            }
            HttpResponse response = context.Response;
            response.ContentType = "application/x-javascript";
            HttpCachePolicy cache = response.Cache;
            cache.SetCacheability(HttpCacheability.Public);
            cache.VaryByParams["_TSM_HiddenField_"] = true;
            cache.VaryByParams["_TSM_CombinedScripts_"] = true;
            cache.SetOmitVaryStar(true);
            cache.SetExpires(DateTime.Now.AddDays(365.0));
            cache.SetValidUntilExpires(true);
            cache.SetLastModifiedFromFileDependencies();
            Stream outputStream = response.OutputStream;
            if (!request.Browser.IsBrowser("IE") || (6 < request.Browser.MajorVersion))
            {
                foreach (string str3 in (request.Headers["Accept-Encoding"] ?? "").ToUpperInvariant().Split(new char[] { ',' }))
                {
                    if ("GZIP" == str3)
                    {
                        response.AddHeader("Content-encoding", "gzip");
                        outputStream = new GZipStream(outputStream, CompressionMode.Compress);
                        break;
                    }
                    if ("DEFLATE" == str3)
                    {
                        response.AddHeader("Content-encoding", "deflate");
                        outputStream = new DeflateStream(outputStream, CompressionMode.Compress);
                        break;
                    }
                }
            }
            using (StreamWriter writer = new StreamWriter(outputStream))
            {
                List<ScriptEntry> scriptEntries = DeserializeScriptEntries(HttpUtility.UrlDecode(str2), false);
                WriteScripts(scriptEntries, writer);
                writer.WriteLine("if(typeof(Sys)!=='undefined')Sys.Application.notifyScriptLoaded();");
                writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "(function() {{var fn = function() {{$get(\"{0}\").value += '{1}';Sys.Application.remove_load(fn);}};Sys.Application.add_load(fn);}})();", new object[] { QuoteString(str), SerializeScriptEntries(scriptEntries, true) }));
            }
            return true;
        }

        protected static string QuoteString(string value)
        {
            StringBuilder builder = null;
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            int startIndex = 0;
            int count = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if ((((c == '\r') || (c == '\t')) || ((c == '"') || (c == '\''))) || ((((c == '<') || (c == '>')) || ((c == '\\') || (c == '\n'))) || (((c == '\b') || (c == '\f')) || (c < ' '))))
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder(value.Length + 5);
                    }
                    if (count > 0)
                    {
                        builder.Append(value, startIndex, count);
                    }
                    startIndex = i + 1;
                    count = 0;
                }
                switch (c)
                {
                    case '<':
                    case '>':
                    case '\'':
                    {
                        AppendCharAsUnicode(builder, c);
                        continue;
                    }
                    case '\\':
                    {
                        builder.Append(@"\\");
                        continue;
                    }
                    case '\b':
                    {
                        builder.Append(@"\b");
                        continue;
                    }
                    case '\t':
                    {
                        builder.Append(@"\t");
                        continue;
                    }
                    case '\n':
                    {
                        builder.Append(@"\n");
                        continue;
                    }
                    case '\f':
                    {
                        builder.Append(@"\f");
                        continue;
                    }
                    case '\r':
                    {
                        builder.Append(@"\r");
                        continue;
                    }
                    case '"':
                    {
                        builder.Append("\\\"");
                        continue;
                    }
                }
                if (c < ' ')
                {
                    AppendCharAsUnicode(builder, c);
                }
                else
                {
                    count++;
                }
            }
            if (builder == null)
            {
                return value;
            }
            if (count > 0)
            {
                builder.Append(value, startIndex, count);
            }
            return builder.ToString();
        }

        private static string SerializeScriptEntries(List<ScriptEntry> scriptEntries, bool allScripts)
        {
            StringBuilder builder = new StringBuilder(";");
            string assembly = null;
            foreach (ScriptEntry entry in scriptEntries)
            {
                if (allScripts || !entry.Loaded)
                {
                    if (assembly != entry.Assembly)
                    {
                        builder.Append(";");
                        builder.Append(entry.Assembly);
                        builder.Append(":");
                        builder.Append(CultureInfo.CurrentUICulture.IetfLanguageTag);
                        builder.Append(":");
                        builder.Append(entry.LoadAssembly().ManifestModule.ModuleVersionId);
                        assembly = entry.Assembly;
                    }
                    builder.Append(":");
                    builder.Append(entry.Name.GetHashCode().ToString("x", CultureInfo.InvariantCulture));
                }
            }
            return builder.ToString();
        }

        private static void WriteScripts(List<ScriptEntry> scriptEntries, TextWriter outputWriter)
        {
            foreach (ScriptEntry entry in scriptEntries)
            {
                if (!entry.Loaded)
                {
                    if (!IsScriptCombinable(entry))
                    {
                        throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Combined script request includes uncombinable script \"{0}\".", new object[] { entry.Name }));
                    }
                    outputWriter.Write("//START ");
                    outputWriter.WriteLine(entry.Name);
                    string script = entry.GetScript();
                    if (WebResourceRegex.IsMatch(script))
                    {
                        throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "ToolkitScriptManager does not support <%= WebResource/ScriptResource(...) %> substitution as used by script file \"{0}\".", new object[] { entry.Name }));
                    }
                    outputWriter.WriteLine(script);
                    CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
                    try
                    {
                        try
                        {
                            Thread.CurrentThread.CurrentUICulture = new CultureInfo(entry.Culture);
                        }
                        catch (ArgumentException)
                        {
                        }
                        Assembly assembly = entry.LoadAssembly();
                        foreach (ScriptResourceAttribute attribute in GetScriptResourceAttributes(assembly))
                        {
                            if (attribute.ScriptName == entry.Name)
                            {
                                outputWriter.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}={{", new object[] { attribute.TypeName }));
                                string scriptResourceName = attribute.ScriptResourceName;
                                if (scriptResourceName.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
                                {
                                    scriptResourceName = scriptResourceName.Substring(0, scriptResourceName.Length - 10);
                                }
                                ResourceManager manager = new ResourceManager(scriptResourceName, assembly);
                                using (ResourceSet set = manager.GetResourceSet(CultureInfo.InvariantCulture, true, true))
                                {
                                    bool flag = true;
                                    foreach (DictionaryEntry entry2 in set)
                                    {
                                        if (!flag)
                                        {
                                            outputWriter.Write(",");
                                        }
                                        string key = (string) entry2.Key;
                                        string str4 = manager.GetString(key);
                                        outputWriter.Write(string.Format(CultureInfo.InvariantCulture, "\"{0}\":\"{1}\"", new object[] { QuoteString(key), QuoteString(str4) }));
                                        flag = false;
                                    }
                                }
                                outputWriter.WriteLine("};");
                            }
                        }
                    }
                    finally
                    {
                        Thread.CurrentThread.CurrentUICulture = currentUICulture;
                    }
                    outputWriter.Write("//END ");
                    outputWriter.WriteLine(entry.Name);
                }
                entry.Loaded = true;
            }
        }

        public bool CombineScripts
        {
            get => 
                this._combineScripts;
            set
            {
                this._combineScripts = value;
            }
        }

        [UrlProperty]
        public Uri CombineScriptsHandlerUrl
        {
            get => 
                this._combineScriptsHandlerUrl;
            set
            {
                this._combineScriptsHandlerUrl = value;
            }
        }

        protected string HiddenFieldName =>
            (this.ClientID + "_HiddenField");

        private class RedirectScriptReference : ScriptReference
        {
            public RedirectScriptReference(string name)
            {
                base.Name = name;
                base.Assembly = typeof(ToolkitScriptManager).Assembly.FullName;
            }

            public string GetBaseUrl(ScriptManager sm) => 
                base.GetUrl(sm, true);
        }

        private class ScriptEntry
        {
            private System.Reflection.Assembly _loadedAssembly;
            public readonly string Assembly;
            public readonly string Culture;
            public bool Loaded;
            public readonly string Name;

            public ScriptEntry(ScriptReference scriptReference) : this(scriptReference.Assembly, scriptReference.Name, null)
            {
            }

            public ScriptEntry(string assembly, string name, string culture)
            {
                this.Assembly = assembly;
                this.Name = name;
                this.Culture = culture;
            }

            public override bool Equals(object obj)
            {
                ToolkitScriptManager.ScriptEntry entry = (ToolkitScriptManager.ScriptEntry) obj;
                return ((entry.Assembly == this.Assembly) && (entry.Name == this.Name));
            }

            public override int GetHashCode() => 
                (this.Assembly.GetHashCode() ^ this.Name.GetHashCode());

            public string GetScript()
            {
                string str;
                using (Stream stream = this.LoadAssembly().GetManifestResourceStream(this.Name))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        str = reader.ReadToEnd();
                    }
                }
                return str;
            }

            public System.Reflection.Assembly LoadAssembly()
            {
                if (this._loadedAssembly == null)
                {
                    this._loadedAssembly = System.Reflection.Assembly.Load(this.Assembly);
                }
                return this._loadedAssembly;
            }
        }
    }
}

