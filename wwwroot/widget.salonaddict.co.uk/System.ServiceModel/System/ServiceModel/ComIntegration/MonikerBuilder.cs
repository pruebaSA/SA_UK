namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Services;
    using System.ServiceModel;

    internal class MonikerBuilder : IProxyCreator, IDisposable
    {
        private ComProxy comProxy;

        private MonikerBuilder()
        {
        }

        public static MarshalByRefObject CreateMonikerInstance()
        {
            IProxyCreator proxyCreator = new MonikerBuilder();
            IProxyManager proxyManager = new ProxyManager(proxyCreator);
            Guid gUID = typeof(IMoniker).GUID;
            IntPtr punk = OuterProxyWrapper.CreateOuterProxyInstance(proxyManager, ref gUID);
            MarshalByRefObject obj2 = EnterpriseServicesHelper.WrapIUnknownWithComObject(punk) as MarshalByRefObject;
            Marshal.Release(punk);
            return obj2;
        }

        void IDisposable.Dispose()
        {
        }

        ComProxy IProxyCreator.CreateProxy(IntPtr outer, ref Guid riid)
        {
            if ((riid != typeof(IMoniker).GUID) && (riid != typeof(IParseDisplayName).GUID))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidCastException(System.ServiceModel.SR.GetString("NoInterface", new object[] { (Guid) riid })));
            }
            if (outer == IntPtr.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            return this.comProxy?.Clone();
        }

        bool IProxyCreator.SupportsDispatch() => 
            false;

        bool IProxyCreator.SupportsErrorInfo(ref Guid riid)
        {
            if ((riid != typeof(IMoniker).GUID) && (riid != typeof(IParseDisplayName).GUID))
            {
                return false;
            }
            return true;
        }

        bool IProxyCreator.SupportsIntrinsics() => 
            false;
    }
}

