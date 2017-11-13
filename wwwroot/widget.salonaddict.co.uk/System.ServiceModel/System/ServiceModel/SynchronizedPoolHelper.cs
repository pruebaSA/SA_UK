namespace System.ServiceModel
{
    using System;
    using System.Security;
    using System.Security.Permissions;

    internal static class SynchronizedPoolHelper
    {
        public static readonly int ProcessorCount = GetProcessorCount();

        [SecurityCritical, SecurityTreatAsSafe, EnvironmentPermission(SecurityAction.Assert, Read="NUMBER_OF_PROCESSORS")]
        private static int GetProcessorCount() => 
            Environment.ProcessorCount;
    }
}

