namespace System.Security.Policy
{
    using System;

    [Serializable]
    internal class ApplicationTrustExtraInfo
    {
        private bool requestsShellIntegration = true;

        public bool RequestsShellIntegration
        {
            get => 
                this.requestsShellIntegration;
            set
            {
                this.requestsShellIntegration = value;
            }
        }
    }
}

