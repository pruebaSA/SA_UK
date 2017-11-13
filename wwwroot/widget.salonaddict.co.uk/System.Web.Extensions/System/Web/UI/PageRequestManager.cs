namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Text;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Resources;
    using System.Web.UI.HtmlControls;

    internal sealed class PageRequestManager
    {
        private List<UpdatePanel> _allUpdatePanels;
        private List<Control> _asyncPostBackControls;
        private string _asyncPostBackSourceElementID;
        private List<UpdatePanel> _childUpdatePanelsToRefresh;
        private Control _focusedControl;
        private string _focusedControlID;
        private ScriptManager _owner;
        private bool _panelsInitialized;
        private List<Control> _postBackControls;
        private bool _requireFocusScript;
        private ScriptDataItemCollection _scriptDataItems;
        private string _updatePanelRequiresUpdate;
        private List<UpdatePanel> _updatePanelsToRefresh;
        private HtmlTextWriter _updatePanelWriter;
        internal const string ArrayDeclarationToken = "arrayDeclaration";
        private const string AsyncPostBackControlIDsToken = "asyncPostBackControlIDs";
        internal const string AsyncPostBackErrorHttpCodeKey = "System.Web.UI.PageRequestManager:AsyncPostBackErrorHttpCode";
        internal const string AsyncPostBackErrorKey = "System.Web.UI.PageRequestManager:AsyncPostBackError";
        internal const string AsyncPostBackErrorMessageKey = "System.Web.UI.PageRequestManager:AsyncPostBackErrorMessage";
        private const string AsyncPostBackTimeoutToken = "asyncPostBackTimeout";
        private const string AsyncPostFormField = "__ASYNCPOST";
        private const string ChildUpdatePanelIDsToken = "childUpdatePanelIDs";
        private const string DataItemJsonToken = "dataItemJson";
        internal const string DataItemToken = "dataItem";
        internal const string ErrorToken = "error";
        internal const string ExpandoToken = "expando";
        private static readonly Version FocusMinimumEcmaVersion = new Version("1.4");
        private static readonly Version FocusMinimumJScriptVersion = new Version("3.0");
        private const string FocusToken = "focus";
        private const string FormActionToken = "formAction";
        internal const string HiddenFieldToken = "hiddenField";
        private const char LengthEncodeDelimiter = '|';
        private static readonly Version MinimumEcmaScriptVersion = new Version(1, 0);
        private static readonly Version MinimumW3CDomVersion = new Version(1, 0);
        internal const string OnSubmitToken = "onSubmit";
        internal const string PageRedirectToken = "pageRedirect";
        private const string PageTitleToken = "pageTitle";
        private const string PostBackControlIDsToken = "postBackControlIDs";
        internal const string ScriptBlockToken = "scriptBlock";
        internal const string ScriptDisposeToken = "scriptDispose";
        internal const string ScriptStartupBlockToken = "scriptStartupBlock";
        private const string UpdatePanelIDsToken = "updatePanelIDs";
        private const string UpdatePanelsToRefreshToken = "panelsToRefreshIDs";

        public PageRequestManager(ScriptManager owner)
        {
            this._owner = owner;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private bool CustomErrorsSectionHasRedirect(int httpCode)
        {
            bool flag = this._owner.CustomErrorsSection.DefaultRedirect != null;
            if (!flag && (this._owner.CustomErrorsSection.Errors != null))
            {
                foreach (CustomError error in this._owner.CustomErrorsSection.Errors)
                {
                    if (error.StatusCode == httpCode)
                    {
                        return true;
                    }
                }
            }
            return flag;
        }

        internal static void EncodeString(TextWriter writer, string type, string id, string content)
        {
            if (id == null)
            {
                id = string.Empty;
            }
            if (content == null)
            {
                content = string.Empty;
            }
            writer.Write(content.Length.ToString(CultureInfo.InvariantCulture));
            writer.Write('|');
            writer.Write(type);
            writer.Write('|');
            writer.Write(id);
            writer.Write('|');
            writer.Write(content);
            writer.Write('|');
        }

        private string GetAllUpdatePanelIDs() => 
            GetUpdatePanelIDsFromList(this._allUpdatePanels, true);

        private string GetAsyncPostBackControlIDs(bool includeQuotes) => 
            GetControlIDsFromList(this._asyncPostBackControls, includeQuotes);

        private string GetChildUpdatePanelIDs() => 
            GetUpdatePanelIDsFromList(this._childUpdatePanelsToRefresh, false);

        private static string GetControlIDsFromList(List<Control> list, bool includeQuotes)
        {
            if ((list == null) || (list.Count <= 0))
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Visible)
                {
                    if (!flag)
                    {
                        builder.Append(',');
                    }
                    flag = false;
                    if (includeQuotes)
                    {
                        builder.Append('\'');
                    }
                    builder.Append(list[i].UniqueID);
                    if (includeQuotes)
                    {
                        builder.Append('\'');
                    }
                }
            }
            return builder.ToString();
        }

        private static Exception GetControlRegistrationException(Control control)
        {
            if (control == null)
            {
                return new ArgumentNullException("control");
            }
            if (((control is INamingContainer) || (control is IPostBackDataHandler)) || (control is IPostBackEventHandler))
            {
                return null;
            }
            return new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ScriptManager_InvalidControlRegistration, new object[] { control.ID }));
        }

        private static int GetHttpCodeForException(Exception e)
        {
            HttpException exception = e as HttpException;
            if (exception != null)
            {
                return exception.GetHttpCode();
            }
            if (e is UnauthorizedAccessException)
            {
                return 0x191;
            }
            if (e is PathTooLongException)
            {
                return 0x19e;
            }
            if (e.InnerException != null)
            {
                return GetHttpCodeForException(e.InnerException);
            }
            return 500;
        }

        private string GetPostBackControlIDs(bool includeQuotes) => 
            GetControlIDsFromList(this._postBackControls, includeQuotes);

        private string GetRefreshingUpdatePanelIDs() => 
            GetUpdatePanelIDsFromList(this._updatePanelsToRefresh, false);

        private static string GetUpdatePanelIDsFromList(List<UpdatePanel> list, bool includeChildrenAsTriggersPrefix)
        {
            if ((list == null) || (list.Count <= 0))
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Visible)
                {
                    if (!flag)
                    {
                        builder.Append(',');
                    }
                    flag = false;
                    if (includeChildrenAsTriggersPrefix)
                    {
                        builder.Append(list[i].ChildrenAsTriggers ? 't' : 'f');
                    }
                    builder.Append(list[i].UniqueID);
                }
            }
            return builder.ToString();
        }

        internal static bool IsAsyncPostBackRequest(HttpRequestBase request)
        {
            string[] values = request.Headers.GetValues("X-MicrosoftAjax");
            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    string[] strArray2 = values[i].Split(new char[] { ',' });
                    for (int j = 0; j < strArray2.Length; j++)
                    {
                        if (strArray2[j].Trim() == "Delta=true")
                        {
                            return true;
                        }
                    }
                }
            }
            string str = request.Form["__ASYNCPOST"];
            return (!string.IsNullOrEmpty(str) && (str.Trim() == "true"));
        }

        internal void LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            string str = postCollection[postDataKey];
            if (str != null)
            {
                string str2;
                int index = str.IndexOf('|');
                if (index != -1)
                {
                    str2 = str.Substring(0, index);
                    this._asyncPostBackSourceElementID = str.Substring(index + 1);
                }
                else
                {
                    str2 = str;
                    this._asyncPostBackSourceElementID = string.Empty;
                }
                if (str2 != this._owner.UniqueID)
                {
                    this._updatePanelRequiresUpdate = str2;
                }
            }
            if ((this._allUpdatePanels != null) && (this._allUpdatePanels.Count != 0))
            {
                foreach (UpdatePanel panel in this._allUpdatePanels)
                {
                    panel.Initialize();
                }
            }
            this._panelsInitialized = true;
        }

        internal void OnInit()
        {
            if (this._owner.EnablePartialRendering && !this._owner._supportsPartialRenderingSetByUser)
            {
                HttpBrowserCapabilitiesBase browser = this._owner.IPage.Request.Browser;
                bool flag = ((browser.W3CDomVersion >= MinimumW3CDomVersion) && (browser.EcmaScriptVersion >= MinimumEcmaScriptVersion)) && browser.SupportsCallback;
                if (flag)
                {
                    flag = !this.EnableLegacyRendering;
                }
                this._owner.SupportsPartialRendering = flag;
            }
            if (this._owner.IsInAsyncPostBack)
            {
                this._owner.IPage.Error += new EventHandler(this.OnPageError);
            }
        }

        private void OnPageError(object sender, EventArgs e)
        {
            Exception lastError = this._owner.IPage.Server.GetLastError();
            this._owner.OnAsyncPostBackError(new AsyncPostBackErrorEventArgs(lastError));
            string asyncPostBackErrorMessage = this._owner.AsyncPostBackErrorMessage;
            if (string.IsNullOrEmpty(asyncPostBackErrorMessage) && !this._owner.Control.Context.IsCustomErrorEnabled)
            {
                asyncPostBackErrorMessage = lastError.Message;
            }
            int httpCodeForException = GetHttpCodeForException(lastError);
            bool flag = false;
            if (this._owner.AllowCustomErrorsRedirect && this._owner.Control.Context.IsCustomErrorEnabled)
            {
                if (!this.CustomErrorsSectionHasRedirect(httpCodeForException))
                {
                    flag = true;
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                IDictionary items = this._owner.Control.Context.Items;
                items["System.Web.UI.PageRequestManager:AsyncPostBackError"] = true;
                items["System.Web.UI.PageRequestManager:AsyncPostBackErrorMessage"] = asyncPostBackErrorMessage;
                items["System.Web.UI.PageRequestManager:AsyncPostBackErrorHttpCode"] = httpCodeForException;
            }
        }

        internal void OnPreRender()
        {
            this._owner.IPage.SetRenderMethodDelegate(new RenderMethod(this.RenderPageCallback));
        }

        private void ProcessFocus(HtmlTextWriter writer)
        {
            if (this._requireFocusScript)
            {
                string content = string.Empty;
                if (!string.IsNullOrEmpty(this._focusedControlID))
                {
                    content = this._focusedControlID;
                }
                else if ((this._focusedControl != null) && this._focusedControl.Visible)
                {
                    content = this._focusedControl.ClientID;
                }
                if (content.Length > 0)
                {
                    string scriptResourceUrl = this._owner.GetScriptResourceUrl("Focus.js", typeof(HtmlForm).Assembly);
                    EncodeString(writer, "scriptBlock", "ScriptPath", scriptResourceUrl);
                    EncodeString(writer, "focus", string.Empty, content);
                }
            }
        }

        private void ProcessScriptRegistration(HtmlTextWriter writer)
        {
            this._owner.ScriptRegistration.RenderActiveArrayDeclarations(this._updatePanelsToRefresh, writer);
            this._owner.ScriptRegistration.RenderActiveScripts(this._updatePanelsToRefresh, writer);
            this._owner.ScriptRegistration.RenderActiveSubmitStatements(this._updatePanelsToRefresh, writer);
            this._owner.ScriptRegistration.RenderActiveExpandos(this._updatePanelsToRefresh, writer);
            this._owner.ScriptRegistration.RenderActiveHiddenFields(this._updatePanelsToRefresh, writer);
            this._owner.ScriptRegistration.RenderActiveScriptDisposes(this._updatePanelsToRefresh, writer);
        }

        private void ProcessUpdatePanels()
        {
            if (this._allUpdatePanels != null)
            {
                this._updatePanelsToRefresh = new List<UpdatePanel>(this._allUpdatePanels.Count);
                this._childUpdatePanelsToRefresh = new List<UpdatePanel>(this._allUpdatePanels.Count);
                HtmlForm form = this._owner.Page.Form;
                for (int i = 0; i < this._allUpdatePanels.Count; i++)
                {
                    UpdatePanel item = this._allUpdatePanels[i];
                    bool flag2 = ((this._updatePanelRequiresUpdate != null) && string.Equals(item.UniqueID, this._updatePanelRequiresUpdate, StringComparison.Ordinal)) || item.RequiresUpdate;
                    Control parent = item.Parent;
                    while (parent != form)
                    {
                        UpdatePanel panel2 = parent as UpdatePanel;
                        if ((panel2 != null) && (this._updatePanelsToRefresh.Contains(panel2) || this._childUpdatePanelsToRefresh.Contains(panel2)))
                        {
                            flag2 = false;
                            this._childUpdatePanelsToRefresh.Add(item);
                            break;
                        }
                        parent = parent.Parent;
                        if (parent == null)
                        {
                            flag2 = false;
                            break;
                        }
                    }
                    if (flag2)
                    {
                        item.SetAsyncPostBackMode(true);
                        this._updatePanelsToRefresh.Add(item);
                    }
                    else
                    {
                        item.SetAsyncPostBackMode(false);
                    }
                }
            }
        }

        public void RegisterAsyncPostBackControl(Control control)
        {
            Exception controlRegistrationException = GetControlRegistrationException(control);
            if (controlRegistrationException != null)
            {
                throw controlRegistrationException;
            }
            if ((this._postBackControls != null) && this._postBackControls.Contains(control))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ScriptManager_CannotRegisterBothPostBacks, new object[] { control.ID }));
            }
            if (this._asyncPostBackControls == null)
            {
                this._asyncPostBackControls = new List<Control>();
            }
            if (!this._asyncPostBackControls.Contains(control))
            {
                this._asyncPostBackControls.Add(control);
            }
        }

        public void RegisterDataItem(Control control, string dataItem, bool isJsonSerialized)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control");
            }
            if (!this._owner.IsInAsyncPostBack)
            {
                throw new InvalidOperationException(AtlasWeb.PageRequestManager_RegisterDataItemInNonAsyncRequest);
            }
            if (this._scriptDataItems == null)
            {
                this._scriptDataItems = new ScriptDataItemCollection();
            }
            else if (this._scriptDataItems.ContainsControl(control))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.PageRequestManager_RegisterDataItemTwice, new object[] { control.ID }), "control");
            }
            this._scriptDataItems.Add(new ScriptDataItem(control, dataItem, isJsonSerialized));
        }

        private void RegisterFocusScript()
        {
            if (this.ClientSupportsFocus && !this._requireFocusScript)
            {
                this._requireFocusScript = true;
            }
        }

        public void RegisterPostBackControl(Control control)
        {
            Exception controlRegistrationException = GetControlRegistrationException(control);
            if (controlRegistrationException != null)
            {
                throw controlRegistrationException;
            }
            if ((this._asyncPostBackControls != null) && this._asyncPostBackControls.Contains(control))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ScriptManager_CannotRegisterBothPostBacks, new object[] { control.ID }));
            }
            if (this._postBackControls == null)
            {
                this._postBackControls = new List<Control>();
            }
            if (!this._postBackControls.Contains(control))
            {
                this._postBackControls.Add(control);
            }
        }

        internal void RegisterUpdatePanel(UpdatePanel updatePanel)
        {
            if (this._allUpdatePanels == null)
            {
                this._allUpdatePanels = new List<UpdatePanel>();
            }
            this._allUpdatePanels.Add(updatePanel);
            if (this._panelsInitialized)
            {
                updatePanel.Initialize();
            }
        }

        internal void Render(HtmlTextWriter writer)
        {
            if ((!((IControl) this._owner).DesignMode && !this._owner.IsInAsyncPostBack) && this._owner.SupportsPartialRendering)
            {
                this._owner.IPage.VerifyRenderingInServerForm(this._owner);
                this.RenderPageRequestManagerScript(writer);
            }
        }

        private void RenderDataItems(HtmlTextWriter writer)
        {
            if (this._scriptDataItems != null)
            {
                foreach (ScriptDataItem item in this._scriptDataItems)
                {
                    EncodeString(writer, item.IsJsonSerialized ? "dataItemJson" : "dataItem", item.Control.ClientID, item.DataItem);
                }
            }
        }

        private void RenderFormCallback(HtmlTextWriter writer, Control containerControl)
        {
            ParserStringWriter innerWriter = writer.InnerWriter as ParserStringWriter;
            innerWriter.ParseWrites = false;
            if (this._updatePanelsToRefresh != null)
            {
                foreach (UpdatePanel panel in this._updatePanelsToRefresh)
                {
                    if (panel.Visible)
                    {
                        panel.RenderControl(this._updatePanelWriter);
                    }
                }
            }
            IPage iPage = this._owner.IPage;
            if (iPage.EnableEventValidation)
            {
                TextWriter writer3 = null;
                bool flag = false;
                try
                {
                    writer3 = iPage.Response.SwitchWriter(TextWriter.Null);
                    flag = true;
                    HtmlTextWriter writer4 = new HtmlTextWriter(TextWriter.Null);
                    foreach (Control control in containerControl.Controls)
                    {
                        control.RenderControl(writer4);
                    }
                }
                finally
                {
                    if (flag)
                    {
                        iPage.Response.SwitchWriter(writer3);
                    }
                }
            }
            innerWriter.ParseWrites = true;
        }

        private void RenderPageCallback(HtmlTextWriter writer, Control pageControl)
        {
            this.ProcessUpdatePanels();
            HttpResponseBase response = this._owner.IPage.Response;
            response.ContentType = "text/plain";
            response.Cache.SetNoServerCaching();
            IHtmlForm form = this._owner.IPage.Form;
            form.SetRenderMethodDelegate(new RenderMethod(this.RenderFormCallback));
            this._updatePanelWriter = writer;
            ParserStringWriter writer2 = new ParserStringWriter();
            ParserHtmlTextWriter writer3 = new ParserHtmlTextWriter(writer2);
            writer2.ParseWrites = true;
            form.RenderControl(writer3);
            writer2.ParseWrites = false;
            foreach (KeyValuePair<string, string> pair in writer2.HiddenFields)
            {
                if (ControlUtil.IsBuiltInHiddenField(pair.Key))
                {
                    EncodeString(writer, "hiddenField", pair.Key, pair.Value);
                }
            }
            EncodeString(writer, "asyncPostBackControlIDs", string.Empty, this.GetAsyncPostBackControlIDs(false));
            EncodeString(writer, "postBackControlIDs", string.Empty, this.GetPostBackControlIDs(false));
            EncodeString(writer, "updatePanelIDs", string.Empty, this.GetAllUpdatePanelIDs());
            EncodeString(writer, "childUpdatePanelIDs", string.Empty, this.GetChildUpdatePanelIDs());
            EncodeString(writer, "panelsToRefreshIDs", string.Empty, this.GetRefreshingUpdatePanelIDs());
            EncodeString(writer, "asyncPostBackTimeout", string.Empty, this._owner.AsyncPostBackTimeout.ToString(CultureInfo.InvariantCulture));
            if (writer3.FormAction != null)
            {
                EncodeString(writer, "formAction", string.Empty, writer3.FormAction);
            }
            if (this._owner.IPage.Header != null)
            {
                string title = this._owner.IPage.Title;
                if (!string.IsNullOrEmpty(title))
                {
                    EncodeString(writer, "pageTitle", string.Empty, title);
                }
            }
            this.RenderDataItems(writer);
            this.ProcessScriptRegistration(writer);
            this.ProcessFocus(writer);
        }

        internal void RenderPageRequestManagerScript(HtmlTextWriter writer)
        {
            writer.Write("<script type=\"text/javascript\">\r\n//<![CDATA[\r\nSys.WebForms.PageRequestManager._initialize('");
            writer.Write(this._owner.UniqueID);
            writer.Write("', document.getElementById('");
            writer.Write(this._owner.IPage.Form.ClientID);
            writer.Write("'));\r\nSys.WebForms.PageRequestManager.getInstance()._updateControls([");
            RenderUpdatePanelIDsFromList(writer, this._allUpdatePanels);
            writer.Write("], [");
            writer.Write(this.GetAsyncPostBackControlIDs(true));
            writer.Write("], [");
            writer.Write(this.GetPostBackControlIDs(true));
            writer.Write("], ");
            writer.Write(this._owner.AsyncPostBackTimeout.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine(");");
            writer.Write("//]]>\r\n</script>\r\n");
        }

        private static void RenderUpdatePanelIDsFromList(HtmlTextWriter writer, List<UpdatePanel> list)
        {
            if ((list != null) && (list.Count > 0))
            {
                bool flag = true;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Visible)
                    {
                        if (!flag)
                        {
                            writer.Write(',');
                        }
                        flag = false;
                        writer.Write('\'');
                        writer.Write(list[i].ChildrenAsTriggers ? 't' : 'f');
                        writer.Write(list[i].UniqueID);
                        writer.Write('\'');
                    }
                }
            }
        }

        public void SetFocus(string clientID)
        {
            this._owner.IPage.SetFocus(clientID);
            this.SetFocusInternal(clientID);
        }

        public void SetFocus(Control control)
        {
            this._owner.IPage.SetFocus(control);
            if (this._owner.IsInAsyncPostBack)
            {
                this._focusedControl = control;
                this._focusedControlID = null;
                this.RegisterFocusScript();
            }
        }

        internal void SetFocusInternal(string clientID)
        {
            if (this._owner.IsInAsyncPostBack)
            {
                this._focusedControlID = clientID.Trim();
                this._focusedControl = null;
                this.RegisterFocusScript();
            }
        }

        internal void UnregisterUpdatePanel(UpdatePanel updatePanel)
        {
            if ((this._allUpdatePanels == null) || !this._allUpdatePanels.Contains(updatePanel))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ScriptManager_UpdatePanelNotRegistered, new object[] { updatePanel.ID }), "updatePanel");
            }
            this._allUpdatePanels.Remove(updatePanel);
        }

        public string AsyncPostBackSourceElementID
        {
            get
            {
                if (this._asyncPostBackSourceElementID == null)
                {
                    return string.Empty;
                }
                return this._asyncPostBackSourceElementID;
            }
        }

        private bool ClientSupportsFocus
        {
            get
            {
                HttpBrowserCapabilitiesBase browser = this._owner.IPage.Request.Browser;
                if (browser.EcmaScriptVersion < FocusMinimumEcmaVersion)
                {
                    return (browser.JScriptVersion >= FocusMinimumJScriptVersion);
                }
                return true;
            }
        }

        private bool EnableLegacyRendering =>
            this._owner.EnableLegacyRendering;

        private sealed class ParserHtmlTextWriter : HtmlTextWriter
        {
            private string _formAction;
            private bool _writingForm;

            public ParserHtmlTextWriter(TextWriter writer) : base(writer)
            {
            }

            public override void WriteAttribute(string name, string value, bool fEncode)
            {
                base.WriteAttribute(name, value, fEncode);
                if (this._writingForm && (name == "action"))
                {
                    this._formAction = value;
                }
            }

            public override void WriteBeginTag(string tagName)
            {
                base.WriteBeginTag(tagName);
                this._writingForm = tagName == "form";
            }

            public string FormAction =>
                this._formAction;
        }

        private sealed class ParserStringWriter : StringWriter
        {
            private Dictionary<string, string> _hiddenFields;
            private string _matchingHiddenFieldName;
            private ParserState _parserState;
            private bool _parseWrites;
            private List<string> _pendingWrites;
            private string _proposedHiddenFieldName;
            private StringBuilder _proposedHiddenFieldValue;
            private bool _secondTry;

            public ParserStringWriter() : base(CultureInfo.CurrentCulture)
            {
                this._pendingWrites = new List<string>();
                this._proposedHiddenFieldName = string.Empty;
                this._matchingHiddenFieldName = string.Empty;
                this._proposedHiddenFieldValue = new StringBuilder();
                this._hiddenFields = new Dictionary<string, string>();
            }

            private void FlushPendingWrites()
            {
                if (this._pendingWrites.Count > 0)
                {
                    foreach (string str in this._pendingWrites)
                    {
                        base.Write(str);
                    }
                    this._pendingWrites.Clear();
                }
            }

            private void ParseString(string s)
            {
                switch (this._parserState)
                {
                    case ParserState.Ready:
                        if (s != "<input type=\"hidden\" name=\"")
                        {
                            goto Label_019F;
                        }
                        this._parserState = ParserState.ReadHiddenFieldNameValue;
                        this._pendingWrites.Add(s);
                        return;

                    case ParserState.ReadHiddenFieldNameValue:
                        break;

                    case ParserState.ReadHiddenFieldIdAttribute:
                        if (s == "\" id=\"")
                        {
                            this._parserState = ParserState.ReadHiddenFieldIdValue;
                            this._pendingWrites.Add(s);
                            this._secondTry = false;
                            return;
                        }
                        if (!this._secondTry)
                        {
                            this._secondTry = true;
                            break;
                        }
                        this._parserState = ParserState.Ready;
                        goto Label_019F;

                    case ParserState.ReadHiddenFieldIdValue:
                        goto Label_00B9;

                    case ParserState.ReadHiddenFieldValueAttribute:
                        if ((this._matchingHiddenFieldName == this._proposedHiddenFieldName) && (s == "\" value=\""))
                        {
                            this._parserState = ParserState.ReadHiddenFieldValueValue;
                            this._pendingWrites.Add(s);
                            this._secondTry = false;
                            return;
                        }
                        if (!this._secondTry)
                        {
                            this._secondTry = true;
                            goto Label_00B9;
                        }
                        this._parserState = ParserState.Ready;
                        goto Label_019F;

                    case ParserState.ReadHiddenFieldValueValue:
                        if (s == "\" />")
                        {
                            this._pendingWrites.Clear();
                            this._hiddenFields.Add(this._proposedHiddenFieldName, this._proposedHiddenFieldValue.ToString());
                            this._proposedHiddenFieldName = string.Empty;
                            this._matchingHiddenFieldName = string.Empty;
                            this._proposedHiddenFieldValue = new StringBuilder();
                            this._parserState = ParserState.Ready;
                            return;
                        }
                        this._proposedHiddenFieldValue.Append(s);
                        return;

                    default:
                        goto Label_019F;
                }
                this._proposedHiddenFieldName = this._proposedHiddenFieldName + s;
                this._pendingWrites.Add(s);
                this._parserState = ParserState.ReadHiddenFieldIdAttribute;
                return;
            Label_00B9:
                this._matchingHiddenFieldName = this._matchingHiddenFieldName + s;
                this._pendingWrites.Add(s);
                this._parserState = ParserState.ReadHiddenFieldValueAttribute;
                return;
            Label_019F:
                this._secondTry = false;
                this.FlushPendingWrites();
                base.Write(s);
            }

            public override void Write(string s)
            {
                if (!this.ParseWrites)
                {
                    base.Write(s);
                }
                else
                {
                    this.ParseString(s);
                }
            }

            public override void WriteLine(string value)
            {
                if (!this.ParseWrites)
                {
                    base.WriteLine(value);
                }
                else
                {
                    this.ParseString(value);
                    this.ParseString(this.NewLine);
                }
            }

            public IDictionary<string, string> HiddenFields =>
                this._hiddenFields;

            public bool ParseWrites
            {
                get => 
                    this._parseWrites;
                set
                {
                    this._parseWrites = value;
                }
            }

            private enum ParserState
            {
                Ready,
                ReadHiddenFieldNameValue,
                ReadHiddenFieldIdAttribute,
                ReadHiddenFieldIdValue,
                ReadHiddenFieldValueAttribute,
                ReadHiddenFieldValueValue
            }
        }

        private sealed class ScriptDataItem
        {
            private System.Web.UI.Control _control;
            private string _dataItem;
            private bool _isJsonSerialized;

            public ScriptDataItem(System.Web.UI.Control control, string dataItem, bool isJsonSerialized)
            {
                this._control = control;
                this._dataItem = (dataItem == null) ? string.Empty : dataItem;
                this._isJsonSerialized = isJsonSerialized;
            }

            public System.Web.UI.Control Control =>
                this._control;

            public string DataItem =>
                this._dataItem;

            public bool IsJsonSerialized =>
                this._isJsonSerialized;
        }

        private sealed class ScriptDataItemCollection : List<PageRequestManager.ScriptDataItem>
        {
            public bool ContainsControl(Control control)
            {
                foreach (PageRequestManager.ScriptDataItem item in this)
                {
                    if (item.Control == control)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}

