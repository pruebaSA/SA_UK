namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WebPartZoneCollection : ReadOnlyCollectionBase
    {
        public WebPartZoneCollection()
        {
        }

        public WebPartZoneCollection(ICollection webPartZones)
        {
            if (webPartZones == null)
            {
                throw new ArgumentNullException("webPartZones");
            }
            foreach (object obj2 in webPartZones)
            {
                if (obj2 == null)
                {
                    throw new ArgumentException(System.Web.SR.GetString("Collection_CantAddNull"), "webPartZones");
                }
                if (!(obj2 is WebPartZone))
                {
                    throw new ArgumentException(System.Web.SR.GetString("Collection_InvalidType", new object[] { "WebPartZone" }), "webPartZones");
                }
                base.InnerList.Add(obj2);
            }
        }

        internal int Add(WebPartZoneBase value) => 
            base.InnerList.Add(value);

        public bool Contains(WebPartZoneBase value) => 
            base.InnerList.Contains(value);

        public void CopyTo(WebPartZoneBase[] array, int index)
        {
            base.InnerList.CopyTo(array, index);
        }

        public int IndexOf(WebPartZoneBase value) => 
            base.InnerList.IndexOf(value);

        public WebPartZoneBase this[int index] =>
            ((WebPartZoneBase) base.InnerList[index]);

        public WebPartZoneBase this[string id]
        {
            get
            {
                foreach (WebPartZoneBase base3 in base.InnerList)
                {
                    if (string.Equals(base3.ID, id, StringComparison.OrdinalIgnoreCase))
                    {
                        return base3;
                    }
                }
                return null;
            }
        }
    }
}

