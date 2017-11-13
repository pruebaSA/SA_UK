namespace System.Security.RightsManagement
{
    using MS.Internal;
    using MS.Internal.Security.RightsManagement;
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Security;
    using System.Windows;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    public class SecureEnvironment : IDisposable
    {
        private string _applicationManifest;
        private MS.Internal.Security.RightsManagement.ClientSession _clientSession;
        private ContentUser _user;

        private SecureEnvironment(string applicationManifest, ContentUser user, MS.Internal.Security.RightsManagement.ClientSession clientSession)
        {
            Invariant.Assert(applicationManifest != null);
            Invariant.Assert(user != null);
            Invariant.Assert(clientSession != null);
            this._user = user;
            this._applicationManifest = applicationManifest;
            this._clientSession = clientSession;
        }

        private void CheckDisposed()
        {
            if (this._clientSession == null)
            {
                throw new ObjectDisposedException("SecureEnvironment");
            }
        }

        public static SecureEnvironment Create(string applicationManifest, ContentUser user)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return CriticalCreate(applicationManifest, user);
        }

        public static SecureEnvironment Create(string applicationManifest, AuthenticationType authentication, UserActivationMode userActivationMode)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return CriticalCreate(applicationManifest, authentication, userActivationMode);
        }

        private static SecureEnvironment CriticalCreate(string applicationManifest, ContentUser user)
        {
            SecureEnvironment environment;
            if (applicationManifest == null)
            {
                throw new ArgumentNullException("applicationManifest");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if ((user.AuthenticationType != AuthenticationType.Windows) && (user.AuthenticationType != AuthenticationType.Passport))
            {
                throw new ArgumentOutOfRangeException("user");
            }
            if (!IsUserActivated(user))
            {
                throw new RightsManagementException(RightsManagementFailureCode.NeedsGroupIdentityActivation);
            }
            MS.Internal.Security.RightsManagement.ClientSession clientSession = new MS.Internal.Security.RightsManagement.ClientSession(user);
            try
            {
                clientSession.BuildSecureEnvironment(applicationManifest);
                environment = new SecureEnvironment(applicationManifest, user, clientSession);
            }
            catch
            {
                clientSession.Dispose();
                throw;
            }
            return environment;
        }

        private static SecureEnvironment CriticalCreate(string applicationManifest, AuthenticationType authentication, UserActivationMode userActivationMode)
        {
            ContentUser user;
            SecureEnvironment environment;
            if (applicationManifest == null)
            {
                throw new ArgumentNullException("applicationManifest");
            }
            if ((authentication != AuthenticationType.Windows) && (authentication != AuthenticationType.Passport))
            {
                throw new ArgumentOutOfRangeException("authentication");
            }
            if ((userActivationMode != UserActivationMode.Permanent) && (userActivationMode != UserActivationMode.Temporary))
            {
                throw new ArgumentOutOfRangeException("userActivationMode");
            }
            using (MS.Internal.Security.RightsManagement.ClientSession session = MS.Internal.Security.RightsManagement.ClientSession.DefaultUserClientSession(authentication))
            {
                if (!session.IsMachineActivated())
                {
                    session.ActivateMachine(authentication);
                }
                user = session.ActivateUser(authentication, userActivationMode);
            }
            MS.Internal.Security.RightsManagement.ClientSession clientSession = new MS.Internal.Security.RightsManagement.ClientSession(user, userActivationMode);
            try
            {
                try
                {
                    clientSession.AcquireClientLicensorCertificate();
                }
                catch (RightsManagementException)
                {
                }
                clientSession.BuildSecureEnvironment(applicationManifest);
                environment = new SecureEnvironment(applicationManifest, user, clientSession);
            }
            catch
            {
                clientSession.Dispose();
                throw;
            }
            return environment;
        }

        public void Dispose()
        {
            SecurityHelper.DemandRightsManagementPermission();
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this._clientSession != null))
                {
                    this._clientSession.Dispose();
                }
            }
            finally
            {
                this._clientSession = null;
            }
        }

        public static ReadOnlyCollection<ContentUser> GetActivatedUsers()
        {
            SecurityHelper.DemandRightsManagementPermission();
            using (MS.Internal.Security.RightsManagement.ClientSession session = MS.Internal.Security.RightsManagement.ClientSession.DefaultUserClientSession(AuthenticationType.Windows))
            {
                List<ContentUser> list = new List<ContentUser>();
                if (session.IsMachineActivated())
                {
                    int index = 0;
                    while (true)
                    {
                        string certificateChain = session.EnumerateLicense(EnumerateLicenseFlags.GroupIdentity, index);
                        if (certificateChain == null)
                        {
                            break;
                        }
                        ContentUser user = MS.Internal.Security.RightsManagement.ClientSession.ExtractUserFromCertificateChain(certificateChain);
                        using (MS.Internal.Security.RightsManagement.ClientSession session2 = new MS.Internal.Security.RightsManagement.ClientSession(user))
                        {
                            if (session2.IsUserActivated())
                            {
                                list.Add(user);
                            }
                        }
                        index++;
                    }
                }
                return new ReadOnlyCollection<ContentUser>(list);
            }
        }

        public static bool IsUserActivated(ContentUser user)
        {
            SecurityHelper.DemandRightsManagementPermission();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if ((user.AuthenticationType != AuthenticationType.Windows) && (user.AuthenticationType != AuthenticationType.Passport))
            {
                throw new ArgumentOutOfRangeException("user", System.Windows.SR.Get("OnlyPassportOrWindowsAuthenticatedUsersAreAllowed"));
            }
            using (MS.Internal.Security.RightsManagement.ClientSession session = new MS.Internal.Security.RightsManagement.ClientSession(user))
            {
                return (session.IsMachineActivated() && session.IsUserActivated());
            }
        }

        public static void RemoveActivatedUser(ContentUser user)
        {
            SecurityHelper.DemandRightsManagementPermission();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if ((user.AuthenticationType != AuthenticationType.Windows) && (user.AuthenticationType != AuthenticationType.Passport))
            {
                throw new ArgumentOutOfRangeException("user", System.Windows.SR.Get("OnlyPassportOrWindowsAuthenticatedUsersAreAllowed"));
            }
            using (MS.Internal.Security.RightsManagement.ClientSession session = new MS.Internal.Security.RightsManagement.ClientSession(user))
            {
                foreach (string str in session.EnumerateUsersCertificateIds(user, EnumerateLicenseFlags.ClientLicensor))
                {
                    session.DeleteLicense(str);
                }
                foreach (string str2 in session.EnumerateUsersCertificateIds(user, EnumerateLicenseFlags.GroupIdentity))
                {
                    session.DeleteLicense(str2);
                }
            }
        }

        public string ApplicationManifest
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                this.CheckDisposed();
                return this._applicationManifest;
            }
        }

        internal MS.Internal.Security.RightsManagement.ClientSession ClientSession
        {
            get
            {
                Invariant.Assert(this._clientSession != null);
                return this._clientSession;
            }
        }

        public ContentUser User
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                this.CheckDisposed();
                return this._user;
            }
        }
    }
}

