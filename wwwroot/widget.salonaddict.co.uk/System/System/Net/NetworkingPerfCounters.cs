namespace System.Net
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;

    internal static class NetworkingPerfCounters
    {
        private static PerformanceCounter BytesReceived;
        private const string BytesReceivedName = "Bytes Received";
        private static PerformanceCounter BytesSent;
        private const string BytesSentName = "Bytes Sent";
        private const string CategoryName = ".NET CLR Networking";
        private static PerformanceCounter ConnectionsEstablished;
        private const string ConnectionsEstablishedName = "Connections Established";
        private static PerformanceCounter DatagramsReceived;
        private const string DatagramsReceivedName = "Datagrams Received";
        private static PerformanceCounter DatagramsSent;
        private const string DatagramsSentName = "Datagrams Sent";
        private static PerformanceCounter globalBytesReceived;
        private static PerformanceCounter globalBytesSent;
        private static PerformanceCounter globalConnectionsEstablished;
        private static PerformanceCounter globalDatagramsReceived;
        private static PerformanceCounter globalDatagramsSent;
        private const string GlobalInstanceName = "_Global_";
        private static bool initialized = false;
        private static object syncObject = new object();

        internal static void AddBytesReceived(int increment)
        {
            if (BytesReceived != null)
            {
                BytesReceived.IncrementBy((long) increment);
            }
            if (globalBytesReceived != null)
            {
                globalBytesReceived.IncrementBy((long) increment);
            }
        }

        internal static void AddBytesSent(int increment)
        {
            if (BytesSent != null)
            {
                BytesSent.IncrementBy((long) increment);
            }
            if (globalBytesSent != null)
            {
                globalBytesSent.IncrementBy((long) increment);
            }
        }

        private static void Cleanup()
        {
            PerformanceCounter connectionsEstablished = ConnectionsEstablished;
            if (connectionsEstablished != null)
            {
                connectionsEstablished.RemoveInstance();
            }
            connectionsEstablished = BytesReceived;
            if (connectionsEstablished != null)
            {
                connectionsEstablished.RemoveInstance();
            }
            connectionsEstablished = BytesSent;
            if (connectionsEstablished != null)
            {
                connectionsEstablished.RemoveInstance();
            }
            connectionsEstablished = DatagramsReceived;
            if (connectionsEstablished != null)
            {
                connectionsEstablished.RemoveInstance();
            }
            connectionsEstablished = DatagramsSent;
            if (connectionsEstablished != null)
            {
                connectionsEstablished.RemoveInstance();
            }
        }

        private static void ExceptionEventHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                Cleanup();
            }
        }

        private static void ExitOrUnloadEventHandler(object sender, EventArgs e)
        {
            Cleanup();
        }

        [FileIOPermission(SecurityAction.Assert, Unrestricted=true)]
        private static string GetAssemblyName()
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

        [SecurityPermission(SecurityAction.Assert, Unrestricted=true)]
        private static string GetInstanceName()
        {
            string assemblyName = GetAssemblyName();
            if ((assemblyName == null) || (assemblyName.Length == 0))
            {
                assemblyName = AppDomain.CurrentDomain.FriendlyName;
            }
            StringBuilder builder = new StringBuilder(assemblyName);
            for (int i = 0; i < builder.Length; i++)
            {
                switch (builder[i])
                {
                    case '/':
                    case '\\':
                    case '#':
                        builder[i] = '_';
                        break;

                    case '(':
                        builder[i] = '[';
                        break;

                    case ')':
                        builder[i] = ']';
                        break;
                }
            }
            return string.Format(CultureInfo.CurrentCulture, "{0}[{1}]", new object[] { builder.ToString(), Process.GetCurrentProcess().Id });
        }

        internal static void IncrementConnectionsEstablished()
        {
            if (ConnectionsEstablished != null)
            {
                ConnectionsEstablished.Increment();
            }
            if (globalConnectionsEstablished != null)
            {
                globalConnectionsEstablished.Increment();
            }
        }

        internal static void IncrementDatagramsReceived()
        {
            if (DatagramsReceived != null)
            {
                DatagramsReceived.Increment();
            }
            if (globalDatagramsReceived != null)
            {
                globalDatagramsReceived.Increment();
            }
        }

        internal static void IncrementDatagramsSent()
        {
            if (DatagramsSent != null)
            {
                DatagramsSent.Increment();
            }
            if (globalDatagramsSent != null)
            {
                globalDatagramsSent.Increment();
            }
        }

        internal static void Initialize()
        {
            if (!initialized)
            {
                lock (syncObject)
                {
                    if (!initialized)
                    {
                        if (ComNetOS.IsWin2K)
                        {
                            new PerformanceCounterPermission(PermissionState.Unrestricted).Assert();
                            try
                            {
                                string instanceName = GetInstanceName();
                                ConnectionsEstablished = new PerformanceCounter();
                                ConnectionsEstablished.CategoryName = ".NET CLR Networking";
                                ConnectionsEstablished.CounterName = "Connections Established";
                                ConnectionsEstablished.InstanceName = instanceName;
                                ConnectionsEstablished.InstanceLifetime = PerformanceCounterInstanceLifetime.Process;
                                ConnectionsEstablished.ReadOnly = false;
                                ConnectionsEstablished.RawValue = 0L;
                                BytesReceived = new PerformanceCounter();
                                BytesReceived.CategoryName = ".NET CLR Networking";
                                BytesReceived.CounterName = "Bytes Received";
                                BytesReceived.InstanceName = instanceName;
                                BytesReceived.InstanceLifetime = PerformanceCounterInstanceLifetime.Process;
                                BytesReceived.ReadOnly = false;
                                BytesReceived.RawValue = 0L;
                                BytesSent = new PerformanceCounter();
                                BytesSent.CategoryName = ".NET CLR Networking";
                                BytesSent.CounterName = "Bytes Sent";
                                BytesSent.InstanceName = instanceName;
                                BytesSent.InstanceLifetime = PerformanceCounterInstanceLifetime.Process;
                                BytesSent.ReadOnly = false;
                                BytesSent.RawValue = 0L;
                                DatagramsReceived = new PerformanceCounter();
                                DatagramsReceived.CategoryName = ".NET CLR Networking";
                                DatagramsReceived.CounterName = "Datagrams Received";
                                DatagramsReceived.InstanceName = instanceName;
                                DatagramsReceived.InstanceLifetime = PerformanceCounterInstanceLifetime.Process;
                                DatagramsReceived.ReadOnly = false;
                                DatagramsReceived.RawValue = 0L;
                                DatagramsSent = new PerformanceCounter();
                                DatagramsSent.CategoryName = ".NET CLR Networking";
                                DatagramsSent.CounterName = "Datagrams Sent";
                                DatagramsSent.InstanceName = instanceName;
                                DatagramsSent.InstanceLifetime = PerformanceCounterInstanceLifetime.Process;
                                DatagramsSent.ReadOnly = false;
                                DatagramsSent.RawValue = 0L;
                                globalConnectionsEstablished = new PerformanceCounter(".NET CLR Networking", "Connections Established", "_Global_", false);
                                globalBytesReceived = new PerformanceCounter(".NET CLR Networking", "Bytes Received", "_Global_", false);
                                globalBytesSent = new PerformanceCounter(".NET CLR Networking", "Bytes Sent", "_Global_", false);
                                globalDatagramsReceived = new PerformanceCounter(".NET CLR Networking", "Datagrams Received", "_Global_", false);
                                globalDatagramsSent = new PerformanceCounter(".NET CLR Networking", "Datagrams Sent", "_Global_", false);
                                AppDomain.CurrentDomain.DomainUnload += new EventHandler(NetworkingPerfCounters.ExitOrUnloadEventHandler);
                                AppDomain.CurrentDomain.ProcessExit += new EventHandler(NetworkingPerfCounters.ExitOrUnloadEventHandler);
                                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(NetworkingPerfCounters.ExceptionEventHandler);
                            }
                            catch (Win32Exception)
                            {
                            }
                            catch (InvalidOperationException)
                            {
                            }
                            finally
                            {
                                CodeAccessPermission.RevertAssert();
                            }
                        }
                        initialized = true;
                    }
                }
            }
        }
    }
}

