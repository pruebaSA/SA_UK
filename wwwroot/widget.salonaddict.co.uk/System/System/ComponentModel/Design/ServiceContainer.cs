namespace System.ComponentModel.Design
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class ServiceContainer : IServiceContainer, IServiceProvider, IDisposable
    {
        private static Type[] _defaultServices = new Type[] { typeof(IServiceContainer), typeof(ServiceContainer) };
        private IServiceProvider parentProvider;
        private Hashtable services;
        private static TraceSwitch TRACESERVICE = new TraceSwitch("TRACESERVICE", "ServiceProvider: Trace service provider requests.");

        public ServiceContainer()
        {
        }

        public ServiceContainer(IServiceProvider parentProvider)
        {
            this.parentProvider = parentProvider;
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback)
        {
            this.AddService(serviceType, callback, false);
        }

        public void AddService(Type serviceType, object serviceInstance)
        {
            this.AddService(serviceType, serviceInstance, false);
        }

        public virtual void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
        {
            if (promote)
            {
                IServiceContainer container = this.Container;
                if (container != null)
                {
                    container.AddService(serviceType, callback, promote);
                    return;
                }
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }
            if (this.Services.ContainsKey(serviceType))
            {
                throw new ArgumentException(SR.GetString("ErrorServiceExists", new object[] { serviceType.FullName }), "serviceType");
            }
            this.Services[serviceType] = callback;
        }

        public virtual void AddService(Type serviceType, object serviceInstance, bool promote)
        {
            if (promote)
            {
                IServiceContainer container = this.Container;
                if (container != null)
                {
                    container.AddService(serviceType, serviceInstance, promote);
                    return;
                }
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }
            if (serviceInstance == null)
            {
                throw new ArgumentNullException("serviceInstance");
            }
            if ((!(serviceInstance is ServiceCreatorCallback) && !serviceInstance.GetType().IsCOMObject) && !serviceType.IsAssignableFrom(serviceInstance.GetType()))
            {
                throw new ArgumentException(SR.GetString("ErrorInvalidServiceInstance", new object[] { serviceType.FullName }));
            }
            if (this.Services.ContainsKey(serviceType))
            {
                throw new ArgumentException(SR.GetString("ErrorServiceExists", new object[] { serviceType.FullName }), "serviceType");
            }
            this.Services[serviceType] = serviceInstance;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Hashtable services = this.services;
                this.services = null;
                if (services != null)
                {
                    foreach (object obj2 in services.Values)
                    {
                        if (obj2 is IDisposable)
                        {
                            ((IDisposable) obj2).Dispose();
                        }
                    }
                }
            }
        }

        public virtual object GetService(Type serviceType)
        {
            object service = null;
            Type[] defaultServices = this.DefaultServices;
            for (int i = 0; i < defaultServices.Length; i++)
            {
                if (serviceType == defaultServices[i])
                {
                    service = this;
                    break;
                }
            }
            if (service == null)
            {
                service = this.Services[serviceType];
            }
            if (service is ServiceCreatorCallback)
            {
                service = ((ServiceCreatorCallback) service)(this, serviceType);
                if (((service != null) && !service.GetType().IsCOMObject) && !serviceType.IsAssignableFrom(service.GetType()))
                {
                    service = null;
                }
                this.Services[serviceType] = service;
            }
            if ((service == null) && (this.parentProvider != null))
            {
                service = this.parentProvider.GetService(serviceType);
            }
            return service;
        }

        public void RemoveService(Type serviceType)
        {
            this.RemoveService(serviceType, false);
        }

        public virtual void RemoveService(Type serviceType, bool promote)
        {
            if (promote)
            {
                IServiceContainer container = this.Container;
                if (container != null)
                {
                    container.RemoveService(serviceType, promote);
                    return;
                }
            }
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }
            this.Services.Remove(serviceType);
        }

        private IServiceContainer Container
        {
            get
            {
                IServiceContainer service = null;
                if (this.parentProvider != null)
                {
                    service = (IServiceContainer) this.parentProvider.GetService(typeof(IServiceContainer));
                }
                return service;
            }
        }

        protected virtual Type[] DefaultServices =>
            _defaultServices;

        private Hashtable Services
        {
            get
            {
                if (this.services == null)
                {
                    this.services = new Hashtable();
                }
                return this.services;
            }
        }
    }
}

