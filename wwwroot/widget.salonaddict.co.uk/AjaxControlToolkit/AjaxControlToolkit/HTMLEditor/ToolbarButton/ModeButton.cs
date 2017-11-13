namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using AjaxControlToolkit.HTMLEditor;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [PersistChildren(false), RequiredScript(typeof(Enums)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.ModeButton", "HTMLEditor.Toolbar_buttons.ModeButton.js"), ParseChildren(true), RequiredScript(typeof(CommonToolkitScripts))]
    public abstract class ModeButton : ImageButton
    {
        public ModeButton()
        {
            base.ActiveModes.Add(ActiveModeType.Design);
            base.ActiveModes.Add(ActiveModeType.Html);
            base.ActiveModes.Add(ActiveModeType.Preview);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeActiveMode() => 
            base.IsRenderingScript;

        [ExtenderControlProperty, ClientPropertyName("activeMode"), Category("Behavior"), DefaultValue(0)]
        public ActiveModeType ActiveMode
        {
            get => 
                (this.ViewState["ActiveMode"] ?? ActiveModeType.Design);
            set
            {
                this.ViewState["ActiveMode"] = value;
            }
        }
    }
}

