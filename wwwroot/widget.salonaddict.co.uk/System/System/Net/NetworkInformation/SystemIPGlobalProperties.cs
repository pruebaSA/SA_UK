namespace System.Net.NetworkInformation
{
    using System;
    using System.Collections;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;

    internal class SystemIPGlobalProperties : IPGlobalProperties
    {
        private static string domainName = null;
        private System.Net.NetworkInformation.FixedInfo fixedInfo;
        private bool fixedInfoInitialized;
        private static string hostName = null;
        private static object syncObject = new object();

        internal SystemIPGlobalProperties()
        {
        }

        public override TcpConnectionInformation[] GetActiveTcpConnections()
        {
            ArrayList list = new ArrayList();
            foreach (TcpConnectionInformation information in this.GetAllTcpConnections())
            {
                if (information.State != TcpState.Listen)
                {
                    list.Add(information);
                }
            }
            TcpConnectionInformation[] informationArray = new TcpConnectionInformation[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                informationArray[i] = (TcpConnectionInformation) list[i];
            }
            return informationArray;
        }

        public override IPEndPoint[] GetActiveTcpListeners()
        {
            ArrayList list = new ArrayList();
            foreach (TcpConnectionInformation information in this.GetAllTcpConnections())
            {
                if (information.State == TcpState.Listen)
                {
                    list.Add(information.LocalEndPoint);
                }
            }
            IPEndPoint[] pointArray = new IPEndPoint[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                pointArray[i] = (IPEndPoint) list[i];
            }
            return pointArray;
        }

        public override IPEndPoint[] GetActiveUdpListeners()
        {
            uint dwOutBufLen = 0;
            SafeLocalFree pUdpTable = null;
            IPEndPoint[] pointArray = null;
            uint num2 = UnsafeNetInfoNativeMethods.GetUdpTable(SafeLocalFree.Zero, ref dwOutBufLen, true);
            while (num2 == 0x7a)
            {
                try
                {
                    pUdpTable = SafeLocalFree.LocalAlloc((int) dwOutBufLen);
                    num2 = UnsafeNetInfoNativeMethods.GetUdpTable(pUdpTable, ref dwOutBufLen, true);
                    if (num2 == 0)
                    {
                        IntPtr handle = pUdpTable.DangerousGetHandle();
                        MibUdpTable table = (MibUdpTable) Marshal.PtrToStructure(handle, typeof(MibUdpTable));
                        if (table.numberOfEntries > 0)
                        {
                            pointArray = new IPEndPoint[table.numberOfEntries];
                            handle = (IntPtr) (((long) handle) + Marshal.SizeOf(table.numberOfEntries));
                            for (int i = 0; i < table.numberOfEntries; i++)
                            {
                                MibUdpRow structure = (MibUdpRow) Marshal.PtrToStructure(handle, typeof(MibUdpRow));
                                int port = (((structure.localPort3 << 0x18) | (structure.localPort4 << 0x10)) | (structure.localPort1 << 8)) | structure.localPort2;
                                pointArray[i] = new IPEndPoint((long) structure.localAddr, port);
                                handle = (IntPtr) (((long) handle) + Marshal.SizeOf(structure));
                            }
                        }
                    }
                    continue;
                }
                finally
                {
                    if (pUdpTable != null)
                    {
                        pUdpTable.Close();
                    }
                }
            }
            if ((num2 != 0) && (num2 != 0xe8))
            {
                throw new NetworkInformationException((int) num2);
            }
            if (pointArray == null)
            {
                return new IPEndPoint[0];
            }
            return pointArray;
        }

        private TcpConnectionInformation[] GetAllTcpConnections()
        {
            uint dwOutBufLen = 0;
            SafeLocalFree pTcpTable = null;
            SystemTcpConnectionInformation[] informationArray = null;
            uint num2 = UnsafeNetInfoNativeMethods.GetTcpTable(SafeLocalFree.Zero, ref dwOutBufLen, true);
            while (num2 == 0x7a)
            {
                try
                {
                    pTcpTable = SafeLocalFree.LocalAlloc((int) dwOutBufLen);
                    num2 = UnsafeNetInfoNativeMethods.GetTcpTable(pTcpTable, ref dwOutBufLen, true);
                    if (num2 == 0)
                    {
                        IntPtr handle = pTcpTable.DangerousGetHandle();
                        MibTcpTable table = (MibTcpTable) Marshal.PtrToStructure(handle, typeof(MibTcpTable));
                        if (table.numberOfEntries > 0)
                        {
                            informationArray = new SystemTcpConnectionInformation[table.numberOfEntries];
                            handle = (IntPtr) (((long) handle) + Marshal.SizeOf(table.numberOfEntries));
                            for (int i = 0; i < table.numberOfEntries; i++)
                            {
                                MibTcpRow row = (MibTcpRow) Marshal.PtrToStructure(handle, typeof(MibTcpRow));
                                informationArray[i] = new SystemTcpConnectionInformation(row);
                                handle = (IntPtr) (((long) handle) + Marshal.SizeOf(row));
                            }
                        }
                    }
                    continue;
                }
                finally
                {
                    if (pTcpTable != null)
                    {
                        pTcpTable.Close();
                    }
                }
            }
            if ((num2 != 0) && (num2 != 0xe8))
            {
                throw new NetworkInformationException((int) num2);
            }
            if (informationArray == null)
            {
                return new SystemTcpConnectionInformation[0];
            }
            return informationArray;
        }

        internal static System.Net.NetworkInformation.FixedInfo GetFixedInfo()
        {
            uint pOutBufLen = 0;
            SafeLocalFree pFixedInfo = null;
            System.Net.NetworkInformation.FixedInfo info = new System.Net.NetworkInformation.FixedInfo();
            uint networkParams = UnsafeNetInfoNativeMethods.GetNetworkParams(SafeLocalFree.Zero, ref pOutBufLen);
            while (networkParams == 0x6f)
            {
                try
                {
                    pFixedInfo = SafeLocalFree.LocalAlloc((int) pOutBufLen);
                    networkParams = UnsafeNetInfoNativeMethods.GetNetworkParams(pFixedInfo, ref pOutBufLen);
                    if (networkParams == 0)
                    {
                        info = new System.Net.NetworkInformation.FixedInfo((FIXED_INFO) Marshal.PtrToStructure(pFixedInfo.DangerousGetHandle(), typeof(FIXED_INFO)));
                    }
                    continue;
                }
                finally
                {
                    if (pFixedInfo != null)
                    {
                        pFixedInfo.Close();
                    }
                }
            }
            if (networkParams != 0)
            {
                throw new NetworkInformationException((int) networkParams);
            }
            return info;
        }

        public override IcmpV4Statistics GetIcmpV4Statistics() => 
            new SystemIcmpV4Statistics();

        public override IcmpV6Statistics GetIcmpV6Statistics() => 
            new SystemIcmpV6Statistics();

        public override IPGlobalStatistics GetIPv4GlobalStatistics() => 
            new SystemIPGlobalStatistics(AddressFamily.InterNetwork);

        public override IPGlobalStatistics GetIPv6GlobalStatistics() => 
            new SystemIPGlobalStatistics(AddressFamily.InterNetworkV6);

        public override TcpStatistics GetTcpIPv4Statistics() => 
            new SystemTcpStatistics(AddressFamily.InterNetwork);

        public override TcpStatistics GetTcpIPv6Statistics() => 
            new SystemTcpStatistics(AddressFamily.InterNetworkV6);

        public override UdpStatistics GetUdpIPv4Statistics() => 
            new SystemUdpStatistics(AddressFamily.InterNetwork);

        public override UdpStatistics GetUdpIPv6Statistics() => 
            new SystemUdpStatistics(AddressFamily.InterNetworkV6);

        public override string DhcpScopeName =>
            this.FixedInfo.ScopeId;

        public override string DomainName
        {
            get
            {
                if (domainName == null)
                {
                    lock (syncObject)
                    {
                        if (domainName == null)
                        {
                            hostName = this.FixedInfo.HostName;
                            domainName = this.FixedInfo.DomainName;
                        }
                    }
                }
                return domainName;
            }
        }

        internal System.Net.NetworkInformation.FixedInfo FixedInfo
        {
            get
            {
                if (!this.fixedInfoInitialized)
                {
                    lock (this)
                    {
                        if (!this.fixedInfoInitialized)
                        {
                            this.fixedInfo = GetFixedInfo();
                            this.fixedInfoInitialized = true;
                        }
                    }
                }
                return this.fixedInfo;
            }
        }

        public override string HostName
        {
            get
            {
                if (hostName == null)
                {
                    lock (syncObject)
                    {
                        if (hostName == null)
                        {
                            hostName = this.FixedInfo.HostName;
                            domainName = this.FixedInfo.DomainName;
                        }
                    }
                }
                return hostName;
            }
        }

        public override bool IsWinsProxy =>
            this.FixedInfo.EnableProxy;

        public override NetBiosNodeType NodeType =>
            this.FixedInfo.NodeType;
    }
}

