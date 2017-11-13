namespace System.Deployment.Application.Manifest
{
    using System;
    using System.Deployment.Application;
    using System.Deployment.Internal.Isolation.Manifest;

    internal class EntryPoint
    {
        private readonly string _commandLineFile;
        private readonly string _commandLineParamater;
        private readonly bool _customHostSpecified;
        private readonly bool _customUX;
        private readonly DependentAssembly _dependentAssembly;
        private readonly bool _hostInBrowser;
        private readonly string _name;

        public EntryPoint(System.Deployment.Internal.Isolation.Manifest.EntryPointEntry entryPointEntry, AssemblyManifest manifest)
        {
            this._name = entryPointEntry.Name;
            this._commandLineFile = entryPointEntry.CommandLine_File;
            this._commandLineParamater = entryPointEntry.CommandLine_Parameters;
            this._hostInBrowser = (entryPointEntry.Flags & 1) != 0;
            this._customHostSpecified = (entryPointEntry.Flags & 2) != 0;
            this._customUX = (entryPointEntry.Flags & 4) != 0;
            if (!this._customHostSpecified)
            {
                if (entryPointEntry.Identity != null)
                {
                    this._dependentAssembly = manifest.GetDependentAssemblyByIdentity(entryPointEntry.Identity);
                }
                if (this._dependentAssembly == null)
                {
                    throw new InvalidDeploymentException(ExceptionTypes.ManifestParse, Resources.GetString("Ex_NoMatchingAssemblyForEntryPoint"));
                }
            }
        }

        public DependentAssembly Assembly =>
            this._dependentAssembly;

        public string CommandFile =>
            this._commandLineFile;

        public string CommandParameters =>
            this._commandLineParamater;

        public bool CustomHostSpecified =>
            this._customHostSpecified;

        public bool CustomUX =>
            this._customUX;

        public bool HostInBrowser =>
            this._hostInBrowser;
    }
}

