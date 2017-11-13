namespace System.Runtime.Remoting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Activation;
    using System.Security.Permissions;

    [ComVisible(true)]
    public static class RemotingConfiguration
    {
        private static bool s_ListeningForActivationRequests;

        [Obsolete("Use System.Runtime.Remoting.RemotingConfiguration.Configure(string fileName, bool ensureSecurity) instead.", false), SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static void Configure(string filename)
        {
            Configure(filename, false);
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static void Configure(string filename, bool ensureSecurity)
        {
            RemotingConfigHandler.DoConfiguration(filename, ensureSecurity);
            RemotingServices.InternalSetRemoteActivationConfigured();
        }

        public static bool CustomErrorsEnabled(bool isLocalRequest)
        {
            switch (CustomErrorsMode)
            {
                case CustomErrorsModes.On:
                    return true;

                case CustomErrorsModes.Off:
                    return false;

                case CustomErrorsModes.RemoteOnly:
                    return !isLocalRequest;
            }
            return true;
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static ActivatedClientTypeEntry[] GetRegisteredActivatedClientTypes() => 
            RemotingConfigHandler.GetRegisteredActivatedClientTypes();

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static ActivatedServiceTypeEntry[] GetRegisteredActivatedServiceTypes() => 
            RemotingConfigHandler.GetRegisteredActivatedServiceTypes();

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static WellKnownClientTypeEntry[] GetRegisteredWellKnownClientTypes() => 
            RemotingConfigHandler.GetRegisteredWellKnownClientTypes();

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static WellKnownServiceTypeEntry[] GetRegisteredWellKnownServiceTypes() => 
            RemotingConfigHandler.GetRegisteredWellKnownServiceTypes();

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static bool IsActivationAllowed(Type svrType) => 
            RemotingConfigHandler.IsActivationAllowed(svrType);

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static ActivatedClientTypeEntry IsRemotelyActivatedClientType(Type svrType) => 
            RemotingConfigHandler.IsRemotelyActivatedClientType(svrType);

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static ActivatedClientTypeEntry IsRemotelyActivatedClientType(string typeName, string assemblyName) => 
            RemotingConfigHandler.IsRemotelyActivatedClientType(typeName, assemblyName);

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static WellKnownClientTypeEntry IsWellKnownClientType(Type svrType) => 
            RemotingConfigHandler.IsWellKnownClientType(svrType);

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static WellKnownClientTypeEntry IsWellKnownClientType(string typeName, string assemblyName) => 
            RemotingConfigHandler.IsWellKnownClientType(typeName, assemblyName);

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static void RegisterActivatedClientType(ActivatedClientTypeEntry entry)
        {
            RemotingConfigHandler.RegisterActivatedClientType(entry);
            RemotingServices.InternalSetRemoteActivationConfigured();
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static void RegisterActivatedClientType(Type type, string appUrl)
        {
            ActivatedClientTypeEntry entry = new ActivatedClientTypeEntry(type, appUrl);
            RegisterActivatedClientType(entry);
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static void RegisterActivatedServiceType(ActivatedServiceTypeEntry entry)
        {
            RemotingConfigHandler.RegisterActivatedServiceType(entry);
            if (!s_ListeningForActivationRequests)
            {
                s_ListeningForActivationRequests = true;
                ActivationServices.StartListeningForRemoteRequests();
            }
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static void RegisterActivatedServiceType(Type type)
        {
            ActivatedServiceTypeEntry entry = new ActivatedServiceTypeEntry(type);
            RegisterActivatedServiceType(entry);
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static void RegisterWellKnownClientType(WellKnownClientTypeEntry entry)
        {
            RemotingConfigHandler.RegisterWellKnownClientType(entry);
            RemotingServices.InternalSetRemoteActivationConfigured();
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static void RegisterWellKnownClientType(Type type, string objectUrl)
        {
            WellKnownClientTypeEntry entry = new WellKnownClientTypeEntry(type, objectUrl);
            RegisterWellKnownClientType(entry);
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static void RegisterWellKnownServiceType(WellKnownServiceTypeEntry entry)
        {
            RemotingConfigHandler.RegisterWellKnownServiceType(entry);
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
        public static void RegisterWellKnownServiceType(Type type, string objectUri, WellKnownObjectMode mode)
        {
            WellKnownServiceTypeEntry entry = new WellKnownServiceTypeEntry(type, objectUri, mode);
            RegisterWellKnownServiceType(entry);
        }

        public static string ApplicationId =>
            Identity.AppDomainUniqueId;

        public static string ApplicationName
        {
            get
            {
                if (!RemotingConfigHandler.HasApplicationNameBeenSet())
                {
                    return null;
                }
                return RemotingConfigHandler.ApplicationName;
            }
            [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
            set
            {
                RemotingConfigHandler.ApplicationName = value;
            }
        }

        public static CustomErrorsModes CustomErrorsMode
        {
            get => 
                RemotingConfigHandler.CustomErrorsMode;
            [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
            set
            {
                RemotingConfigHandler.CustomErrorsMode = value;
            }
        }

        public static string ProcessId =>
            Identity.ProcessGuid;
    }
}

