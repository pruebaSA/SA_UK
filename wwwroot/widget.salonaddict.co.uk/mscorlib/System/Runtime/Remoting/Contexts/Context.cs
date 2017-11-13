namespace System.Runtime.Remoting.Contexts
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Activation;
    using System.Runtime.Remoting.Messaging;
    using System.Security.Permissions;
    using System.Threading;

    [ComVisible(true)]
    public class Context
    {
        private System.AppDomain _appDomain;
        private IMessageSink _clientContextChain;
        private int _ctxFlags;
        private int _ctxID;
        private static int _ctxIDCounter = 0;
        private IContextProperty[] _ctxProps;
        private object[] _ctxStatics;
        private int _ctxStaticsCurrentBucket;
        private int _ctxStaticsFreeIndex;
        private DynamicPropertyHolder _dphCtx;
        private static DynamicPropertyHolder _dphGlobal = new DynamicPropertyHolder();
        private IntPtr _internalContext;
        private LocalDataStore _localDataStore;
        private static LocalDataStoreMgr _localDataStoreMgr = new LocalDataStoreMgr();
        private int _numCtxProps;
        private IMessageSink _serverContextChain;
        internal const int CTX_DEFAULT_CONTEXT = 1;
        internal const int CTX_FROZEN = 2;
        internal const int CTX_THREADPOOL_AWARE = 4;
        private const int GROW_BY = 8;
        private const int STATICS_BUCKET_SIZE = 8;

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public Context() : this(0)
        {
        }

        private Context(int flags)
        {
            this._ctxFlags = flags;
            if ((this._ctxFlags & 1) != 0)
            {
                this._ctxID = 0;
            }
            else
            {
                this._ctxID = Interlocked.Increment(ref _ctxIDCounter);
            }
            DomainSpecificRemotingData remotingData = Thread.GetDomain().RemotingData;
            if (remotingData != null)
            {
                IContextProperty[] appDomainContextProperties = remotingData.AppDomainContextProperties;
                if (appDomainContextProperties != null)
                {
                    for (int i = 0; i < appDomainContextProperties.Length; i++)
                    {
                        this.SetProperty(appDomainContextProperties[i]);
                    }
                }
            }
            if ((this._ctxFlags & 1) != 0)
            {
                this.Freeze();
            }
            this.SetupInternalContext((this._ctxFlags & 1) == 1);
        }

        internal static bool AddDynamicProperty(Context ctx, IDynamicProperty prop)
        {
            if (ctx != null)
            {
                return ctx.AddPerContextDynamicProperty(prop);
            }
            return AddGlobalDynamicProperty(prop);
        }

        private static bool AddGlobalDynamicProperty(IDynamicProperty prop) => 
            _dphGlobal.AddDynamicProperty(prop);

        private bool AddPerContextDynamicProperty(IDynamicProperty prop) => 
            this._dphCtx?.AddDynamicProperty(prop);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public static LocalDataStoreSlot AllocateDataSlot() => 
            _localDataStoreMgr.AllocateDataSlot();

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public static LocalDataStoreSlot AllocateNamedDataSlot(string name) => 
            _localDataStoreMgr.AllocateNamedDataSlot(name);

        internal static void CheckPropertyNameClash(string name, IContextProperty[] props, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (props[i].Name.Equals(name))
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_DuplicatePropertyName"));
                }
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void CleanupInternalContext();
        internal static Context CreateDefaultContext() => 
            new Context(1);

        internal virtual IMessageSink CreateEnvoyChain(MarshalByRefObject objectOrProxy)
        {
            IMessageSink messageSink = EnvoyTerminatorSink.MessageSink;
            object obj2 = null;
            int index = 0;
            MarshalByRefObject obj3 = objectOrProxy;
            while (index < this._numCtxProps)
            {
                obj2 = this._ctxProps[index];
                IContributeEnvoySink sink2 = obj2 as IContributeEnvoySink;
                if (sink2 != null)
                {
                    messageSink = sink2.GetEnvoySink(obj3, messageSink);
                    if (messageSink == null)
                    {
                        throw new RemotingException(Environment.GetResourceString("Remoting_Contexts_BadProperty"));
                    }
                }
                index++;
            }
            return messageSink;
        }

        internal virtual IMessageSink CreateServerObjectChain(MarshalByRefObject serverObj)
        {
            IMessageSink nextSink = new ServerObjectTerminatorSink(serverObj);
            object obj2 = null;
            int index = this._numCtxProps;
            while (index-- > 0)
            {
                obj2 = this._ctxProps[index];
                IContributeObjectSink sink2 = obj2 as IContributeObjectSink;
                if (sink2 != null)
                {
                    nextSink = sink2.GetObjectSink(serverObj, nextSink);
                    if (nextSink == null)
                    {
                        throw new RemotingException(Environment.GetResourceString("Remoting_Contexts_BadProperty"));
                    }
                }
            }
            return nextSink;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public void DoCallBack(CrossContextDelegate deleg)
        {
            if (deleg == null)
            {
                throw new ArgumentNullException("deleg");
            }
            if ((this._ctxFlags & 2) == 0)
            {
                throw new RemotingException(Environment.GetResourceString("Remoting_Contexts_ContextNotFrozenForCallBack"));
            }
            Context currentContext = Thread.CurrentContext;
            if (currentContext == this)
            {
                deleg();
            }
            else
            {
                currentContext.DoCallBackGeneric(this.InternalContextID, deleg);
                GC.KeepAlive(this);
            }
        }

        internal static void DoCallBackFromEE(IntPtr targetCtxID, IntPtr privateData, int targetDomainID)
        {
            if (targetDomainID == 0)
            {
                CallBackHelper helper = new CallBackHelper(privateData, true, targetDomainID);
                CrossContextDelegate deleg = new CrossContextDelegate(helper.Func);
                Thread.CurrentContext.DoCallBackGeneric(targetCtxID, deleg);
            }
            else
            {
                TransitionCall msg = new TransitionCall(targetCtxID, privateData, targetDomainID);
                Message.PropagateCallContextFromThreadToMessage(msg);
                IMessage message = Thread.CurrentContext.GetClientContextChain().SyncProcessMessage(msg);
                Message.PropagateCallContextFromMessageToThread(message);
                IMethodReturnMessage message2 = message as IMethodReturnMessage;
                if ((message2 != null) && (message2.Exception != null))
                {
                    throw message2.Exception;
                }
            }
        }

        internal void DoCallBackGeneric(IntPtr targetCtxID, CrossContextDelegate deleg)
        {
            TransitionCall msg = new TransitionCall(targetCtxID, deleg);
            Message.PropagateCallContextFromThreadToMessage(msg);
            IMessage message = this.GetClientContextChain().SyncProcessMessage(msg);
            if (message != null)
            {
                Message.PropagateCallContextFromMessageToThread(message);
            }
            IMethodReturnMessage message2 = message as IMethodReturnMessage;
            if ((message2 != null) && (message2.Exception != null))
            {
                throw message2.Exception;
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void ExecuteCallBackInEE(IntPtr privateData);
        ~Context()
        {
            if ((this._internalContext != IntPtr.Zero) && ((this._ctxFlags & 1) == 0))
            {
                this.CleanupInternalContext();
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public static void FreeNamedDataSlot(string name)
        {
            _localDataStoreMgr.FreeNamedDataSlot(name);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public virtual void Freeze()
        {
            lock (this)
            {
                if ((this._ctxFlags & 2) != 0)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ContextAlreadyFrozen"));
                }
                this.InternalFreeze();
            }
        }

        internal virtual IMessageSink GetClientContextChain()
        {
            if (this._clientContextChain == null)
            {
                IMessageSink messageSink = ClientContextTerminatorSink.MessageSink;
                object obj2 = null;
                for (int i = 0; i < this._numCtxProps; i++)
                {
                    obj2 = this._ctxProps[i];
                    IContributeClientContextSink sink2 = obj2 as IContributeClientContextSink;
                    if (sink2 != null)
                    {
                        messageSink = sink2.GetClientContextSink(messageSink);
                        if (messageSink == null)
                        {
                            throw new RemotingException(Environment.GetResourceString("Remoting_Contexts_BadProperty"));
                        }
                    }
                }
                lock (this)
                {
                    if (this._clientContextChain == null)
                    {
                        this._clientContextChain = messageSink;
                    }
                }
            }
            return this._clientContextChain;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public static object GetData(LocalDataStoreSlot slot) => 
            Thread.CurrentContext.MyLocalStore.GetData(slot);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public static LocalDataStoreSlot GetNamedDataSlot(string name) => 
            _localDataStoreMgr.GetNamedDataSlot(name);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public virtual IContextProperty GetProperty(string name)
        {
            if ((this._ctxProps == null) || (name == null))
            {
                return null;
            }
            for (int i = 0; i < this._numCtxProps; i++)
            {
                if (this._ctxProps[i].Name.Equals(name))
                {
                    return this._ctxProps[i];
                }
            }
            return null;
        }

        internal virtual IMessageSink GetServerContextChain()
        {
            if (this._serverContextChain == null)
            {
                IMessageSink messageSink = ServerContextTerminatorSink.MessageSink;
                object obj2 = null;
                int index = this._numCtxProps;
                while (index-- > 0)
                {
                    obj2 = this._ctxProps[index];
                    IContributeServerContextSink sink2 = obj2 as IContributeServerContextSink;
                    if (sink2 != null)
                    {
                        messageSink = sink2.GetServerContextSink(messageSink);
                        if (messageSink == null)
                        {
                            throw new RemotingException(Environment.GetResourceString("Remoting_Contexts_BadProperty"));
                        }
                    }
                }
                lock (this)
                {
                    if (this._serverContextChain == null)
                    {
                        this._serverContextChain = messageSink;
                    }
                }
            }
            return this._serverContextChain;
        }

        internal static IContextProperty[] GrowPropertiesArray(IContextProperty[] props)
        {
            int num = ((props != null) ? props.Length : 0) + 8;
            IContextProperty[] destinationArray = new IContextProperty[num];
            if (props != null)
            {
                Array.Copy(props, destinationArray, props.Length);
            }
            return destinationArray;
        }

        internal virtual void InternalFreeze()
        {
            this._ctxFlags |= 2;
            for (int i = 0; i < this._numCtxProps; i++)
            {
                this._ctxProps[i].Freeze(this);
            }
        }

        internal IMessage NotifyActivatorProperties(IMessage msg, bool bServerSide)
        {
            IMessage message = null;
            try
            {
                int index = this._numCtxProps;
                object obj2 = null;
                while (index-- != 0)
                {
                    obj2 = this._ctxProps[index];
                    IContextPropertyActivator activator = obj2 as IContextPropertyActivator;
                    if (activator != null)
                    {
                        IConstructionCallMessage message2 = msg as IConstructionCallMessage;
                        if (message2 != null)
                        {
                            if (!bServerSide)
                            {
                                activator.CollectFromClientContext(message2);
                            }
                            else
                            {
                                activator.DeliverClientContextToServerContext(message2);
                            }
                        }
                        else if (bServerSide)
                        {
                            activator.CollectFromServerContext((IConstructionReturnMessage) msg);
                        }
                        else
                        {
                            activator.DeliverServerContextToClientContext((IConstructionReturnMessage) msg);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                IMethodCallMessage mcm = null;
                if (msg is IConstructionCallMessage)
                {
                    mcm = (IMethodCallMessage) msg;
                }
                else
                {
                    mcm = new ErrorMessage();
                }
                message = new ReturnMessage(exception, mcm);
                if (msg != null)
                {
                    ((ReturnMessage) message).SetLogicalCallContext((LogicalCallContext) msg.Properties[Message.CallContextKey]);
                }
            }
            return message;
        }

        internal virtual bool NotifyDynamicSinks(IMessage msg, bool bCliSide, bool bStart, bool bAsync, bool bNotifyGlobals)
        {
            bool flag = false;
            if (bNotifyGlobals && (_dphGlobal.DynamicProperties != null))
            {
                ArrayWithSize globalDynamicSinks = GlobalDynamicSinks;
                if (globalDynamicSinks != null)
                {
                    DynamicPropertyHolder.NotifyDynamicSinks(msg, globalDynamicSinks, bCliSide, bStart, bAsync);
                    flag = true;
                }
            }
            ArrayWithSize dynamicSinks = this.DynamicSinks;
            if (dynamicSinks != null)
            {
                DynamicPropertyHolder.NotifyDynamicSinks(msg, dynamicSinks, bCliSide, bStart, bAsync);
                flag = true;
            }
            return flag;
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.Infrastructure)]
        public static bool RegisterDynamicProperty(IDynamicProperty prop, ContextBoundObject obj, Context ctx)
        {
            if (((prop == null) || (prop.Name == null)) || !(prop is IContributeDynamicSink))
            {
                throw new ArgumentNullException("prop");
            }
            if ((obj != null) && (ctx != null))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_NonNullObjAndCtx"));
            }
            if (obj != null)
            {
                return IdentityHolder.AddDynamicProperty(obj, prop);
            }
            return AddDynamicProperty(ctx, prop);
        }

        internal static bool RemoveDynamicProperty(Context ctx, string name)
        {
            if (ctx != null)
            {
                return ctx.RemovePerContextDynamicProperty(name);
            }
            return RemoveGlobalDynamicProperty(name);
        }

        private static bool RemoveGlobalDynamicProperty(string name) => 
            _dphGlobal.RemoveDynamicProperty(name);

        private bool RemovePerContextDynamicProperty(string name) => 
            this._dphCtx?.RemoveDynamicProperty(name);

        private int ReserveSlot()
        {
            if (this._ctxStatics == null)
            {
                this._ctxStatics = new object[8];
                this._ctxStatics[0] = null;
                this._ctxStaticsFreeIndex = 1;
                this._ctxStaticsCurrentBucket = 0;
            }
            if (this._ctxStaticsFreeIndex == 8)
            {
                object[] objArray = new object[8];
                object[] objArray2 = this._ctxStatics;
                while (objArray2[0] != null)
                {
                    objArray2 = (object[]) objArray2[0];
                }
                objArray2[0] = objArray;
                this._ctxStaticsFreeIndex = 1;
                this._ctxStaticsCurrentBucket++;
            }
            return (this._ctxStaticsFreeIndex++ | (this._ctxStaticsCurrentBucket << 0x10));
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public static void SetData(LocalDataStoreSlot slot, object data)
        {
            Thread.CurrentContext.MyLocalStore.SetData(slot, data);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public virtual void SetProperty(IContextProperty prop)
        {
            if ((prop == null) || (prop.Name == null))
            {
                throw new ArgumentNullException((prop == null) ? "prop" : "property name");
            }
            if ((this._ctxFlags & 2) != 0)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_AddContextFrozen"));
            }
            lock (this)
            {
                CheckPropertyNameClash(prop.Name, this._ctxProps, this._numCtxProps);
                if ((this._ctxProps == null) || (this._numCtxProps == this._ctxProps.Length))
                {
                    this._ctxProps = GrowPropertiesArray(this._ctxProps);
                }
                this._ctxProps[this._numCtxProps++] = prop;
            }
        }

        internal virtual void SetThreadPoolAware()
        {
            this._ctxFlags |= 4;
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void SetupInternalContext(bool bDefault);
        public override string ToString() => 
            ("ContextID: " + this._ctxID);

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.Infrastructure)]
        public static bool UnregisterDynamicProperty(string name, ContextBoundObject obj, Context ctx)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if ((obj != null) && (ctx != null))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_NonNullObjAndCtx"));
            }
            if (obj != null)
            {
                return IdentityHolder.RemoveDynamicProperty(obj, name);
            }
            return RemoveDynamicProperty(ctx, name);
        }

        internal virtual System.AppDomain AppDomain =>
            this._appDomain;

        public virtual int ContextID =>
            this._ctxID;

        public virtual IContextProperty[] ContextProperties
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
            get
            {
                if (this._ctxProps == null)
                {
                    return null;
                }
                lock (this)
                {
                    IContextProperty[] destinationArray = new IContextProperty[this._numCtxProps];
                    Array.Copy(this._ctxProps, destinationArray, this._numCtxProps);
                    return destinationArray;
                }
            }
        }

        public static Context DefaultContext =>
            Thread.GetDomain().GetDefaultContext();

        internal virtual ArrayWithSize DynamicSinks =>
            this._dphCtx?.DynamicSinks;

        internal static ArrayWithSize GlobalDynamicSinks =>
            _dphGlobal.DynamicSinks;

        internal virtual IntPtr InternalContextID =>
            this._internalContext;

        internal bool IsDefaultContext =>
            (this._ctxID == 0);

        internal virtual bool IsThreadPoolAware =>
            ((this._ctxFlags & 4) == 4);

        private LocalDataStore MyLocalStore
        {
            get
            {
                if (this._localDataStore == null)
                {
                    lock (_localDataStoreMgr)
                    {
                        if (this._localDataStore == null)
                        {
                            this._localDataStore = _localDataStoreMgr.CreateLocalDataStore();
                        }
                    }
                }
                return this._localDataStore;
            }
        }

        internal virtual IDynamicProperty[] PerContextDynamicProperties =>
            this._dphCtx?.DynamicProperties;
    }
}

