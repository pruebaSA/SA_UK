﻿namespace System.Data.ProviderBase
{
    using System;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.ConstrainedExecution;
    using System.Security.Permissions;

    internal abstract class DbConnectionPoolCounters
    {
        internal readonly Counter HardConnectsPerSecond;
        internal readonly Counter HardDisconnectsPerSecond;
        internal readonly Counter NumberOfActiveConnectionPoolGroups;
        internal readonly Counter NumberOfActiveConnectionPools;
        internal readonly Counter NumberOfActiveConnections;
        internal readonly Counter NumberOfFreeConnections;
        internal readonly Counter NumberOfInactiveConnectionPoolGroups;
        internal readonly Counter NumberOfInactiveConnectionPools;
        internal readonly Counter NumberOfNonPooledConnections;
        internal readonly Counter NumberOfPooledConnections;
        internal readonly Counter NumberOfReclaimedConnections;
        internal readonly Counter NumberOfStasisConnections;
        internal readonly Counter SoftConnectsPerSecond;
        internal readonly Counter SoftDisconnectsPerSecond;

        protected DbConnectionPoolCounters() : this(null, null)
        {
        }

        protected DbConnectionPoolCounters(string categoryName, string categoryHelp)
        {
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(this.UnloadEventHandler);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(this.ExitEventHandler);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.ExceptionEventHandler);
            string instanceName = null;
            if (!System.Data.Common.ADP.IsEmpty(categoryName) && System.Data.Common.ADP.IsPlatformNT5)
            {
                instanceName = this.GetInstanceName();
            }
            string str2 = categoryName;
            this.HardConnectsPerSecond = new Counter(str2, instanceName, CreationData.HardConnectsPerSecond.CounterName, CreationData.HardConnectsPerSecond.CounterType);
            this.HardDisconnectsPerSecond = new Counter(str2, instanceName, CreationData.HardDisconnectsPerSecond.CounterName, CreationData.HardDisconnectsPerSecond.CounterType);
            this.NumberOfNonPooledConnections = new Counter(str2, instanceName, CreationData.NumberOfNonPooledConnections.CounterName, CreationData.NumberOfNonPooledConnections.CounterType);
            this.NumberOfPooledConnections = new Counter(str2, instanceName, CreationData.NumberOfPooledConnections.CounterName, CreationData.NumberOfPooledConnections.CounterType);
            this.NumberOfActiveConnectionPoolGroups = new Counter(str2, instanceName, CreationData.NumberOfActiveConnectionPoolGroups.CounterName, CreationData.NumberOfActiveConnectionPoolGroups.CounterType);
            this.NumberOfInactiveConnectionPoolGroups = new Counter(str2, instanceName, CreationData.NumberOfInactiveConnectionPoolGroups.CounterName, CreationData.NumberOfInactiveConnectionPoolGroups.CounterType);
            this.NumberOfActiveConnectionPools = new Counter(str2, instanceName, CreationData.NumberOfActiveConnectionPools.CounterName, CreationData.NumberOfActiveConnectionPools.CounterType);
            this.NumberOfInactiveConnectionPools = new Counter(str2, instanceName, CreationData.NumberOfInactiveConnectionPools.CounterName, CreationData.NumberOfInactiveConnectionPools.CounterType);
            this.NumberOfStasisConnections = new Counter(str2, instanceName, CreationData.NumberOfStasisConnections.CounterName, CreationData.NumberOfStasisConnections.CounterType);
            this.NumberOfReclaimedConnections = new Counter(str2, instanceName, CreationData.NumberOfReclaimedConnections.CounterName, CreationData.NumberOfReclaimedConnections.CounterType);
            string str3 = null;
            if (!System.Data.Common.ADP.IsEmpty(categoryName))
            {
                TraceSwitch switch2 = new TraceSwitch("ConnectionPoolPerformanceCounterDetail", "level of detail to track with connection pool performance counters");
                if (TraceLevel.Verbose == switch2.Level)
                {
                    str3 = categoryName;
                }
            }
            this.SoftConnectsPerSecond = new Counter(str3, instanceName, CreationData.SoftConnectsPerSecond.CounterName, CreationData.SoftConnectsPerSecond.CounterType);
            this.SoftDisconnectsPerSecond = new Counter(str3, instanceName, CreationData.SoftDisconnectsPerSecond.CounterName, CreationData.SoftDisconnectsPerSecond.CounterType);
            this.NumberOfActiveConnections = new Counter(str3, instanceName, CreationData.NumberOfActiveConnections.CounterName, CreationData.NumberOfActiveConnections.CounterType);
            this.NumberOfFreeConnections = new Counter(str3, instanceName, CreationData.NumberOfFreeConnections.CounterName, CreationData.NumberOfFreeConnections.CounterType);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public void Dispose()
        {
            this.SafeDispose(this.HardConnectsPerSecond);
            this.SafeDispose(this.HardDisconnectsPerSecond);
            this.SafeDispose(this.SoftConnectsPerSecond);
            this.SafeDispose(this.SoftDisconnectsPerSecond);
            this.SafeDispose(this.NumberOfNonPooledConnections);
            this.SafeDispose(this.NumberOfPooledConnections);
            this.SafeDispose(this.NumberOfActiveConnectionPoolGroups);
            this.SafeDispose(this.NumberOfInactiveConnectionPoolGroups);
            this.SafeDispose(this.NumberOfActiveConnectionPools);
            this.SafeDispose(this.NumberOfActiveConnections);
            this.SafeDispose(this.NumberOfFreeConnections);
            this.SafeDispose(this.NumberOfStasisConnections);
            this.SafeDispose(this.NumberOfReclaimedConnections);
        }

        [PrePrepareMethod]
        private void ExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if ((e != null) && e.IsTerminating)
            {
                this.Dispose();
            }
        }

        [PrePrepareMethod]
        private void ExitEventHandler(object sender, EventArgs e)
        {
            this.Dispose();
        }

        [FileIOPermission(SecurityAction.Assert, Unrestricted=true)]
        private string GetAssemblyName()
        {
            string str = null;
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                AssemblyName name = entryAssembly.GetName();
                if (name != null)
                {
                    str = name.Name;
                }
            }
            return str;
        }

        private string GetInstanceName()
        {
            string assemblyName = this.GetAssemblyName();
            if (System.Data.Common.ADP.IsEmpty(assemblyName))
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                if (currentDomain != null)
                {
                    assemblyName = currentDomain.FriendlyName;
                }
            }
            int currentProcessId = System.Data.Common.SafeNativeMethods.GetCurrentProcessId();
            return string.Format(null, "{0}[{1}]", new object[] { assemblyName, currentProcessId }).Replace('(', '[').Replace(')', ']').Replace('#', '_').Replace('/', '_').Replace('\\', '_');
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        private void SafeDispose(Counter counter)
        {
            if (counter != null)
            {
                counter.Dispose();
            }
        }

        [PrePrepareMethod]
        private void UnloadEventHandler(object sender, EventArgs e)
        {
            this.Dispose();
        }

        internal sealed class Counter
        {
            private PerformanceCounter _instance;

            internal Counter(string categoryName, string instanceName, string counterName, PerformanceCounterType counterType)
            {
                if (System.Data.Common.ADP.IsPlatformNT5)
                {
                    try
                    {
                        if (!System.Data.Common.ADP.IsEmpty(categoryName) && !System.Data.Common.ADP.IsEmpty(instanceName))
                        {
                            PerformanceCounter counter = new PerformanceCounter {
                                CategoryName = categoryName,
                                CounterName = counterName,
                                InstanceName = instanceName,
                                InstanceLifetime = PerformanceCounterInstanceLifetime.Process,
                                ReadOnly = false,
                                RawValue = 0L
                            };
                            this._instance = counter;
                        }
                    }
                    catch (InvalidOperationException exception)
                    {
                        System.Data.Common.ADP.TraceExceptionWithoutRethrow(exception);
                    }
                }
            }

            internal void Decrement()
            {
                PerformanceCounter counter = this._instance;
                if (counter != null)
                {
                    counter.Decrement();
                }
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            internal void Dispose()
            {
                PerformanceCounter counter = this._instance;
                this._instance = null;
                if (counter != null)
                {
                    counter.RemoveInstance();
                }
            }

            internal void Increment()
            {
                PerformanceCounter counter = this._instance;
                if (counter != null)
                {
                    counter.Increment();
                }
            }
        }

        private static class CreationData
        {
            internal static readonly CounterCreationData HardConnectsPerSecond = new CounterCreationData("HardConnectsPerSecond", "The number of actual connections per second that are being made to servers", PerformanceCounterType.RateOfCountsPerSecond32);
            internal static readonly CounterCreationData HardDisconnectsPerSecond = new CounterCreationData("HardDisconnectsPerSecond", "The number of actual disconnects per second that are being made to servers", PerformanceCounterType.RateOfCountsPerSecond32);
            internal static readonly CounterCreationData NumberOfActiveConnectionPoolGroups = new CounterCreationData("NumberOfActiveConnectionPoolGroups", "The number of unique connection strings", PerformanceCounterType.NumberOfItems32);
            internal static readonly CounterCreationData NumberOfActiveConnectionPools = new CounterCreationData("NumberOfActiveConnectionPools", "The number of connection pools", PerformanceCounterType.NumberOfItems32);
            internal static readonly CounterCreationData NumberOfActiveConnections = new CounterCreationData("NumberOfActiveConnections", "The number of connections currently in-use", PerformanceCounterType.NumberOfItems32);
            internal static readonly CounterCreationData NumberOfFreeConnections = new CounterCreationData("NumberOfFreeConnections", "The number of connections currently available for use", PerformanceCounterType.NumberOfItems32);
            internal static readonly CounterCreationData NumberOfInactiveConnectionPoolGroups = new CounterCreationData("NumberOfInactiveConnectionPoolGroups", "The number of unique connection strings waiting for pruning", PerformanceCounterType.NumberOfItems32);
            internal static readonly CounterCreationData NumberOfInactiveConnectionPools = new CounterCreationData("NumberOfInactiveConnectionPools", "The number of connection pools", PerformanceCounterType.NumberOfItems32);
            internal static readonly CounterCreationData NumberOfNonPooledConnections = new CounterCreationData("NumberOfNonPooledConnections", "The number of connections that are not using connection pooling", PerformanceCounterType.NumberOfItems32);
            internal static readonly CounterCreationData NumberOfPooledConnections = new CounterCreationData("NumberOfPooledConnections", "The number of connections that are managed by the connection pooler", PerformanceCounterType.NumberOfItems32);
            internal static readonly CounterCreationData NumberOfReclaimedConnections = new CounterCreationData("NumberOfReclaimedConnections", "The number of connections we reclaim from GC'd external connections", PerformanceCounterType.NumberOfItems32);
            internal static readonly CounterCreationData NumberOfStasisConnections = new CounterCreationData("NumberOfStasisConnections", "The number of connections currently waiting to be made ready for use", PerformanceCounterType.NumberOfItems32);
            internal static readonly CounterCreationData SoftConnectsPerSecond = new CounterCreationData("SoftConnectsPerSecond", "The number of connections we get from the pool per second", PerformanceCounterType.RateOfCountsPerSecond32);
            internal static readonly CounterCreationData SoftDisconnectsPerSecond = new CounterCreationData("SoftDisconnectsPerSecond", "The number of connections we return to the pool per second", PerformanceCounterType.RateOfCountsPerSecond32);
        }
    }
}

