namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [Serializable, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class UserPersonalizationStateInfo : PersonalizationStateInfo
    {
        private DateTime _lastActivityDate;
        private string _username;

        public UserPersonalizationStateInfo(string path, DateTime lastUpdatedDate, int size, string username, DateTime lastActivityDate) : base(path, lastUpdatedDate, size)
        {
            this._username = StringUtil.CheckAndTrimString(username, "username");
            this._lastActivityDate = lastActivityDate.ToUniversalTime();
        }

        public DateTime LastActivityDate =>
            this._lastActivityDate.ToLocalTime();

        public string Username =>
            this._username;
    }
}

