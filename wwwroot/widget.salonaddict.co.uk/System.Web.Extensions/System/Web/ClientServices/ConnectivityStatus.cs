namespace System.Web.ClientServices
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    public static class ConnectivityStatus
    {
        private static bool _IsOffline;
        private static bool _IsOfflineFetched;

        private static void FetchIsOffline()
        {
            _IsOffline = File.Exists(Path.Combine(Application.UserAppDataPath, "AppIsOffline"));
            _IsOfflineFetched = true;
        }

        private static void StoreIsOffline()
        {
            string path = Path.Combine(Application.UserAppDataPath, "AppIsOffline");
            if (!_IsOffline)
            {
                File.Delete(path);
            }
            else
            {
                using (FileStream stream = File.Create(path))
                {
                    stream.Write(new byte[0], 0, 0);
                }
            }
        }

        public static bool IsOffline
        {
            get
            {
                if (!_IsOfflineFetched)
                {
                    FetchIsOffline();
                }
                return _IsOffline;
            }
            set
            {
                if (IsOffline != value)
                {
                    _IsOffline = value;
                    StoreIsOffline();
                }
            }
        }
    }
}

