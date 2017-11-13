namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    internal class ServiceModelPerformanceCounters
    {
        private SortedList<string, string> actionToOperation;
        private System.ServiceModel.Diagnostics.DefaultPerformanceCounters defaultPerfCounters;
        private System.ServiceModel.Diagnostics.EndpointPerformanceCounters endpointPerfCounters;
        private bool initialized;
        private Dictionary<string, OperationPerformanceCounters> operationPerfCounters;
        private string perfCounterId;
        private System.ServiceModel.Diagnostics.ServicePerformanceCounters servicePerfCounters;

        internal ServiceModelPerformanceCounters(ServiceHostBase serviceHost, ContractDescription contractDescription, EndpointDispatcher endpointDispatcher)
        {
            this.perfCounterId = endpointDispatcher.PerfCounterId;
            if (PerformanceCounters.Scope == PerformanceCounterScope.All)
            {
                this.operationPerfCounters = new Dictionary<string, OperationPerformanceCounters>(contractDescription.Operations.Count);
                this.actionToOperation = new SortedList<string, string>(contractDescription.Operations.Count);
                foreach (OperationDescription description in contractDescription.Operations)
                {
                    OperationPerformanceCounters counters;
                    if ((description.Messages[0].Action != null) && !this.actionToOperation.Keys.Contains(description.Messages[0].Action))
                    {
                        this.actionToOperation.Add(description.Messages[0].Action, description.Name);
                    }
                    if (!this.operationPerfCounters.TryGetValue(description.Name, out counters))
                    {
                        OperationPerformanceCounters counters2 = new OperationPerformanceCounters(serviceHost.Description.Name, contractDescription.Name, description.Name, endpointDispatcher.PerfCounterBaseId);
                        if (counters2.Initialized)
                        {
                            this.operationPerfCounters.Add(description.Name, counters2);
                        }
                        else
                        {
                            this.ReleasePerformanceCounters();
                            this.initialized = false;
                            return;
                        }
                    }
                }
                System.ServiceModel.Diagnostics.EndpointPerformanceCounters counters3 = new System.ServiceModel.Diagnostics.EndpointPerformanceCounters(serviceHost.Description.Name, contractDescription.Name, endpointDispatcher.PerfCounterBaseId);
                if (counters3.Initialized)
                {
                    this.endpointPerfCounters = counters3;
                }
            }
            if (PerformanceCounters.PerformanceCountersEnabled)
            {
                this.servicePerfCounters = serviceHost.Counters;
            }
            if (PerformanceCounters.MinimalPerformanceCountersEnabled)
            {
                this.defaultPerfCounters = serviceHost.DefaultCounters;
            }
            this.initialized = true;
        }

        internal OperationPerformanceCounters GetOperationPerformanceCounters(string operation)
        {
            OperationPerformanceCounters counters;
            Dictionary<string, OperationPerformanceCounters> operationPerfCounters = this.operationPerfCounters;
            if ((operationPerfCounters != null) && operationPerfCounters.TryGetValue(operation, out counters))
            {
                return counters;
            }
            return null;
        }

        internal OperationPerformanceCounters GetOperationPerformanceCountersFromMessage(Message message)
        {
            string str;
            if (this.actionToOperation.TryGetValue(message.Headers.Action, out str))
            {
                return this.GetOperationPerformanceCounters(str);
            }
            return null;
        }

        internal void ReleasePerformanceCounters()
        {
            if (PerformanceCounters.Scope == PerformanceCounterScope.All)
            {
                if (this.operationPerfCounters != null)
                {
                    try
                    {
                        IDictionaryEnumerator enumerator = this.operationPerfCounters.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            ((OperationPerformanceCounters) enumerator.Value).ReleasePerformanceCounters();
                        }
                    }
                    catch (Exception exception)
                    {
                        if (ExceptionUtility.IsFatal(exception))
                        {
                            throw;
                        }
                        if (DiagnosticUtility.ShouldTraceError)
                        {
                            TraceUtility.TraceEvent(TraceEventType.Error, TraceCode.PerformanceCountersFailedOnRelease, null, exception);
                        }
                    }
                    this.operationPerfCounters.Clear();
                    this.operationPerfCounters = null;
                }
                if (this.endpointPerfCounters != null)
                {
                    this.endpointPerfCounters.ReleasePerformanceCounters();
                    this.endpointPerfCounters = null;
                }
            }
            this.servicePerfCounters = null;
            this.defaultPerfCounters = null;
        }

        internal System.ServiceModel.Diagnostics.DefaultPerformanceCounters DefaultPerformanceCounters =>
            this.defaultPerfCounters;

        internal System.ServiceModel.Diagnostics.EndpointPerformanceCounters EndpointPerformanceCounters =>
            this.endpointPerfCounters;

        internal bool Initialized =>
            this.initialized;

        internal string PerfCounterId =>
            this.perfCounterId;

        internal System.ServiceModel.Diagnostics.ServicePerformanceCounters ServicePerformanceCounters =>
            this.servicePerfCounters;
    }
}

