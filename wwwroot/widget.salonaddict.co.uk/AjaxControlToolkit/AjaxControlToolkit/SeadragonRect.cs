namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Web;

    [TypeConverter(typeof(ExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SeadragonRect
    {
        private SeadragonPoint point;

        public SeadragonRect()
        {
        }

        public SeadragonRect(float width, float height)
        {
            this.Height = height;
            this.Width = width;
        }

        public float Height { get; set; }

        [NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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

        public float Width { get; set; }
    }
}

