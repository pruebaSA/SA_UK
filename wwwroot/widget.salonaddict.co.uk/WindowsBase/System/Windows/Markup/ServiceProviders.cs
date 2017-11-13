namespace System.Windows.Markup
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;

    [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
    public class ServiceProviders : IServiceProvider
    {
        private Dictionary<Type, object> _objDict = new Dictionary<Type, object>();

        public void AddService(Type serviceType, object service)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }
            if (service == null)
            {
                throw new ArgumentNullException("service");
            }
            if (!this._objDict.ContainsKey(serviceType))
            {
                this._objDict.Add(serviceType, service);
            }
            else if (this._objDict[serviceType] != service)
            {
                throw new ArgumentException(System.Windows.SR.Get("ServiceTypeAlreadyAdded"), "serviceType");
            }
        }

        public object GetService(Type serviceType)
        {
            if (this._objDict.ContainsKey(serviceType))
            {
                return this._objDict[serviceType];
            }
            return null;
        }
    }
}

