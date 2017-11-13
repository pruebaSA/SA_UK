namespace System.Deployment.Application
{
    using System;
    using System.Runtime.InteropServices;

    [ComImport, Guid("B3CA4E79-0107-4CA7-9708-3BE0A97957FB"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IManagedDeploymentServiceCom
    {
        void ActivateDeployment(string deploymentLocation, bool isShortcut);
        void ActivateDeploymentEx(string deploymentLocation, int unsignedPolicy, int signedPolicy);
        void ActivateApplicationExtension(string textualSubId, string deploymentProviderUrl, string targetAssociatedFile);
        void MaintainSubscription(string textualSubId);
        void CheckForDeploymentUpdate(string textualSubId);
        void EndServiceRightNow();
        void CleanOnlineAppCache();
    }
}

