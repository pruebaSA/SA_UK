namespace System.EnterpriseServices.Thunk
{
    using System;
    using System.Runtime.InteropServices;

    internal class ServiceActivityThunk
    {
        public unsafe IServiceActivity* m_pSA;

        public unsafe ServiceActivityThunk(ServiceConfigThunk psct)
        {
            IServiceActivity* activityPtr;
            IUnknown* serviceConfigUnknown = psct.ServiceConfigUnknown;
            this.m_pSA = null;
            **(((int*) serviceConfigUnknown))[8](serviceConfigUnknown);
            Marshal.ThrowExceptionForHR(*ServiceDomainThunk.CoCreateActivity(serviceConfigUnknown, &IID_IServiceActivity, &activityPtr));
            this.m_pSA = activityPtr;
        }

        public void {dtor}()
        {
            GC.SuppressFinalize(this);
            this.Finalize();
        }

        public unsafe void AsynchronousCall(object pObj)
        {
            IUnknown* iUnknownForObject = null;
            IServiceCall* callPtr = null;
            try
            {
                iUnknownForObject = (IUnknown*) Marshal.GetIUnknownForObject(pObj);
                Marshal.ThrowExceptionForHR(**(*((int*) iUnknownForObject))(iUnknownForObject, &IID_IServiceCall, &callPtr));
                IServiceActivity* pSA = this.m_pSA;
                Marshal.ThrowExceptionForHR(**(((int*) pSA))[0x10](pSA, callPtr));
            }
            finally
            {
                if (callPtr != null)
                {
                    **(((int*) callPtr))[8](callPtr);
                }
                if (iUnknownForObject != null)
                {
                    **(((int*) iUnknownForObject))[8](iUnknownForObject);
                }
            }
        }

        public unsafe void BindToCurrentThread()
        {
            Marshal.ThrowExceptionForHR(**(((int*) this.m_pSA))[20](this.m_pSA));
        }

        protected override unsafe void Finalize()
        {
            IServiceActivity* pSA = this.m_pSA;
            if (pSA != null)
            {
                **(((int*) pSA))[8](pSA);
                this.m_pSA = null;
            }
        }

        public unsafe void SynchronousCall(object pObj)
        {
            IUnknown* iUnknownForObject = null;
            IServiceCall* callPtr = null;
            try
            {
                iUnknownForObject = (IUnknown*) Marshal.GetIUnknownForObject(pObj);
                Marshal.ThrowExceptionForHR(**(*((int*) iUnknownForObject))(iUnknownForObject, &IID_IServiceCall, &callPtr));
                IServiceActivity* pSA = this.m_pSA;
                Marshal.ThrowExceptionForHR(**(((int*) pSA))[12](pSA, callPtr));
            }
            finally
            {
                if (callPtr != null)
                {
                    **(((int*) callPtr))[8](callPtr);
                }
                if (iUnknownForObject != null)
                {
                    **(((int*) iUnknownForObject))[8](iUnknownForObject);
                }
            }
        }

        public unsafe void UnbindFromThread()
        {
            Marshal.ThrowExceptionForHR(**(((int*) this.m_pSA))[0x18](this.m_pSA));
        }
    }
}

