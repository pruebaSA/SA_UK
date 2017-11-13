namespace System.Deployment.Application
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security.Permissions;

    [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
    public class InPlaceHostingManager : IDisposable
    {
        private ApplicationIdentity _applicationId;
        private AppType _appType;
        private DeploymentManager _deploymentManager;
        private bool _isCached;
        private bool _isLaunchInHostProcess;
        private object _lock;
        private State _state;

        public event EventHandler<DownloadApplicationCompletedEventArgs> DownloadApplicationCompleted;

        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgressChanged;

        public event EventHandler<GetManifestCompletedEventArgs> GetManifestCompleted;

        public InPlaceHostingManager(Uri deploymentManifest) : this(deploymentManifest, true)
        {
        }

        public InPlaceHostingManager(Uri deploymentManifest, bool launchInHostProcess)
        {
            if (!PlatformSpecific.OnXPOrAbove)
            {
                throw new PlatformNotSupportedException(Resources.GetString("Ex_RequiresXPOrHigher"));
            }
            if (deploymentManifest == null)
            {
                throw new ArgumentNullException("deploymentManifest");
            }
            UriHelper.ValidateSupportedSchemeInArgument(deploymentManifest, "deploymentSource");
            this._deploymentManager = new DeploymentManager(deploymentManifest, false, true, null, null);
            this._isLaunchInHostProcess = launchInHostProcess;
            this._Initialize();
        }

        private void _Initialize()
        {
            this._lock = new object();
            this._deploymentManager.BindCompleted += new BindCompletedEventHandler(this.OnBindCompleted);
            this._deploymentManager.SynchronizeCompleted += new SynchronizeCompletedEventHandler(this.OnSynchronizeCompleted);
            this._deploymentManager.ProgressChanged += new DeploymentProgressChangedEventHandler(this.OnProgressChanged);
            this._state = State.Ready;
        }

        public void AssertApplicationRequirements()
        {
            lock (this._lock)
            {
                if (this._appType == AppType.CustomHostSpecified)
                {
                    throw new InvalidOperationException(Resources.GetString("Ex_CannotCallAssertApplicationRequirements"));
                }
                this.AssertApplicationRequirements(false);
            }
        }

        public void AssertApplicationRequirements(bool grantApplicationTrust)
        {
            lock (this._lock)
            {
                if (this._appType == AppType.CustomHostSpecified)
                {
                    throw new InvalidOperationException(Resources.GetString("Ex_CannotCallAssertApplicationRequirements"));
                }
                this.AssertState(State.GetManifestSucceeded, State.DownloadingApplication);
                try
                {
                    this.ChangeState(State.VerifyingRequirements);
                    if (grantApplicationTrust)
                    {
                        this._deploymentManager.PersistTrustWithoutEvaluation();
                    }
                    else
                    {
                        TrustParams trustParams = new TrustParams {
                            NoPrompt = true
                        };
                        this._deploymentManager.DetermineTrust(trustParams);
                    }
                    this._deploymentManager.DeterminePlatformRequirements();
                    this.ChangeState(State.VerifyRequirementsSucceeded);
                }
                catch
                {
                    this.ChangeState(State.Done);
                    throw;
                }
            }
        }

        private void AssertState(State validState)
        {
            if (this._state == State.Done)
            {
                throw new InvalidOperationException(Resources.GetString("Ex_NoFurtherOperations"));
            }
            if (validState != this._state)
            {
                throw new InvalidOperationException(Resources.GetString("Ex_InvalidSequence"));
            }
        }

        private void AssertState(State validState0, State validState1)
        {
            if (((this._state == State.Done) && (validState0 != this._state)) && (validState1 != this._state))
            {
                throw new InvalidOperationException(Resources.GetString("Ex_NoFurtherOperations"));
            }
            if ((validState0 != this._state) && (validState1 != this._state))
            {
                throw new InvalidOperationException(Resources.GetString("Ex_InvalidSequence"));
            }
        }

        private void AssertState(State validState0, State validState1, State validState2)
        {
            if (((this._state == State.Done) && (validState0 != this._state)) && ((validState1 != this._state) && (validState2 != this._state)))
            {
                throw new InvalidOperationException(Resources.GetString("Ex_NoFurtherOperations"));
            }
            if (((validState0 != this._state) && (validState1 != this._state)) && (validState2 != this._state))
            {
                throw new InvalidOperationException(Resources.GetString("Ex_InvalidSequence"));
            }
        }

        public void CancelAsync()
        {
            lock (this._lock)
            {
                this.ChangeState(State.Done);
                this._deploymentManager.CancelAsync();
            }
        }

        private void ChangeState(State nextState)
        {
            this._state = nextState;
        }

        private void ChangeState(State nextState, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled || (e.Error != null))
            {
                this._state = State.Done;
            }
            else
            {
                this._state = nextState;
            }
        }

        public void Dispose()
        {
            lock (this._lock)
            {
                this.ChangeState(State.Done);
                this._deploymentManager.BindCompleted -= new BindCompletedEventHandler(this.OnBindCompleted);
                this._deploymentManager.SynchronizeCompleted -= new SynchronizeCompletedEventHandler(this.OnSynchronizeCompleted);
                this._deploymentManager.ProgressChanged -= new DeploymentProgressChangedEventHandler(this.OnProgressChanged);
                this._deploymentManager.Dispose();
            }
        }

        public void DownloadApplicationAsync()
        {
            lock (this._lock)
            {
                if (this._appType == AppType.CustomHostSpecified)
                {
                    this.AssertState(State.GetManifestSucceeded);
                }
                else if (this._isCached)
                {
                    this.AssertState(State.GetManifestSucceeded, State.VerifyRequirementsSucceeded);
                }
                else
                {
                    this.AssertState(State.GetManifestSucceeded, State.VerifyRequirementsSucceeded);
                }
                try
                {
                    this.ChangeState(State.DownloadingApplication);
                    this._deploymentManager.SynchronizeAsync();
                }
                catch
                {
                    this.ChangeState(State.Done);
                    throw;
                }
            }
        }

        public ObjectHandle Execute()
        {
            lock (this._lock)
            {
                this.AssertState(State.DownloadApplicationSucceeded);
                this.ChangeState(State.Done);
                return this._deploymentManager.ExecuteNewDomain();
            }
        }

        public void GetManifestAsync()
        {
            lock (this._lock)
            {
                this.AssertState(State.Ready);
                try
                {
                    this.ChangeState(State.GettingManifest);
                    this._deploymentManager.BindAsync();
                }
                catch
                {
                    this.ChangeState(State.Done);
                    throw;
                }
            }
        }

        private static DefinitionIdentity GetSubIdAndValidate(string subscriptionId)
        {
            if (subscriptionId == null)
            {
                throw new ArgumentNullException("subscriptionId", Resources.GetString("Ex_ComArgSubIdentityNull"));
            }
            DefinitionIdentity identity = null;
            try
            {
                identity = new DefinitionIdentity(subscriptionId);
            }
            catch (COMException exception)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.GetString("Ex_ComArgSubIdentityNotValid"), new object[] { subscriptionId }), exception);
            }
            catch (SEHException exception2)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.GetString("Ex_ComArgSubIdentityNotValid"), new object[] { subscriptionId }), exception2);
            }
            catch (ArgumentException exception3)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.GetString("Ex_ComArgSubIdentityNotValid"), new object[] { subscriptionId }), exception3);
            }
            if (identity.Name == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.GetString("Ex_ComArgSubIdentityNotValid"), new object[] { subscriptionId }));
            }
            if (identity.PublicKeyToken == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.GetString("Ex_ComArgSubIdentityNotValid"), new object[] { subscriptionId }));
            }
            if (identity.ProcessorArchitecture == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.GetString("Ex_ComArgSubIdentityNotValid"), new object[] { subscriptionId }));
            }
            if (identity.Version != null)
            {
                throw new ArgumentException(Resources.GetString("Ex_ComArgSubIdentityWithVersion"));
            }
            return identity;
        }

        private void OnBindCompleted(object sender, BindCompletedEventArgs e)
        {
            lock (this._lock)
            {
                this.AssertState(State.GettingManifest, State.Done);
                GetManifestCompletedEventArgs args = null;
                try
                {
                    if (this._state != State.Done)
                    {
                        if (e.Cancelled || (e.Error != null))
                        {
                            this.ChangeState(State.Done);
                        }
                        else
                        {
                            this.ChangeState(State.GetManifestSucceeded, e);
                        }
                    }
                    if (this.GetManifestCompleted == null)
                    {
                        goto Label_025E;
                    }
                    if ((e.Error != null) || e.Cancelled)
                    {
                        args = new GetManifestCompletedEventArgs(e, this._deploymentManager.LogFilePath);
                    }
                    else
                    {
                        this._isCached = e.IsCached;
                        this._applicationId = e.ActivationContext.Identity;
                        bool install = this._deploymentManager.ActivationDescription.DeployManifest.Deployment.Install;
                        bool hostInBrowser = this._deploymentManager.ActivationDescription.AppManifest.EntryPoints[0].HostInBrowser;
                        this._appType = this._deploymentManager.ActivationDescription.appType;
                        bool useManifestForTrust = this._deploymentManager.ActivationDescription.AppManifest.UseManifestForTrust;
                        Uri providerCodebaseUri = this._deploymentManager.ActivationDescription.DeployManifest.Deployment.ProviderCodebaseUri;
                        if ((this._isLaunchInHostProcess && (this._appType != AppType.CustomHostSpecified)) && !hostInBrowser)
                        {
                            args = new GetManifestCompletedEventArgs(e, new InvalidOperationException(Resources.GetString("Ex_HostInBrowserFlagMustBeTrue")), this._deploymentManager.LogFilePath);
                        }
                        else if (install && (this._isLaunchInHostProcess || (this._appType == AppType.CustomHostSpecified)))
                        {
                            args = new GetManifestCompletedEventArgs(e, new InvalidOperationException(Resources.GetString("Ex_InstallFlagMustBeFalse")), this._deploymentManager.LogFilePath);
                        }
                        else if (useManifestForTrust && (this._appType == AppType.CustomHostSpecified))
                        {
                            args = new GetManifestCompletedEventArgs(e, new InvalidOperationException(Resources.GetString("Ex_CannotHaveUseManifestForTrustFlag")), this._deploymentManager.LogFilePath);
                        }
                        else if ((providerCodebaseUri != null) && (this._appType == AppType.CustomHostSpecified))
                        {
                            args = new GetManifestCompletedEventArgs(e, new InvalidOperationException(Resources.GetString("Ex_CannotHaveDeploymentProvider")), this._deploymentManager.LogFilePath);
                        }
                        else if (hostInBrowser && (this._appType == AppType.CustomUX))
                        {
                            args = new GetManifestCompletedEventArgs(e, new InvalidOperationException(Resources.GetString("Ex_CannotHaveCustomUXFlag")), this._deploymentManager.LogFilePath);
                        }
                        else
                        {
                            args = new GetManifestCompletedEventArgs(e, this._deploymentManager.ActivationDescription, this._deploymentManager.LogFilePath);
                        }
                    }
                }
                catch
                {
                    this.ChangeState(State.Done);
                    throw;
                }
                this.GetManifestCompleted(this, args);
            Label_025E:;
            }
        }

        private void OnProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            lock (this._lock)
            {
                if (this.DownloadProgressChanged != null)
                {
                    DownloadProgressChangedEventArgs args = new DownloadProgressChangedEventArgs(e.ProgressPercentage, e.UserState, e.BytesCompleted, e.BytesTotal, e.State);
                    this.DownloadProgressChanged(this, args);
                }
            }
        }

        private void OnSynchronizeCompleted(object sender, SynchronizeCompletedEventArgs e)
        {
            lock (this._lock)
            {
                this.AssertState(State.DownloadingApplication, State.VerifyRequirementsSucceeded, State.Done);
                if (this._state != State.Done)
                {
                    if (e.Cancelled || (e.Error != null))
                    {
                        this.ChangeState(State.Done);
                    }
                    else
                    {
                        this.ChangeState(State.DownloadApplicationSucceeded, e);
                    }
                }
                if ((!this._isLaunchInHostProcess || (this._appType == AppType.CustomHostSpecified)) && (this._appType != AppType.CustomUX))
                {
                    this.ChangeState(State.Done);
                }
                if (this.DownloadApplicationCompleted != null)
                {
                    DownloadApplicationCompletedEventArgs args = new DownloadApplicationCompletedEventArgs(e, this._deploymentManager.LogFilePath, this._deploymentManager.ShortcutAppId);
                    this.DownloadApplicationCompleted(this, args);
                }
            }
        }

        public static void UninstallCustomAddIn(string subscriptionId)
        {
            DefinitionIdentity subId = null;
            subId = GetSubIdAndValidate(subscriptionId);
            SubscriptionStore currentUser = SubscriptionStore.CurrentUser;
            currentUser.RefreshStorePointer();
            SubscriptionState subscriptionState = currentUser.GetSubscriptionState(subId);
            subscriptionState.SubscriptionStore.UninstallCustomHostSpecifiedSubscription(subscriptionState);
        }

        public static void UninstallCustomUXApplication(string subscriptionId)
        {
            DefinitionIdentity subId = null;
            subId = GetSubIdAndValidate(subscriptionId);
            SubscriptionStore currentUser = SubscriptionStore.CurrentUser;
            currentUser.RefreshStorePointer();
            SubscriptionState subscriptionState = currentUser.GetSubscriptionState(subId);
            subscriptionState.SubscriptionStore.UninstallCustomUXSubscription(subscriptionState);
        }

        private enum State
        {
            Ready,
            GettingManifest,
            GetManifestSucceeded,
            VerifyingRequirements,
            VerifyRequirementsSucceeded,
            DownloadingApplication,
            DownloadApplicationSucceeded,
            Done
        }
    }
}

