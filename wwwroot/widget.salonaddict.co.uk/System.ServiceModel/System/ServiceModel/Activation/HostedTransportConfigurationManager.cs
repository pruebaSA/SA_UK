namespace System.ServiceModel.Activation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.ServiceModel;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Diagnostics;

    internal class HostedTransportConfigurationManager
    {
        private IDictionary<string, HostedTransportConfiguration> configurations;
        private const string CreateMetabaseSettingsIis7MethodName = "CreateMetabaseSettings";
        private bool initialized;
        private System.ServiceModel.Activation.MetabaseSettings metabaseSettings;
        private const string MetabaseSettingsIis7FactoryTypeName = "System.ServiceModel.WasHosting.MetabaseSettingsIis7Factory, System.ServiceModel.WasHosting, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        private static HostedTransportConfigurationManager singleton;
        private static object syncRoot = new object();
        private const string WasHostingAssemblyName = "System.ServiceModel.WasHosting, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        private HostedTransportConfigurationManager()
        {
            this.configurations = new Dictionary<string, HostedTransportConfiguration>(StringComparer.Ordinal);
            if (!Iis7Helper.IsIis7)
            {
                this.metabaseSettings = new MetabaseSettingsIis6();
            }
            else
            {
                this.metabaseSettings = CreateWasHostingMetabaseSettings();
            }
        }

        private HostedTransportConfigurationManager(System.ServiceModel.Activation.MetabaseSettings metabaseSettings)
        {
            this.configurations = new Dictionary<string, HostedTransportConfiguration>(StringComparer.Ordinal);
            this.metabaseSettings = metabaseSettings;
        }

        private void AddHostedTransportConfigurationIis7(string protocol)
        {
            HostedTransportConfiguration configuration = null;
            try
            {
                ServiceHostingEnvironmentSection section = ServiceHostingEnvironmentSection.GetSection();
                if (!section.TransportConfigurationTypes.ContainsKey(protocol))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_ProtocolNoConfiguration", new object[] { protocol })));
                }
                TransportConfigurationTypeElement element = section.TransportConfigurationTypes[protocol];
                configuration = Activator.CreateInstance(Type.GetType(element.TransportConfigurationType)) as HostedTransportConfiguration;
                this.configurations.Add(protocol, configuration);
            }
            catch (Exception exception)
            {
                if (!DiagnosticUtility.IsFatal(exception) && DiagnosticUtility.ShouldTraceError)
                {
                    TraceUtility.TraceEvent(TraceEventType.Error, TraceCode.WebHostProtocolMisconfigured, new StringTraceRecord("Protocol", protocol), this, exception);
                }
                throw;
            }
        }

        [SecurityCritical]
        private static MetabaseSettingsIis CreateMetabaseSettings(Type type)
        {
            object obj2 = null;
            MethodInfo method = type.GetMethod("CreateMetabaseSettings", BindingFlags.NonPublic | BindingFlags.Static);
            try
            {
                new ReflectionPermission(ReflectionPermissionFlag.MemberAccess).Assert();
                obj2 = method.Invoke(null, null);
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            if (!(obj2 is MetabaseSettingsIis))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_BadMetabaseSettingsIis7Type", new object[] { type.AssemblyQualifiedName })));
            }
            return (MetabaseSettingsIis) obj2;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private static MetabaseSettingsIis CreateWasHostingMetabaseSettings()
        {
            Type type = Type.GetType("System.ServiceModel.WasHosting.MetabaseSettingsIis7Factory, System.ServiceModel.WasHosting, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", false);
            if (type == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_MetabaseSettingsIis7TypeNotFound", new object[] { "System.ServiceModel.WasHosting.MetabaseSettingsIis7Factory, System.ServiceModel.WasHosting, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", "System.ServiceModel.WasHosting, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" })));
            }
            return CreateMetabaseSettings(type);
        }

        private void EnsureInitialized()
        {
            if (!this.initialized)
            {
                lock (this.ThisLock)
                {
                    if (!this.initialized)
                    {
                        foreach (string str in this.metabaseSettings.GetProtocols())
                        {
                            if ((string.CompareOrdinal(str, Uri.UriSchemeHttp) == 0) || (string.CompareOrdinal(str, Uri.UriSchemeHttps) == 0))
                            {
                                HttpHostedTransportConfiguration configuration = null;
                                if (string.CompareOrdinal(str, Uri.UriSchemeHttp) == 0)
                                {
                                    configuration = new HttpHostedTransportConfiguration();
                                }
                                else
                                {
                                    configuration = new HttpsHostedTransportConfiguration();
                                }
                                this.configurations.Add(str, configuration);
                            }
                            else
                            {
                                if (!Iis7Helper.IsIis7)
                                {
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(true);
                                }
                                if (PartialTrustHelpers.NeedPartialTrustInvoke)
                                {
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("PartialTrustNonHttpActivation", new object[] { str, HostingEnvironmentWrapper.ApplicationVirtualPath })));
                                }
                                this.AddHostedTransportConfigurationIis7(str);
                            }
                        }
                        this.initialized = true;
                    }
                }
            }
        }

        internal static void EnsureInitializedForSimpleApplicationHost(HostedHttpRequestAsyncResult result)
        {
            if (singleton == null)
            {
                lock (syncRoot)
                {
                    if (singleton == null)
                    {
                        singleton = new HostedTransportConfigurationManager(new MetabaseSettingsCassini(result));
                    }
                }
            }
        }

        internal static Uri[] GetBaseAddresses(string virtualPath) => 
            Value.InternalGetBaseAddresses(virtualPath);

        internal static HostedTransportConfiguration GetConfiguration(string scheme) => 
            Value.InternalGetConfiguration(scheme);

        private Uri[] InternalGetBaseAddresses(string virtualPath)
        {
            this.EnsureInitialized();
            List<Uri> list = new List<Uri>();
            foreach (HostedTransportConfiguration configuration in this.configurations.Values)
            {
                list.AddRange(configuration.GetBaseAddresses(virtualPath));
            }
            return list.ToArray();
        }

        private HostedTransportConfiguration InternalGetConfiguration(string scheme)
        {
            this.EnsureInitialized();
            if (!this.configurations.ContainsKey(scheme))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_NotSupportedProtocol", new object[] { scheme })));
            }
            return this.configurations[scheme];
        }

        internal static System.ServiceModel.Activation.MetabaseSettings MetabaseSettings =>
            Value.metabaseSettings;

        private object ThisLock =>
            this;

        private static HostedTransportConfigurationManager Value
        {
            get
            {
                if (singleton == null)
                {
                    lock (syncRoot)
                    {
                        if (singleton == null)
                        {
                            ServiceHostingEnvironment.EnsureInitialized();
                            singleton = new HostedTransportConfigurationManager();
                        }
                    }
                }
                return singleton;
            }
        }
    }
}

