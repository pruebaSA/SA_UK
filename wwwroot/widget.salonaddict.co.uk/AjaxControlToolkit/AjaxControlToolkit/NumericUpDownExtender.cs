namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxBitmap(typeof(NumericUpDownExtender), "NumericUpDown.NumericUpDown.ico"), Designer("AjaxControlToolkit.NumericUpDownDesigner, AjaxControlToolkit"), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.NumericUpDownBehavior", "NumericUpDown.NumericUpDownBehavior.js"), TargetControlType(typeof(TextBox))]
    public class NumericUpDownExtender : ExtenderControlBase
    {
        private bool ShouldSerializeServiceUpPath() => 
            !string.IsNullOrEmpty(this.ServiceUpMethod);

        private bool ShouldSerializeServieDownPath() => 
            !string.IsNullOrEmpty(this.ServiceDownMethod);

        [ExtenderControlProperty]
        public double Maximum
        {
            get => 
                base.GetPropertyValue<double>("Maximum", double.MaxValue);
            set
            {
                base.SetPropertyValue<double>("Maximum", value);
            }
        }

        [ExtenderControlProperty]
        public double Minimum
        {
            get => 
                base.GetPropertyValue<double>("Minimum", double.MinValue);
            set
            {
                base.SetPropertyValue<double>("Minimum", value);
            }
        }

        [Editor("System.ComponentModel.Design.MultilineStringEditor", typeof(UITypeEditor)), ExtenderControlProperty]
        public string RefValues
        {
            get => 
                base.GetPropertyValue<string>("RefValues", "");
            set
            {
                base.SetPropertyValue<string>("RefValues", value);
            }
        }

        [ExtenderControlProperty]
        public string ServiceDownMethod
        {
            get => 
                base.GetPropertyValue<string>("ServiceDownMethod", "");
            set
            {
                base.SetPropertyValue<string>("ServiceDownMethod", value);
            }
        }

        [TypeConverter(typeof(ServicePathConverter)), UrlProperty, Editor("System.Web.UI.Design.UrlEditor", typeof(UITypeEditor)), ExtenderControlProperty]
        public string ServiceDownPath
        {
            get => 
                base.GetPropertyValue<string>("ServiceDownPath", "");
            set
            {
                base.SetPropertyValue<string>("ServiceDownPath", value);
            }
        }

        [ExtenderControlProperty]
        public string ServiceUpMethod
        {
            get => 
                base.GetPropertyValue<string>("ServiceUpMethod", "");
            set
            {
                base.SetPropertyValue<string>("ServiceUpMethod", value);
            }
        }

        [ExtenderControlProperty, TypeConverter(typeof(ServicePathConverter)), UrlProperty, Editor("System.Web.UI.Design.UrlEditor", typeof(UITypeEditor))]
        public string ServiceUpPath
        {
            get => 
                base.GetPropertyValue<string>("ServiceUpPath", "");
            set
            {
                base.SetPropertyValue<string>("ServiceUpPath", value);
            }
        }

        [ExtenderControlProperty, DefaultValue((double) 1.0)]
        public double Step
        {
            get => 
                base.GetPropertyValue<double>("Step", 1.0);
            set
            {
                base.SetPropertyValue<double>("Step", value);
            }
        }

        [ExtenderControlProperty]
        public string Tag
        {
            get => 
                base.GetPropertyValue<string>("Tag", "");
            set
            {
                base.SetPropertyValue<string>("Tag", value);
            }
        }

        [ExtenderControlProperty, IDReferenceProperty(typeof(Control))]
        public string TargetButtonDownID
        {
            get => 
                base.GetPropertyValue<string>("TargetButtonDownID", "");
            set
            {
                base.SetPropertyValue<string>("TargetButtonDownID", value);
            }
        }

        [ExtenderControlProperty, IDReferenceProperty(typeof(Control))]
        public string TargetButtonUpID
        {
            get => 
                base.GetPropertyValue<string>("TargetButtonUpID", "");
            set
            {
                base.SetPropertyValue<string>("TargetButtonUpID", value);
            }
        }

        [RequiredProperty, ExtenderControlProperty]
        public int Width
        {
            get => 
                base.GetPropertyValue<int>("Width", 0);
            set
            {
                base.SetPropertyValue<int>("Width", value);
            }
        }
    }
}

