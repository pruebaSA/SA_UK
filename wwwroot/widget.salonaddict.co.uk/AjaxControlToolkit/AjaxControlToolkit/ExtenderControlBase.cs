namespace AjaxControlToolkit
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [PersistChildren(false), ClientScriptResource(null, "ExtenderBase.BaseScripts.js"), ParseChildren(true), Themeable(true)]
    public abstract class ExtenderControlBase : ExtenderControl, IControlResolver
    {
        private string _clientState;
        private bool _enableClientState;
        private Dictionary<string, Control> _findControlHelperCache = new Dictionary<string, Control>();
        private bool _isDisposed;
        private bool _loadedClientStateValues;
        private ProfilePropertyBindingCollection _profileBindings;
        private bool _renderingScript;
        internal static string[] ForceSerializationProps = new string[] { "ClientStateFieldID" };

        protected event EventHandler ClientStateValuesLoaded;

        public event ResolveControlEventHandler ResolveControlID;

        protected ExtenderControlBase()
        {
        }

        protected virtual bool CheckIfValid(bool throwException)
        {
            bool flag = true;
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(this))
            {
                if ((descriptor.Attributes[typeof(RequiredPropertyAttribute)] != null) && ((descriptor.GetValue(this) == null) || !descriptor.ShouldSerializeValue(this)))
                {
                    flag = false;
                    if (throwException)
                    {
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "{0} missing required {1} property value for {2}.", new object[] { base.GetType().ToString(), descriptor.Name, this.ID }), descriptor.Name);
                    }
                }
            }
            return flag;
        }

        private HiddenField CreateClientStateField()
        {
            HiddenField child = new HiddenField {
                ID = this.GetClientStateFieldID()
            };
            this.Controls.Add(child);
            this.ClientStateFieldID = child.ID;
            return child;
        }

        public override void Dispose()
        {
            this._isDisposed = true;
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        internal IEnumerable<ScriptReference> EnsureScripts()
        {
            List<ScriptReference> list = new List<ScriptReference>();
            list.AddRange(ScriptObjectBuilder.GetScriptReferences(base.GetType(), null != this.ScriptPath));
            string scriptPath = this.ScriptPath;
            if (!string.IsNullOrEmpty(scriptPath))
            {
                list.Add(new ScriptReference(scriptPath));
            }
            return list;
        }

        public virtual void EnsureValid()
        {
            this.CheckIfValid(true);
        }

        public override Control FindControl(string id) => 
            this.FindControlHelper(id);

        protected Control FindControlHelper(string id)
        {
            Control control = null;
            if (this._findControlHelperCache.ContainsKey(id))
            {
                return this._findControlHelperCache[id];
            }
            control = base.FindControl(id);
            for (Control control2 = this.NamingContainer; (control == null) && (control2 != null); control2 = control2.NamingContainer)
            {
                control = control2.FindControl(id);
            }
            if (control == null)
            {
                ResolveControlEventArgs e = new ResolveControlEventArgs(id);
                this.OnResolveControlID(e);
                control = e.Control;
            }
            if (control != null)
            {
                this._findControlHelperCache[id] = control;
            }
            return control;
        }

        protected string GetClientID(string controlId)
        {
            Control control = this.FindControlHelper(controlId);
            if (control != null)
            {
                controlId = control.ClientID;
            }
            return controlId;
        }

        private string GetClientStateFieldID() => 
            string.Format(CultureInfo.InvariantCulture, "{0}_ClientState", new object[] { this.ID });

        [Obsolete("Use GetPropertyValue<V> instead")]
        protected bool GetPropertyBoolValue(string propertyName) => 
            this.GetPropertyValue<bool>(propertyName, false);

        [Obsolete("Use GetPropertyValue<V> instead")]
        protected int GetPropertyIntValue(string propertyName) => 
            this.GetPropertyValue<int>(propertyName, 0);

        [Obsolete("Use GetPropertyValue<V> instead")]
        protected string GetPropertyStringValue(string propertyName) => 
            this.GetPropertyValue<string>(propertyName, "");

        protected V GetPropertyValue<V>(string propertyName, V nullValue)
        {
            if (this.ViewState[propertyName] == null)
            {
                return nullValue;
            }
            return (V) this.ViewState[propertyName];
        }

        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors(Control targetControl)
        {
            if (!this.Enabled || !targetControl.Visible)
            {
                return null;
            }
            this.EnsureValid();
            ScriptBehaviorDescriptor descriptor = new ScriptBehaviorDescriptor(this.ClientControlType, targetControl.ClientID);
            this.RenderScriptAttributes(descriptor);
            this.RenderInnerScript(descriptor);
            return new List<ScriptDescriptor>(new ScriptDescriptor[] { descriptor });
        }

        protected override IEnumerable<ScriptReference> GetScriptReferences()
        {
            if (this.Enabled)
            {
                return this.EnsureScripts();
            }
            return null;
        }

        private void LoadClientStateValues()
        {
            if (this.EnableClientState && !string.IsNullOrEmpty(this.ClientStateFieldID))
            {
                HiddenField field = (HiddenField) this.NamingContainer.FindControl(this.ClientStateFieldID);
                if ((field != null) && !string.IsNullOrEmpty(field.Value))
                {
                    this.ClientState = field.Value;
                }
            }
            if (this.ClientStateValuesLoaded != null)
            {
                this.ClientStateValuesLoaded(this, EventArgs.Empty);
            }
            this._loadedClientStateValues = true;
        }

        protected override void OnInit(EventArgs e)
        {
            if (this.EnableClientState)
            {
                this.CreateClientStateField();
            }
            this.Page.PreLoad += new EventHandler(this.Page_PreLoad);
            base.OnInit(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this._loadedClientStateValues)
            {
                this.LoadClientStateValues();
            }
            base.OnLoad(e);
            ScriptObjectBuilder.RegisterCssReferences(this);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.Enabled && this.TargetControl.Visible)
            {
                this.SaveClientStateValues();
            }
        }

        protected virtual void OnResolveControlID(ResolveControlEventArgs e)
        {
            if (this.ResolveControlID != null)
            {
                this.ResolveControlID(this, e);
            }
        }

        private void Page_PreLoad(object sender, EventArgs e)
        {
            this.LoadClientStateValues();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (this.Page != null)
            {
                this.Page.VerifyRenderingInServerForm(this);
            }
            base.Render(writer);
        }

        protected virtual void RenderInnerScript(ScriptBehaviorDescriptor descriptor)
        {
        }

        protected virtual void RenderScriptAttributes(ScriptBehaviorDescriptor descriptor)
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
        }

        public Control ResolveControl(string controlId) => 
            this.FindControl(controlId);

        private void SaveClientStateValues()
        {
            if (this.EnableClientState)
            {
                HiddenField field = null;
                if (string.IsNullOrEmpty(this.ClientStateFieldID))
                {
                    field = this.CreateClientStateField();
                }
                else
                {
                    field = (HiddenField) this.NamingContainer.FindControl(this.ClientStateFieldID);
                }
                if (field != null)
                {
                    field.Value = this.ClientState;
                }
            }
        }

        [Obsolete("Replaced by a call to ScriptObjectBuilder")]
        protected object SerializeProperty(PropertyDescriptor prop) => 
            this.SerializeProperty(prop, false);

        [Obsolete("Replaced by a call to ScriptObjectBuilder")]
        protected virtual object SerializeProperty(PropertyDescriptor prop, bool force)
        {
            if (prop == null)
            {
                return null;
            }
            object obj2 = prop.GetValue(this);
            if (obj2 != null)
            {
                bool flag = prop.ShouldSerializeValue(this);
                if (flag)
                {
                    DesignerSerializationVisibilityAttribute attribute = (DesignerSerializationVisibilityAttribute) prop.Attributes[typeof(DesignerSerializationVisibilityAttribute)];
                    if ((attribute != null) && (attribute.Visibility == DesignerSerializationVisibility.Hidden))
                    {
                        flag = -1 != Array.IndexOf<string>(ForceSerializationProps, prop.Name);
                    }
                }
                if (!force && !flag)
                {
                    return null;
                }
                if (!prop.PropertyType.IsPrimitive && !prop.PropertyType.IsEnum)
                {
                    if (prop.PropertyType == typeof(Color))
                    {
                        obj2 = ColorTranslator.ToHtml((Color) obj2);
                    }
                    else
                    {
                        obj2 = prop.Converter.ConvertToString(null, CultureInfo.InvariantCulture, obj2);
                    }
                }
                if ((prop.PropertyType == typeof(string)) && (prop.Attributes[typeof(UrlPropertyAttribute)] != null))
                {
                    obj2 = base.ResolveClientUrl((string) obj2);
                }
            }
            return obj2;
        }

        [Obsolete("Use SetPropertyValue<V> instead")]
        protected void SetPropertyBoolValue(string propertyName, bool value)
        {
            this.SetPropertyValue<bool>(propertyName, value);
        }

        [Obsolete("Use SetPropertyValue<V> instead")]
        protected void SetPropertyIntValue(string propertyName, int value)
        {
            this.SetPropertyValue<int>(propertyName, value);
        }

        [Obsolete("Use SetPropertyValue<V> instead")]
        protected void SetPropertyStringValue(string propertyName, string value)
        {
            this.SetPropertyValue<string>(propertyName, value);
        }

        protected void SetPropertyValue<V>(string propertyName, V value)
        {
            this.ViewState[propertyName] = value;
        }

        private bool ShouldSerializeBehaviorID()
        {
            if (!this.IsRenderingScript)
            {
                return (0 != string.Compare(this.ClientID, this.BehaviorID, StringComparison.OrdinalIgnoreCase));
            }
            return true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeClientStateFieldID() => 
            this.EnableClientState;

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeProfileBindings() => 
            false;

        protected static void SuppressUnusedParameterWarning(object unused)
        {
            if (unused != null)
            {
                unused.GetType();
            }
        }

        protected virtual bool AllowScriptPath =>
            true;

        [ClientPropertyName("id"), ExtenderControlProperty]
        public string BehaviorID
        {
            get
            {
                string propertyValue = this.GetPropertyValue<string>("BehaviorID", "");
                if (!string.IsNullOrEmpty(propertyValue))
                {
                    return propertyValue;
                }
                return this.ClientID;
            }
            set
            {
                this.SetPropertyValue<string>("BehaviorID", value);
            }
        }

        protected virtual string ClientControlType
        {
            get
            {
                ClientScriptResourceAttribute attribute = (ClientScriptResourceAttribute) TypeDescriptor.GetAttributes(this)[typeof(ClientScriptResourceAttribute)];
                return attribute.ComponentType;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ClientState
        {
            get => 
                this._clientState;
            set
            {
                this._clientState = value;
            }
        }

        [Browsable(false), IDReferenceProperty(typeof(HiddenField)), DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), ExtenderControlProperty]
        public string ClientStateFieldID
        {
            get => 
                this.GetPropertyValue<string>("ClientStateFieldID", "");
            set
            {
                this.SetPropertyValue<string>("ClientStateFieldID", value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool EnableClientState
        {
            get => 
                this._enableClientState;
            set
            {
                this._enableClientState = value;
            }
        }

        public bool Enabled
        {
            get
            {
                if (this._isDisposed)
                {
                    return false;
                }
                return this.GetPropertyValue<bool>("Enabled", true);
            }
            set
            {
                this.SetPropertyValue<bool>("Enabled", value);
            }
        }

        protected bool IsRenderingScript =>
            this._renderingScript;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Obsolete("WARNING: ProfileBindings are disabled for this Toolkit release pending technical issues.  We hope to re-enable this in an upcoming release"), PersistenceMode(PersistenceMode.InnerProperty)]
        public ProfilePropertyBindingCollection ProfileBindings
        {
            get
            {
                if (this._profileBindings == null)
                {
                    this._profileBindings = new ProfilePropertyBindingCollection();
                }
                return this._profileBindings;
            }
        }

        public string ScriptPath
        {
            get => 
                this.GetPropertyValue<string>("ScriptPath", null);
            set
            {
                if (!this.AllowScriptPath)
                {
                    throw new InvalidOperationException("This class does not allow setting of ScriptPath.");
                }
                this.SetPropertyValue<string>("ScriptPath", value);
            }
        }

        [Browsable(true)]
        public override string SkinID
        {
            get => 
                base.SkinID;
            set
            {
                base.SkinID = value;
            }
        }

        protected Control TargetControl =>
            this.FindControlHelper(base.TargetControlID);
    }
}

