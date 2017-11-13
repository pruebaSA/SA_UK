namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;

    [RequiredScript(typeof(CommonToolkitScripts)), Designer("AjaxControlToolkit.DynamicPopulateDesigner, AjaxControlToolkit"), ClientScriptResource("Sys.Extended.UI.DynamicPopulateBehavior", "DynamicPopulate.DynamicPopulateBehavior.js"), ToolboxBitmap(typeof(DynamicPopulateExtender), "DynamicPopulate.DynamicPopulate.ico"), TargetControlType(typeof(Control))]
    public class DynamicPopulateExtender : ExtenderControlBase
    {
        protected override bool CheckIfValid(bool throwException)
        {
            if (!string.IsNullOrEmpty(this.CustomScript) || !string.IsNullOrEmpty(this.ServiceMethod))
            {
                return base.CheckIfValid(throwException);
            }
            if (throwException)
            {
                throw new InvalidOperationException("CustomScript or ServiceMethod must be set.");
            }
            return false;
        }

        private bool ShouldSerializeServicePath() => 
            !string.IsNullOrEmpty(this.ServiceMethod);

        [Category("Behavior"), ClientPropertyName("cacheDynamicResults"), ExtenderControlProperty, DefaultValue(false)]
        public bool CacheDynamicResults
        {
            get => 
                base.GetPropertyValue<bool>("CacheDynamicResults", false);
            set
            {
                base.SetPropertyValue<bool>("CacheDynamicResults", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(true), Category("Behavior")]
        public bool ClearContentsDuringUpdate
        {
            get => 
                base.GetPropertyValue<bool>("ClearContentsDuringUpdate", true);
            set
            {
                base.SetPropertyValue<bool>("ClearContentsDuringUpdate", value);
            }
        }

        [Category("Behavior"), DefaultValue(""), ExtenderControlProperty]
        public string ContextKey
        {
            get => 
                base.GetPropertyValue<string>("ContextKey", "");
            set
            {
                base.SetPropertyValue<string>("ContextKey", value);
            }
        }

        [Category("Behavior"), ExtenderControlProperty, DefaultValue("")]
        public string CustomScript
        {
            get => 
                base.GetPropertyValue<string>("CustomScript", "");
            set
            {
                if (!string.IsNullOrEmpty(this.ServiceMethod))
                {
                    throw new InvalidOperationException("CustomScript can not be set if a ServiceMethod is set.");
                }
                base.SetPropertyValue<string>("CustomScript", value);
            }
        }

        [ExtenderControlProperty, Category("Behavior"), IDReferenceProperty(typeof(Control)), ClientPropertyName("PopulateTriggerID")]
        public string PopulateTriggerControlID
        {
            get => 
                base.GetPropertyValue<string>("PopulateTriggerControlID", "");
            set
            {
                base.SetPropertyValue<string>("PopulateTriggerControlID", value);
            }
        }

        [Category("Behavior"), ExtenderControlProperty, DefaultValue("")]
        public string ServiceMethod
        {
            get => 
                base.GetPropertyValue<string>("ServiceMethod", "");
            set
            {
                if (!string.IsNullOrEmpty(this.CustomScript))
                {
                    throw new InvalidOperationException("ServiceMethod can not be set if a CustomScript is set.");
                }
                base.SetPropertyValue<string>("ServiceMethod", value);
            }
        }

        [UrlProperty, ExtenderControlProperty, TypeConverter(typeof(ServicePathConverter)), Category("Behavior")]
        public string ServicePath
        {
            get => 
                base.GetPropertyValue<string>("ServicePath", "");
            set
            {
                base.SetPropertyValue<string>("ServicePath", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(""), Category("Behavior")]
        public string UpdatingCssClass
        {
            get => 
                base.GetPropertyValue<string>("UpdatingCss", "");
            set
            {
                base.SetPropertyValue<string>("UpdatingCss", value);
            }
        }
    }
}

