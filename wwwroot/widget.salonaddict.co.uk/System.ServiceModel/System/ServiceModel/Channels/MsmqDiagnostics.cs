namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Xml;

    internal static class MsmqDiagnostics
    {
        private static bool poolFullReported;

        public static ServiceModelActivity BoundDecodeOperation()
        {
            ServiceModelActivity activity = null;
            if (DiagnosticUtility.ShouldUseActivity)
            {
                activity = ServiceModelActivity.CreateBoundedActivity(true);
                ServiceModelActivity.Start(activity, System.ServiceModel.SR.GetString("ActivityProcessingMessage", new object[] { TraceUtility.RetrieveMessageNumber() }), ActivityType.ProcessMessage);
            }
            return activity;
        }

        public static Activity BoundOpenOperation(MsmqReceiveHelper receiver)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.TransportListen, System.ServiceModel.SR.GetString("TraceCodeTransportListen", new object[] { receiver.ListenUri.ToString() }), null, null, receiver);
            }
            return ServiceModelActivity.BoundOperation(receiver.Activity);
        }

        public static ServiceModelActivity BoundReceiveBytesOperation()
        {
            ServiceModelActivity activity = null;
            if (DiagnosticUtility.ShouldUseActivity)
            {
                activity = ServiceModelActivity.CreateBoundedActivityWithTransferInOnly(Guid.NewGuid());
                ServiceModelActivity.Start(activity, System.ServiceModel.SR.GetString("ActivityReceiveBytes", new object[] { TraceUtility.RetrieveMessageNumber() }), ActivityType.ReceiveBytes);
            }
            return activity;
        }

        public static Activity BoundReceiveOperation(MsmqReceiveHelper receiver)
        {
            if ((DiagnosticUtility.ShouldUseActivity && (ServiceModelActivity.Current != null)) && (ActivityType.ProcessAction != ServiceModelActivity.Current.ActivityType))
            {
                return ServiceModelActivity.BoundOperation(receiver.Activity);
            }
            return null;
        }

        public static void CannotPeekOnQueue(string formatName, Exception ex)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.MsmqCannotPeekOnQueue, new StringTraceRecord("QueueFormatName", formatName), null, ex);
            }
        }

        public static void CannotReadQueues(string host, bool publicQueues, Exception ex)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>(2) {
                    ["Host"] = host,
                    ["PublicQueues"] = Convert.ToString(publicQueues, CultureInfo.InvariantCulture)
                };
                TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.MsmqCannotReadQueues, new DictionaryTraceRecord(dictionary), null, ex);
            }
        }

        public static void DatagramReceived(NativeMsmqMessage.BufferProperty messageId, Message message)
        {
            DatagramSentOrReceived(messageId, message, TraceCode.MsmqDatagramReceived);
        }

        public static void DatagramSent(NativeMsmqMessage.BufferProperty messageId, Message message)
        {
            DatagramSentOrReceived(messageId, message, TraceCode.MsmqDatagramSent);
        }

        private static void DatagramSentOrReceived(NativeMsmqMessage.BufferProperty messageId, Message message, TraceCode code)
        {
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                Guid guid = MessageIdToGuid(messageId);
                UniqueId id = message.Headers.MessageId;
                TraceRecord extendedData = null;
                if (null == id)
                {
                    extendedData = new StringTraceRecord("MSMQMessageId", guid.ToString());
                }
                else
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>(2) {
                        ["MSMQMessageId"] = guid.ToString(),
                        ["WCFMessageId"] = id.ToString()
                    };
                    extendedData = new DictionaryTraceRecord(dictionary);
                }
                TraceUtility.TraceEvent(TraceEventType.Verbose, code, extendedData, null, null);
            }
        }

        public static void ExpectedException(Exception ex)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.ExceptionUtility.TraceHandledException(ex, TraceEventType.Information);
            }
        }

        public static void FoundBaseAddress(Uri uri, string virtualPath)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>(2) {
                    ["Uri"] = uri.ToString(),
                    ["VirtualPath"] = virtualPath
                };
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.MsmqFoundBaseAddress, new DictionaryTraceRecord(dictionary), null, null);
            }
        }

        public static void MatchedApplicationFound(string host, string queueName, bool isPrivate, string canonicalPath)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>(4) {
                    ["Host"] = host,
                    ["QueueName"] = queueName,
                    ["Private"] = Convert.ToString(isPrivate, CultureInfo.InvariantCulture),
                    ["CanonicalPath"] = canonicalPath
                };
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.MsmqMatchedApplicationFound, new DictionaryTraceRecord(dictionary), null, null);
            }
        }

        public static void MessageConsumed(string uri, string messageId, bool rejected)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                TraceUtility.TraceEvent(TraceEventType.Warning, rejected ? TraceCode.MsmqMessageRejected : TraceCode.MsmqMessageDropped, new StringTraceRecord("MSMQMessageId", messageId), null, null);
            }
            if (PerformanceCounters.PerformanceCountersEnabled)
            {
                if (rejected)
                {
                    PerformanceCounters.MsmqRejectedMessage(uri);
                }
                else
                {
                    PerformanceCounters.MsmqDroppedMessage(uri);
                }
            }
        }

        private static Guid MessageIdToGuid(NativeMsmqMessage.BufferProperty messageId)
        {
            int length = messageId.Buffer.Length;
            byte[] dst = new byte[0x10];
            System.Buffer.BlockCopy(messageId.Buffer, 4, dst, 0, 0x10);
            return new Guid(dst);
        }

        public static void MessageLockedUnderTheTransaction(long lookupId)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.MsmqMessageLockedUnderTheTransaction, new StringTraceRecord("MSMQMessageLookupId", Convert.ToString(lookupId, CultureInfo.InvariantCulture)), null, null);
            }
        }

        public static void MoveOrDeleteAttemptFailed(long lookupId)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.MsmqMoveOrDeleteAttemptFailed, new StringTraceRecord("MSMQMessageLookupId", Convert.ToString(lookupId, CultureInfo.InvariantCulture)), null, null);
            }
        }

        public static void MsmqDetected(Version version)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.MsmqDetected, new StringTraceRecord("MSMQVersion", version.ToString()), null, null);
            }
        }

        public static void PoisonMessageMoved(string messageId, bool poisonQueue, string uri)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                TraceUtility.TraceEvent(TraceEventType.Warning, poisonQueue ? TraceCode.MsmqPoisonMessageMovedPoison : TraceCode.MsmqPoisonMessageMovedRetry, new StringTraceRecord("MSMQMessageId", messageId), null, null);
            }
            if (poisonQueue && PerformanceCounters.PerformanceCountersEnabled)
            {
                PerformanceCounters.MsmqPoisonMessage(uri);
            }
        }

        public static void PoisonMessageRejected(string messageId, string uri)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.MsmqPoisonMessageRejected, new StringTraceRecord("MSMQMessageId", messageId), null, null);
            }
            if (PerformanceCounters.PerformanceCountersEnabled)
            {
                PerformanceCounters.MsmqPoisonMessage(uri);
            }
        }

        public static void PoolFull(int poolSize)
        {
            if (DiagnosticUtility.ShouldTraceInformation && !poolFullReported)
            {
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.MsmqPoolFull, null, null, null);
                poolFullReported = true;
            }
        }

        public static void PotentiallyPoisonMessageDetected(string messageId)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.MsmqPotentiallyPoisonMessageDetected, new StringTraceRecord("MSMQMessageId", messageId), null, null);
            }
        }

        public static void QueueClosed(string formatName)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.MsmqQueueClosed, new StringTraceRecord("FormatName", formatName), null, null);
            }
        }

        public static void QueueOpened(string formatName)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.MsmqQueueOpened, new StringTraceRecord("FormatName", formatName), null, null);
            }
        }

        public static void QueueTransactionalStatusUnknown(string formatName)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.MsmqQueueTransactionalStatusUnknown, new StringTraceRecord("FormatName", formatName), null, null);
            }
        }

        public static void ScanStarted()
        {
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.MsmqScanStarted, null, null, null);
            }
        }

        public static void SessiongramReceived(string sessionId, NativeMsmqMessage.BufferProperty messageId, int numberOfMessages)
        {
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>(3) {
                    ["SessionId"] = sessionId,
                    ["MSMQMessageId"] = MsmqMessageId.ToString(messageId.Buffer),
                    ["NumberOfMessages"] = Convert.ToString(numberOfMessages, CultureInfo.InvariantCulture)
                };
                TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.MsmqSessiongramReceived, new DictionaryTraceRecord(dictionary), null, null);
            }
        }

        public static void SessiongramSent(string sessionId, NativeMsmqMessage.BufferProperty messageId, int numberOfMessages)
        {
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>(3) {
                    ["SessionId"] = sessionId,
                    ["MSMQMessageId"] = MsmqMessageId.ToString(messageId.Buffer),
                    ["NumberOfMessages"] = Convert.ToString(numberOfMessages, CultureInfo.InvariantCulture)
                };
                TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.MsmqSessiongramSent, new DictionaryTraceRecord(dictionary), null, null);
            }
        }

        public static void StartingApplication(string application)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.MsmqStartingApplication, new StringTraceRecord("Application", application), null, null);
            }
        }

        public static void StartingService(string host, string name, bool isPrivate, string processedVirtualPath)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>(4) {
                    ["Host"] = host,
                    ["Name"] = name,
                    ["Private"] = Convert.ToString(isPrivate, CultureInfo.InvariantCulture),
                    ["VirtualPath"] = processedVirtualPath
                };
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.MsmqStartingService, new DictionaryTraceRecord(dictionary), null, null);
            }
        }

        public static ServiceModelActivity StartListenAtActivity(MsmqReceiveHelper receiver)
        {
            ServiceModelActivity activity = receiver.Activity;
            if (DiagnosticUtility.ShouldUseActivity && (activity == null))
            {
                activity = ServiceModelActivity.CreateActivity(true);
                DiagnosticUtility.DiagnosticTrace.TraceTransfer(activity.Id);
                ServiceModelActivity.Start(activity, System.ServiceModel.SR.GetString("ActivityListenAt", new object[] { receiver.ListenUri.ToString() }), ActivityType.ListenAt);
            }
            return activity;
        }

        public static void TransferFromTransport(Message message)
        {
            if (DiagnosticUtility.ShouldUseActivity)
            {
                TraceUtility.TransferFromTransport(message);
            }
        }

        public static void UnexpectedAcknowledgment(string messageId, int acknowledgment)
        {
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>(2) {
                    ["MSMQMessageId"] = messageId,
                    ["Acknowledgment"] = Convert.ToString(acknowledgment, CultureInfo.InvariantCulture)
                };
                TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.MsmqUnexpectedAcknowledgment, new DictionaryTraceRecord(dictionary), null, null);
            }
        }
    }
}

