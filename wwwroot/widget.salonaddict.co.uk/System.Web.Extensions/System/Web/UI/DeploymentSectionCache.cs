namespace System.Web.UI
{
    using System;
    using System.Configuration;
    using System.Security;
    using System.Security.Permissions;
    using System.Web.Configuration;

    internal sealed class DeploymentSectionCache : IDeploymentSection
    {
        private static readonly DeploymentSectionCache _instance = new DeploymentSectionCache();
        private bool? _retail;

        private DeploymentSectionCache()
        {
        }

        [SecurityCritical, SecurityTreatAsSafe, ConfigurationPermission(SecurityAction.Assert, Unrestricted=true)]
        private static bool GetRetailFromConfig()
        {
            DeploymentSection section = (DeploymentSection) WebConfigurationManager.GetSection("system.web/deployment");
            return section.Retail;
        }

        public static DeploymentSectionCache Instance =>
            _instance;

        public bool Retail
        {
            get
            {
                if (!this._retail.HasValue)
                {
                    this._retail = new bool?(GetRetailFromConfig());
                }
                return this._retail.Value;
            }
        }
    }
}

