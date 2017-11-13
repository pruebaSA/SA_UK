namespace System.Deployment.Application
{
    using System;
    using System.Deployment.Application.Manifest;

    internal class SubscriptionStateInternal
    {
        public AppType appType;
        public DefinitionIdentity CurrentApplication;
        public AssemblyManifest CurrentApplicationManifest;
        public Uri CurrentApplicationSourceUri;
        public DefinitionAppId CurrentBind;
        public DefinitionIdentity CurrentDeployment;
        public AssemblyManifest CurrentDeploymentManifest;
        public Uri CurrentDeploymentSourceUri;
        public Uri DeploymentProviderUri;
        public DefinitionIdentity ExcludedDeployment;
        public bool IsInstalled;
        public bool IsShellVisible;
        public DateTime LastCheckTime;
        public Version MinimumRequiredVersion;
        public DefinitionAppId PendingBind;
        public DefinitionIdentity PendingDeployment;
        public DefinitionIdentity PreviousApplication;
        public AssemblyManifest PreviousApplicationManifest;
        public DefinitionAppId PreviousBind;
        public DefinitionIdentity RollbackDeployment;
        public DefinitionIdentity UpdateSkippedDeployment;
        public DateTime UpdateSkipTime;

        public SubscriptionStateInternal()
        {
            this.Reset();
        }

        public SubscriptionStateInternal(SubscriptionState subState)
        {
            this.IsInstalled = subState.IsInstalled;
            this.IsShellVisible = subState.IsShellVisible;
            this.CurrentBind = subState.CurrentBind;
            this.PreviousBind = subState.PreviousBind;
            this.PendingBind = subState.PreviousBind;
            this.PendingDeployment = subState.PendingDeployment;
            this.ExcludedDeployment = subState.ExcludedDeployment;
            this.DeploymentProviderUri = subState.DeploymentProviderUri;
            this.MinimumRequiredVersion = subState.MinimumRequiredVersion;
            this.LastCheckTime = subState.LastCheckTime;
            this.UpdateSkippedDeployment = subState.UpdateSkippedDeployment;
            this.UpdateSkipTime = subState.UpdateSkipTime;
            this.appType = subState.appType;
        }

        public void Reset()
        {
            this.IsInstalled = this.IsShellVisible = false;
            this.CurrentBind = this.PreviousBind = (DefinitionAppId) (this.PendingBind = null);
            this.ExcludedDeployment = (DefinitionIdentity) (this.PendingDeployment = null);
            this.DeploymentProviderUri = null;
            this.MinimumRequiredVersion = null;
            this.LastCheckTime = DateTime.MinValue;
            this.UpdateSkippedDeployment = null;
            this.UpdateSkipTime = DateTime.MinValue;
            this.CurrentDeployment = null;
            this.RollbackDeployment = null;
            this.CurrentDeploymentManifest = null;
            this.CurrentDeploymentSourceUri = null;
            this.CurrentApplication = null;
            this.CurrentApplicationManifest = null;
            this.CurrentApplicationSourceUri = null;
            this.PreviousApplication = null;
            this.PreviousApplicationManifest = null;
            this.appType = AppType.None;
        }
    }
}

