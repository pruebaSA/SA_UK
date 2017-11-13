namespace MS.Internal
{
    using System;

    internal static class Utilities
    {
        private static readonly Version _osVersion = Environment.OSVersion.Version;

        internal static bool IsOSVistaOrNewer =>
            (_osVersion >= new Version(6, 0));
    }
}

