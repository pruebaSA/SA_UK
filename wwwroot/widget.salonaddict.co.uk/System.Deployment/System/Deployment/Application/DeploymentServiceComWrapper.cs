namespace System.Deployment.Application
{
    using System;

    internal class DeploymentServiceComWrapper : IManagedDeploymentServiceCom
    {
        private DeploymentServiceCom m_deploymentServiceCom = new DeploymentServiceCom();

        public void ActivateApplicationExtension(string textualSubId, string deploymentProviderUrl, string targetAssociatedFile)
        {
            this.m_deploymentServiceCom.ActivateApplicationExtension(textualSubId, deploymentProviderUrl, targetAssociatedFile);
        }

        public void ActivateDeployment(string deploymentLocation, bool isShortcut)
        {
            this.m_deploymentServiceCom.ActivateDeployment(deploymentLocation, isShortcut);
        }

        public void ActivateDeploymentEx(string deploymentLocation, int unsignedPolicy, int signedPolicy)
        {
            this.m_deploymentServiceCom.ActivateDeploymentEx(deploymentLocation, unsignedPolicy, signedPolicy);
        }

        public void CheckForDeploymentUpdate(string textualSubId)
        {
            this.m_deploymentServiceCom.CheckForDeploymentUpdate(textualSubId);
        }

        public void CleanOnlineAppCache()
        {
            this.m_deploymentServiceCom.CleanOnlineAppCache();
        }

        public void EndServiceRightNow()
        {
            this.m_deploymentServiceCom.EndServiceRightNow();
        }

        public void MaintainSubscription(string textualSubId)
        {
            this.m_deploymentServiceCom.MaintainSubscription(textualSubId);
        }
    }
}

