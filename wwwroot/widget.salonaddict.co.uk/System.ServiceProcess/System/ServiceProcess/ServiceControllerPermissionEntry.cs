namespace System.ServiceProcess
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;

    [Serializable]
    public class ServiceControllerPermissionEntry
    {
        private string machineName;
        private ServiceControllerPermissionAccess permissionAccess;
        private string serviceName;

        public ServiceControllerPermissionEntry()
        {
            this.machineName = ".";
            this.serviceName = "*";
            this.permissionAccess = ServiceControllerPermissionAccess.Browse;
        }

        internal ServiceControllerPermissionEntry(ResourcePermissionBaseEntry baseEntry)
        {
            this.permissionAccess = (ServiceControllerPermissionAccess) baseEntry.PermissionAccess;
            this.machineName = baseEntry.PermissionAccessPath[0];
            this.serviceName = baseEntry.PermissionAccessPath[1];
        }

        public ServiceControllerPermissionEntry(ServiceControllerPermissionAccess permissionAccess, string machineName, string serviceName)
        {
            if (serviceName == null)
            {
                throw new ArgumentNullException("serviceName");
            }
            if (!ServiceController.ValidServiceName(serviceName))
            {
                object[] args = new object[] { serviceName, 80.ToString(CultureInfo.CurrentCulture) };
                throw new ArgumentException(Res.GetString("ServiceName", args));
            }
            if (!SyntaxCheck.CheckMachineName(machineName))
            {
                throw new ArgumentException(Res.GetString("BadMachineName", new object[] { machineName }));
            }
            this.permissionAccess = permissionAccess;
            this.machineName = machineName;
            this.serviceName = serviceName;
        }

        internal ResourcePermissionBaseEntry GetBaseEntry() => 
            new ResourcePermissionBaseEntry((int) this.PermissionAccess, new string[] { this.MachineName, this.ServiceName });

        public string MachineName =>
            this.machineName;

        public ServiceControllerPermissionAccess PermissionAccess =>
            this.permissionAccess;

        public string ServiceName =>
            this.serviceName;
    }
}

