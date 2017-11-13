namespace System.Configuration.Internal
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;

    public class DelegatingConfigHost : IInternalConfigHost
    {
        private IInternalConfigHost _host;

        protected DelegatingConfigHost()
        {
        }

        public virtual object CreateConfigurationContext(string configPath, string locationSubPath) => 
            this.Host.CreateConfigurationContext(configPath, locationSubPath);

        public virtual object CreateDeprecatedConfigContext(string configPath) => 
            this.Host.CreateDeprecatedConfigContext(configPath);

        public virtual string DecryptSection(string encryptedXml, ProtectedConfigurationProvider protectionProvider, ProtectedConfigurationSection protectedConfigSection) => 
            this.Host.DecryptSection(encryptedXml, protectionProvider, protectedConfigSection);

        public virtual void DeleteStream(string streamName)
        {
            this.Host.DeleteStream(streamName);
        }

        public virtual string EncryptSection(string clearTextXml, ProtectedConfigurationProvider protectionProvider, ProtectedConfigurationSection protectedConfigSection) => 
            this.Host.EncryptSection(clearTextXml, protectionProvider, protectedConfigSection);

        public virtual string GetConfigPathFromLocationSubPath(string configPath, string locationSubPath) => 
            this.Host.GetConfigPathFromLocationSubPath(configPath, locationSubPath);

        public virtual Type GetConfigType(string typeName, bool throwOnError) => 
            this.Host.GetConfigType(typeName, throwOnError);

        public virtual string GetConfigTypeName(Type t) => 
            this.Host.GetConfigTypeName(t);

        public virtual void GetRestrictedPermissions(IInternalConfigRecord configRecord, out PermissionSet permissionSet, out bool isHostReady)
        {
            this.Host.GetRestrictedPermissions(configRecord, out permissionSet, out isHostReady);
        }

        public virtual string GetStreamName(string configPath) => 
            this.Host.GetStreamName(configPath);

        public virtual string GetStreamNameForConfigSource(string streamName, string configSource) => 
            this.Host.GetStreamNameForConfigSource(streamName, configSource);

        public virtual object GetStreamVersion(string streamName) => 
            this.Host.GetStreamVersion(streamName);

        public virtual IDisposable Impersonate() => 
            this.Host.Impersonate();

        public virtual void Init(IInternalConfigRoot configRoot, params object[] hostInitParams)
        {
            this.Host.Init(configRoot, hostInitParams);
        }

        public virtual void InitForConfiguration(ref string locationSubPath, out string configPath, out string locationConfigPath, IInternalConfigRoot configRoot, params object[] hostInitConfigurationParams)
        {
            this.Host.InitForConfiguration(ref locationSubPath, out configPath, out locationConfigPath, configRoot, hostInitConfigurationParams);
        }

        public virtual bool IsAboveApplication(string configPath) => 
            this.Host.IsAboveApplication(configPath);

        public virtual bool IsConfigRecordRequired(string configPath) => 
            this.Host.IsConfigRecordRequired(configPath);

        public virtual bool IsDefinitionAllowed(string configPath, ConfigurationAllowDefinition allowDefinition, ConfigurationAllowExeDefinition allowExeDefinition) => 
            this.Host.IsDefinitionAllowed(configPath, allowDefinition, allowExeDefinition);

        public virtual bool IsFile(string streamName) => 
            this.Host.IsFile(streamName);

        public virtual bool IsFullTrustSectionWithoutAptcaAllowed(IInternalConfigRecord configRecord) => 
            this.Host.IsFullTrustSectionWithoutAptcaAllowed(configRecord);

        public virtual bool IsInitDelayed(IInternalConfigRecord configRecord) => 
            this.Host.IsInitDelayed(configRecord);

        public virtual bool IsLocationApplicable(string configPath) => 
            this.Host.IsLocationApplicable(configPath);

        public virtual bool IsSecondaryRoot(string configPath) => 
            this.Host.IsSecondaryRoot(configPath);

        public virtual bool IsTrustedConfigPath(string configPath) => 
            this.Host.IsTrustedConfigPath(configPath);

        public virtual Stream OpenStreamForRead(string streamName) => 
            this.Host.OpenStreamForRead(streamName);

        public virtual Stream OpenStreamForRead(string streamName, bool assertPermissions) => 
            this.Host.OpenStreamForRead(streamName, assertPermissions);

        public virtual Stream OpenStreamForWrite(string streamName, string templateStreamName, ref object writeContext) => 
            this.Host.OpenStreamForWrite(streamName, templateStreamName, ref writeContext);

        public virtual Stream OpenStreamForWrite(string streamName, string templateStreamName, ref object writeContext, bool assertPermissions) => 
            this.Host.OpenStreamForWrite(streamName, templateStreamName, ref writeContext, assertPermissions);

        public virtual bool PrefetchAll(string configPath, string streamName) => 
            this.Host.PrefetchAll(configPath, streamName);

        public virtual bool PrefetchSection(string sectionGroupName, string sectionName) => 
            this.Host.PrefetchSection(sectionGroupName, sectionName);

        public virtual void RequireCompleteInit(IInternalConfigRecord configRecord)
        {
            this.Host.RequireCompleteInit(configRecord);
        }

        public virtual object StartMonitoringStreamForChanges(string streamName, StreamChangeCallback callback) => 
            this.Host.StartMonitoringStreamForChanges(streamName, callback);

        public virtual void StopMonitoringStreamForChanges(string streamName, StreamChangeCallback callback)
        {
            this.Host.StopMonitoringStreamForChanges(streamName, callback);
        }

        public virtual void VerifyDefinitionAllowed(string configPath, ConfigurationAllowDefinition allowDefinition, ConfigurationAllowExeDefinition allowExeDefinition, IConfigErrorInfo errorInfo)
        {
            this.Host.VerifyDefinitionAllowed(configPath, allowDefinition, allowExeDefinition, errorInfo);
        }

        public virtual void WriteCompleted(string streamName, bool success, object writeContext)
        {
            this.Host.WriteCompleted(streamName, success, writeContext);
        }

        public virtual void WriteCompleted(string streamName, bool success, object writeContext, bool assertPermissions)
        {
            this.Host.WriteCompleted(streamName, success, writeContext, assertPermissions);
        }

        protected IInternalConfigHost Host
        {
            get => 
                this._host;
            set
            {
                this._host = value;
            }
        }

        public virtual bool IsRemote =>
            this.Host.IsRemote;

        public virtual bool SupportsChangeNotifications =>
            this.Host.SupportsChangeNotifications;

        public virtual bool SupportsLocation =>
            this.Host.SupportsLocation;

        public virtual bool SupportsPath =>
            this.Host.SupportsPath;

        public virtual bool SupportsRefresh =>
            this.Host.SupportsRefresh;
    }
}

