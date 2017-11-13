namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Administration;

    internal class ServicePerformanceCounters : PerformanceCountersBase
    {
        private PerformanceCounter[] counters;
        private const int hashLength = 2;
        private string instanceName;
        private const int maxCounterLength = 0x40;
        private string[] perfCounterNames = new string[] { 
            "Calls", "Calls Per Second", "Calls Outstanding", "Calls Failed", "Calls Failed Per Second", "Calls Faulted", "Calls Faulted Per Second", "Calls Duration", "Calls Duration Base", "Security Validation and Authentication Failures", "Security Validation and Authentication Failures Per Second", "Security Calls Not Authorized", "Security Calls Not Authorized Per Second", "Instances", "Instances Created Per Second", "Reliable Messaging Sessions Faulted",
            "Reliable Messaging Sessions Faulted Per Second", "Reliable Messaging Messages Dropped", "Reliable Messaging Messages Dropped Per Second", "Transactions Flowed", "Transactions Flowed Per Second", "Transacted Operations Committed", "Transacted Operations Committed Per Second", "Transacted Operations Aborted", "Transacted Operations Aborted Per Second", "Transacted Operations In Doubt", "Transacted Operations In Doubt Per Second", "Queued Poison Messages", "Queued Poison Messages Per Second", "Queued Messages Rejected", "Queued Messages Rejected Per Second", "Queued Messages Dropped",
            "Queued Messages Dropped Per Second"
        };

        internal ServicePerformanceCounters(ServiceHostBase serviceHost)
        {
            this.instanceName = CreateFriendlyInstanceName(serviceHost);
            this.counters = new PerformanceCounter[0x21];
            for (int i = 0; i < 0x21; i++)
            {
                PerformanceCounter servicePerformanceCounter = PerformanceCounters.GetServicePerformanceCounter(this.perfCounterNames[i], this.instanceName);
                if (servicePerformanceCounter == null)
                {
                    break;
                }
                try
                {
                    servicePerformanceCounter.RawValue = 0L;
                    this.counters[i] = servicePerformanceCounter;
                }
                catch (Exception exception)
                {
                    if (ExceptionUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    if (DiagnosticUtility.ShouldTraceError)
                    {
                        TraceUtility.TraceEvent(TraceEventType.Error, TraceCode.PerformanceCountersFailedForService, null, exception);
                    }
                    break;
                }
            }
        }

        internal void AuthenticationFailed()
        {
            base.Increment(9);
            base.Increment(10);
        }

        internal void AuthorizationFailed()
        {
            base.Increment(11);
            base.Increment(12);
        }

        internal static string CreateFriendlyInstanceName(ServiceHostBase serviceHost)
        {
            ServiceInfo info = new ServiceInfo(serviceHost);
            string serviceName = info.ServiceName;
            string firstAddress = info.FirstAddress;
            int totalLen = (serviceName.Length + firstAddress.Length) + 2;
            if (totalLen > 0x40)
            {
                int num2 = 0;
                truncOptions options = GetCompressionTasks(totalLen, serviceName.Length, firstAddress.Length);
                if ((options & (truncOptions.NoBits | truncOptions.service32)) > truncOptions.NoBits)
                {
                    num2 = 0x20;
                    serviceName = PerformanceCountersBase.GetHashedString(serviceName, num2 - 2, (serviceName.Length - num2) + 2, true);
                }
                if ((options & (truncOptions.NoBits | truncOptions.uri31)) > truncOptions.NoBits)
                {
                    num2 = 0x1f;
                    firstAddress = PerformanceCountersBase.GetHashedString(firstAddress, 0, (firstAddress.Length - num2) + 2, false);
                }
            }
            return (serviceName + "@" + firstAddress.Replace('/', '|'));
        }

        private static truncOptions GetCompressionTasks(int totalLen, int serviceLen, int uriLen)
        {
            truncOptions noBits = truncOptions.NoBits;
            if (totalLen > 0x40)
            {
                int num = totalLen;
                if ((num > 0x40) && (serviceLen > 0x20))
                {
                    noBits |= truncOptions.NoBits | truncOptions.service32;
                    num -= serviceLen - 0x20;
                }
                if ((num > 0x40) && (uriLen > 0x1f))
                {
                    noBits |= truncOptions.NoBits | truncOptions.uri31;
                }
            }
            return noBits;
        }

        internal void MessageDropped()
        {
            base.Increment(0x11);
            base.Increment(0x12);
        }

        internal void MethodCalled()
        {
            base.Increment(0);
            base.Increment(1);
            base.Increment(2);
        }

        internal void MethodReturnedError()
        {
            base.Increment(3);
            base.Increment(4);
            base.Decrement(2);
        }

        internal void MethodReturnedFault()
        {
            base.Increment(5);
            base.Increment(6);
            base.Decrement(2);
        }

        internal void MethodReturnedSuccess()
        {
            base.Decrement(2);
        }

        internal void MsmqDroppedMessage()
        {
            base.Increment(0x1f);
            base.Increment(0x20);
        }

        internal void MsmqPoisonMessage()
        {
            base.Increment(0x1b);
            base.Increment(0x1c);
        }

        internal void MsmqRejectedMessage()
        {
            base.Increment(0x1d);
            base.Increment(30);
        }

        internal void SaveCallDuration(long time)
        {
            base.IncrementBy(7, time);
            base.Increment(8);
        }

        internal void ServiceInstanceCreated()
        {
            base.Increment(13);
            base.Increment(14);
        }

        internal void ServiceInstanceRemoved()
        {
            base.Decrement(13);
        }

        internal void SessionFaulted()
        {
            base.Increment(15);
            base.Increment(0x10);
        }

        internal void TxAborted(long count)
        {
            base.IncrementBy(0x17, count);
            base.IncrementBy(0x18, count);
        }

        internal void TxCommitted(long count)
        {
            base.IncrementBy(0x15, count);
            base.IncrementBy(0x16, count);
        }

        internal void TxFlowed()
        {
            base.Increment(0x13);
            base.Increment(20);
        }

        internal void TxInDoubt(long count)
        {
            base.IncrementBy(0x19, count);
            base.IncrementBy(0x1a, count);
        }

        internal override string[] CounterNames =>
            this.perfCounterNames;

        internal override PerformanceCounter[] Counters
        {
            get => 
                this.counters;
            set
            {
                this.counters = value;
            }
        }

        internal bool Initialized =>
            (this.counters != null);

        internal override string InstanceName =>
            this.instanceName;

        internal override int PerfCounterEnd =>
            0x21;

        internal override int PerfCounterStart =>
            0;

        private enum PerfCounters
        {
            Calls,
            CallsPerSecond,
            CallsOutstanding,
            CallsFailed,
            CallsFailedPerSecond,
            CallsFaulted,
            CallsFaultedPerSecond,
            CallDuration,
            CallDurationBase,
            SecurityValidationAuthenticationFailures,
            SecurityValidationAuthenticationFailuresPerSecond,
            CallsNotAuthorized,
            CallsNotAuthorizedPerSecond,
            Instances,
            InstancesRate,
            RMSessionsFaulted,
            RMSessionsFaultedPerSecond,
            RMMessagesDropped,
            RMMessagesDroppedPerSecond,
            TxFlowed,
            TxFlowedPerSecond,
            TxCommitted,
            TxCommittedPerSecond,
            TxAborted,
            TxAbortedPerSecond,
            TxInDoubt,
            TxInDoubtPerSecond,
            MsmqPoisonMessages,
            MsmqPoisonMessagesPerSecond,
            MsmqRejectedMessages,
            MsmqRejectedMessagesPerSecond,
            MsmqDroppedMessages,
            MsmqDroppedMessagesPerSecond,
            TotalCounters
        }

        [Flags]
        private enum truncOptions : uint
        {
            NoBits = 0,
            service32 = 1,
            uri31 = 4
        }
    }
}

