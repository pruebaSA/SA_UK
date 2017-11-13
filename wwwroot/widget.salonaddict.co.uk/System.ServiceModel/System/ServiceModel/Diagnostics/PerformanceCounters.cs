namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    internal static class PerformanceCounters
    {
        private static bool endpointOOM = false;
        internal const int MaxInstanceNameLength = 0x7f;
        private static bool operationOOM = false;
        private static object perfCounterDictionarySyncObject = new object();
        private static Dictionary<string, ServiceModelPerformanceCounters> performanceCounters = null;
        private static Dictionary<string, ServiceModelPerformanceCountersEntry> performanceCountersBaseUri = null;
        private static PerformanceCounterScope scope = PerformanceCounterScope.Default;
        private static bool serviceOOM = false;

        static PerformanceCounters()
        {
            PerformanceCounterScope performanceCountersFromConfig = GetPerformanceCountersFromConfig();
            if (performanceCountersFromConfig != PerformanceCounterScope.Off)
            {
                try
                {
                    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(PerformanceCounters.ExitOrUnloadEventHandler);
                    AppDomain.CurrentDomain.DomainUnload += new EventHandler(PerformanceCounters.ExitOrUnloadEventHandler);
                    AppDomain.CurrentDomain.ProcessExit += new EventHandler(PerformanceCounters.ExitOrUnloadEventHandler);
                    scope = performanceCountersFromConfig;
                }
                catch (SecurityException exception)
                {
                    scope = PerformanceCounterScope.Off;
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Warning);
                        DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.PerformanceCounterFailedToLoad, System.ServiceModel.SR.GetString("PartialTrustPerformanceCountersNotEnabled"));
                    }
                }
            }
            else
            {
                scope = PerformanceCounterScope.Off;
            }
        }

        internal static void AddPerformanceCountersForEndpoint(ServiceHostBase serviceHost, ContractDescription contractDescription, EndpointDispatcher endpointDispatcher)
        {
            if ((PerformanceCountersEnabled || MinimalPerformanceCountersEnabled) && endpointDispatcher.SetPerfCounterId())
            {
                ServiceModelPerformanceCounters counters;
                lock (perfCounterDictionarySyncObject)
                {
                    if (!PerformanceCountersForEndpoint.TryGetValue(endpointDispatcher.PerfCounterId, out counters))
                    {
                        counters = new ServiceModelPerformanceCounters(serviceHost, contractDescription, endpointDispatcher);
                        if (counters.Initialized)
                        {
                            PerformanceCountersForEndpoint.Add(endpointDispatcher.PerfCounterId, counters);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                lock (perfCounterDictionarySyncObject)
                {
                    ServiceModelPerformanceCountersEntry entry;
                    if (!PerformanceCountersForBaseUri.TryGetValue(endpointDispatcher.PerfCounterBaseId, out entry))
                    {
                        if (PerformanceCountersEnabled)
                        {
                            entry = new ServiceModelPerformanceCountersEntry(serviceHost.Counters);
                        }
                        else if (MinimalPerformanceCountersEnabled)
                        {
                            entry = new ServiceModelPerformanceCountersEntry(serviceHost.DefaultCounters);
                        }
                        PerformanceCountersForBaseUri.Add(endpointDispatcher.PerfCounterBaseId, entry);
                    }
                    entry.Add(counters);
                }
            }
        }

        internal static void AuthenticationFailed(Message message, Uri listenUri)
        {
            CallOnAllCounters("AuthenticationFailed", message, listenUri, true);
        }

        internal static void AuthorizationFailed(string operationName)
        {
            EndpointDispatcher endpointDispatcher = GetEndpointDispatcher();
            if (endpointDispatcher != null)
            {
                string perfCounterId = endpointDispatcher.PerfCounterId;
                if (Scope == PerformanceCounterScope.All)
                {
                    OperationPerformanceCounters operationPerformanceCounters = GetOperationPerformanceCounters(perfCounterId, operationName);
                    if (operationPerformanceCounters != null)
                    {
                        operationPerformanceCounters.AuthorizationFailed();
                    }
                    EndpointPerformanceCounters endpointPerformanceCounters = GetEndpointPerformanceCounters(perfCounterId);
                    if (endpointPerformanceCounters != null)
                    {
                        endpointPerformanceCounters.AuthorizationFailed();
                    }
                }
                ServicePerformanceCounters servicePerformanceCounters = GetServicePerformanceCounters(endpointDispatcher.PerfCounterId);
                if (servicePerformanceCounters != null)
                {
                    servicePerformanceCounters.AuthorizationFailed();
                }
            }
        }

        private static void CallOnAllCounters(string methodName, Message message, Uri listenUri, bool includeOperations)
        {
            if (((message != null) && (message.Headers != null)) && ((null != message.Headers.To) && (null != listenUri)))
            {
                ServiceModelPerformanceCountersEntry serviceModelPerformanceCountersBaseUri = GetServiceModelPerformanceCountersBaseUri(listenUri.AbsoluteUri.ToUpperInvariant());
                if (serviceModelPerformanceCountersBaseUri != null)
                {
                    InvokeMethod(serviceModelPerformanceCountersBaseUri.ServicePerformanceCounters, methodName);
                    if (Scope == PerformanceCounterScope.All)
                    {
                        foreach (ServiceModelPerformanceCounters counters in serviceModelPerformanceCountersBaseUri.CounterList)
                        {
                            if (counters.EndpointPerformanceCounters != null)
                            {
                                InvokeMethod(counters.EndpointPerformanceCounters, methodName);
                            }
                            if (includeOperations)
                            {
                                OperationPerformanceCounters operationPerformanceCountersFromMessage = counters.GetOperationPerformanceCountersFromMessage(message);
                                if (operationPerformanceCountersFromMessage != null)
                                {
                                    InvokeMethod(operationPerformanceCountersFromMessage, methodName);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void ExitOrUnloadEventHandler(object sender, EventArgs e)
        {
            Dictionary<string, ServiceModelPerformanceCounters> performanceCounters = null;
            if (PerformanceCounters.performanceCounters != null)
            {
                lock (perfCounterDictionarySyncObject)
                {
                    if (PerformanceCounters.performanceCounters != null)
                    {
                        performanceCounters = PerformanceCounters.performanceCounters;
                        PerformanceCounters.performanceCounters = null;
                        performanceCountersBaseUri = null;
                    }
                }
            }
            if (performanceCounters != null)
            {
                IDictionaryEnumerator enumerator = performanceCounters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ServiceModelPerformanceCounters counters = (ServiceModelPerformanceCounters) enumerator.Value;
                    ServicePerformanceCounters servicePerformanceCounters = counters.ServicePerformanceCounters;
                    DefaultPerformanceCounters defaultPerformanceCounters = counters.DefaultPerformanceCounters;
                    counters.ReleasePerformanceCounters();
                    if (servicePerformanceCounters != null)
                    {
                        servicePerformanceCounters.ReleasePerformanceCounters();
                    }
                    if (defaultPerformanceCounters != null)
                    {
                        defaultPerformanceCounters.ReleasePerformanceCounters();
                    }
                }
                performanceCounters.Clear();
            }
        }

        internal static PerformanceCounter GetDefaultPerformanceCounter(string perfCounterName, string instanceName) => 
            GetPerformanceCounter("ServiceModelService 3.0.0.0", perfCounterName, instanceName, PerformanceCounterInstanceLifetime.Global);

        internal static EndpointDispatcher GetEndpointDispatcher()
        {
            EndpointDispatcher endpointDispatcher = null;
            if ((OperationContext.Current != null) && (OperationContext.Current.InternalServiceChannel != null))
            {
                endpointDispatcher = OperationContext.Current.EndpointDispatcher;
            }
            return endpointDispatcher;
        }

        internal static PerformanceCounter GetEndpointPerformanceCounter(string perfCounterName, string instanceName) => 
            GetPerformanceCounter("ServiceModelEndpoint 3.0.0.0", perfCounterName, instanceName, PerformanceCounterInstanceLifetime.Process);

        private static EndpointPerformanceCounters GetEndpointPerformanceCounters(string uri)
        {
            ServiceModelPerformanceCounters serviceModelPerformanceCounters = GetServiceModelPerformanceCounters(uri);
            if (serviceModelPerformanceCounters != null)
            {
                return serviceModelPerformanceCounters.EndpointPerformanceCounters;
            }
            return null;
        }

        internal static PerformanceCounter GetOperationPerformanceCounter(string perfCounterName, string instanceName) => 
            GetPerformanceCounter("ServiceModelOperation 3.0.0.0", perfCounterName, instanceName, PerformanceCounterInstanceLifetime.Process);

        private static OperationPerformanceCounters GetOperationPerformanceCounters(string uri, string operation)
        {
            ServiceModelPerformanceCounters serviceModelPerformanceCounters = GetServiceModelPerformanceCounters(uri);
            if (serviceModelPerformanceCounters != null)
            {
                return serviceModelPerformanceCounters.GetOperationPerformanceCounters(operation);
            }
            return null;
        }

        internal static PerformanceCounter GetPerformanceCounter(string categoryName, string perfCounterName, string instanceName, PerformanceCounterInstanceLifetime instanceLifetime)
        {
            PerformanceCounter counter = null;
            if (!PerformanceCountersEnabled && !MinimalPerformanceCountersEnabled)
            {
                return counter;
            }
            return GetPerformanceCounterInternal(categoryName, perfCounterName, instanceName, instanceLifetime);
        }

        internal static PerformanceCounter GetPerformanceCounterInternal(string categoryName, string perfCounterName, string instanceName, PerformanceCounterInstanceLifetime instanceLifetime)
        {
            PerformanceCounter counter = null;
            try
            {
                counter = new PerformanceCounter {
                    CategoryName = categoryName,
                    CounterName = perfCounterName,
                    InstanceName = instanceName,
                    ReadOnly = false,
                    InstanceLifetime = instanceLifetime
                };
                try
                {
                    long rawValue = counter.RawValue;
                }
                catch (InvalidOperationException)
                {
                    counter = null;
                    throw;
                }
                catch (SecurityException exception)
                {
                    if (MinimalPerformanceCountersEnabled)
                    {
                        scope = PerformanceCounterScope.Off;
                    }
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(new SecurityException(System.ServiceModel.SR.GetString("PartialTrustPerformanceCountersNotEnabled"), exception), TraceEventType.Warning);
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityException(System.ServiceModel.SR.GetString("PartialTrustPerformanceCountersNotEnabled")));
                }
            }
            catch (Exception exception2)
            {
                if (ExceptionUtility.IsFatal(exception2))
                {
                    throw;
                }
                if (counter != null)
                {
                    if (!counter.ReadOnly)
                    {
                        try
                        {
                            counter.RemoveInstance();
                        }
                        catch (Exception exception3)
                        {
                            if (ExceptionUtility.IsFatal(exception3))
                            {
                                throw;
                            }
                        }
                    }
                    counter = null;
                }
                bool flag = true;
                if (categoryName == "ServiceModelService 3.0.0.0")
                {
                    if (!serviceOOM)
                    {
                        serviceOOM = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else if (categoryName == "ServiceModelOperation 3.0.0.0")
                {
                    if (!operationOOM)
                    {
                        operationOOM = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else if (categoryName == "ServiceModelEndpoint 3.0.0.0")
                {
                    if (!endpointOOM)
                    {
                        endpointOOM = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    DiagnosticUtility.EventLog.LogEvent(TraceEventType.Error, EventLogCategory.PerformanceCounter, (EventLogEventId) (-1073610742), new string[] { categoryName, perfCounterName, exception2.ToString() });
                }
            }
            return counter;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private static PerformanceCounterScope GetPerformanceCountersFromConfig() => 
            DiagnosticSection.UnsafeGetSection().PerformanceCounters;

        private static ServiceModelPerformanceCounters GetServiceModelPerformanceCounters(string uri)
        {
            ServiceModelPerformanceCounters counters = null;
            if (!string.IsNullOrEmpty(uri))
            {
                PerformanceCountersForEndpoint.TryGetValue(uri, out counters);
            }
            return counters;
        }

        private static ServiceModelPerformanceCountersEntry GetServiceModelPerformanceCountersBaseUri(string uri)
        {
            ServiceModelPerformanceCountersEntry entry = null;
            if (!string.IsNullOrEmpty(uri))
            {
                PerformanceCountersForBaseUri.TryGetValue(uri, out entry);
            }
            return entry;
        }

        internal static PerformanceCounter GetServicePerformanceCounter(string perfCounterName, string instanceName) => 
            GetPerformanceCounter("ServiceModelService 3.0.0.0", perfCounterName, instanceName, PerformanceCounterInstanceLifetime.Process);

        private static ServicePerformanceCounters GetServicePerformanceCounters(string uri)
        {
            ServiceModelPerformanceCounters serviceModelPerformanceCounters = GetServiceModelPerformanceCounters(uri);
            if (serviceModelPerformanceCounters != null)
            {
                return serviceModelPerformanceCounters.ServicePerformanceCounters;
            }
            return null;
        }

        private static void InvokeMethod(object o, string methodName)
        {
            o.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance).Invoke(o, null);
        }

        internal static void MessageDropped(string uri)
        {
            ServiceModelPerformanceCountersEntry serviceModelPerformanceCountersBaseUri = GetServiceModelPerformanceCountersBaseUri(uri);
            if (serviceModelPerformanceCountersBaseUri != null)
            {
                serviceModelPerformanceCountersBaseUri.ServicePerformanceCounters.MessageDropped();
                if (Scope == PerformanceCounterScope.All)
                {
                    foreach (ServiceModelPerformanceCounters counters in serviceModelPerformanceCountersBaseUri.CounterList)
                    {
                        if (counters.EndpointPerformanceCounters != null)
                        {
                            counters.EndpointPerformanceCounters.MessageDropped();
                        }
                    }
                }
            }
        }

        internal static void MethodCalled(string operationName)
        {
            EndpointDispatcher endpointDispatcher = GetEndpointDispatcher();
            if (endpointDispatcher != null)
            {
                string perfCounterId = endpointDispatcher.PerfCounterId;
                if (Scope == PerformanceCounterScope.All)
                {
                    OperationPerformanceCounters operationPerformanceCounters = GetOperationPerformanceCounters(perfCounterId, operationName);
                    if (operationPerformanceCounters != null)
                    {
                        operationPerformanceCounters.MethodCalled();
                    }
                    EndpointPerformanceCounters endpointPerformanceCounters = GetEndpointPerformanceCounters(perfCounterId);
                    if (endpointPerformanceCounters != null)
                    {
                        endpointPerformanceCounters.MethodCalled();
                    }
                }
                ServicePerformanceCounters servicePerformanceCounters = GetServicePerformanceCounters(perfCounterId);
                if (servicePerformanceCounters != null)
                {
                    servicePerformanceCounters.MethodCalled();
                }
            }
        }

        internal static void MethodReturnedError(string operationName)
        {
            EndpointDispatcher endpointDispatcher = GetEndpointDispatcher();
            if (endpointDispatcher != null)
            {
                string perfCounterId = endpointDispatcher.PerfCounterId;
                if (Scope == PerformanceCounterScope.All)
                {
                    OperationPerformanceCounters operationPerformanceCounters = GetOperationPerformanceCounters(perfCounterId, operationName);
                    if (operationPerformanceCounters != null)
                    {
                        operationPerformanceCounters.MethodReturnedError();
                    }
                    EndpointPerformanceCounters endpointPerformanceCounters = GetEndpointPerformanceCounters(perfCounterId);
                    if (endpointPerformanceCounters != null)
                    {
                        endpointPerformanceCounters.MethodReturnedError();
                    }
                }
                ServicePerformanceCounters servicePerformanceCounters = GetServicePerformanceCounters(endpointDispatcher.PerfCounterId);
                if (servicePerformanceCounters != null)
                {
                    servicePerformanceCounters.MethodReturnedError();
                }
            }
        }

        internal static void MethodReturnedFault(string operationName)
        {
            EndpointDispatcher endpointDispatcher = GetEndpointDispatcher();
            if (endpointDispatcher != null)
            {
                string perfCounterId = endpointDispatcher.PerfCounterId;
                if (Scope == PerformanceCounterScope.All)
                {
                    OperationPerformanceCounters operationPerformanceCounters = GetOperationPerformanceCounters(perfCounterId, operationName);
                    if (operationPerformanceCounters != null)
                    {
                        operationPerformanceCounters.MethodReturnedFault();
                    }
                    EndpointPerformanceCounters endpointPerformanceCounters = GetEndpointPerformanceCounters(perfCounterId);
                    if (endpointPerformanceCounters != null)
                    {
                        endpointPerformanceCounters.MethodReturnedFault();
                    }
                }
                ServicePerformanceCounters servicePerformanceCounters = GetServicePerformanceCounters(endpointDispatcher.PerfCounterId);
                if (servicePerformanceCounters != null)
                {
                    servicePerformanceCounters.MethodReturnedFault();
                }
            }
        }

        internal static void MethodReturnedSuccess(string operationName)
        {
            MethodReturnedSuccess(operationName, -1L);
        }

        internal static void MethodReturnedSuccess(string operationName, long time)
        {
            EndpointDispatcher endpointDispatcher = GetEndpointDispatcher();
            if (endpointDispatcher != null)
            {
                string perfCounterId = endpointDispatcher.PerfCounterId;
                if (Scope == PerformanceCounterScope.All)
                {
                    OperationPerformanceCounters operationPerformanceCounters = GetOperationPerformanceCounters(perfCounterId, operationName);
                    if (operationPerformanceCounters != null)
                    {
                        operationPerformanceCounters.MethodReturnedSuccess();
                        if (time > 0L)
                        {
                            operationPerformanceCounters.SaveCallDuration(time);
                        }
                    }
                    EndpointPerformanceCounters endpointPerformanceCounters = GetEndpointPerformanceCounters(perfCounterId);
                    if (endpointPerformanceCounters != null)
                    {
                        endpointPerformanceCounters.MethodReturnedSuccess();
                        if (time > 0L)
                        {
                            endpointPerformanceCounters.SaveCallDuration(time);
                        }
                    }
                }
                ServicePerformanceCounters servicePerformanceCounters = GetServicePerformanceCounters(endpointDispatcher.PerfCounterId);
                if (servicePerformanceCounters != null)
                {
                    servicePerformanceCounters.MethodReturnedSuccess();
                    if (time > 0L)
                    {
                        servicePerformanceCounters.SaveCallDuration(time);
                    }
                }
            }
        }

        internal static void MsmqDroppedMessage(string uri)
        {
            if (Scope == PerformanceCounterScope.All)
            {
                ServiceModelPerformanceCountersEntry serviceModelPerformanceCountersBaseUri = GetServiceModelPerformanceCountersBaseUri(uri);
                if (serviceModelPerformanceCountersBaseUri != null)
                {
                    serviceModelPerformanceCountersBaseUri.ServicePerformanceCounters.MsmqDroppedMessage();
                }
            }
        }

        internal static void MsmqPoisonMessage(string uri)
        {
            if (Scope == PerformanceCounterScope.All)
            {
                ServiceModelPerformanceCountersEntry serviceModelPerformanceCountersBaseUri = GetServiceModelPerformanceCountersBaseUri(uri);
                if (serviceModelPerformanceCountersBaseUri != null)
                {
                    serviceModelPerformanceCountersBaseUri.ServicePerformanceCounters.MsmqPoisonMessage();
                }
            }
        }

        internal static void MsmqRejectedMessage(string uri)
        {
            if (Scope == PerformanceCounterScope.All)
            {
                ServiceModelPerformanceCountersEntry serviceModelPerformanceCountersBaseUri = GetServiceModelPerformanceCountersBaseUri(uri);
                if (serviceModelPerformanceCountersBaseUri != null)
                {
                    serviceModelPerformanceCountersBaseUri.ServicePerformanceCounters.MsmqRejectedMessage();
                }
            }
        }

        internal static void ReleasePerformanceCounter(ref PerformanceCounter counter)
        {
            if (counter != null)
            {
                try
                {
                    counter.RemoveInstance();
                    counter = null;
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                }
            }
        }

        internal static void ReleasePerformanceCountersForEndpoint(string id, string baseId)
        {
            if (PerformanceCountersEnabled)
            {
                lock (perfCounterDictionarySyncObject)
                {
                    ServiceModelPerformanceCounters counters;
                    ServiceModelPerformanceCountersEntry entry;
                    if (!string.IsNullOrEmpty(id) && PerformanceCountersForEndpoint.TryGetValue(id, out counters))
                    {
                        counters.ReleasePerformanceCounters();
                        PerformanceCountersForEndpoint.Remove(id);
                    }
                    if (!string.IsNullOrEmpty(baseId) && PerformanceCountersForBaseUri.TryGetValue(baseId, out entry))
                    {
                        entry.Remove(id);
                    }
                }
            }
        }

        internal static void ReleasePerformanceCountersForService(PerformanceCountersBase counters, bool isDefaultCounters)
        {
            if ((PerformanceCountersEnabled || MinimalPerformanceCountersEnabled) && (counters != null))
            {
                List<string> list = null;
                bool flag = false;
                lock (perfCounterDictionarySyncObject)
                {
                    try
                    {
                        IDictionaryEnumerator enumerator = PerformanceCountersForBaseUri.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            ServiceModelPerformanceCountersEntry entry = (ServiceModelPerformanceCountersEntry) enumerator.Value;
                            PerformanceCountersBase defaultPerformanceCounters = null;
                            if (isDefaultCounters)
                            {
                                defaultPerformanceCounters = entry.DefaultPerformanceCounters;
                            }
                            else
                            {
                                defaultPerformanceCounters = entry.ServicePerformanceCounters;
                            }
                            if ((defaultPerformanceCounters != null) && (defaultPerformanceCounters.InstanceName == counters.InstanceName))
                            {
                                if (list == null)
                                {
                                    list = new List<string>();
                                }
                                list.Add((string) enumerator.Key);
                                if (!flag)
                                {
                                    counters.ReleasePerformanceCounters();
                                    flag = true;
                                }
                            }
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
                    if (list != null)
                    {
                        foreach (string str in list)
                        {
                            PerformanceCountersForBaseUri.Remove(str);
                        }
                    }
                }
            }
        }

        internal static void SessionFaulted(string uri)
        {
            ServiceModelPerformanceCountersEntry serviceModelPerformanceCountersBaseUri = GetServiceModelPerformanceCountersBaseUri(uri);
            if (serviceModelPerformanceCountersBaseUri != null)
            {
                serviceModelPerformanceCountersBaseUri.ServicePerformanceCounters.SessionFaulted();
                if (Scope == PerformanceCounterScope.All)
                {
                    foreach (ServiceModelPerformanceCounters counters in serviceModelPerformanceCountersBaseUri.CounterList)
                    {
                        if (counters.EndpointPerformanceCounters != null)
                        {
                            counters.EndpointPerformanceCounters.SessionFaulted();
                        }
                    }
                }
            }
        }

        internal static void TracePerformanceCounterUpdateFailure(string instanceName, string perfCounterName)
        {
            if (DiagnosticUtility.ShouldTraceError)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Error, TraceCode.PerformanceCountersFailedDuringUpdate, System.ServiceModel.SR.GetString("TraceCodePerformanceCountersFailedDuringUpdate", new object[] { perfCounterName + "::" + instanceName }));
            }
        }

        internal static void TxAborted(EndpointDispatcher el, long count)
        {
            if (PerformanceCountersEnabled && (el != null))
            {
                ServicePerformanceCounters servicePerformanceCounters = GetServicePerformanceCounters(el.PerfCounterId);
                if (servicePerformanceCounters != null)
                {
                    servicePerformanceCounters.TxAborted(count);
                }
            }
        }

        internal static void TxCommitted(EndpointDispatcher el, long count)
        {
            if (PerformanceCountersEnabled && (el != null))
            {
                ServicePerformanceCounters servicePerformanceCounters = GetServicePerformanceCounters(el.PerfCounterId);
                if (servicePerformanceCounters != null)
                {
                    servicePerformanceCounters.TxCommitted(count);
                }
            }
        }

        internal static void TxFlowed(EndpointDispatcher el, string operation)
        {
            if (el != null)
            {
                ServicePerformanceCounters servicePerformanceCounters = GetServicePerformanceCounters(el.PerfCounterId);
                if (servicePerformanceCounters != null)
                {
                    servicePerformanceCounters.TxFlowed();
                }
                if (Scope == PerformanceCounterScope.All)
                {
                    OperationPerformanceCounters operationPerformanceCounters = GetOperationPerformanceCounters(el.PerfCounterId, operation);
                    if (operationPerformanceCounters != null)
                    {
                        operationPerformanceCounters.TxFlowed();
                    }
                    EndpointPerformanceCounters endpointPerformanceCounters = GetEndpointPerformanceCounters(el.PerfCounterId);
                    if (servicePerformanceCounters != null)
                    {
                        endpointPerformanceCounters.TxFlowed();
                    }
                }
            }
        }

        internal static void TxInDoubt(EndpointDispatcher el, long count)
        {
            if (PerformanceCountersEnabled && (el != null))
            {
                ServicePerformanceCounters servicePerformanceCounters = GetServicePerformanceCounters(el.PerfCounterId);
                if (servicePerformanceCounters != null)
                {
                    servicePerformanceCounters.TxInDoubt(count);
                }
            }
        }

        internal static bool MinimalPerformanceCountersEnabled =>
            (scope == PerformanceCounterScope.Default);

        internal static bool PerformanceCountersEnabled =>
            ((scope != PerformanceCounterScope.Off) && (scope != PerformanceCounterScope.Default));

        internal static Dictionary<string, ServiceModelPerformanceCountersEntry> PerformanceCountersForBaseUri
        {
            get
            {
                if (performanceCountersBaseUri == null)
                {
                    lock (perfCounterDictionarySyncObject)
                    {
                        if (performanceCountersBaseUri == null)
                        {
                            performanceCountersBaseUri = new Dictionary<string, ServiceModelPerformanceCountersEntry>();
                        }
                    }
                }
                return performanceCountersBaseUri;
            }
        }

        internal static Dictionary<string, ServiceModelPerformanceCounters> PerformanceCountersForEndpoint
        {
            get
            {
                if (performanceCounters == null)
                {
                    lock (perfCounterDictionarySyncObject)
                    {
                        if (performanceCounters == null)
                        {
                            performanceCounters = new Dictionary<string, ServiceModelPerformanceCounters>();
                        }
                    }
                }
                return performanceCounters;
            }
        }

        internal static PerformanceCounterScope Scope =>
            scope;
    }
}

