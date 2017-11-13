namespace AjaxControlToolkit
{
    using AjaxControlToolkit.Properties;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Web;
    using System.Web.UI;

    [ClientScriptResource(null, "ExtenderBase.BaseScripts.js")]
    public class ScriptControlBase : ScriptControl, INamingContainer, IControlResolver, IPostBackDataHandler, ICallbackEventHandler, IClientStateManager
    {
        private string _cachedClientStateFieldID;
        private string _callbackArgument;
        private bool _enableClientState;
        private bool _renderingScript;
        private System.Web.UI.ScriptManager _scriptManager;
        private HtmlTextWriterTag _tagKey;
        private string _tagName;

        protected ScriptControlBase() : this(false)
        {
        }

        protected ScriptControlBase(bool enableClientState)
        {
            this._enableClientState = enableClientState;
        }

        protected ScriptControlBase(string tag) : this(false, tag)
        {
        }

        public ScriptControlBase(HtmlTextWriterTag tag) : this(false, tag)
        {
        }

        protected ScriptControlBase(bool enableClientState, string tag)
        {
            this._tagKey = HtmlTextWriterTag.Unknown;
            this._tagName = tag;
            this._enableClientState = enableClientState;
        }

        protected ScriptControlBase(bool enableClientState, HtmlTextWriterTag tag)
        {
            this._tagKey = tag;
            this._enableClientState = enableClientState;
        }

        void IClientStateManager.LoadClientState(string clientState)
        {
            this.LoadClientState(clientState);
        }

        string IClientStateManager.SaveClientState() => 
            this.SaveClientState();

        protected virtual void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            try
            {
                this._renderingScript = true;
                ScriptObjectBuilder.DescribeComponent(this, descriptor, this, this);
            }
            finally
            {
                this._renderingScript = false;
            }
            if (this.SupportsClientState)
            {
                descriptor.AddElementProperty("clientStateField", this.ClientStateFieldID);
            }
        }

        private void EnsureScriptManager()
        {
            if (this._scriptManager == null)
            {
                this._scriptManager = System.Web.UI.ScriptManager.GetCurrent(this.Page);
                if (this._scriptManager == null)
                {
                    throw new HttpException(Resources.E_NoScriptManager);
                }
            }
        }

        public override Control FindControl(string id)
        {
            Control control = base.FindControl(id);
            if (control != null)
            {
                return control;
            }
            for (Control control2 = this.NamingContainer; control2 != null; control2 = control2.NamingContainer)
            {
                control = control2.FindControl(id);
                if (control != null)
                {
                    return control;
                }
            }
            return null;
        }

        protected virtual string GetCallbackResult()
        {
            string callbackArgument = this._callbackArgument;
            this._callbackArgument = null;
            return ScriptObjectBuilder.ExecuteCallbackMethod(this, callbackArgument);
        }

        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            if (!this.Visible)
            {
                return null;
            }
            base.EnsureID();
            List<ScriptDescriptor> list = new List<ScriptDescriptor>();
            ScriptControlDescriptor descriptor = new ScriptControlDescriptor(this.ClientControlType, this.ClientID);
            this.DescribeComponent(descriptor);
            list.Add(descriptor);
            return list;
        }

        protected override IEnumerable<ScriptReference> GetScriptReferences()
        {
            if (!this.Visible)
            {
                return null;
            }
            List<ScriptReference> list = new List<ScriptReference>();
            list.AddRange(ScriptObjectBuilder.GetScriptReferences(base.GetType()));
            if (this.ScriptPath.Length > 0)
            {
                list.Add(new ScriptReference(this.ScriptPath));
            }
            return list;
        }

        protected virtual void LoadClientState(string clientState)
        {
        }

        protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            if (this.SupportsClientState)
            {
                string str = postCollection[this.ClientStateFieldID];
                if (!string.IsNullOrEmpty(str))
                {
                    this.LoadClientState(str);
                }
            }
            return false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ScriptObjectBuilder.RegisterCssReferences(this);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            base.EnsureID();
            this.EnsureScriptManager();
            if (this.SupportsClientState)
            {
                System.Web.UI.ScriptManager.RegisterHiddenField(this, this.ClientStateFieldID, this.SaveClientState());
                this.Page.RegisterRequiresPostBack(this);
            }
        }

        protected virtual void RaiseCallbackEvent(string eventArgument)
        {
            this._callbackArgument = eventArgument;
        }

        protected virtual void RaisePostDataChangedEvent()
        {
        }

        public Control ResolveControl(string controlId) => 
            this.FindControl(controlId);

        protected virtual string SaveClientState() => 
            null;

        string ICallbackEventHandler.GetCallbackResult() => 
            this.GetCallbackResult();

        void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            this.RaiseCallbackEvent(eventArgument);
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) => 
            this.LoadPostData(postDataKey, postCollection);

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            this.RaisePostDataChangedEvent();
        }

        bool IClientStateManager.SupportsClientState =>
            this.SupportsClientState;

        protected virtual string ClientControlType
        {
            get
            {
                ClientScriptResourceAttribute attribute = (ClientScriptResourceAttribute) TypeDescriptor.GetAttributes(this)[typeof(ClientScriptResourceAttribute)];
                return attribute.ComponentType;
            }
        }

        protected string ClientStateFieldID
        {
            get
            {
                if (this._cachedClientStateFieldID == null)
                {
                    this._cachedClientStateFieldID = this.ClientID + "_ClientState";
                }
                return this._cachedClientStateFieldID;
            }
        }

        protected bool IsRenderingScript =>
            this._renderingScript;

        protected System.Web.UI.ScriptManager ScriptManager
        {
            get
            {
                this.EnsureScriptManager();
                return this._scriptManager;
            }
        }

        [DefaultValue("")]
        public virtual string ScriptPath
        {
            get => 
                (this.ViewState["ScriptPath"] ?? string.Empty);
            set
            {
                this.ViewState["ScriptPath"] = value;
            }
        }

        protected virtual bool SupportsClientState =>
            this._enableClientState;

        protected override HtmlTextWriterTag TagKey =>
            this._tagKey;

        protected override string TagName
        {
            get
            {
                if ((this._tagName == null) && (this.TagKey != HtmlTextWriterTag.Unknown))
                {
                    this._tagName = Enum.Format(typeof(HtmlTextWriterTag), this.TagKey, "G").ToLower(CultureInfo.InvariantCulture);
                }
                return this._tagName;
            }
        }
    }
}

