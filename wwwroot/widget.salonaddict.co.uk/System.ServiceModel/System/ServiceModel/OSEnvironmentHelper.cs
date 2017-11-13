namespace System.ServiceModel
{
    using System;

    internal static class OSEnvironmentHelper
    {
        private const int VistaMajorVersion = 6;

        internal static bool IsVistaOrGreater =>
            (Environment.OSVersion.Version.Major >= 6);
    }
}

