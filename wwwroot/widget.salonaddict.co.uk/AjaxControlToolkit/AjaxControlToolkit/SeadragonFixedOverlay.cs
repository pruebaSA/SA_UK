namespace AjaxControlToolkit
{
    using System.ComponentModel;

    [ToolboxItem(false)]
    public class SeadragonFixedOverlay : SeadragonOverlay
    {
        private SeadragonPoint point;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
        public SeadragonPoint Point
        {
            get
            {
                if (this.point == null)
                {
                    this.point = new SeadragonPoint();
                }
                return this.point;
            }
        }
    }
}

