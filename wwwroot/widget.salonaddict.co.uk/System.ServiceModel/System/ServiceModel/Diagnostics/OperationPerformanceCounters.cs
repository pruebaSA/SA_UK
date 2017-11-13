namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;

    internal class OperationPerformanceCounters : PerformanceCountersBase
    {
        private PerformanceCounter[] counters;
        private const int hashLength = 2;
        private string instanceName;
        private const int maxCounterLength = 0x40;
        private string operationName;
        private string[] perfCounterNames = new string[] { "Calls", "Calls Per Second", "Calls Outstanding", "Calls Failed", "Call Failed Per Second", "Calls Faulted", "Calls Faulted Per Second", "Calls Duration", "Calls Duration Base", "Security Validation and Authentication Failures", "Security Validation and Authentication Failures Per Second", "Security Calls Not Authorized", "Security Calls Not Authorized Per Second", "Transactions Flowed", "Transactions Flowed Per Second" };

        internal OperationPerformanceCounters(string service, string contract, string operationName, string uri)
        {
            this.operationName = operationName;
            this.instanceName = CreateFriendlyInstanceName(service, contract, operationName, uri);
            this.counters = new PerformanceCounter[15];
            for (int i = 0; i < 15; i++)
            {
                PerformanceCounter operationPerformanceCounter = PerformanceCounters.GetOperationPerformanceCounter(this.perfCounterNames[i], this.instanceName);
                if (operationPerformanceCounter == null)
                {
                    break;
                }
                try
                {
                    operationPerformanceCounter.RawValue = 0L;
                    this.counters[i] = operationPerformanceCounter;
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

        internal static string CreateFriendlyInstanceName(string service, string contract, string operation, string uri)
        {
            int totalLen = (((service.Length + contract.Length) + operation.Length) + uri.Length) + 3;
            if (totalLen > 0x40)
            {
                int num2 = 0;
                truncOptions options = GetCompressionTasks(totalLen, service.Length, contract.Length, operation.Length, uri.Length);
                if ((options & (truncOptions.NoBits | truncOptions.service7)) > truncOptions.NoBits)
                {
                    num2 = 7;
                    service = PerformanceCountersBase.GetHashedString(service, num2 - 2, (service.Length - num2) + 2, true);
                }
                if ((options & truncOptions.contract7) > truncOptions.NoBits)
                {
                    num2 = 7;
                    contract = PerformanceCountersBase.GetHashedString(contract, num2 - 2, (contract.Length - num2) + 2, true);
                }
                if ((options & (truncOptions.NoBits | truncOptions.operation15)) > truncOptions.NoBits)
                {
                    num2 = 15;
                    operation = PerformanceCountersBase.GetHashedString(operation, num2 - 2, (operation.Length - num2) + 2, true);
                }
                if ((options & (truncOptions.NoBits | truncOptions.uri32)) > truncOptions.NoBits)
                {
                    num2 = 0x20;
                    uri = PerformanceCountersBase.GetHashedString(uri, 0, (uri.Length - num2) + 2, false);
                }
            }
            return (service + "." + contract + "." + operation + "@" + uri.Replace('/', '|'));
        }

        private static truncOptions GetCompressionTasks(int totalLen, int serviceLen, int contractLen, int operationLen, int uriLen)
        {
            truncOptions noBits = truncOptions.NoBits;
            if (totalLen > 0x40)
            {
                int num = totalLen;
                if ((num > 0x40) && (serviceLen > 8))
                {
                    noBits |= truncOptions.NoBits | truncOptions.service7;
                    num -= serviceLen - 7;
                }
                if ((num > 0x40) && (contractLen > 7))
                {
                    noBits |= truncOptions.contract7;
                    num -= contractLen - 7;
                }
                if ((num > 0x40) && (operationLen > 15))
                {
                    noBits |= truncOptions.NoBits | truncOptions.operation15;
                    num -= operationLen - 15;
                }
                if ((num > 0x40) && (uriLen > 0x20))
                {
                    noBits |= truncOptions.NoBits | truncOptions.uri32;
                }
            }
            return noBits;
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

        internal void TxFlowed()
        {
            base.Increment(13);
            base.Increment(14);
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

        internal string OperationName =>
            this.operationName;

        internal override int PerfCounterEnd =>
            15;

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
            TxFlowed,
            TxFlowedPerSecond,
            TotalCounters
        }

        [Flags]
        private enum truncOptions : uint
        {
            contract7 = 2,
            NoBits = 0,
            operation15 = 4,
            service7 = 1,
            uri32 = 8
        }
    }
}

