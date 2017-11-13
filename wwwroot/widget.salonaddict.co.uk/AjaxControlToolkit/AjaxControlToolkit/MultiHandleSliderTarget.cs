namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [PersistChildren(false), ParseChildren(true)]
    public class MultiHandleSliderTarget
    {
        private string _controlID;
        private int _decimals;
        private string _handleCssClass;
        private int _offset;

        [ExtenderControlProperty, IDReferenceProperty(typeof(WebControl)), NotifyParentProperty(true), Description("Sets the ID of the control that is bound to the location of this handle.")]
        public string ControlID
        {
            get => 
                this._controlID;
            set
            {
                this._controlID = value;
            }
        }

        [DefaultValue(0), ExtenderControlProperty, Description("Sets the number of decimal places to store with the value."), NotifyParentProperty(true)]
        public int Decimals
        {
            get => 
                this._decimals;
            set
            {
                this._decimals = value;
            }
        }

        [ExtenderControlProperty, DefaultValue(""), NotifyParentProperty(true), Description("Sets the style of the handle associated with the MultiHandleSliderTarget, if custom styles are used.")]
        public string HandleCssClass
        {
            get => 
                this._handleCssClass;
            set
            {
                this._handleCssClass = value;
            }
        }

        [ExtenderControlProperty, NotifyParentProperty(true), DefaultValue(0), Description("Sets the number of pixels to offset the width of the handle, for handles with transparent space.")]
        public int Offset
        {
            get => 
                this._offset;
            set
            {
                this._offset = value;
            }
        }
    }
}

