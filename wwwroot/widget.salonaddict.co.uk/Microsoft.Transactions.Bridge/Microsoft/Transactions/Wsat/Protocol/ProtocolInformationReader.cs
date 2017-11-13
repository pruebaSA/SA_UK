namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions;
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Recovery;
    using System;
    using System.IO;

    internal class ProtocolInformationReader
    {
        private string basePath;
        private ProtocolInformationFlags flags;
        private string hostName;
        private int httpsPort;
        private bool isV10Enabled;
        private bool isV11Enabled;
        private TimeSpan maxTimeout;
        private string nodeName;

        public ProtocolInformationReader(MemoryStream mem)
        {
            this.ReadProtocolInformation(mem);
        }

        private void CheckFlags(ProtocolInformationFlags flags)
        {
            if (((byte) (flags | (ProtocolInformationFlags.IsClustered | ProtocolInformationFlags.IssuedTokensEnabled | ProtocolInformationFlags.NetworkClientAccess | ProtocolInformationFlags.NetworkInboundAccess | ProtocolInformationFlags.NetworkOutboundAccess))) != 0x1f)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(Microsoft.Transactions.SR.GetString("ProtocolInfoInvalidFlags", new object[] { flags })));
            }
            if (((byte) (flags & (ProtocolInformationFlags.NetworkInboundAccess | ProtocolInformationFlags.NetworkOutboundAccess))) == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(Microsoft.Transactions.SR.GetString("ProtocolInfoInvalidFlags", new object[] { flags })));
            }
        }

        private void ReadProtocolInformation(MemoryStream mem)
        {
            ProtocolInformationMajorVersion version = (ProtocolInformationMajorVersion) ((byte) mem.ReadByte());
            if (version != ProtocolInformationMajorVersion.v1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(Microsoft.Transactions.SR.GetString("ProtocolInfoUnsupportedVersion", new object[] { version })));
            }
            ProtocolInformationMinorVersion version2 = (ProtocolInformationMinorVersion) ((byte) mem.ReadByte());
            this.flags = (ProtocolInformationFlags) ((byte) mem.ReadByte());
            this.CheckFlags(this.flags);
            this.httpsPort = SerializationUtils.ReadInt(mem);
            if ((this.httpsPort < 0) || (this.httpsPort > 0xffff))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(Microsoft.Transactions.SR.GetString("ProtocolInfoInvalidHttpsPort", new object[] { this.httpsPort })));
            }
            this.maxTimeout = SerializationUtils.ReadTimeout(mem);
            if (this.maxTimeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(Microsoft.Transactions.SR.GetString("ProtocolInfoInvalidMaxTimeout", new object[] { this.maxTimeout })));
            }
            this.hostName = SerializationUtils.ReadString(mem);
            if (string.IsNullOrEmpty(this.hostName))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(Microsoft.Transactions.SR.GetString("ProtocolInfoInvalidHostName")));
            }
            this.basePath = SerializationUtils.ReadString(mem);
            if (string.IsNullOrEmpty(this.basePath))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(Microsoft.Transactions.SR.GetString("ProtocolInfoInvalidBasePath")));
            }
            this.nodeName = SerializationUtils.ReadString(mem);
            if (string.IsNullOrEmpty(this.nodeName))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SerializationException(Microsoft.Transactions.SR.GetString("ProtocolInfoInvalidNodeName")));
            }
            byte num = 2;
            if (version2 >= num)
            {
                ProtocolVersion version3 = (ProtocolVersion) SerializationUtils.ReadUShort(mem);
                if (((ushort) (version3 & ProtocolVersion.Version10)) != 0)
                {
                    this.isV10Enabled = true;
                }
                if (((ushort) (version3 & ProtocolVersion.Version11)) != 0)
                {
                    this.isV11Enabled = true;
                }
            }
            else
            {
                this.isV10Enabled = true;
            }
        }

        public string BasePath =>
            this.basePath;

        public string HostName =>
            this.hostName;

        public int HttpsPort =>
            this.httpsPort;

        public bool IsClustered =>
            (((byte) (this.flags & ProtocolInformationFlags.IsClustered)) != 0);

        public bool IssuedTokensEnabled =>
            (((byte) (this.flags & ProtocolInformationFlags.IssuedTokensEnabled)) != 0);

        public bool IsV10Enabled =>
            this.isV10Enabled;

        public bool IsV11Enabled =>
            this.isV11Enabled;

        public TimeSpan MaxTimeout =>
            this.maxTimeout;

        public bool NetworkClientAccess =>
            (((byte) (this.flags & ProtocolInformationFlags.NetworkClientAccess)) != 0);

        public bool NetworkInboundAccess =>
            (((byte) (this.flags & ProtocolInformationFlags.NetworkInboundAccess)) != 0);

        public bool NetworkOutboundAccess =>
            (((byte) (this.flags & ProtocolInformationFlags.NetworkOutboundAccess)) != 0);

        public string NodeName =>
            this.nodeName;
    }
}

