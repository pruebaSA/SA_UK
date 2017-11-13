namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.Script.Serialization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxBitmap(typeof(AutoCompleteExtender), "AutoComplete.AutoComplete.ico"), ToolboxItem("System.Web.UI.Design.WebControlToolboxItem, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), Designer("AjaxControlToolkit.AutoCompleteDesigner, AjaxControlToolkit"), ClientScriptResource("Sys.Extended.UI.AutoCompleteBehavior", "AutoComplete.AutoCompleteBehavior.js"), RequiredScript(typeof(CommonToolkitScripts)), RequiredScript(typeof(PopupExtender)), RequiredScript(typeof(TimerScript)), RequiredScript(typeof(AnimationExtender)), TargetControlType(typeof(TextBox))]
    public class AutoCompleteExtender : AnimationExtenderControlBase
    {
        private Animation _onHide;
        private Animation _onShow;

        public static string CreateAutoCompleteItem(string text, string value) => 
            new JavaScriptSerializer().Serialize(new Pair(text, value));

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            base.ResolveControlIDs(this._onShow);
            base.ResolveControlIDs(this._onHide);
        }

        [DefaultValue(0x3e8), ExtenderControlProperty, ClientPropertyName("completionInterval")]
        public virtual int CompletionInterval
        {
            get => 
                base.GetPropertyValue<int>("CompletionInterval", 0x3e8);
            set
            {
                base.SetPropertyValue<int>("CompletionInterval", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("completionListCssClass"), DefaultValue("")]
        public string CompletionListCssClass
        {
            get => 
                base.GetPropertyValue<string>("CompletionListCssClass", "");
            set
            {
                base.SetPropertyValue<string>("CompletionListCssClass", value);
            }
        }

        [Obsolete("Instead of passing in CompletionListElementID, use the default flyout and style that using the CssClass properties."), ExtenderControlProperty, IDReferenceProperty(typeof(WebControl)), DefaultValue(""), ClientPropertyName("completionListElementID")]
        public virtual string CompletionListElementID
        {
            get => 
                base.GetPropertyValue<string>("CompletionListElementID", string.Empty);
            set
            {
                base.SetPropertyValue<string>("CompletionListElementID", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("highlightedItemCssClass"), DefaultValue("")]
        public string CompletionListHighlightedItemCssClass
        {
            get => 
                base.GetPropertyValue<string>("CompletionListHighlightedItemCssClass", "");
            set
            {
                base.SetPropertyValue<string>("CompletionListHighlightedItemCssClass", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(""), ClientPropertyName("completionListItemCssClass")]
        public string CompletionListItemCssClass
        {
            get => 
                base.GetPropertyValue<string>("CompletionListItemCssClass", "");
            set
            {
                base.SetPropertyValue<string>("CompletionListItemCssClass", value);
            }
        }

        [DefaultValue(10), ClientPropertyName("completionSetCount"), ExtenderControlProperty]
        public virtual int CompletionSetCount
        {
            get => 
                base.GetPropertyValue<int>("CompletionSetCount", 10);
            set
            {
                base.SetPropertyValue<int>("CompletionSetCount", value);
            }
        }

        [DefaultValue((string) null), ExtenderControlProperty, ClientPropertyName("contextKey")]
        public string ContextKey
        {
            get => 
                base.GetPropertyValue<string>("ContextKey", null);
            set
            {
                base.SetPropertyValue<string>("ContextKey", value);
                this.UseContextKey = true;
            }
        }

        [ClientPropertyName("delimiterCharacters"), ExtenderControlProperty]
        public virtual string DelimiterCharacters
        {
            get => 
                base.GetPropertyValue<string>("DelimiterCharacters", string.Empty);
            set
            {
                base.SetPropertyValue<string>("DelimiterCharacters", value);
            }
        }

        [DefaultValue(true), ExtenderControlProperty, ClientPropertyName("enableCaching")]
        public virtual bool EnableCaching
        {
            get => 
                base.GetPropertyValue<bool>("EnableCaching", true);
            set
            {
                base.SetPropertyValue<bool>("EnableCaching", value);
            }
        }

        [DefaultValue(false), ClientPropertyName("firstRowSelected"), ExtenderControlProperty]
        public virtual bool FirstRowSelected
        {
            get => 
                base.GetPropertyValue<bool>("FirstRowSelected", false);
            set
            {
                base.SetPropertyValue<bool>("FirstRowSelected", value);
            }
        }

        [ClientPropertyName("minimumPrefixLength"), DefaultValue(3), ExtenderControlProperty]
        public virtual int MinimumPrefixLength
        {
            get => 
                base.GetPropertyValue<int>("MinimumPrefixLength", 3);
            set
            {
                base.SetPropertyValue<int>("MinimumPrefixLength", value);
            }
        }

        [DefaultValue(""), ClientPropertyName("hidden"), ExtenderControlEvent]
        public string OnClientHidden
        {
            get => 
                base.GetPropertyValue<string>("OnClientHidden", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientHidden", value);
            }
        }

        [DefaultValue(""), ExtenderControlEvent, ClientPropertyName("hiding")]
        public string OnClientHiding
        {
            get => 
                base.GetPropertyValue<string>("OnClientHiding", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientHiding", value);
            }
        }

        [ClientPropertyName("itemOut"), DefaultValue(""), ExtenderControlEvent]
        public string OnClientItemOut
        {
            get => 
                base.GetPropertyValue<string>("OnClientItemOut", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientItemOut", value);
            }
        }

        [ExtenderControlEvent, DefaultValue(""), ClientPropertyName("itemOver")]
        public string OnClientItemOver
        {
            get => 
                base.GetPropertyValue<string>("OnClientItemOver", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientItemOver", value);
            }
        }

        [DefaultValue(""), ClientPropertyName("itemSelected"), ExtenderControlEvent]
        public string OnClientItemSelected
        {
            get => 
                base.GetPropertyValue<string>("OnClientItemSelected", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientItemSelected", value);
            }
        }

        [ClientPropertyName("populated"), DefaultValue(""), ExtenderControlEvent]
        public string OnClientPopulated
        {
            get => 
                base.GetPropertyValue<string>("OnClientPopulated", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientPopulated", value);
            }
        }

        [DefaultValue(""), ClientPropertyName("populating"), ExtenderControlEvent]
        public string OnClientPopulating
        {
            get => 
                base.GetPropertyValue<string>("OnClientPopulating", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientPopulating", value);
            }
        }

        [ExtenderControlEvent, ClientPropertyName("showing"), DefaultValue("")]
        public string OnClientShowing
        {
            get => 
                base.GetPropertyValue<string>("OnClientShowing", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientShowing", value);
            }
        }

        [ClientPropertyName("shown"), DefaultValue(""), ExtenderControlEvent]
        public string OnClientShown
        {
            get => 
                base.GetPropertyValue<string>("OnClientShown", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientShown", value);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ExtenderControlProperty, ClientPropertyName("onHide"), DefaultValue((string) null)]
        public Animation OnHide
        {
            get => 
                base.GetAnimation(ref this._onHide, "OnHide");
            set
            {
                base.SetAnimation(ref this._onHide, "OnHide", value);
            }
        }

        [ExtenderControlProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ClientPropertyName("onShow"), Browsable(false), DefaultValue((string) null)]
        public Animation OnShow
        {
            get => 
                base.GetAnimation(ref this._onShow, "OnShow");
            set
            {
                base.SetAnimation(ref this._onShow, "OnShow", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty, ClientPropertyName("serviceMethod"), RequiredProperty]
        public virtual string ServiceMethod
        {
            get => 
                base.GetPropertyValue<string>("ServiceMethod", string.Empty);
            set
            {
                base.SetPropertyValue<string>("ServiceMethod", value);
            }
        }

        [ClientPropertyName("servicePath"), UrlProperty, ExtenderControlProperty, TypeConverter(typeof(ServicePathConverter))]
        public virtual string ServicePath
        {
            get => 
                base.GetPropertyValue<string>("ServicePath", string.Empty);
            set
            {
                base.SetPropertyValue<string>("ServicePath", value);
            }
        }

        [ClientPropertyName("showOnlyCurrentWordInCompletionListItem"), ExtenderControlProperty, DefaultValue(false)]
        public bool ShowOnlyCurrentWordInCompletionListItem
        {
            get => 
                base.GetPropertyValue<bool>("ShowOnlyCurrentWordInCompletionListItem", false);
            set
            {
                base.SetPropertyValue<bool>("ShowOnlyCurrentWordInCompletionListItem", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(false), ClientPropertyName("useContextKey")]
        public bool UseContextKey
        {
            get => 
                base.GetPropertyValue<bool>("UseContextKey", false);
            set
            {
                base.SetPropertyValue<bool>("UseContextKey", value);
            }
        }
    }
}

