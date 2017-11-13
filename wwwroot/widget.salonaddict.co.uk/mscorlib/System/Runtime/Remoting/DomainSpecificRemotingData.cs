namespace System.Runtime.Remoting
{
    using System;
    using System.Runtime.Remoting.Activation;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Contexts;
    using System.Runtime.Remoting.Lifetime;
    using System.Threading;

    internal class DomainSpecificRemotingData
    {
        private System.Runtime.Remoting.Activation.ActivationListener _ActivationListener;
        private IContextProperty[] _appDomainProperties = new IContextProperty[] { new LeaseLifeTimeServiceProperty() };
        private System.Runtime.Remoting.Channels.ChannelServicesData _ChannelServicesData = new System.Runtime.Remoting.Channels.ChannelServicesData();
        private object _ConfigLock = new object();
        private int _flags = 0;
        private ReaderWriterLock _IDTableLock = new ReaderWriterLock();
        private System.Runtime.Remoting.Lifetime.LeaseManager _LeaseManager;
        private System.Runtime.Remoting.Activation.LocalActivator _LocalActivator;
        private const int ACTIVATION_INITIALIZED = 2;
        private const int ACTIVATION_INITIALIZING = 1;
        private const int ACTIVATOR_LISTENING = 4;

        internal DomainSpecificRemotingData()
        {
        }

        internal bool ActivationInitialized
        {
            get => 
                ((this._flags & 2) == 2);
            set
            {
                if (value)
                {
                    this._flags |= 2;
                }
                else
                {
                    this._flags &= -3;
                }
            }
        }

        internal System.Runtime.Remoting.Activation.ActivationListener ActivationListener
        {
            get => 
                this._ActivationListener;
            set
            {
                this._ActivationListener = value;
            }
        }

        internal bool ActivatorListening
        {
            get => 
                ((this._flags & 4) == 4);
            set
            {
                if (value)
                {
                    this._flags |= 4;
                }
                else
                {
                    this._flags &= -5;
                }
            }
        }

        internal IContextProperty[] AppDomainContextProperties =>
            this._appDomainProperties;

        internal System.Runtime.Remoting.Channels.ChannelServicesData ChannelServicesData =>
            this._ChannelServicesData;

        internal object ConfigLock =>
            this._ConfigLock;

        internal ReaderWriterLock IDTableLock =>
            this._IDTableLock;

        internal bool InitializingActivation
        {
            get => 
                ((this._flags & 1) == 1);
            set
            {
                if (value)
                {
                    this._flags |= 1;
                }
                else
                {
                    this._flags &= -2;
                }
            }
        }

        internal System.Runtime.Remoting.Lifetime.LeaseManager LeaseManager
        {
            get => 
                this._LeaseManager;
            set
            {
                this._LeaseManager = value;
            }
        }

        internal System.Runtime.Remoting.Activation.LocalActivator LocalActivator
        {
            get => 
                this._LocalActivator;
            set
            {
                this._LocalActivator = value;
            }
        }
    }
}

