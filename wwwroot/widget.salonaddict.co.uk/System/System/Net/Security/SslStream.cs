namespace System.Net.Security
{
    using System;
    using System.IO;
    using System.Net;
    using System.Security.Authentication;
    using System.Security.Authentication.ExtendedProtection;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Permissions;

    public class SslStream : AuthenticatedStream
    {
        private SslState _SslState;
        private LocalCertificateSelectionCallback _userCertificateSelectionCallback;
        private RemoteCertificateValidationCallback _userCertificateValidationCallback;
        private object m_RemoteCertificateOrBytes;

        public SslStream(Stream innerStream) : this(innerStream, false, null, null)
        {
        }

        public SslStream(Stream innerStream, bool leaveInnerStreamOpen) : this(innerStream, leaveInnerStreamOpen, null, null)
        {
        }

        public SslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback) : this(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, null)
        {
        }

        public SslStream(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback) : base(innerStream, leaveInnerStreamOpen)
        {
            this._userCertificateValidationCallback = userCertificateValidationCallback;
            this._userCertificateSelectionCallback = userCertificateSelectionCallback;
            RemoteCertValidationCallback certValidationCallback = new RemoteCertValidationCallback(this.userCertValidationCallbackWrapper);
            LocalCertSelectionCallback certSelectionCallback = (userCertificateSelectionCallback == null) ? null : new LocalCertSelectionCallback(this.userCertSelectionCallbackWrapper);
            this._SslState = new SslState(innerStream, certValidationCallback, certSelectionCallback);
        }

        public virtual void AuthenticateAsClient(string targetHost)
        {
            this.AuthenticateAsClient(targetHost, new X509CertificateCollection(), ServicePointManager.DefaultSslProtocols, false);
        }

        public virtual void AuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
        {
            this._SslState.ValidateCreateContext(false, targetHost, enabledSslProtocols, null, clientCertificates, true, checkCertificateRevocation);
            this._SslState.ProcessAuthentication(null);
        }

        public virtual void AuthenticateAsServer(X509Certificate serverCertificate)
        {
            this.AuthenticateAsServer(serverCertificate, false, ServicePointManager.DefaultSslProtocols, false);
        }

        public virtual void AuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation)
        {
            if (!ComNetOS.IsWin2K)
            {
                throw new PlatformNotSupportedException(SR.GetString("Win2000Required"));
            }
            this._SslState.ValidateCreateContext(true, string.Empty, enabledSslProtocols, serverCertificate, null, clientCertificateRequired, checkCertificateRevocation);
            this._SslState.ProcessAuthentication(null);
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, AsyncCallback asyncCallback, object asyncState) => 
            this.BeginAuthenticateAsClient(targetHost, new X509CertificateCollection(), ServicePointManager.DefaultSslProtocols, false, asyncCallback, asyncState);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public virtual IAsyncResult BeginAuthenticateAsClient(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
        {
            this._SslState.ValidateCreateContext(false, targetHost, enabledSslProtocols, null, clientCertificates, true, checkCertificateRevocation);
            LazyAsyncResult lazyResult = new LazyAsyncResult(this._SslState, asyncState, asyncCallback);
            this._SslState.ProcessAuthentication(lazyResult);
            return lazyResult;
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, AsyncCallback asyncCallback, object asyncState) => 
            this.BeginAuthenticateAsServer(serverCertificate, false, ServicePointManager.DefaultSslProtocols, false, asyncCallback, asyncState);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, bool clientCertificateRequired, SslProtocols enabledSslProtocols, bool checkCertificateRevocation, AsyncCallback asyncCallback, object asyncState)
        {
            if (!ComNetOS.IsWin2K)
            {
                throw new PlatformNotSupportedException(SR.GetString("Win2000Required"));
            }
            this._SslState.ValidateCreateContext(true, string.Empty, enabledSslProtocols, serverCertificate, null, clientCertificateRequired, checkCertificateRevocation);
            LazyAsyncResult lazyResult = new LazyAsyncResult(this._SslState, asyncState, asyncCallback);
            this._SslState.ProcessAuthentication(lazyResult);
            return lazyResult;
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState) => 
            this._SslState.SecureStream.BeginRead(buffer, offset, count, asyncCallback, asyncState);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState) => 
            this._SslState.SecureStream.BeginWrite(buffer, offset, count, asyncCallback, asyncState);

        protected override void Dispose(bool disposing)
        {
            try
            {
                this._SslState.Close();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public virtual void EndAuthenticateAsClient(IAsyncResult asyncResult)
        {
            this._SslState.EndProcessAuthentication(asyncResult);
        }

        public virtual void EndAuthenticateAsServer(IAsyncResult asyncResult)
        {
            this._SslState.EndProcessAuthentication(asyncResult);
        }

        public override int EndRead(IAsyncResult asyncResult) => 
            this._SslState.SecureStream.EndRead(asyncResult);

        public override void EndWrite(IAsyncResult asyncResult)
        {
            this._SslState.SecureStream.EndWrite(asyncResult);
        }

        public override void Flush()
        {
            this._SslState.Flush();
        }

        internal ChannelBinding GetChannelBinding(ChannelBindingKind kind) => 
            this._SslState.GetChannelBinding(kind);

        public override int Read(byte[] buffer, int offset, int count) => 
            this._SslState.SecureStream.Read(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(SR.GetString("net_noseek"));
        }

        public override void SetLength(long value)
        {
            base.InnerStream.SetLength(value);
        }

        private X509Certificate userCertSelectionCallbackWrapper(string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers) => 
            this._userCertificateSelectionCallback(this, targetHost, localCertificates, remoteCertificate, acceptableIssuers);

        private bool userCertValidationCallbackWrapper(string hostName, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            this.m_RemoteCertificateOrBytes = certificate?.GetRawCertData();
            if (this._userCertificateValidationCallback != null)
            {
                return this._userCertificateValidationCallback(this, certificate, chain, sslPolicyErrors);
            }
            if (!this._SslState.RemoteCertRequired)
            {
                sslPolicyErrors &= ~SslPolicyErrors.RemoteCertificateNotAvailable;
            }
            return (sslPolicyErrors == SslPolicyErrors.None);
        }

        public void Write(byte[] buffer)
        {
            this._SslState.SecureStream.Write(buffer, 0, buffer.Length);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this._SslState.SecureStream.Write(buffer, offset, count);
        }

        public override bool CanRead =>
            (this._SslState.IsAuthenticated && base.InnerStream.CanRead);

        public override bool CanSeek =>
            false;

        public override bool CanTimeout =>
            base.InnerStream.CanTimeout;

        public override bool CanWrite =>
            (this._SslState.IsAuthenticated && base.InnerStream.CanWrite);

        public virtual bool CheckCertRevocationStatus =>
            this._SslState.CheckCertRevocationStatus;

        public virtual CipherAlgorithmType CipherAlgorithm =>
            this._SslState.CipherAlgorithm;

        public virtual int CipherStrength =>
            this._SslState.CipherStrength;

        public virtual HashAlgorithmType HashAlgorithm =>
            this._SslState.HashAlgorithm;

        public virtual int HashStrength =>
            this._SslState.HashStrength;

        public override bool IsAuthenticated =>
            this._SslState.IsAuthenticated;

        public override bool IsEncrypted =>
            this.IsAuthenticated;

        public override bool IsMutuallyAuthenticated =>
            this._SslState.IsMutuallyAuthenticated;

        public override bool IsServer =>
            this._SslState.IsServer;

        public override bool IsSigned =>
            this.IsAuthenticated;

        public virtual ExchangeAlgorithmType KeyExchangeAlgorithm =>
            this._SslState.KeyExchangeAlgorithm;

        public virtual int KeyExchangeStrength =>
            this._SslState.KeyExchangeStrength;

        public override long Length =>
            base.InnerStream.Length;

        public virtual X509Certificate LocalCertificate =>
            this._SslState.LocalCertificate;

        public override long Position
        {
            get => 
                base.InnerStream.Position;
            set
            {
                throw new NotSupportedException(SR.GetString("net_noseek"));
            }
        }

        public override int ReadTimeout
        {
            get => 
                base.InnerStream.ReadTimeout;
            set
            {
                base.InnerStream.ReadTimeout = value;
            }
        }

        public virtual X509Certificate RemoteCertificate
        {
            get
            {
                this._SslState.CheckThrow(true);
                object remoteCertificateOrBytes = this.m_RemoteCertificateOrBytes;
                if ((remoteCertificateOrBytes != null) && (remoteCertificateOrBytes.GetType() == typeof(byte[])))
                {
                    return (this.m_RemoteCertificateOrBytes = new X509Certificate((byte[]) remoteCertificateOrBytes));
                }
                return (remoteCertificateOrBytes as X509Certificate);
            }
        }

        public virtual SslProtocols SslProtocol =>
            this._SslState.SslProtocol;

        public System.Net.TransportContext TransportContext =>
            new SslStreamContext(this);

        public override int WriteTimeout
        {
            get => 
                base.InnerStream.WriteTimeout;
            set
            {
                base.InnerStream.WriteTimeout = value;
            }
        }
    }
}

