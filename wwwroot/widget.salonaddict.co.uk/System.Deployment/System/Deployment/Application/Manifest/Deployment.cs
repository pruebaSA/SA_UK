﻿namespace System.Deployment.Application.Manifest
{
    using System;
    using System.Deployment.Internal.Isolation.Manifest;

    internal class Deployment
    {
        private readonly Uri _codebaseUri;
        private readonly bool _createDesktopShortcut;
        private readonly bool _disallowUrlActivation;
        private readonly bool _install;
        private readonly bool _mapFileExtensions;
        private readonly Version _minimumRequiredVersion;
        private readonly bool _trustURLParameters;
        private readonly System.Deployment.Application.Manifest.DeploymentUpdate _update;

        public Deployment(System.Deployment.Internal.Isolation.Manifest.DeploymentMetadataEntry deploymentMetadataEntry)
        {
            this._disallowUrlActivation = (deploymentMetadataEntry.DeploymentFlags & 0x80) != 0;
            this._install = (deploymentMetadataEntry.DeploymentFlags & 0x20) != 0;
            this._trustURLParameters = (deploymentMetadataEntry.DeploymentFlags & 0x40) != 0;
            this._mapFileExtensions = (deploymentMetadataEntry.DeploymentFlags & 0x100) != 0;
            this._createDesktopShortcut = (deploymentMetadataEntry.DeploymentFlags & 0x200) != 0;
            this._update = new System.Deployment.Application.Manifest.DeploymentUpdate(deploymentMetadataEntry);
            this._minimumRequiredVersion = (deploymentMetadataEntry.MinimumRequiredVersion != null) ? new Version(deploymentMetadataEntry.MinimumRequiredVersion) : null;
            this._codebaseUri = AssemblyManifest.UriFromMetadataEntry(deploymentMetadataEntry.DeploymentProviderCodebase, "Ex_DepProviderNotValid");
        }

        public bool CreateDesktopShortcut =>
            this._createDesktopShortcut;

        public System.Deployment.Application.Manifest.DeploymentUpdate DeploymentUpdate =>
            this._update;

        public bool DisallowUrlActivation =>
            this._disallowUrlActivation;

        public bool Install =>
            this._install;

        public bool IsInstalledAndNoDeploymentProvider =>
            (this.Install && (this.ProviderCodebaseUri == null));

        public bool IsUpdateSectionPresent
        {
            get
            {
                if (!this.DeploymentUpdate.BeforeApplicationStartup && !this.DeploymentUpdate.MaximumAgeSpecified)
                {
                    return false;
                }
                return true;
            }
        }

        public bool MapFileExtensions =>
            this._mapFileExtensions;

        public Version MinimumRequiredVersion =>
            this._minimumRequiredVersion;

        public Uri ProviderCodebaseUri =>
            this._codebaseUri;

        public bool TrustURLParameters =>
            this._trustURLParameters;
    }
}

