namespace System.EnterpriseServices.Thunk
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal class IdentityManager
    {
        private IdentityManager()
        {
        }

        public static unsafe string CreateIdentityUri(IntPtr pUnk)
        {
            ulong num4;
            ulong num5;
            int errorCode = System.EnterpriseServices.Thunk.InitSpy();
            if (errorCode < 0)
            {
                Marshal.ThrowExceptionForHR(errorCode);
            }
            int num2 = **(((int*) System.EnterpriseServices.Thunk.g_pSpy))[40](System.EnterpriseServices.Thunk.g_pSpy);
            if (num2 < 0)
            {
                Marshal.ThrowExceptionForHR(num2);
            }
            int num6 = *(((int*) System.EnterpriseServices.Thunk.g_pSpy)) + 0x20;
            int num = *num6[0](System.EnterpriseServices.Thunk.g_pSpy, pUnk.ToInt32(), &num5, &num4);
            if (num < 0)
            {
                Marshal.ThrowExceptionForHR(num);
            }
            ulong num8 = num4;
            ulong num7 = num5;
            return ("servicedcomponent-local-identity://" + num7.ToString(CultureInfo.InvariantCulture) + ":" + num8.ToString(CultureInfo.InvariantCulture));
        }

        private static void Init()
        {
            int errorCode = System.EnterpriseServices.Thunk.InitSpy();
            if (errorCode < 0)
            {
                Marshal.ThrowExceptionForHR(errorCode);
            }
        }

        [return: MarshalAs(UnmanagedType.U1)]
        public static unsafe bool IsInProcess(IntPtr pUnk)
        {
            int errorCode = System.EnterpriseServices.Thunk.InitSpy();
            if (errorCode < 0)
            {
                Marshal.ThrowExceptionForHR(errorCode);
            }
            int num5 = 1;
            int num4 = *(((int*) System.EnterpriseServices.Thunk.g_pSpy)) + 0x2c;
            int num = *num4[0](System.EnterpriseServices.Thunk.g_pSpy, pUnk.ToInt32(), &num5);
            if (num < 0)
            {
                Marshal.ThrowExceptionForHR(num);
            }
            byte num3 = (num5 != 0) ? ((byte) 1) : ((byte) 0);
            return (bool) num3;
        }

        public static unsafe void NoticeApartment()
        {
            int errorCode = System.EnterpriseServices.Thunk.InitSpy();
            if (errorCode < 0)
            {
                Marshal.ThrowExceptionForHR(errorCode);
            }
            int num2 = System.EnterpriseServices.Thunk.InitSpy();
            if (num2 < 0)
            {
                Marshal.ThrowExceptionForHR(num2);
            }
            if (System.EnterpriseServices.Thunk.InitializeSpy.GetEnabled(System.EnterpriseServices.Thunk.g_pSpy) != 0)
            {
                int num = **(((int*) System.EnterpriseServices.Thunk.g_pSpy))[40](System.EnterpriseServices.Thunk.g_pSpy);
                if (num < 0)
                {
                    Marshal.ThrowExceptionForHR(num);
                }
            }
        }

        public static bool Enabled
        {
            [return: MarshalAs(UnmanagedType.U1)]
            get
            {
                int errorCode = System.EnterpriseServices.Thunk.InitSpy();
                if (errorCode < 0)
                {
                    Marshal.ThrowExceptionForHR(errorCode);
                }
                byte num2 = (System.EnterpriseServices.Thunk.InitializeSpy.GetEnabled(System.EnterpriseServices.Thunk.g_pSpy) != 0) ? ((byte) 1) : ((byte) 0);
                return (bool) num2;
            }
        }
    }
}

