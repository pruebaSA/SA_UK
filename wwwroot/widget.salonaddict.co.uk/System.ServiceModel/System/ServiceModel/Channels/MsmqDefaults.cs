namespace System.ServiceModel.Channels
{
    using System;
    using System.Net.Security;
    using System.ServiceModel;

    internal static class MsmqDefaults
    {
        internal const Uri CustomDeadLetterQueue = null;
        internal const System.ServiceModel.DeadLetterQueue DeadLetterQueue = System.ServiceModel.DeadLetterQueue.System;
        internal const bool Durable = true;
        internal const bool ExactlyOnce = true;
        internal const int MaxPoolSize = 8;
        internal const int MaxRetryCycles = 2;
        internal const System.ServiceModel.MsmqAuthenticationMode MsmqAuthenticationMode = System.ServiceModel.MsmqAuthenticationMode.WindowsDomain;
        internal const System.ServiceModel.MsmqEncryptionAlgorithm MsmqEncryptionAlgorithm = System.ServiceModel.MsmqEncryptionAlgorithm.RC4Stream;
        internal const ProtectionLevel MsmqProtectionLevel = ProtectionLevel.Sign;
        internal const System.ServiceModel.MsmqSecureHashAlgorithm MsmqSecureHashAlgorithm = System.ServiceModel.MsmqSecureHashAlgorithm.Sha1;
        internal const System.ServiceModel.QueueTransferProtocol QueueTransferProtocol = System.ServiceModel.QueueTransferProtocol.Native;
        internal const System.ServiceModel.ReceiveErrorHandling ReceiveErrorHandling = System.ServiceModel.ReceiveErrorHandling.Fault;
        internal const int ReceiveRetryCount = 5;
        internal const string RetryCycleDelayString = "00:30:00";
        internal const string TimeToLiveString = "1.00:00:00";
        internal const bool UseActiveDirectory = false;
        internal const bool UseMsmqTracing = false;
        internal const bool UseSourceJournal = false;

        internal static TimeSpan RetryCycleDelay =>
            TimeSpanHelper.FromMinutes(30, "00:30:00");

        internal static TimeSpan TimeToLive =>
            TimeSpanHelper.FromDays(1, "1.00:00:00");
    }
}

