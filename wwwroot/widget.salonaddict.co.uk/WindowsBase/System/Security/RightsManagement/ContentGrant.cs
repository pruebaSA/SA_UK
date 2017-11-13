namespace System.Security.RightsManagement
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Security;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    public class ContentGrant
    {
        private ContentRight _right;
        private ContentUser _user;
        private DateTime _validFrom;
        private DateTime _validUntil;

        public ContentGrant(ContentUser user, ContentRight right) : this(user, right, DateTime.MinValue, DateTime.MaxValue)
        {
        }

        public ContentGrant(ContentUser user, ContentRight right, DateTime validFrom, DateTime validUntil)
        {
            SecurityHelper.DemandRightsManagementPermission();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (((((right != ContentRight.View) && (right != ContentRight.Edit)) && ((right != ContentRight.Print) && (right != ContentRight.Extract))) && (((right != ContentRight.ObjectModel) && (right != ContentRight.Owner)) && ((right != ContentRight.ViewRightsData) && (right != ContentRight.Forward)))) && ((((right != ContentRight.Reply) && (right != ContentRight.ReplyAll)) && ((right != ContentRight.Sign) && (right != ContentRight.DocumentEdit))) && (right != ContentRight.Export)))
            {
                throw new ArgumentOutOfRangeException("right");
            }
            if (validFrom > validUntil)
            {
                throw new ArgumentOutOfRangeException("validFrom");
            }
            this._user = user;
            this._right = right;
            this._validFrom = validFrom;
            this._validUntil = validUntil;
        }

        public ContentRight Right
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._right;
            }
        }

        public ContentUser User
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._user;
            }
        }

        public DateTime ValidFrom
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._validFrom;
            }
        }

        public DateTime ValidUntil
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._validUntil;
            }
        }
    }
}

