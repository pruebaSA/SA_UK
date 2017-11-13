﻿namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Security;
    using System.Xml;

    internal abstract class PeerChannelListenerBase : TransportChannelListener, IPeerFactory, ITransportFactorySettings, IDefaultCommunicationTimeouts
    {
        private IPAddress listenIPAddress;
        private PeerNode peerNode;
        private int port;
        private PeerNodeImplementation privatePeerNode;
        private XmlDictionaryReaderQuotas readerQuotas;
        private System.ServiceModel.Channels.PeerNodeImplementation.Registration registration;
        private bool released;
        private PeerResolver resolver;
        private ISecurityCapabilities securityCapabilities;
        private PeerSecurityManager securityManager;
        private System.ServiceModel.Security.SecurityProtocol securityProtocol;
        private static UriPrefixTable<ITransportManagerRegistration> transportManagerTable = new UriPrefixTable<ITransportManagerRegistration>(true);

        internal PeerChannelListenerBase(PeerTransportBindingElement bindingElement, BindingContext context, PeerResolver peerResolver) : base(bindingElement, context)
        {
            this.listenIPAddress = bindingElement.ListenIPAddress;
            this.port = bindingElement.Port;
            this.resolver = peerResolver;
            this.readerQuotas = new XmlDictionaryReaderQuotas();
            BinaryMessageEncodingBindingElement element = context.Binding.Elements.Find<BinaryMessageEncodingBindingElement>();
            if (element != null)
            {
                element.ReaderQuotas.CopyTo(this.readerQuotas);
            }
            else
            {
                EncoderDefaults.ReaderQuotas.CopyTo(this.readerQuotas);
            }
            this.securityManager = PeerSecurityManager.Create(bindingElement.Security, context, this.readerQuotas);
            this.securityCapabilities = bindingElement.GetProperty<ISecurityCapabilities>(context);
        }

        public override T GetProperty<T>() where T: class
        {
            if (typeof(T) == typeof(PeerNode))
            {
                return (this.peerNode as T);
            }
            if (typeof(T) == typeof(IOnlineStatus))
            {
                return (this.peerNode as T);
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T) this.securityCapabilities;
            }
            return base.GetProperty<T>();
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            if ((base.State < CommunicationState.Closed) && (this.peerNode != null))
            {
                try
                {
                    this.peerNode.InnerNode.Abort();
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Information);
                }
            }
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => 
            new TypedCompletedAsyncResult<TimeoutHelper>(new TimeoutHelper(timeout), callback, state);

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state) => 
            new TypedCompletedAsyncResult<TimeoutHelper>(new TimeoutHelper(timeout), callback, state);

        protected override void OnClose(TimeSpan timeout)
        {
            this.OnCloseCore(timeout);
        }

        private void OnCloseCore(TimeSpan timeout)
        {
            TimeoutHelper helper = new TimeoutHelper(timeout);
            this.peerNode.OnClose();
            this.peerNode.InnerNode.Close(helper.RemainingTime());
            base.OnClose(helper.RemainingTime());
        }

        protected override void OnClosing()
        {
            base.OnClosing();
            if (!this.released)
            {
                bool flag = false;
                lock (base.ThisLock)
                {
                    if (!this.released)
                    {
                        flag = this.released = true;
                    }
                }
                if (flag && (this.peerNode != null))
                {
                    this.peerNode.InnerNode.Release();
                }
            }
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            TypedCompletedAsyncResult<TimeoutHelper> result2 = result as TypedCompletedAsyncResult<TimeoutHelper>;
            if (result2 != null)
            {
                this.OnCloseCore(result2.Data.RemainingTime());
            }
            else
            {
                base.OnEndClose(result);
            }
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            TypedCompletedAsyncResult<TimeoutHelper> result2 = result as TypedCompletedAsyncResult<TimeoutHelper>;
            if (result2 != null)
            {
                this.OnOpenCore(result2.Data.RemainingTime());
            }
            else
            {
                base.OnEndOpen(result);
            }
        }

        protected override void OnFaulted()
        {
            this.OnAbort();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            this.OnOpenCore(timeout);
        }

        private void OnOpenCore(TimeSpan timeout)
        {
            TimeoutHelper helper = new TimeoutHelper(timeout);
            base.OnOpen(helper.RemainingTime());
            this.peerNode.OnOpen();
            this.peerNode.InnerNode.Open(helper.RemainingTime(), false);
        }

        internal override IList<TransportManager> SelectTransportManagers()
        {
            if (this.peerNode == null)
            {
                PeerNodeImplementation peerNode = null;
                if ((this.privatePeerNode != null) && (this.Uri.Host == this.privatePeerNode.MeshId))
                {
                    peerNode = this.privatePeerNode;
                    this.registration = null;
                }
                else
                {
                    this.registration = new System.ServiceModel.Channels.PeerNodeImplementation.Registration(this.Uri, this);
                    peerNode = PeerNodeImplementation.Get(this.Uri, this.registration);
                }
                if (peerNode.MaxReceivedMessageSize < this.MaxReceivedMessageSize)
                {
                    peerNode.Release();
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("PeerMaxReceivedMessageSizeConflict", new object[] { this.MaxReceivedMessageSize, peerNode.MaxReceivedMessageSize, this.Uri })));
                }
                this.peerNode = new PeerNode(peerNode);
            }
            return null;
        }

        internal PeerNodeImplementation InnerNode =>
            this.peerNode?.InnerNode;

        public IPAddress ListenIPAddress =>
            this.listenIPAddress;

        public int Port =>
            this.port;

        public PeerNodeImplementation PrivatePeerNode
        {
            get => 
                this.privatePeerNode;
            set
            {
                this.privatePeerNode = value;
            }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas =>
            this.readerQuotas;

        internal System.ServiceModel.Channels.PeerNodeImplementation.Registration Registration =>
            this.registration;

        public PeerResolver Resolver =>
            this.resolver;

        public override string Scheme =>
            "net.p2p";

        public PeerSecurityManager SecurityManager
        {
            get => 
                this.securityManager;
            set
            {
                this.securityManager = value;
            }
        }

        protected System.ServiceModel.Security.SecurityProtocol SecurityProtocol
        {
            get => 
                this.securityProtocol;
            set
            {
                this.securityProtocol = value;
            }
        }

        internal static UriPrefixTable<ITransportManagerRegistration> StaticTransportManagerTable =>
            transportManagerTable;

        internal override UriPrefixTable<ITransportManagerRegistration> TransportManagerTable =>
            transportManagerTable;
    }
}

