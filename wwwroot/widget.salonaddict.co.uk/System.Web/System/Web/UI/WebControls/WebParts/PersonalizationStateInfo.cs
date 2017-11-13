namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [Serializable, AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class PersonalizationStateInfo
    {
        private DateTime _lastUpdatedDate;
        private string _path;
        private int _size;

        internal PersonalizationStateInfo(string path, DateTime lastUpdatedDate, int size)
        {
            this._path = StringUtil.CheckAndTrimString(path, "path");
            PersonalizationProviderHelper.CheckNegativeInteger(size, "size");
            this._lastUpdatedDate = lastUpdatedDate.ToUniversalTime();
            this._size = size;
        }

        public DateTime LastUpdatedDate =>
            this._lastUpdatedDate.ToLocalTime();

        public string Path =>
            this._path;

        public int Size =>
            this._size;
    }
}

