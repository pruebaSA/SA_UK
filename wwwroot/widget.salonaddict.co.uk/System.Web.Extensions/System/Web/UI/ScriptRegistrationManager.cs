namespace System.Web.UI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Resources;
    using System.Web.Script.Serialization;

    internal sealed class ScriptRegistrationManager
    {
        private List<RegisteredScript> _clientScriptBlocks;
        private List<RegisteredExpandoAttribute> _expandos;
        private List<RegisteredHiddenField> _hiddenFields;
        private List<RegisteredArrayDeclaration> _scriptArrays;
        private List<RegisteredDisposeScript> _scriptDisposes;
        private ScriptManager _scriptManager;
        private List<RegisteredScript> _startupScriptBlocks;
        private List<RegisteredScript> _submitStatements;
        private static Regex ScriptTagRegex = new Regex("<script(\\s+(?<attrname>\\w[-\\w:]*)(\\s*=\\s*\"(?<attrval>[^\"]*)\"|\\s*=\\s*'(?<attrval>[^']*)'))*\\s*(?<empty>/)?>", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

        public ScriptRegistrationManager(ScriptManager scriptManager)
        {
            this._scriptManager = scriptManager;
        }

        private static void CheckScriptTagTweenSpace(RegisteredScript entry, string text, int start, int length)
        {
            string str = text.Substring(start, length);
            if (str.Trim().Length != 0)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ScriptRegistrationManager_InvalidChars, new object[] { entry.Type.FullName, entry.Key, str }));
            }
        }

        private bool IsControlRegistrationActive(List<UpdatePanel> updatingUpdatePanels, Control child, bool pageAlwaysActive)
        {
            if (pageAlwaysActive)
            {
                Page page = child as Page;
                if (page == this._scriptManager.Page)
                {
                    return true;
                }
            }
            if ((updatingUpdatePanels != null) && (updatingUpdatePanels.Count > 0))
            {
                while (child != null)
                {
                    if (child is UpdatePanel)
                    {
                        for (int i = 0; i < updatingUpdatePanels.Count; i++)
                        {
                            if (child == updatingUpdatePanels[i])
                            {
                                return true;
                            }
                        }
                    }
                    child = child.Parent;
                }
            }
            return false;
        }

        public static void RegisterArrayDeclaration(Control control, string arrayName, string arrayValue)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (control.Page == null)
            {
                throw new ArgumentException(AtlasWeb.ScriptRegistrationManager_ControlNotOnPage, "control");
            }
            control.Page.ClientScript.RegisterArrayDeclaration(arrayName, arrayValue);
            ScriptManager current = ScriptManager.GetCurrent(control.Page);
            if (current != null)
            {
                RegisteredArrayDeclaration item = new RegisteredArrayDeclaration(control, arrayName, arrayValue);
                current.ScriptRegistration.ScriptArrays.Add(item);
            }
        }

        public static void RegisterClientScriptBlock(Control control, Type type, string key, string script, bool addScriptTags)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (control.Page == null)
            {
                throw new ArgumentException(AtlasWeb.ScriptRegistrationManager_ControlNotOnPage, "control");
            }
            control.Page.ClientScript.RegisterClientScriptBlock(type, key, script, addScriptTags);
            ScriptManager current = ScriptManager.GetCurrent(control.Page);
            if (current != null)
            {
                RegisteredScript item = new RegisteredScript(RegisteredScriptType.ClientScriptBlock, control, type, key, script, addScriptTags);
                current.ScriptRegistration.ScriptBlocks.Add(item);
            }
        }

        public static void RegisterClientScriptInclude(Control control, Type type, string key, string url)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (control.Page == null)
            {
                throw new ArgumentException(AtlasWeb.ScriptRegistrationManager_ControlNotOnPage, "control");
            }
            control.Page.ClientScript.RegisterClientScriptInclude(type, key, url);
            ScriptManager current = ScriptManager.GetCurrent(control.Page);
            if (current != null)
            {
                RegisteredScript item = new RegisteredScript(control, type, key, url);
                current.ScriptRegistration.ScriptBlocks.Add(item);
            }
        }

        public static void RegisterClientScriptResource(Control control, Type type, string resourceName)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (control.Page == null)
            {
                throw new ArgumentException(AtlasWeb.ScriptRegistrationManager_ControlNotOnPage, "control");
            }
            ScriptManager current = ScriptManager.GetCurrent(control.Page);
            if (current == null)
            {
                control.Page.ClientScript.RegisterClientScriptResource(type, resourceName);
            }
            else
            {
                string scriptResourceUrl = current.GetScriptResourceUrl(resourceName, type.Assembly);
                control.Page.ClientScript.RegisterClientScriptInclude(type, resourceName, scriptResourceUrl);
                RegisteredScript item = new RegisteredScript(control, type, resourceName, scriptResourceUrl);
                current.ScriptRegistration.ScriptBlocks.Add(item);
            }
        }

        internal void RegisterDispose(Control control, string disposeScript)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (control.Page == null)
            {
                throw new ArgumentException(AtlasWeb.ScriptRegistrationManager_ControlNotOnPage, "control");
            }
            if (disposeScript == null)
            {
                throw new ArgumentNullException("disposeScript");
            }
            Control parent = control.Parent;
            UpdatePanel parentUpdatePanel = null;
            while (parent != null)
            {
                parentUpdatePanel = parent as UpdatePanel;
                if (parentUpdatePanel != null)
                {
                    break;
                }
                parent = parent.Parent;
            }
            if (parentUpdatePanel != null)
            {
                RegisteredDisposeScript item = new RegisteredDisposeScript(control, disposeScript, parentUpdatePanel);
                this.ScriptDisposes.Add(item);
                if (!this._scriptManager.IsInAsyncPostBack)
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    StringBuilder output = new StringBuilder(0x100);
                    output.Append("Sys.WebForms.PageRequestManager.getInstance()._registerDisposeScript(");
                    serializer.Serialize(parentUpdatePanel.ClientID, output);
                    output.Append(", ");
                    serializer.Serialize(disposeScript, output);
                    output.AppendLine(");");
                    this._scriptManager.IPage.ClientScript.RegisterStartupScript(typeof(ScriptRegistrationManager), this._scriptManager.CreateUniqueScriptKey(), output.ToString(), true);
                }
            }
        }

        public static void RegisterExpandoAttribute(Control control, string controlId, string attributeName, string attributeValue, bool encode)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (control.Page == null)
            {
                throw new ArgumentException(AtlasWeb.ScriptRegistrationManager_ControlNotOnPage, "control");
            }
            control.Page.ClientScript.RegisterExpandoAttribute(controlId, attributeName, attributeValue, encode);
            ScriptManager current = ScriptManager.GetCurrent(control.Page);
            if (current != null)
            {
                RegisteredExpandoAttribute item = new RegisteredExpandoAttribute(control, controlId, attributeName, attributeValue, encode);
                current.ScriptRegistration.ScriptExpandos.Add(item);
            }
        }

        public static void RegisterHiddenField(Control control, string hiddenFieldName, string hiddenFieldInitialValue)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (control.Page == null)
            {
                throw new ArgumentException(AtlasWeb.ScriptRegistrationManager_ControlNotOnPage, "control");
            }
            control.Page.ClientScript.RegisterHiddenField(hiddenFieldName, hiddenFieldInitialValue);
            ScriptManager current = ScriptManager.GetCurrent(control.Page);
            if (current != null)
            {
                RegisteredHiddenField item = new RegisteredHiddenField(control, hiddenFieldName, hiddenFieldInitialValue);
                current.ScriptRegistration.ScriptHiddenFields.Add(item);
            }
        }

        public static void RegisterOnSubmitStatement(Control control, Type type, string key, string script)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (control.Page == null)
            {
                throw new ArgumentException(AtlasWeb.ScriptRegistrationManager_ControlNotOnPage, "control");
            }
            control.Page.ClientScript.RegisterOnSubmitStatement(type, key, script);
            ScriptManager current = ScriptManager.GetCurrent(control.Page);
            if (current != null)
            {
                RegisteredScript item = new RegisteredScript(RegisteredScriptType.OnSubmitStatement, control, type, key, script, false);
                current.ScriptRegistration.ScriptSubmitStatements.Add(item);
            }
        }

        public static void RegisterStartupScript(Control control, Type type, string key, string script, bool addScriptTags)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (control.Page == null)
            {
                throw new ArgumentException(AtlasWeb.ScriptRegistrationManager_ControlNotOnPage, "control");
            }
            control.Page.ClientScript.RegisterStartupScript(type, key, script, addScriptTags);
            ScriptManager current = ScriptManager.GetCurrent(control.Page);
            if (current != null)
            {
                RegisteredScript item = new RegisteredScript(RegisteredScriptType.ClientStartupScript, control, type, key, script, addScriptTags);
                current.ScriptRegistration.ScriptStartupBlocks.Add(item);
            }
        }

        public void RenderActiveArrayDeclarations(List<UpdatePanel> updatePanels, HtmlTextWriter writer)
        {
            List<RegisteredArrayDeclaration> list = new List<RegisteredArrayDeclaration>();
            Control control = null;
            foreach (RegisteredArrayDeclaration declaration in this.ScriptArrays)
            {
                Control child = declaration.Control;
                if (((control != null) && (child == control)) || this.IsControlRegistrationActive(updatePanels, child, true))
                {
                    control = child;
                    if (!list.Contains(declaration))
                    {
                        list.Add(declaration);
                    }
                }
            }
            foreach (RegisteredArrayDeclaration declaration2 in list)
            {
                PageRequestManager.EncodeString(writer, "arrayDeclaration", declaration2.Name, declaration2.Value);
            }
        }

        public void RenderActiveExpandos(List<UpdatePanel> updatePanels, HtmlTextWriter writer)
        {
            if (updatePanels != null)
            {
                List<RegisteredExpandoAttribute> list = new List<RegisteredExpandoAttribute>();
                Control control = null;
                foreach (RegisteredExpandoAttribute attribute in this.ScriptExpandos)
                {
                    Control child = attribute.Control;
                    if (((control != null) && (child == control)) || this.IsControlRegistrationActive(updatePanels, child, false))
                    {
                        control = child;
                        if (!list.Contains(attribute))
                        {
                            list.Add(attribute);
                        }
                    }
                }
                foreach (RegisteredExpandoAttribute attribute2 in list)
                {
                    string str2;
                    string id = "document.getElementById('" + attribute2.ControlId + "')['" + attribute2.Name + "']";
                    if (attribute2.Encode)
                    {
                        str2 = "\"" + JavaScriptString.QuoteString(attribute2.Value) + "\"";
                    }
                    else if (attribute2.Value != null)
                    {
                        str2 = "\"" + attribute2.Value + "\"";
                    }
                    else
                    {
                        str2 = "null";
                    }
                    PageRequestManager.EncodeString(writer, "expando", id, str2);
                }
            }
        }

        public void RenderActiveHiddenFields(List<UpdatePanel> updatePanels, HtmlTextWriter writer)
        {
            List<RegisteredHiddenField> list = new List<RegisteredHiddenField>();
            ListDictionary dictionary = new ListDictionary(StringComparer.Ordinal);
            Control control = null;
            foreach (RegisteredHiddenField field in this.ScriptHiddenFields)
            {
                Control child = field.Control;
                if (((control != null) && (child == control)) || this.IsControlRegistrationActive(updatePanels, child, true))
                {
                    control = child;
                    if (!dictionary.Contains(field.Name))
                    {
                        list.Add(field);
                        dictionary.Add(field.Name, field);
                    }
                }
            }
            foreach (RegisteredHiddenField field2 in list)
            {
                PageRequestManager.EncodeString(writer, "hiddenField", field2.Name, field2.InitialValue);
            }
        }

        private void RenderActiveScriptBlocks(List<UpdatePanel> updatePanels, HtmlTextWriter writer, string token, List<RegisteredScript> scriptRegistrations)
        {
            List<RegisteredScript> list = new List<RegisteredScript>();
            ListDictionary dictionary = new ListDictionary();
            Control control = null;
            foreach (RegisteredScript script in scriptRegistrations)
            {
                Control child = script.Control;
                if (((control != null) && (child == control)) || this.IsControlRegistrationActive(updatePanels, child, true))
                {
                    control = child;
                    ScriptKey key = new ScriptKey(script.Type, script.Key);
                    if (!dictionary.Contains(key))
                    {
                        list.Add(script);
                        dictionary.Add(key, script);
                    }
                }
            }
            foreach (RegisteredScript script2 in list)
            {
                if (string.IsNullOrEmpty(script2.Url))
                {
                    if (script2.AddScriptTags)
                    {
                        PageRequestManager.EncodeString(writer, token, "ScriptContentNoTags", script2.Script);
                    }
                    else
                    {
                        WriteScriptWithTags(writer, token, script2);
                    }
                }
                else
                {
                    PageRequestManager.EncodeString(writer, token, "ScriptPath", script2.Url);
                }
            }
        }

        public void RenderActiveScriptDisposes(List<UpdatePanel> updatePanels, HtmlTextWriter writer)
        {
            if (updatePanels != null)
            {
                foreach (RegisteredDisposeScript script in this.ScriptDisposes)
                {
                    if (this.IsControlRegistrationActive(updatePanels, script.ParentUpdatePanel, false))
                    {
                        PageRequestManager.EncodeString(writer, "scriptDispose", script.ParentUpdatePanel.ClientID, script.Script);
                    }
                }
            }
        }

        public void RenderActiveScripts(List<UpdatePanel> updatePanels, HtmlTextWriter writer)
        {
            this.RenderActiveScriptBlocks(updatePanels, writer, "scriptBlock", this.ScriptBlocks);
            this.RenderActiveScriptBlocks(updatePanels, writer, "scriptStartupBlock", this.ScriptStartupBlocks);
        }

        public void RenderActiveSubmitStatements(List<UpdatePanel> updatePanels, HtmlTextWriter writer)
        {
            List<RegisteredScript> list = new List<RegisteredScript>();
            ListDictionary dictionary = new ListDictionary();
            Control control = null;
            foreach (RegisteredScript script in this.ScriptSubmitStatements)
            {
                Control child = script.Control;
                if (((control != null) && (child == control)) || this.IsControlRegistrationActive(updatePanels, child, true))
                {
                    control = child;
                    ScriptKey key = new ScriptKey(script.Type, script.Key);
                    if (!dictionary.Contains(key))
                    {
                        list.Add(script);
                        dictionary.Add(key, script);
                    }
                }
            }
            foreach (RegisteredScript script2 in list)
            {
                PageRequestManager.EncodeString(writer, "onSubmit", null, script2.Script);
            }
        }

        private static void WriteScriptWithTags(HtmlTextWriter writer, string token, RegisteredScript activeRegistration)
        {
            string script = activeRegistration.Script;
            int startat = 0;
            for (Match match = ScriptTagRegex.Match(script, startat); match.Success; match = ScriptTagRegex.Match(script, startat))
            {
                CheckScriptTagTweenSpace(activeRegistration, script, startat, match.Index - startat);
                OrderedDictionary dictionary = new OrderedDictionary();
                if (match.Groups["empty"].Captures.Count > 0)
                {
                    startat = match.Index + match.Length;
                }
                else
                {
                    int startIndex = match.Index + match.Length;
                    int num3 = script.IndexOf("</script>", startIndex, StringComparison.OrdinalIgnoreCase);
                    if (num3 == -1)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ScriptRegistrationManager_NoCloseTag, new object[] { activeRegistration.Type.FullName, activeRegistration.Key }));
                    }
                    string str2 = script.Substring(startIndex, num3 - startIndex);
                    dictionary.Add("text", str2);
                    startat = num3 + 9;
                }
                CaptureCollection captures = match.Groups["attrname"].Captures;
                CaptureCollection captures2 = match.Groups["attrval"].Captures;
                for (int i = 0; i < captures.Count; i++)
                {
                    string key = captures[i].ToString();
                    string str4 = HttpUtility.HtmlDecode(captures2[i].ToString());
                    dictionary.Add(key, str4);
                }
                string content = new JavaScriptSerializer().Serialize(dictionary);
                PageRequestManager.EncodeString(writer, token, "ScriptContentWithTags", content);
            }
            CheckScriptTagTweenSpace(activeRegistration, script, startat, script.Length - startat);
            if (startat == 0)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ScriptRegistrationManager_NoTags, new object[] { activeRegistration.Type.FullName, activeRegistration.Key }));
            }
        }

        public List<RegisteredArrayDeclaration> ScriptArrays
        {
            get
            {
                if (this._scriptArrays == null)
                {
                    this._scriptArrays = new List<RegisteredArrayDeclaration>();
                }
                return this._scriptArrays;
            }
        }

        public List<RegisteredScript> ScriptBlocks
        {
            get
            {
                if (this._clientScriptBlocks == null)
                {
                    this._clientScriptBlocks = new List<RegisteredScript>();
                }
                return this._clientScriptBlocks;
            }
        }

        public List<RegisteredDisposeScript> ScriptDisposes
        {
            get
            {
                if (this._scriptDisposes == null)
                {
                    this._scriptDisposes = new List<RegisteredDisposeScript>();
                }
                return this._scriptDisposes;
            }
        }

        public List<RegisteredExpandoAttribute> ScriptExpandos
        {
            get
            {
                if (this._expandos == null)
                {
                    this._expandos = new List<RegisteredExpandoAttribute>();
                }
                return this._expandos;
            }
        }

        public List<RegisteredHiddenField> ScriptHiddenFields
        {
            get
            {
                if (this._hiddenFields == null)
                {
                    this._hiddenFields = new List<RegisteredHiddenField>();
                }
                return this._hiddenFields;
            }
        }

        public List<RegisteredScript> ScriptStartupBlocks
        {
            get
            {
                if (this._startupScriptBlocks == null)
                {
                    this._startupScriptBlocks = new List<RegisteredScript>();
                }
                return this._startupScriptBlocks;
            }
        }

        public List<RegisteredScript> ScriptSubmitStatements
        {
            get
            {
                if (this._submitStatements == null)
                {
                    this._submitStatements = new List<RegisteredScript>();
                }
                return this._submitStatements;
            }
        }
    }
}

