namespace System.ServiceModel.Activation
{
    using System;
    using System.Collections.ObjectModel;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AspNetCompatibilityRequirementsAttribute : Attribute, IServiceBehavior
    {
        private AspNetCompatibilityRequirementsMode requirementsMode;

        void IServiceBehavior.AddBindingParameters(System.ServiceModel.Description.ServiceDescription description, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(System.ServiceModel.Description.ServiceDescription description, ServiceHostBase serviceHostBase)
        {
        }

        void IServiceBehavior.Validate(System.ServiceModel.Description.ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            if (description == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("description");
            }
            if (!ServiceHostingEnvironment.IsHosted)
            {
                if (this.requirementsMode == AspNetCompatibilityRequirementsMode.Required)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_CompatibilityServiceNotHosted")));
                }
            }
            else if (this.requirementsMode != AspNetCompatibilityRequirementsMode.Allowed)
            {
                if (ServiceHostingEnvironment.AspNetCompatibilityEnabled && (this.requirementsMode == AspNetCompatibilityRequirementsMode.NotAllowed))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_ServiceCompatibilityNotAllowed")));
                }
                if (!ServiceHostingEnvironment.AspNetCompatibilityEnabled && (this.requirementsMode == AspNetCompatibilityRequirementsMode.Required))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_ServiceCompatibilityRequire")));
                }
            }
        }

        public AspNetCompatibilityRequirementsMode RequirementsMode
        {
            get => 
                this.requirementsMode;
            set
            {
                AspNetCompatibilityRequirementsModeHelper.Validate(value);
                this.requirementsMode = value;
            }
        }
    }
}

