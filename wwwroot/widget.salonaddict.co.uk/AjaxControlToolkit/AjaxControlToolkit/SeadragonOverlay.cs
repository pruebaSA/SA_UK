namespace AjaxControlToolkit
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxData("<{0}:SeadragonOverlay runat=server></{0}:SeadragonOverlay>")]
    public abstract class SeadragonOverlay : Panel
    {
        protected SeadragonOverlay()
        {
        }

        public virtual SeadragonOverlayPlacement Placement { get; set; }

        protected override HtmlTextWriterTag TagKey =>
            HtmlTextWriterTag.Div;
    }
}

