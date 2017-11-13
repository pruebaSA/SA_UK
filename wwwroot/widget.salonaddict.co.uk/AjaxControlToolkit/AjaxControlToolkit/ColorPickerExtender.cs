namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [TargetControlType(typeof(TextBox)), Designer(typeof(ColorPickerDesigner)), RequiredScript(typeof(CommonToolkitScripts), 0), RequiredScript(typeof(PopupExtender), 1), RequiredScript(typeof(ThreadingScripts), 2), ClientCssResource("ColorPicker.ColorPicker.css"), ClientScriptResource("Sys.Extended.UI.ColorPickerBehavior", "ColorPicker.ColorPickerBehavior.js"), ToolboxBitmap(typeof(ColorPickerExtender), "ColorPicker.ColorPicker.ico")]
    public class ColorPickerExtender : ExtenderControlBase
    {
        [ClientPropertyName("enabled"), ExtenderControlProperty, DefaultValue(true)]
        public virtual bool EnabledOnClient
        {
            get => 
                base.GetPropertyValue<bool>("EnabledOnClient", true);
            set
            {
                base.SetPropertyValue<bool>("EnabledOnClient", value);
            }
        }

        [ClientPropertyName("colorSelectionChanged"), DefaultValue(""), ExtenderControlEvent]
        public virtual string OnClientColorSelectionChanged
        {
            get => 
                base.GetPropertyValue<string>("OnClientColorSelectionChanged", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientColorSelectionChanged", value);
            }
        }

        [ExtenderControlEvent, ClientPropertyName("hidden"), DefaultValue("")]
        public virtual string OnClientHidden
        {
            get => 
                base.GetPropertyValue<string>("OnClientHidden", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientHidden", value);
            }
        }

        [ExtenderControlEvent, DefaultValue(""), ClientPropertyName("hiding")]
        public virtual string OnClientHiding
        {
            get => 
                base.GetPropertyValue<string>("OnClientHiding", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientHiding", value);
            }
        }

        [ClientPropertyName("showing"), DefaultValue(""), ExtenderControlEvent]
        public virtual string OnClientShowing
        {
            get => 
                base.GetPropertyValue<string>("OnClientShowing", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientShowing", value);
            }
        }

        [ExtenderControlEvent, ClientPropertyName("shown"), DefaultValue("")]
        public virtual string OnClientShown
        {
            get => 
                base.GetPropertyValue<string>("OnClientShown", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientShown", value);
            }
        }

        [ClientPropertyName("button"), ExtenderControlProperty, IDReferenceProperty, DefaultValue(""), ElementReference]
        public virtual string PopupButtonID
        {
            get => 
                base.GetPropertyValue<string>("PopupButtonID", string.Empty);
            set
            {
                base.SetPropertyValue<string>("PopupButtonID", value);
            }
        }

        [ClientPropertyName("popupPosition"), Description("Indicates where you want the color picker displayed relative to the textbox."), ExtenderControlProperty, DefaultValue(2)]
        public virtual PositioningMode PopupPosition
        {
            get => 
                base.GetPropertyValue<PositioningMode>("PopupPosition", PositioningMode.BottomLeft);
            set
            {
                base.SetPropertyValue<PositioningMode>("PopupPosition", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(""), ClientPropertyName("sample"), ElementReference, IDReferenceProperty]
        public virtual string SampleControlID
        {
            get => 
                base.GetPropertyValue<string>("SampleControlID", string.Empty);
            set
            {
                base.SetPropertyValue<string>("SampleControlID", value);
            }
        }

        [ClientPropertyName("selectedColor"), ExtenderControlProperty, DefaultValue("")]
        public string SelectedColor
        {
            get => 
                base.GetPropertyValue<string>("SelectedColor", string.Empty);
            set
            {
                base.SetPropertyValue<string>("SelectedColor", value);
            }
        }
    }
}

