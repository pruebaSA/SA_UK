namespace System.Web.UI
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Web.Resources;
    using System.Web.Util;

    internal class ScriptControlManager
    {
        private OrderedDictionary<IExtenderControl, List<Control>> _extenderControls;
        private bool _pagePreRenderRaised;
        private OrderedDictionary<IScriptControl, int> _scriptControls;
        private ScriptManager _scriptManager;
        private bool _scriptReferencesRegistered;

        public ScriptControlManager(ScriptManager scriptManager)
        {
            this._scriptManager = scriptManager;
        }

        private static void AddScriptReferenceForExtenderControl(List<ScriptReferenceBase> scriptReferences, IExtenderControl extenderControl)
        {
            IEnumerable<ScriptReference> enumerable = extenderControl.GetScriptReferences();
            if (enumerable != null)
            {
                Control control = (Control) extenderControl;
                ClientUrlResolverWrapper wrapper = null;
                foreach (ScriptReference reference in enumerable)
                {
                    if (reference != null)
                    {
                        if (wrapper == null)
                        {
                            wrapper = new ClientUrlResolverWrapper(control);
                        }
                        reference.ClientUrlResolver = wrapper;
                        reference.IsStaticReference = false;
                        reference.ContainingControl = control;
                        scriptReferences.Add(reference);
                    }
                }
            }
        }

        private static void AddScriptReferenceForScriptControl(List<ScriptReferenceBase> scriptReferences, IScriptControl scriptControl)
        {
            IEnumerable<ScriptReference> enumerable = scriptControl.GetScriptReferences();
            if (enumerable != null)
            {
                Control control = (Control) scriptControl;
                ClientUrlResolverWrapper wrapper = null;
                foreach (ScriptReference reference in enumerable)
                {
                    if (reference != null)
                    {
                        if (wrapper == null)
                        {
                            wrapper = new ClientUrlResolverWrapper(control);
                        }
                        reference.ClientUrlResolver = wrapper;
                        reference.IsStaticReference = false;
                        reference.ContainingControl = control;
                        scriptReferences.Add(reference);
                    }
                }
            }
        }

        public void AddScriptReferences(List<ScriptReferenceBase> scriptReferences)
        {
            this.AddScriptReferencesForScriptControls(scriptReferences);
            this.AddScriptReferencesForExtenderControls(scriptReferences);
            this._scriptReferencesRegistered = true;
        }

        private void AddScriptReferencesForExtenderControls(List<ScriptReferenceBase> scriptReferences)
        {
            if (this._extenderControls != null)
            {
                foreach (IExtenderControl control in this._extenderControls.Keys)
                {
                    AddScriptReferenceForExtenderControl(scriptReferences, control);
                }
            }
        }

        private void AddScriptReferencesForScriptControls(List<ScriptReferenceBase> scriptReferences)
        {
            if (this._scriptControls != null)
            {
                foreach (IScriptControl control in this._scriptControls.Keys)
                {
                    AddScriptReferenceForScriptControl(scriptReferences, control);
                }
            }
        }

        private bool InControlTree(Control targetControl)
        {
            for (Control control = targetControl.Parent; control != null; control = control.Parent)
            {
                if (control == this._scriptManager.Page)
                {
                    return true;
                }
            }
            return false;
        }

        public void OnPagePreRender(object sender, EventArgs e)
        {
            this._pagePreRenderRaised = true;
        }

        public void RegisterExtenderControl<TExtenderControl>(TExtenderControl extenderControl, Control targetControl) where TExtenderControl: Control, IExtenderControl
        {
            List<Control> list;
            if (extenderControl == null)
            {
                throw new ArgumentNullException("extenderControl");
            }
            if (targetControl == null)
            {
                throw new ArgumentNullException("targetControl");
            }
            VerifyTargetControlType<TExtenderControl>(extenderControl, targetControl);
            if (!this._pagePreRenderRaised)
            {
                throw new InvalidOperationException(AtlasWeb.ScriptControlManager_RegisterExtenderControlTooEarly);
            }
            if (this._scriptReferencesRegistered)
            {
                throw new InvalidOperationException(AtlasWeb.ScriptControlManager_RegisterExtenderControlTooLate);
            }
            if (!this.ExtenderControls.TryGetValue(extenderControl, out list))
            {
                list = new List<Control>();
                this.ExtenderControls[extenderControl] = list;
            }
            list.Add(targetControl);
        }

        public void RegisterScriptControl<TScriptControl>(TScriptControl scriptControl) where TScriptControl: Control, IScriptControl
        {
            int num;
            if (scriptControl == null)
            {
                throw new ArgumentNullException("scriptControl");
            }
            if (!this._pagePreRenderRaised)
            {
                throw new InvalidOperationException(AtlasWeb.ScriptControlManager_RegisterScriptControlTooEarly);
            }
            if (this._scriptReferencesRegistered)
            {
                throw new InvalidOperationException(AtlasWeb.ScriptControlManager_RegisterScriptControlTooLate);
            }
            this.ScriptControls.TryGetValue(scriptControl, out num);
            num++;
            this.ScriptControls[scriptControl] = num;
        }

        public void RegisterScriptDescriptors(IExtenderControl extenderControl)
        {
            List<Control> list;
            if (extenderControl == null)
            {
                throw new ArgumentNullException("extenderControl");
            }
            Control control = extenderControl as Control;
            if (control == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.Common_ArgumentInvalidType, new object[] { typeof(Control).FullName }), "extenderControl");
            }
            if (!this.ExtenderControls.TryGetValue(extenderControl, out list))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ScriptControlManager_ExtenderControlNotRegistered, new object[] { control.ID }), "extenderControl");
            }
            foreach (Control control2 in list)
            {
                if (control2.Visible && this.InControlTree(control2))
                {
                    IEnumerable<ScriptDescriptor> scriptDescriptors = extenderControl.GetScriptDescriptors(control2);
                    this.RegisterScriptsForScriptDescriptors(scriptDescriptors, control);
                }
            }
        }

        public void RegisterScriptDescriptors(IScriptControl scriptControl)
        {
            int num;
            if (scriptControl == null)
            {
                throw new ArgumentNullException("scriptControl");
            }
            Control control = scriptControl as Control;
            if (control == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.Common_ArgumentInvalidType, new object[] { typeof(Control).FullName }), "scriptControl");
            }
            if (!this.ScriptControls.TryGetValue(scriptControl, out num))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ScriptControlManager_ScriptControlNotRegistered, new object[] { control.ID }), "scriptControl");
            }
            for (int i = 0; i < num; i++)
            {
                IEnumerable<ScriptDescriptor> scriptDescriptors = scriptControl.GetScriptDescriptors();
                this.RegisterScriptsForScriptDescriptors(scriptDescriptors, control);
            }
        }

        private void RegisterScriptsForScriptDescriptors(IEnumerable<ScriptDescriptor> scriptDescriptors, Control control)
        {
            if (scriptDescriptors != null)
            {
                StringBuilder builder = null;
                foreach (ScriptDescriptor descriptor in scriptDescriptors)
                {
                    if (descriptor != null)
                    {
                        if (builder == null)
                        {
                            builder = new StringBuilder();
                            builder.AppendLine("Sys.Application.add_init(function() {");
                        }
                        builder.Append("    ");
                        builder.AppendLine(descriptor.GetScript());
                        descriptor.RegisterDisposeForDescriptor(this._scriptManager, control);
                    }
                }
                if (builder != null)
                {
                    builder.AppendLine("});");
                    string script = builder.ToString();
                    string key = this._scriptManager.CreateUniqueScriptKey();
                    this._scriptManager.RegisterStartupScriptInternal(control, typeof(ScriptManager), key, script, true);
                }
            }
        }

        private static void VerifyTargetControlType<TExtenderControl>(TExtenderControl extenderControl, Control targetControl) where TExtenderControl: Control, IExtenderControl
        {
            Type extenderControlType = extenderControl.GetType();
            Type[] targetControlTypes = TargetControlTypeCache.GetTargetControlTypes(extenderControlType);
            if (targetControlTypes.Length == 0)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ScriptControlManager_NoTargetControlTypes, new object[] { extenderControlType, typeof(TargetControlTypeAttribute) }));
            }
            Type type = targetControl.GetType();
            foreach (Type type3 in targetControlTypes)
            {
                if (type3.IsAssignableFrom(type))
                {
                    return;
                }
            }
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, AtlasWeb.ScriptControlManager_TargetControlTypeInvalid, new object[] { extenderControl.ID, targetControl.ID, extenderControlType, type }));
        }

        private OrderedDictionary<IExtenderControl, List<Control>> ExtenderControls
        {
            get
            {
                if (this._extenderControls == null)
                {
                    this._extenderControls = new OrderedDictionary<IExtenderControl, List<Control>>();
                }
                return this._extenderControls;
            }
        }

        private OrderedDictionary<IScriptControl, int> ScriptControls
        {
            get
            {
                if (this._scriptControls == null)
                {
                    this._scriptControls = new OrderedDictionary<IScriptControl, int>();
                }
                return this._scriptControls;
            }
        }
    }
}

