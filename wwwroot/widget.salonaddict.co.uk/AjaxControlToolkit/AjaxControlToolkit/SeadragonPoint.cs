namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Web;

    [TypeConverter(typeof(ExpandableObjectConverter)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SeadragonPoint
    {
        public SeadragonPoint()
        {
        }

        public SeadragonPoint(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float X { get; set; }

        public float Y { get; set; }
    }
}

