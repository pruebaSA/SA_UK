namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using AjaxControlToolkit.HTMLEditor.Popups;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [PersistChildren(false), ParseChildren(true), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.ColorSelector", "HTMLEditor.Toolbar_buttons.ColorSelector.js"), RequiredScript(typeof(CommonToolkitScripts))]
    public abstract class ColorSelector : Selector
    {
        private string _fixedColorButtonId = "";

        protected ColorSelector()
        {
        }

        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            if ((this.FixedColorButtonId.Length > 0) && !base.IsDesign)
            {
                FixedColorButton button = this.Parent.FindControl(this.FixedColorButtonId) as FixedColorButton;
                if (button == null)
                {
                    throw new ArgumentException("FixedColorButton control's ID expected");
                }
                descriptor.AddComponentProperty("fixedColorButton", button.ClientID);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            base.RelatedPopup = new BaseColorsPopup();
        }

        protected override void OnPreRender(EventArgs e)
        {
            if ((this.FixedColorButtonId.Length > 0) && !base.IsDesign)
            {
                FixedColorButton button = this.Parent.FindControl(this.FixedColorButtonId) as FixedColorButton;
                if (button != null)
                {
                    this.ToolTip = button.ToolTip;
                }
            }
            base.OnPreRender(e);
        }

        [DefaultValue("")]
        public string FixedColorButtonId
        {
            get => 
                this._fixedColorButtonId;
            set
            {
                this._fixedColorButtonId = value;
            }
        }
    }
}

