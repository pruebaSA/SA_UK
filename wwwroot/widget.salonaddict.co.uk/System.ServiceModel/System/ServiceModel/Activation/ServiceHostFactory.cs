namespace System.ServiceModel.Activation
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.ServiceModel;

    public class ServiceHostFactory : ServiceHostFactoryBase
    {
        private Collection<string> referencedAssemblies = new Collection<string>();

        internal void AddAssemblyReference(string assemblyName)
        {
            this.referencedAssemblies.Add(assemblyName);
        }

        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            if (!ServiceHostingEnvironment.IsHosted)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_ProcessNotExecutingUnderHostedContext", new object[] { "ServiceHostFactory.CreateServiceHost" })));
            }
            if (string.IsNullOrEmpty(constructorString))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_ServiceTypeNotProvided")));
            }
            Type serviceType = Type.GetType(constructorString, false);
            if (serviceType == null)
            {
                foreach (string str in this.referencedAssemblies)
                {
                    serviceType = Assembly.Load(str).GetType(constructorString, false);
                    if (serviceType != null)
                    {
                        break;
                    }
                }
            }
            if (serviceType == null)
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < assemblies.Length; i++)
                {
                    serviceType = assemblies[i].GetType(constructorString, false);
                    if (serviceType != null)
                    {
                        break;
                    }
                }
            }
            if (serviceType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_ServiceTypeNotResolved", new object[] { constructorString })));
            }
            return this.CreateServiceHost(serviceType, baseAddresses);
        }

        protected virtual ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses) => 
            new ServiceHost(serviceType, baseAddresses);
    }
}

