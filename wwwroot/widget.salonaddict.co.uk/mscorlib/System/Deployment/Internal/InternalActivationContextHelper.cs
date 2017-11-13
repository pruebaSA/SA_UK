namespace System.Deployment.Internal
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(false)]
    public static class InternalActivationContextHelper
    {
        public static object GetActivationContextData(ActivationContext appInfo) => 
            appInfo.ActivationContextData;

        public static object GetApplicationComponentManifest(ActivationContext appInfo) => 
            appInfo.ApplicationComponentManifest;

        public static byte[] GetApplicationManifestBytes(ActivationContext appInfo) => 
            appInfo?.GetApplicationManifestBytes();

        public static object GetDeploymentComponentManifest(ActivationContext appInfo) => 
            appInfo.DeploymentComponentManifest;

        public static byte[] GetDeploymentManifestBytes(ActivationContext appInfo) => 
            appInfo?.GetDeploymentManifestBytes();

        public static bool IsFirstRun(ActivationContext appInfo) => 
            (appInfo.LastApplicationStateResult == ActivationContext.ApplicationStateDisposition.RunningFirstTime);

        public static void PrepareForExecution(ActivationContext appInfo)
        {
            appInfo.PrepareForExecution();
        }
    }
}

