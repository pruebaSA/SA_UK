namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxItem(false), ToolboxData("<{0}:SeadragonControl runat=\"server\"></{0}:SeadragonControl>")]
    public class SeadragonControl : Panel
    {
        private ControlAnchor _anchor;

        public SeadragonControl()
        {
        }

        public SeadragonControl(Control ctl, ControlAnchor anchor)
        {
            this._anchor = anchor;
            this.Controls.Add(ctl);
        }

        public ControlAnchor Anchor
        {
            get => 
                this._anchor;
            set
            {
                this._anchor = value;
            }
        }
    }
}

