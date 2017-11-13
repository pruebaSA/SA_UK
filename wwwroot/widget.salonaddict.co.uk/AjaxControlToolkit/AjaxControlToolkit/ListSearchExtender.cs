namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxItem("System.Web.UI.Design.WebControlToolboxItem, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ToolboxBitmap(typeof(ListSearchExtender), "ListSearch.ListSearch.ico"), TargetControlType(typeof(ListControl)), ClientScriptResource("Sys.Extended.UI.ListSearchBehavior", "ListSearch.ListSearchBehavior.js"), RequiredScript(typeof(CommonToolkitScripts), 0), RequiredScript(typeof(PopupControlExtender), 1), RequiredScript(typeof(AnimationExtender), 2), Description("Lets users search incrementally within ListBoxes"), Designer(typeof(ListSearchDesigner))]
    public class ListSearchExtender : AnimationExtenderControlBase
    {
        private Animation _onHide;
        private Animation _onShow;

        public ListSearchExtender()
        {
            base.EnableClientState = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            base.ClientState = (string.Compare(this.Page.Form.DefaultFocus, base.TargetControlID, StringComparison.OrdinalIgnoreCase) == 0) ? "Focused" : null;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            base.ResolveControlIDs(this._onShow);
            base.ResolveControlIDs(this._onHide);
        }

        [ClientPropertyName("isSorted"), DefaultValue(false), ExtenderControlProperty]
        public bool IsSorted
        {
            get => 
                base.GetPropertyValue<bool>("IsSorted", false);
            set
            {
                base.SetPropertyValue<bool>("IsSorted", value);
            }
        }

        [DefaultValue((string) null), Browsable(false), ExtenderControlProperty, ClientPropertyName("onHide"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        [DefaultValue(""), ExtenderControlProperty, Description("CSS class applied to prompt when user clicks list"), ClientPropertyName("promptCssClass")]
        public string PromptCssClass
        {
            get => 
                base.GetPropertyValue<string>("promptCssClass", "");
            set
            {
                base.SetPropertyValue<string>("promptCssClass", value);
            }
        }

        [DefaultValue(0), Description("Indicates where you want the prompt message displayed when the user clicks on the list."), ClientPropertyName("promptPosition"), ExtenderControlProperty]
        public ListSearchPromptPosition PromptPosition
        {
            get => 
                base.GetPropertyValue<ListSearchPromptPosition>("promptPosition", ListSearchPromptPosition.Top);
            set
            {
                base.SetPropertyValue<ListSearchPromptPosition>("promptPosition", value);
            }
        }

        [Description("The prompt text displayed when user clicks the list"), ExtenderControlProperty, ClientPropertyName("promptText"), DefaultValue("Type to search")]
        public string PromptText
        {
            get => 
                base.GetPropertyValue<string>("promptText", "Type to search");
            set
            {
                base.SetPropertyValue<string>("promptText", value);
            }
        }

        [Description("Indicates search criteria to be used to find items."), ExtenderControlProperty, ClientPropertyName("queryPattern"), DefaultValue(0)]
        public ListSearchQueryPattern QueryPattern
        {
            get => 
                base.GetPropertyValue<ListSearchQueryPattern>("QueryPattern", ListSearchQueryPattern.StartsWith);
            set
            {
                base.SetPropertyValue<ListSearchQueryPattern>("QueryPattern", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0), ClientPropertyName("queryTimeout")]
        public int QueryTimeout
        {
            get => 
                base.GetPropertyValue<int>("QueryTimeout", 0);
            set
            {
                base.SetPropertyValue<int>("QueryTimeout", value);
            }
        }
    }
}

