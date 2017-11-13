﻿namespace System.ComponentModel.Design
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public interface IServiceContainer : IServiceProvider
    {
        void AddService(Type serviceType, ServiceCreatorCallback callback);
        void AddService(Type serviceType, object serviceInstance);
        void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote);
        void AddService(Type serviceType, object serviceInstance, bool promote);
        void RemoveService(Type serviceType);
        void RemoveService(Type serviceType, bool promote);
    }
}

