namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;

    [ToolboxBitmap(typeof(ResizableControlExtender), "ResizableControl.ResizableControl.ico"), Designer("AjaxControlToolkit.ResizableControlDesigner, AjaxControlToolkit"), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.ResizableControlBehavior", "ResizableControl.ResizableControlBehavior.js"), TargetControlType(typeof(Control))]
    public class ResizableControlExtender : ExtenderControlBase
    {
        private const int MaximumValue = 0x186a0;

        public ResizableControlExtender()
        {
            base.EnableClientState = true;
        }

        public override void EnsureValid()
        {
            base.EnsureValid();
            if (this.MaximumWidth < this.MinimumWidth)
            {
                throw new ArgumentException("Maximum width must not be less than minimum width");
            }
            if (this.MaximumHeight < this.MinimumHeight)
            {
                throw new ArgumentException("Maximum height must not be less than minimum height");
            }
        }

        [ExtenderControlProperty, DefaultValue(""), RequiredProperty]
        public string HandleCssClass
        {
            get => 
                base.GetPropertyValue<string>("HandleCssClass", "");
            set
            {
                base.SetPropertyValue<string>("HandleCssClass", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public int HandleOffsetX
        {
            get => 
                base.GetPropertyValue<int>("HandleOffsetX", 0);
            set
            {
                base.SetPropertyValue<int>("HandleOffsetX", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public int HandleOffsetY
        {
            get => 
                base.GetPropertyValue<int>("HandleOffsetY", 0);
            set
            {
                base.SetPropertyValue<int>("HandleOffsetY", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0x186a0)]
        public int MaximumHeight
        {
            get => 
                base.GetPropertyValue<int>("MaximumHeight", 0x186a0);
            set
            {
                base.SetPropertyValue<int>("MaximumHeight", value);
            }
        }

        [DefaultValue(0x186a0), ExtenderControlProperty]
        public int MaximumWidth
        {
            get => 
                base.GetPropertyValue<int>("MaximumWidth", 0x186a0);
            set
            {
                base.SetPropertyValue<int>("MaximumWidth", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public int MinimumHeight
        {
            get => 
                base.GetPropertyValue<int>("MinimumHeight", 0);
            set
            {
                base.SetPropertyValue<int>("MinimumHeight", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty]
        public int MinimumWidth
        {
            get => 
                base.GetPropertyValue<int>("MinimumWidth", 0);
            set
            {
                base.SetPropertyValue<int>("MinimumWidth", value);
            }
        }

        [ClientPropertyName("resize"), ExtenderControlProperty, DefaultValue("")]
        public string OnClientResize
        {
            get => 
                base.GetPropertyValue<string>("OnClientResize", "");
            set
            {
                base.SetPropertyValue<string>("OnClientResize", value);
            }
        }

        [ClientPropertyName("resizebegin"), ExtenderControlProperty, DefaultValue("")]
        public string OnClientResizeBegin
        {
            get => 
                base.GetPropertyValue<string>("OnClientResizeBegin", "");
            set
            {
                base.SetPropertyValue<string>("OnClientResizeBegin", value);
            }
        }

        [DefaultValue(""), ClientPropertyName("resizing"), ExtenderControlProperty]
        public string OnClientResizing
        {
            get => 
                base.GetPropertyValue<string>("OnClientResizing", "");
            set
            {
                base.SetPropertyValue<string>("OnClientResizing", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty]
        public string ResizableCssClass
        {
            get => 
                base.GetPropertyValue<string>("ResizableCssClass", "");
            set
            {
                base.SetPropertyValue<string>("ResizableCssClass", value);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Drawing.Size Size
        {
            get
            {
                int num;
                int num2;
                string[] strArray = (base.ClientState ?? string.Empty).Split(new char[] { ',' });
                if (((strArray.Length >= 2) && !string.IsNullOrEmpty(strArray[0])) && ((!string.IsNullOrEmpty(strArray[1]) && int.TryParse(strArray[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out num)) && int.TryParse(strArray[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out num2)))
                {
                    return new System.Drawing.Size(num, num2);
                }
                return System.Drawing.Size.Empty;
            }
            set
            {
                base.ClientState = string.Format(CultureInfo.InvariantCulture, "{0},{1}", new object[] { value.Width, value.Height });
            }
        }
    }
}

