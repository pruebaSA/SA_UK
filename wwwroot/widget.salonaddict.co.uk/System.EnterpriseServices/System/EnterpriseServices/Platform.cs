namespace System.EnterpriseServices
{
    using System;
    using System.Collections;
    using System.EnterpriseServices.Admin;
    using System.EnterpriseServices.Thunk;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal class Platform
    {
        private static Version _current;
        private static Hashtable _features = new Hashtable();
        private static volatile bool _initialized;
        private static Version _mts;
        private static Version _w2k;
        private static Version _whistler;

        private Platform()
        {
        }

        internal static void Assert(bool fSuccess, string function)
        {
            if (!fSuccess)
            {
                throw new PlatformNotSupportedException(Resource.FormatString("Err_PlatformSupport", function));
            }
        }

        internal static void Assert(Version platform, string function)
        {
            Initialize();
            if ((_current.Major < platform.Major) || ((_current.Major == platform.Major) && (_current.Minor < platform.Minor)))
            {
                Assert(false, function);
            }
        }

        internal static bool CheckUserContextPropertySupport()
        {
            bool flag = false;
            Util.OSVERSIONINFOEX ver = new Util.OSVERSIONINFOEX();
            if (Util.GetVersionEx(ver))
            {
                if (ver.MajorVersion > 5)
                {
                    return true;
                }
                if (((ver.MajorVersion == 5) && (ver.MinorVersion == 1)) && (ver.ServicePackMajor >= 2))
                {
                    return true;
                }
                if (((ver.MajorVersion == 5) && (ver.MinorVersion == 2)) && (ver.ServicePackMajor >= 1))
                {
                    flag = true;
                }
            }
            return flag;
        }

        private static object FindFeatureData(PlatformFeature feature) => 
            _features[feature];

        internal static object GetFeatureData(PlatformFeature feature)
        {
            object obj2 = FindFeatureData(feature);
            if (obj2 == null)
            {
                switch (feature)
                {
                    case PlatformFeature.SWC:
                        obj2 = SWCThunk.IsSWCSupported();
                        break;

                    case PlatformFeature.UserContextProperties:
                        obj2 = CheckUserContextPropertySupport();
                        break;

                    default:
                        return null;
                }
                SetFeatureData(feature, obj2);
            }
            return obj2;
        }

        private static void Initialize()
        {
            if (!_initialized)
            {
                lock (typeof(Platform))
                {
                    if (!_initialized)
                    {
                        IntPtr zero = IntPtr.Zero;
                        _mts = new Version(2, 0);
                        _w2k = new Version(3, 0);
                        _whistler = new Version(4, 0);
                        try
                        {
                            try
                            {
                                zero = Security.SuspendImpersonation();
                                IMtsCatalog catalog = (IMtsCatalog) new xMtsCatalog();
                                _current = new Version(catalog.MajorVersion(), catalog.MinorVersion());
                            }
                            catch (COMException)
                            {
                                _current = new Version(0, 0);
                            }
                            finally
                            {
                                Security.ResumeImpersonation(zero);
                            }
                        }
                        catch
                        {
                            throw;
                        }
                        _initialized = true;
                    }
                }
            }
        }

        internal static bool IsLessThan(Version platform)
        {
            Initialize();
            return ((_current.Major < platform.Major) || ((_current.Major == platform.Major) && (_current.Minor < platform.Minor)));
        }

        private static void SetFeatureData(PlatformFeature feature, object value)
        {
            lock (_features)
            {
                if (FindFeatureData(feature) == null)
                {
                    _features.Add(feature, value);
                }
            }
        }

        internal static bool Supports(PlatformFeature feature) => 
            ((bool) GetFeatureData(feature));

        internal static Version MTS
        {
            get
            {
                Initialize();
                return _mts;
            }
        }

        internal static Version W2K
        {
            get
            {
                Initialize();
                return _w2k;
            }
        }

        internal static Version Whistler
        {
            get
            {
                Initialize();
                return _whistler;
            }
        }
    }
}

