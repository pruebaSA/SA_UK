namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [RequiredScript(typeof(DropShadowExtender)), ToolboxBitmap(typeof(ModalPopupExtender), "ModalPopup.ModalPopup.ico"), ClientScriptResource("Sys.Extended.UI.ModalPopupBehavior", "ModalPopup.ModalPopupBehavior.js"), ToolboxItem("System.Web.UI.Design.WebControlToolboxItem, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), Designer("AjaxControlToolkit.ModalPopupDesigner, AjaxControlToolkit"), RequiredScript(typeof(CommonToolkitScripts)), RequiredScript(typeof(DragPanelExtender)), RequiredScript(typeof(AnimationExtender)), TargetControlType(typeof(Control))]
    public class ModalPopupExtender : DynamicPopulateExtenderControlBase
    {
        private Animation _onHidden;
        private Animation _onHiding;
        private Animation _onShowing;
        private Animation _onShown;
        private bool? _show;

        private void ChangeVisibility(bool show)
        {
            if (base.TargetControl == null)
            {
                throw new ArgumentNullException("TargetControl", "TargetControl property cannot be null");
            }
            string dataItem = show ? "show" : "hide";
            if (ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
            {
                ScriptManager.GetCurrent(this.Page).RegisterDataItem(base.TargetControl, dataItem);
            }
            else
            {
                string script = string.Format(CultureInfo.InvariantCulture, "(function() {{var fn = function() {{Sys.Extended.UI.ModalPopupBehavior.invokeViaServer('{0}', {1}); Sys.Application.remove_load(fn);}};Sys.Application.add_load(fn);}})();", new object[] { base.BehaviorID, show ? "true" : "false" });
                ScriptManager.RegisterStartupScript(this, typeof(ModalPopupExtender), dataItem + base.BehaviorID, script, true);
            }
        }

        public void Hide()
        {
            this._show = false;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (this._show.HasValue)
            {
                this.ChangeVisibility(this._show.Value);
            }
            base.ResolveControlIDs(this._onShown);
            base.ResolveControlIDs(this._onHidden);
            base.ResolveControlIDs(this._onShowing);
            base.ResolveControlIDs(this._onHiding);
            base.OnPreRender(e);
        }

        public void Show()
        {
            this._show = true;
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string BackgroundCssClass
        {
            get => 
                base.GetPropertyValue<string>("BackgroundCssClass", "");
            set
            {
                base.SetPropertyValue<string>("BackgroundCssClass", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(""), IDReferenceProperty(typeof(WebControl))]
        public string CancelControlID
        {
            get => 
                base.GetPropertyValue<string>("CancelControlID", "");
            set
            {
                base.SetPropertyValue<string>("CancelControlID", value);
            }
        }

        [DefaultValue(false), ExtenderControlProperty, Obsolete("The drag feature on modal popup will be automatically turned on if you specify the PopupDragHandleControlID property. Setting the Drag property is a noop")]
        public bool Drag
        {
            get => 
                base.GetPropertyValue<bool>("stringDrag", false);
            set
            {
                base.SetPropertyValue<bool>("stringDrag", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(false)]
        public bool DropShadow
        {
            get => 
                base.GetPropertyValue<bool>("stringDropShadow", false);
            set
            {
                base.SetPropertyValue<bool>("stringDropShadow", value);
            }
        }

        [ExtenderControlProperty, IDReferenceProperty(typeof(WebControl)), DefaultValue("")]
        public string OkControlID
        {
            get => 
                base.GetPropertyValue<string>("OkControlID", "");
            set
            {
                base.SetPropertyValue<string>("OkControlID", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string OnCancelScript
        {
            get => 
                base.GetPropertyValue<string>("OnCancelScript", "");
            set
            {
                base.SetPropertyValue<string>("OnCancelScript", value);
            }
        }

        [ExtenderControlProperty, Browsable(false)]
        public Animation OnHidden
        {
            get => 
                base.GetAnimation(ref this._onHidden, "OnHidden");
            set
            {
                base.SetAnimation(ref this._onHidden, "OnHidden", value);
            }
        }

        [ExtenderControlProperty, Browsable(false)]
        public Animation OnHiding
        {
            get => 
                base.GetAnimation(ref this._onHiding, "OnHiding");
            set
            {
                base.SetAnimation(ref this._onHiding, "OnHiding", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty]
        public string OnOkScript
        {
            get => 
                base.GetPropertyValue<string>("OnOkScript", "");
            set
            {
                base.SetPropertyValue<string>("OnOkScript", value);
            }
        }

        [Browsable(false), ExtenderControlProperty]
        public Animation OnShowing
        {
            get => 
                base.GetAnimation(ref this._onShowing, "OnShowing");
            set
            {
                base.SetAnimation(ref this._onShowing, "OnShowing", value);
            }
        }

        [ExtenderControlProperty, Browsable(false)]
        public Animation OnShown
        {
            get => 
                base.GetAnimation(ref this._onShown, "OnShown");
            set
            {
                base.SetAnimation(ref this._onShown, "OnShown", value);
            }
        }

        [RequiredProperty, ExtenderControlProperty, DefaultValue(""), IDReferenceProperty(typeof(WebControl))]
        public string PopupControlID
        {
            get => 
                base.GetPropertyValue<string>("PopupControlID", "");
            set
            {
                base.SetPropertyValue<string>("PopupControlID", value);
            }
        }

        [IDReferenceProperty(typeof(WebControl)), ExtenderControlProperty, DefaultValue("")]
        public string PopupDragHandleControlID
        {
            get => 
                base.GetPropertyValue<string>("PopupDragHandleControlID", "");
            set
            {
                base.SetPropertyValue<string>("PopupDragHandleControlID", value);
            }
        }

        [ClientPropertyName("repositionMode"), ExtenderControlProperty, DefaultValue(3)]
        public ModalPopupRepositionMode RepositionMode
        {
            get => 
                base.GetPropertyValue<ModalPopupRepositionMode>("RepositionMode", ModalPopupRepositionMode.RepositionOnWindowResizeAndScroll);
            set
            {
                base.SetPropertyValue<ModalPopupRepositionMode>("RepositionMode", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(-1)]
        public int X
        {
            get => 
                base.GetPropertyValue<int>("X", -1);
            set
            {
                base.SetPropertyValue<int>("X", value);
            }
        }

        [DefaultValue(-1), ExtenderControlProperty]
        public int Y
        {
            get => 
                base.GetPropertyValue<int>("Y", -1);
            set
            {
                base.SetPropertyValue<int>("Y", value);
            }
        }
    }
}

