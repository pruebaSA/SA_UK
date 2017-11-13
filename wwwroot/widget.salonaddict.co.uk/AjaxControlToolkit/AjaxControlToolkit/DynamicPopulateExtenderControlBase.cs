namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [RequiredScript(typeof(DynamicPopulateExtender))]
    public abstract class DynamicPopulateExtenderControlBase : AnimationExtenderControlBase
    {
        protected DynamicPopulateExtenderControlBase()
        {
        }

        public override void EnsureValid()
        {
            base.EnsureValid();
            if ((!string.IsNullOrEmpty(this.DynamicControlID) || !string.IsNullOrEmpty(this.DynamicContextKey)) || (!string.IsNullOrEmpty(this.DynamicServicePath) || !string.IsNullOrEmpty(this.DynamicServiceMethod)))
            {
                if (string.IsNullOrEmpty(this.DynamicControlID))
                {
                    throw new ArgumentException("DynamicControlID must be set");
                }
                if (string.IsNullOrEmpty(this.DynamicServiceMethod))
                {
                    throw new ArgumentException("DynamicServiceMethod must be set");
                }
            }
        }

        private bool ShouldSerializeServicePath() => 
            !string.IsNullOrEmpty(this.DynamicServiceMethod);

        [ClientPropertyName("cacheDynamicResults"), ExtenderControlProperty, DefaultValue(false), Category("Behavior")]
        public bool CacheDynamicResults
        {
            get => 
                base.GetPropertyValue<bool>("CacheDynamicResults", false);
            set
            {
                base.SetPropertyValue<bool>("CacheDynamicResults", value);
            }
        }

        [ClientPropertyName("dynamicContextKey"), ExtenderControlProperty, Category("Behavior"), DefaultValue("")]
        public string DynamicContextKey
        {
            get => 
                base.GetPropertyValue<string>("DynamicContextKey", "");
            set
            {
                base.SetPropertyValue<string>("DynamicContextKey", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty, ClientPropertyName("dynamicControlID"), IDReferenceProperty(typeof(WebControl)), Category("Behavior")]
        public string DynamicControlID
        {
            get => 
                base.GetPropertyValue<string>("DynamicControlID", "");
            set
            {
                base.SetPropertyValue<string>("DynamicControlID", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty, ClientPropertyName("dynamicServiceMethod"), Category("Behavior")]
        public string DynamicServiceMethod
        {
            get => 
                base.GetPropertyValue<string>("DynamicServiceMethod", "");
            set
            {
                base.SetPropertyValue<string>("DynamicServiceMethod", value);
            }
        }

        [Category("Behavior"), ExtenderControlProperty, ClientPropertyName("dynamicServicePath"), UrlProperty, TypeConverter(typeof(ServicePathConverter))]
        public string DynamicServicePath
        {
            get => 
                base.GetPropertyValue<string>("DynamicServicePath", "");
            set
            {
                base.SetPropertyValue<string>("DynamicServicePath", value);
            }
        }
    }
}

