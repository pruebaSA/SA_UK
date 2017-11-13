namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Web.Resources;
    using System.Web.Util;

    internal class ScriptResourceInfo
    {
        private string _contentType;
        private static readonly IDictionary _duplicateScriptAttributesChecked = Hashtable.Synchronized(new Hashtable());
        private bool _isDebug;
        private bool _performSubstitution;
        private static readonly IDictionary _scriptCache = Hashtable.Synchronized(new Hashtable());
        private string _scriptName;
        private string _scriptResourceName;
        private string _typeName;
        public static readonly ScriptResourceInfo Empty = new ScriptResourceInfo();

        private ScriptResourceInfo()
        {
        }

        public ScriptResourceInfo(ScriptResourceAttribute attr) : this()
        {
            this._scriptName = attr.ScriptName;
            this._scriptResourceName = attr.ScriptResourceName;
            this._typeName = attr.TypeName;
        }

        public static ScriptResourceInfo GetInstance(Assembly assembly, string resourceName)
        {
            if (!_duplicateScriptAttributesChecked.Contains(assembly))
            {
                Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
                foreach (ScriptResourceAttribute attribute in assembly.GetCustomAttributes(typeof(ScriptResourceAttribute), false))
                {
                    string scriptName = attribute.ScriptName;
                    if (dictionary.ContainsKey(scriptName))
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ScriptResourceHandler_DuplicateScriptResources, new object[] { scriptName, assembly.GetName() }));
                    }
                    dictionary.Add(scriptName, true);
                }
                _duplicateScriptAttributesChecked[assembly] = true;
            }
            Pair<Assembly, string> pair = new Pair<Assembly, string>(assembly, resourceName);
            ScriptResourceInfo empty = (ScriptResourceInfo) _scriptCache[pair];
            if (empty == null)
            {
                empty = Empty;
                foreach (ScriptResourceAttribute attribute2 in assembly.GetCustomAttributes(typeof(ScriptResourceAttribute), false))
                {
                    if (string.Equals(attribute2.ScriptName, resourceName, StringComparison.Ordinal))
                    {
                        empty = new ScriptResourceInfo(attribute2);
                        break;
                    }
                }
                foreach (WebResourceAttribute attribute3 in assembly.GetCustomAttributes(typeof(WebResourceAttribute), false))
                {
                    if (string.Equals(attribute3.WebResource, resourceName, StringComparison.Ordinal))
                    {
                        if (empty == Empty)
                        {
                            empty = new ScriptResourceInfo {
                                ScriptName = resourceName
                            };
                        }
                        empty.ContentType = attribute3.ContentType;
                        empty.PerformSubstitution = attribute3.PerformSubstitution;
                        break;
                    }
                }
                empty.IsDebug = resourceName.EndsWith(".debug.js", StringComparison.OrdinalIgnoreCase);
                _scriptCache[pair] = empty;
            }
            return empty;
        }

        public string ContentType
        {
            get => 
                this._contentType;
            set
            {
                this._contentType = value;
            }
        }

        public bool IsDebug
        {
            get => 
                this._isDebug;
            set
            {
                this._isDebug = value;
            }
        }

        public bool PerformSubstitution
        {
            get => 
                this._performSubstitution;
            set
            {
                this._performSubstitution = value;
            }
        }

        public string ScriptName
        {
            get => 
                this._scriptName;
            set
            {
                this._scriptName = value;
            }
        }

        public string ScriptResourceName =>
            this._scriptResourceName;

        public string TypeName =>
            this._typeName;
    }
}

