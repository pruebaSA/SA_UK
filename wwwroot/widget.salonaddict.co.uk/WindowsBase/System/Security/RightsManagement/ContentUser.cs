namespace System.Security.RightsManagement
{
    using MS.Internal;
    using MS.Internal.Security.RightsManagement;
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Text;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    public class ContentUser
    {
        private static ContentUser _anyoneUser;
        private System.Security.RightsManagement.AuthenticationType _authenticationType;
        [SecurityTreatAsSafe]
        internal static readonly ContentUserComparer _contentUserComparer = new ContentUserComparer();
        private string _name;
        private static ContentUser _ownerUser;
        private const string AnyoneUserName = "Anyone";
        private bool hashCalcIsDone;
        private int hashValue;
        private const string OwnerUserName = "Owner";
        private const string PassportAuthProvider = "PassportAuthProvider";
        private const string WindowsAuthProvider = "WindowsAuthProvider";

        public ContentUser(string name, System.Security.RightsManagement.AuthenticationType authenticationType)
        {
            SecurityHelper.DemandRightsManagementPermission();
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Trim().Length == 0)
            {
                throw new ArgumentOutOfRangeException("name");
            }
            if (((authenticationType != System.Security.RightsManagement.AuthenticationType.Windows) && (authenticationType != System.Security.RightsManagement.AuthenticationType.Passport)) && ((authenticationType != System.Security.RightsManagement.AuthenticationType.WindowsPassport) && (authenticationType != System.Security.RightsManagement.AuthenticationType.Internal)))
            {
                throw new ArgumentOutOfRangeException("authenticationType");
            }
            if (((authenticationType == System.Security.RightsManagement.AuthenticationType.Internal) && !CompareToAnyone(name)) && !CompareToOwner(name))
            {
                throw new ArgumentOutOfRangeException("name");
            }
            this._name = name;
            this._authenticationType = authenticationType;
        }

        internal static bool CompareToAnyone(string name) => 
            (0 == string.CompareOrdinal("Anyone".ToUpperInvariant(), name.ToUpperInvariant()));

        internal static bool CompareToOwner(string name) => 
            (0 == string.CompareOrdinal("Owner".ToUpperInvariant(), name.ToUpperInvariant()));

        public override bool Equals(object obj)
        {
            SecurityHelper.DemandRightsManagementPermission();
            if (obj == null)
            {
                return false;
            }
            if (base.GetType() != obj.GetType())
            {
                return false;
            }
            ContentUser user = (ContentUser) obj;
            return ((string.CompareOrdinal(this._name.ToUpperInvariant(), user._name.ToUpperInvariant()) == 0) && this._authenticationType.Equals(user._authenticationType));
        }

        [SecurityTreatAsSafe]
        internal bool GenericEquals(ContentUser userObj)
        {
            if (userObj == null)
            {
                return false;
            }
            return ((string.CompareOrdinal(this._name.ToUpperInvariant(), userObj._name.ToUpperInvariant()) == 0) && this._authenticationType.Equals(userObj._authenticationType));
        }

        public override int GetHashCode()
        {
            SecurityHelper.DemandRightsManagementPermission();
            if (!this.hashCalcIsDone)
            {
                StringBuilder builder = new StringBuilder(this._name.ToUpperInvariant());
                builder.Append(this._authenticationType.ToString());
                this.hashValue = builder.ToString().GetHashCode();
                this.hashCalcIsDone = true;
            }
            return this.hashValue;
        }

        public bool IsAuthenticated()
        {
            SecurityHelper.DemandRightsManagementPermission();
            if ((this._authenticationType != System.Security.RightsManagement.AuthenticationType.Windows) && (this._authenticationType != System.Security.RightsManagement.AuthenticationType.Passport))
            {
                return false;
            }
            using (ClientSession session = new ClientSession(this))
            {
                return (session.IsMachineActivated() && session.IsUserActivated());
            }
        }

        public static ContentUser AnyoneUser
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                if (_anyoneUser == null)
                {
                    _anyoneUser = new ContentUser("Anyone", System.Security.RightsManagement.AuthenticationType.Internal);
                }
                return _anyoneUser;
            }
        }

        internal string AuthenticationProviderType
        {
            get
            {
                if (this._authenticationType == System.Security.RightsManagement.AuthenticationType.Windows)
                {
                    return "WindowsAuthProvider";
                }
                if (this._authenticationType == System.Security.RightsManagement.AuthenticationType.Passport)
                {
                    return "PassportAuthProvider";
                }
                Invariant.Assert(false, "AuthenticationProviderType can only be queried for Windows or Passport authentication");
                return null;
            }
        }

        public System.Security.RightsManagement.AuthenticationType AuthenticationType
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._authenticationType;
            }
        }

        public string Name
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._name;
            }
        }

        public static ContentUser OwnerUser
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                if (_ownerUser == null)
                {
                    _ownerUser = new ContentUser("Owner", System.Security.RightsManagement.AuthenticationType.Internal);
                }
                return _ownerUser;
            }
        }

        [SecurityTreatAsSafe]
        internal sealed class ContentUserComparer : IEqualityComparer<ContentUser>
        {
            bool IEqualityComparer<ContentUser>.Equals(ContentUser user1, ContentUser user2)
            {
                Invariant.Assert(user1 != null, "user1 should not be null");
                return user1.GenericEquals(user2);
            }

            int IEqualityComparer<ContentUser>.GetHashCode(ContentUser user)
            {
                Invariant.Assert(user != null, "user should not be null");
                return user.GetHashCode();
            }
        }
    }
}

