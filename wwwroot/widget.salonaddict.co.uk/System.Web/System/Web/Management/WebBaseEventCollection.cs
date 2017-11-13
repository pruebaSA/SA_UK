namespace System.Web.Management
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WebBaseEventCollection : ReadOnlyCollectionBase
    {
        public WebBaseEventCollection(ICollection events)
        {
            if (events == null)
            {
                throw new ArgumentNullException("events");
            }
            foreach (WebBaseEvent event2 in events)
            {
                base.InnerList.Add(event2);
            }
        }

        internal WebBaseEventCollection(WebBaseEvent eventRaised)
        {
            if (eventRaised == null)
            {
                throw new ArgumentNullException("eventRaised");
            }
            base.InnerList.Add(eventRaised);
        }

        public bool Contains(WebBaseEvent value) => 
            base.InnerList.Contains(value);

        public int IndexOf(WebBaseEvent value) => 
            base.InnerList.IndexOf(value);

        public WebBaseEvent this[int index] =>
            ((WebBaseEvent) base.InnerList[index]);
    }
}

