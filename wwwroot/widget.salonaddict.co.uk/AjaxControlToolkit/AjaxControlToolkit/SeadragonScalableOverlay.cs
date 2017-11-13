namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    [ToolboxItem(false)]
    public class SeadragonScalableOverlay : SeadragonOverlay
    {
        private SeadragonRect rect;

        [Browsable(false), DefaultValue(1)]
        public sealed override SeadragonOverlayPlacement Placement
        {
            get => 
                SeadragonOverlayPlacement.TOP_LEFT;
            [CompilerGenerated]
            set
            {
                base.Placement = value;
            }
        }

        [NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SeadragonRect Rect
        {
            get
            {
                if (this.rect == null)
                {
                    this.rect = new SeadragonRect();
                }
                return this.rect;
            }
        }
    }
}

