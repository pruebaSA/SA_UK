namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;

    internal class EndpointPerformanceCounters : PerformanceCountersBase
    {
        private PerformanceCounter[] counters;
        private const int hashLength = 2;
        private string instanceName;
        private const int maxCounterLength = 0x40;
        private string[] perfCounterNames = new string[] { 
            "Calls", "Calls Per Second", "Calls Outstanding", "Calls Failed", "Calls Failed Per Second", "Calls Faulted", "Calls Faulted Per Second", "Calls Duration", "Calls Duration Base", "Security Validation and Authentication Failures", "Security Validation and Authentication Failures Per Second", "Security Calls Not Authorized", "Security Calls Not Authorized Per Second", "Reliable Messaging Sessions Faulted", "Reliable Messaging Sessions Faulted Per Second", "Reliable Messaging Messages Dropped",
            "Reliable Messaging Messages Dropped Per Second", "Transactions Flowed", "Transactions Flowed Per Second"
        };

        internal EndpointPerformanceCounters(string service, string contract, string uri)
        {
            this.instanceName = CreateFriendlyInstanceName(service, contract, uri);
            this.counters = new PerformanceCounter[0x13];
            for (int i = 0; i < 0x13; i++)
            {
                PerformanceCounter endpointPerformanceCounter = PerformanceCounters.GetEndpointPerformanceCounter(this.perfCounterNames[i], this.instanceName);
                if (endpointPerformanceCounter == null)
                {
                    break;
                }
                try
                {
                    endpointPerformanceCounter.RawValue = 0L;
                    this.counters[i] = endpointPerformanceCounter;
                }
                catch (Exception exception)
                {
                    if (ExceptionUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    if (DiagnosticUtility.ShouldTraceError)
                    {
                        TraceUtility.TraceEvent(TraceEventType.Error, TraceCode.PerformanceCounterFailedToLoad, null, exception);
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

        internal static string CreateFriendlyInstanceName(string service, string contract, string uri)
        {
            int totalLen = ((service.Length + contract.Length) + uri.Length) + 2;
            if (totalLen > 0x40)
            {
                int num2 = 0;
                truncOptions options = GetCompressionTasks(totalLen, service.Length, contract.Length, uri.Length);
                if ((options & (truncOptions.NoBits | truncOptions.service15)) > truncOptions.NoBits)
                {
                    num2 = 15;
                    service = PerformanceCountersBase.GetHashedString(service, num2 - 2, (service.Length - num2) + 2, true);
                }
                if ((options & truncOptions.contract16) > truncOptions.NoBits)
                {
                    num2 = 0x10;
                    contract = PerformanceCountersBase.GetHashedString(contract, num2 - 2, (contract.Length - num2) + 2, true);
                }
                if ((options & (truncOptions.NoBits | truncOptions.uri31)) > truncOptions.NoBits)
                {
                    num2 = 0x1f;
                    uri = PerformanceCountersBase.GetHashedString(uri, 0, (uri.Length - num2) + 2, false);
                }
            }
            return (service + "." + contract + "@" + uri.Replace('/', '|'));
        }

        private static truncOptions GetCompressionTasks(int totalLen, int serviceLen, int contractLen, int uriLen)
        {
            truncOptions noBits = truncOptions.NoBits;
            if (totalLen > 0x40)
            {
                int num = totalLen;
                if ((num > 0x40) && (serviceLen > 15))
                {
                    noBits |= truncOptions.NoBits | truncOptions.service15;
                    num -= serviceLen - 15;
                }
                if ((num > 0x40) && (contractLen > 0x10))
                {
                    noBits |= truncOptions.contract16;
                    num -= contractLen - 0x10;
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
            base.Increment(15);
            base.Increment(0x10);
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

        internal void SaveCallDuration(long time)
        {
            base.IncrementBy(7, time);
            base.Increment(8);
        }

        internal void SessionFaulted()
        {
            base.Increment(13);
            base.Increment(14);
        }

        internal void TxFlowed()
        {
            base.Increment(0x11);
            base.Increment(0x12);
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
            0x13;

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
            RMSessionsFaulted,
            RMSessionsFaultedPerSecond,
            RMMessagesDropped,
            RMMessagesDroppedPerSecond,
            TxFlowed,
            TxFlowedPerSecond,
            TotalCounters
        }

        [Flags]
        private enum truncOptions : uint
        {
            contract16 = 2,
            NoBits = 0,
            service15 = 1,
            uri31 = 4
        }
    }
}

