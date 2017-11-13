namespace System.Web.Security
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [Serializable, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class FormsAuthenticationTicket
    {
        private string _CookiePath;
        private DateTime _Expiration;
        [NonSerialized]
        private DateTime _ExpirationUtc;
        [NonSerialized]
        private bool _ExpirationUtcHasValue;
        private bool _IsPersistent;
        private DateTime _IssueDate;
        [NonSerialized]
        private DateTime _IssueDateUtc;
        [NonSerialized]
        private bool _IssueDateUtcHasValue;
        private string _Name;
        private string _UserData;
        private int _Version;

        public FormsAuthenticationTicket(string name, bool isPersistent, int timeout)
        {
            this._Version = 2;
            this._Name = name;
            this._IssueDateUtcHasValue = true;
            this._IssueDateUtc = DateTime.UtcNow;
            this._IssueDate = DateTime.Now;
            this._IsPersistent = isPersistent;
            this._UserData = "";
            this._ExpirationUtcHasValue = true;
            this._ExpirationUtc = this._IssueDateUtc.AddMinutes((double) timeout);
            this._Expiration = this._IssueDate.AddMinutes((double) timeout);
            this._CookiePath = FormsAuthentication.FormsCookiePath;
        }

        public FormsAuthenticationTicket(int version, string name, DateTime issueDate, DateTime expiration, bool isPersistent, string userData)
        {
            this._Version = version;
            this._Name = name;
            this._Expiration = expiration;
            this._IssueDate = issueDate;
            this._IsPersistent = isPersistent;
            this._UserData = userData;
            this._CookiePath = FormsAuthentication.FormsCookiePath;
        }

        public FormsAuthenticationTicket(int version, string name, DateTime issueDate, DateTime expiration, bool isPersistent, string userData, string cookiePath)
        {
            this._Version = version;
            this._Name = name;
            this._Expiration = expiration;
            this._IssueDate = issueDate;
            this._IsPersistent = isPersistent;
            this._UserData = userData;
            this._CookiePath = cookiePath;
        }

        internal static FormsAuthenticationTicket FromUtc(int version, string name, DateTime issueDateUtc, DateTime expirationUtc, bool isPersistent, string userData, string cookiePath) => 
            new FormsAuthenticationTicket(version, name, issueDateUtc.ToLocalTime(), expirationUtc.ToLocalTime(), isPersistent, userData, cookiePath) { 
                _IssueDateUtcHasValue = true,
                _IssueDateUtc = issueDateUtc,
                _ExpirationUtcHasValue = true,
                _ExpirationUtc = expirationUtc
            };

        public string CookiePath =>
            this._CookiePath;

        public DateTime Expiration =>
            this._Expiration;

        internal DateTime ExpirationUtc
        {
            get
            {
                if (!this._ExpirationUtcHasValue)
                {
                    return this.Expiration.ToUniversalTime();
                }
                return this._ExpirationUtc;
            }
        }

        public bool Expired =>
            (this.ExpirationUtc < DateTime.UtcNow);

        public bool IsPersistent =>
            this._IsPersistent;

        public DateTime IssueDate =>
            this._IssueDate;

        internal DateTime IssueDateUtc
        {
            get
            {
                if (!this._IssueDateUtcHasValue)
                {
                    return this.IssueDate.ToUniversalTime();
                }
                return this._IssueDateUtc;
            }
        }

        public string Name =>
            this._Name;

        public string UserData =>
            this._UserData;

        public int Version =>
            this._Version;
    }
}

