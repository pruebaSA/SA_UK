﻿namespace System.ServiceModel.Channels
{
    using System;
    using System.Net.Security;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal static class Msmq
    {
        private static bool activeDirectoryEnabled;
        private static SafeLibraryHandle errorStrings = null;
        private static System.Version longhornVersion = new System.Version(4, 0);
        private static object staticLock = new object();
        private static UriPrefixTable<ITransportManagerRegistration> transportManagerTable = new UriPrefixTable<ITransportManagerRegistration>();
        private static System.Version version;
        private static object xpSendLock = null;

        static Msmq()
        {
            MsmqQueue.GetMsmqInformation(ref Msmq.version, ref activeDirectoryEnabled);
            MsmqDiagnostics.MsmqDetected(Msmq.version);
            System.Version version = Environment.OSVersion.Version;
            if ((version.Major == 5) && (version.Minor == 1))
            {
                xpSendLock = new object();
            }
        }

        internal static IPoisonHandlingStrategy CreatePoisonHandler(MsmqReceiveHelper receiver)
        {
            if (!receiver.Transactional)
            {
                return new MsmqNonTransactedPoisonHandler(receiver);
            }
            if (Version < longhornVersion)
            {
                return new Msmq3PoisonHandler(receiver);
            }
            if (receiver.ListenUri.AbsoluteUri.Contains(";"))
            {
                return new Msmq4SubqueuePoisonHandler(receiver);
            }
            return new Msmq4PoisonHandler(receiver);
        }

        internal static void EnterXPSendLock(out bool lockHeld, ProtectionLevel protectionLevel)
        {
            lockHeld = false;
            if ((xpSendLock != null) && (protectionLevel != ProtectionLevel.None))
            {
                try
                {
                }
                finally
                {
                    Monitor.Enter(xpSendLock);
                    lockHeld = true;
                }
            }
        }

        internal static void LeaveXPSendLock()
        {
            Monitor.Exit(xpSendLock);
        }

        internal static bool ActiveDirectoryEnabled =>
            activeDirectoryEnabled;

        internal static SafeLibraryHandle ErrorStrings
        {
            get
            {
                if (errorStrings == null)
                {
                    lock (staticLock)
                    {
                        if (errorStrings == null)
                        {
                            errorStrings = UnsafeNativeMethods.LoadLibrary("MQUTIL.DLL");
                        }
                    }
                }
                return errorStrings;
            }
        }

        internal static bool IsAdvancedPoisonHandlingSupported =>
            (Version >= longhornVersion);

        internal static bool IsPerAppDeadLetterQueueSupported =>
            (Version >= longhornVersion);

        internal static bool IsRejectMessageSupported =>
            (Version >= longhornVersion);

        internal static UriPrefixTable<ITransportManagerRegistration> StaticTransportManagerTable =>
            transportManagerTable;

        internal static System.Version Version =>
            version;
    }
}

