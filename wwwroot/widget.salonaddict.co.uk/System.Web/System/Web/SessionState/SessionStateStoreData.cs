namespace System.Web.SessionState
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SessionStateStoreData
    {
        private ISessionStateItemCollection _sessionItems;
        private HttpStaticObjectsCollection _staticObjects;
        private int _timeout;

        public SessionStateStoreData(ISessionStateItemCollection sessionItems, HttpStaticObjectsCollection staticObjects, int timeout)
        {
            this._sessionItems = sessionItems;
            this._staticObjects = staticObjects;
            this._timeout = timeout;
        }

        public virtual ISessionStateItemCollection Items =>
            this._sessionItems;

        public virtual HttpStaticObjectsCollection StaticObjects =>
            this._staticObjects;

        public virtual int Timeout
        {
            get => 
                this._timeout;
            set
            {
                this._timeout = value;
            }
        }
    }
}

