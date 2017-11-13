namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;
    using System.ServiceModel.Dispatcher;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal sealed class ServiceChannelProxy : RealProxy, IRemotingTypeInfo
    {
        private const string activityIdSlotName = "E2ETrace.ActivityID";
        private Type interfaceType;
        private MethodDataCache methodDataCache;
        private MbrObject objectWrapper;
        private Type proxiedType;
        private ImmutableClientRuntime proxyRuntime;
        private ServiceChannel serviceChannel;

        internal ServiceChannelProxy(Type interfaceType, Type proxiedType, MessageDirection direction, ServiceChannel serviceChannel) : base(proxiedType)
        {
            if (!MessageDirectionHelper.IsDefined(direction))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("direction"));
            }
            this.interfaceType = interfaceType;
            this.proxiedType = proxiedType;
            this.serviceChannel = serviceChannel;
            this.proxyRuntime = serviceChannel.ClientRuntime.GetRuntime();
            this.methodDataCache = new MethodDataCache();
            this.objectWrapper = new MbrObject(this, proxiedType);
        }

        private IMethodReturnMessage CreateReturnMessage(Exception e, IMethodCallMessage mcm) => 
            new ReturnMessage(e, mcm);

        private IMethodReturnMessage CreateReturnMessage(object ret, object[] returnArgs, IMethodCallMessage methodCall)
        {
            if (returnArgs != null)
            {
                return this.CreateReturnMessage(ret, returnArgs, returnArgs.Length, SetActivityIdInLogicalCallContext(methodCall.LogicalCallContext), methodCall);
            }
            return new SingleReturnMessage(ret, methodCall);
        }

        private IMethodReturnMessage CreateReturnMessage(object ret, object[] outArgs, int outArgsCount, LogicalCallContext callCtx, IMethodCallMessage mcm) => 
            new ReturnMessage(ret, outArgs, outArgsCount, callCtx, mcm);

        private IMethodReturnMessage ExecuteMessage(object target, IMethodCallMessage methodCall)
        {
            MethodBase methodBase = methodCall.MethodBase;
            object[] args = methodCall.Args;
            object ret = null;
            try
            {
                ret = methodBase.Invoke(target, args);
            }
            catch (TargetInvocationException exception)
            {
                return this.CreateReturnMessage(exception.InnerException, methodCall);
            }
            return this.CreateReturnMessage(ret, args, args.Length, null, methodCall);
        }

        private MethodData GetMethodData(IMethodCallMessage methodCall)
        {
            MethodData data;
            MethodBase methodBase = methodCall.MethodBase;
            if (!this.methodDataCache.TryGetMethodData(methodBase, out data))
            {
                bool flag;
                Type declaringType = methodBase.DeclaringType;
                if (declaringType == typeof(object))
                {
                    MethodType getType;
                    if (methodCall.MethodBase == typeof(object).GetMethod("GetType"))
                    {
                        getType = MethodType.GetType;
                    }
                    else
                    {
                        getType = MethodType.Object;
                    }
                    flag = true;
                    data = new MethodData(methodBase, getType);
                }
                else if (declaringType.IsInstanceOfType(this.serviceChannel))
                {
                    flag = true;
                    data = new MethodData(methodBase, MethodType.Channel);
                }
                else
                {
                    MethodType service;
                    ProxyOperationRuntime operation = this.proxyRuntime.GetOperation(methodBase, methodCall.Args, out flag);
                    if (operation == null)
                    {
                        if (this.serviceChannel.Factory != null)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("SFxMethodNotSupported1", new object[] { methodBase.Name })));
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("SFxMethodNotSupportedOnCallback1", new object[] { methodBase.Name })));
                    }
                    if (operation.IsSyncCall(methodCall))
                    {
                        service = MethodType.Service;
                    }
                    else if (operation.IsBeginCall(methodCall))
                    {
                        service = MethodType.BeginService;
                    }
                    else
                    {
                        service = MethodType.EndService;
                    }
                    data = new MethodData(methodBase, service, operation);
                }
                if (flag)
                {
                    this.methodDataCache.SetMethodData(data);
                }
            }
            return data;
        }

        internal ServiceChannel GetServiceChannel() => 
            this.serviceChannel;

        public override IMessage Invoke(IMessage message)
        {
            IMessage message3;
            try
            {
                IMethodCallMessage methodCall = message as IMethodCallMessage;
                if (methodCall == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("SFxExpectedIMethodCallMessage")));
                }
                MethodData methodData = this.GetMethodData(methodCall);
                switch (methodData.MethodType)
                {
                    case MethodType.Service:
                        return this.InvokeService(methodCall, methodData.Operation);

                    case MethodType.BeginService:
                        return this.InvokeBeginService(methodCall, methodData.Operation);

                    case MethodType.EndService:
                        return this.InvokeEndService(methodCall, methodData.Operation);

                    case MethodType.Channel:
                        return this.InvokeChannel(methodCall);

                    case MethodType.Object:
                        return this.InvokeObject(methodCall);

                    case MethodType.GetType:
                        return this.InvokeGetType(methodCall);
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Invalid proxy method type", new object[0])));
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                message3 = this.CreateReturnMessage(exception, message as IMethodCallMessage);
            }
            return message3;
        }

        private IMethodReturnMessage InvokeBeginService(IMethodCallMessage methodCall, ProxyOperationRuntime operation)
        {
            AsyncCallback callback;
            object obj2;
            object[] ins = operation.MapAsyncBeginInputs(methodCall, out callback, out obj2);
            object ret = this.serviceChannel.BeginCall(operation.Action, operation.IsOneWay, operation, ins, callback, obj2);
            return this.CreateReturnMessage(ret, null, methodCall);
        }

        private IMethodReturnMessage InvokeChannel(IMethodCallMessage methodCall)
        {
            string str = null;
            ActivityType unknown = ActivityType.Unknown;
            if (DiagnosticUtility.ShouldUseActivity && ((ServiceModelActivity.Current == null) || (ServiceModelActivity.Current.ActivityType != ActivityType.Close)))
            {
                MethodData methodData = this.GetMethodData(methodCall);
                if ((methodData.MethodBase.DeclaringType == typeof(ICommunicationObject)) && methodData.MethodBase.Name.Equals("Close", StringComparison.Ordinal))
                {
                    str = System.ServiceModel.SR.GetString("ActivityClose", new object[] { this.serviceChannel.GetType().FullName });
                    unknown = ActivityType.Close;
                }
            }
            using (ServiceModelActivity activity = string.IsNullOrEmpty(str) ? null : ServiceModelActivity.CreateBoundedActivity())
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, str, unknown);
                }
                return this.ExecuteMessage(this.serviceChannel, methodCall);
            }
        }

        private IMethodReturnMessage InvokeEndService(IMethodCallMessage methodCall, ProxyOperationRuntime operation)
        {
            IAsyncResult result;
            object[] objArray;
            operation.MapAsyncEndInputs(methodCall, out result, out objArray);
            object ret = this.serviceChannel.EndCall(operation.Action, objArray, result);
            object[] returnArgs = operation.MapAsyncOutputs(methodCall, objArray, ref ret);
            return this.CreateReturnMessage(ret, returnArgs, methodCall);
        }

        private IMethodReturnMessage InvokeGetType(IMethodCallMessage methodCall) => 
            this.CreateReturnMessage(this.proxiedType, null, 0, SetActivityIdInLogicalCallContext(methodCall.LogicalCallContext), methodCall);

        private IMethodReturnMessage InvokeObject(IMethodCallMessage methodCall) => 
            RemotingServices.ExecuteMessage(this.objectWrapper, methodCall);

        private IMethodReturnMessage InvokeService(IMethodCallMessage methodCall, ProxyOperationRuntime operation)
        {
            object[] objArray;
            object[] ins = operation.MapSyncInputs(methodCall, out objArray);
            object ret = this.serviceChannel.Call(operation.Action, operation.IsOneWay, operation, ins, objArray);
            object[] returnArgs = operation.MapSyncOutputs(methodCall, objArray, ref ret);
            return this.CreateReturnMessage(ret, returnArgs, methodCall);
        }

        private static LogicalCallContext SetActivityIdInLogicalCallContext(LogicalCallContext logicalCallContext)
        {
            logicalCallContext.SetData("E2ETrace.ActivityID", DiagnosticTrace.ActivityId);
            return logicalCallContext;
        }

        bool IRemotingTypeInfo.CanCastTo(Type toType, object o)
        {
            if (!toType.IsAssignableFrom(this.proxiedType))
            {
                return this.serviceChannel.CanCastTo(toType);
            }
            return true;
        }

        string IRemotingTypeInfo.TypeName
        {
            get => 
                this.proxiedType.FullName;
            set
            {
            }
        }

        private class MbrObject : MarshalByRefObject
        {
            private RealProxy proxy;
            private Type targetType;

            internal MbrObject(RealProxy proxy, Type targetType)
            {
                this.proxy = proxy;
                this.targetType = targetType;
            }

            public override bool Equals(object obj) => 
                object.ReferenceEquals(obj, this.proxy.GetTransparentProxy());

            public override int GetHashCode() => 
                this.proxy.GetHashCode();

            public override string ToString() => 
                this.targetType.ToString();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MethodData
        {
            private System.Reflection.MethodBase methodBase;
            private System.ServiceModel.Channels.ServiceChannelProxy.MethodType methodType;
            private ProxyOperationRuntime operation;
            public MethodData(System.Reflection.MethodBase methodBase, System.ServiceModel.Channels.ServiceChannelProxy.MethodType methodType) : this(methodBase, methodType, null)
            {
            }

            public MethodData(System.Reflection.MethodBase methodBase, System.ServiceModel.Channels.ServiceChannelProxy.MethodType methodType, ProxyOperationRuntime operation)
            {
                this.methodBase = methodBase;
                this.methodType = methodType;
                this.operation = operation;
            }

            public System.Reflection.MethodBase MethodBase =>
                this.methodBase;
            public System.ServiceModel.Channels.ServiceChannelProxy.MethodType MethodType =>
                this.methodType;
            public ProxyOperationRuntime Operation =>
                this.operation;
        }

        private class MethodDataCache
        {
            private ServiceChannelProxy.MethodData[] methodDatas = new ServiceChannelProxy.MethodData[4];

            private static int FindMethod(ServiceChannelProxy.MethodData[] methodDatas, MethodBase methodToFind)
            {
                for (int i = 0; i < methodDatas.Length; i++)
                {
                    MethodBase methodBase = methodDatas[i].MethodBase;
                    if (methodBase == null)
                    {
                        break;
                    }
                    if (methodBase == methodToFind)
                    {
                        return i;
                    }
                }
                return -1;
            }

            public void SetMethodData(ServiceChannelProxy.MethodData methodData)
            {
                lock (this.ThisLock)
                {
                    if (FindMethod(this.methodDatas, methodData.MethodBase) < 0)
                    {
                        for (int i = 0; i < this.methodDatas.Length; i++)
                        {
                            if (this.methodDatas[i].MethodBase == null)
                            {
                                this.methodDatas[i] = methodData;
                                goto Label_00A6;
                            }
                        }
                        ServiceChannelProxy.MethodData[] destinationArray = new ServiceChannelProxy.MethodData[this.methodDatas.Length * 2];
                        Array.Copy(this.methodDatas, destinationArray, this.methodDatas.Length);
                        destinationArray[this.methodDatas.Length] = methodData;
                        this.methodDatas = destinationArray;
                    }
                Label_00A6:;
                }
            }

            public bool TryGetMethodData(MethodBase method, out ServiceChannelProxy.MethodData methodData)
            {
                lock (this.ThisLock)
                {
                    ServiceChannelProxy.MethodData[] methodDatas = this.methodDatas;
                    int index = FindMethod(methodDatas, method);
                    if (index >= 0)
                    {
                        methodData = methodDatas[index];
                        return true;
                    }
                    methodData = new ServiceChannelProxy.MethodData();
                    return false;
                }
            }

            private object ThisLock =>
                this;
        }

        private enum MethodType
        {
            Service,
            BeginService,
            EndService,
            Channel,
            Object,
            GetType
        }

        private class SingleReturnMessage : IMethodReturnMessage, IMethodMessage, IMessage
        {
            private IMethodCallMessage methodCall;
            private PropertyDictionary properties;
            private object ret;

            public SingleReturnMessage(object ret, IMethodCallMessage methodCall)
            {
                this.ret = ret;
                this.methodCall = methodCall;
                this.properties = new PropertyDictionary();
            }

            public object GetArg(int index)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index"));
            }

            public string GetArgName(int index)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index"));
            }

            public object GetOutArg(int index)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index"));
            }

            public string GetOutArgName(int index)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index"));
            }

            public int ArgCount =>
                0;

            public object[] Args =>
                EmptyArray.Instance;

            public System.Exception Exception =>
                null;

            public bool HasVarArgs =>
                this.methodCall.HasVarArgs;

            public System.Runtime.Remoting.Messaging.LogicalCallContext LogicalCallContext =>
                ServiceChannelProxy.SetActivityIdInLogicalCallContext(this.methodCall.LogicalCallContext);

            public System.Reflection.MethodBase MethodBase =>
                this.methodCall.MethodBase;

            public string MethodName =>
                this.methodCall.MethodName;

            public object MethodSignature =>
                this.methodCall.MethodSignature;

            public int OutArgCount =>
                0;

            public object[] OutArgs =>
                EmptyArray.Instance;

            public IDictionary Properties =>
                this.properties;

            public object ReturnValue =>
                this.ret;

            public string TypeName =>
                this.methodCall.TypeName;

            public string Uri =>
                null;

            private class PropertyDictionary : IDictionary, ICollection, IEnumerable
            {
                private ListDictionary properties;

                public void Add(object key, object value)
                {
                    this.Properties.Add(key, value);
                }

                public void Clear()
                {
                    this.Properties.Clear();
                }

                public bool Contains(object key) => 
                    this.Properties.Contains(key);

                public void CopyTo(Array array, int index)
                {
                    this.Properties.CopyTo(array, index);
                }

                public IDictionaryEnumerator GetEnumerator() => 
                    this.properties?.GetEnumerator();

                public void Remove(object key)
                {
                    this.Properties.Remove(key);
                }

                IEnumerator IEnumerable.GetEnumerator() => 
                    ((IEnumerable) this.Properties).GetEnumerator();

                public int Count =>
                    this.Properties.Count;

                public bool IsFixedSize =>
                    false;

                public bool IsReadOnly =>
                    false;

                public bool IsSynchronized =>
                    false;

                public object this[object key]
                {
                    get => 
                        this.Properties[key];
                    set
                    {
                        this.Properties[key] = value;
                    }
                }

                public ICollection Keys =>
                    this.Properties.Keys;

                private ListDictionary Properties
                {
                    get
                    {
                        if (this.properties == null)
                        {
                            this.properties = new ListDictionary();
                        }
                        return this.properties;
                    }
                }

                public object SyncRoot =>
                    null;

                public ICollection Values =>
                    this.Properties.Values;

                private class EmptyEnumerator : IDictionaryEnumerator, IEnumerator
                {
                    private static ServiceChannelProxy.SingleReturnMessage.PropertyDictionary.EmptyEnumerator instance = new ServiceChannelProxy.SingleReturnMessage.PropertyDictionary.EmptyEnumerator();

                    private EmptyEnumerator()
                    {
                    }

                    public bool MoveNext() => 
                        false;

                    public void Reset()
                    {
                    }

                    public object Current
                    {
                        get
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxDictionaryIsEmpty")));
                        }
                    }

                    public DictionaryEntry Entry
                    {
                        get
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxDictionaryIsEmpty")));
                        }
                    }

                    public static ServiceChannelProxy.SingleReturnMessage.PropertyDictionary.EmptyEnumerator Instance =>
                        instance;

                    public object Key
                    {
                        get
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxDictionaryIsEmpty")));
                        }
                    }

                    public object Value
                    {
                        get
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxDictionaryIsEmpty")));
                        }
                    }
                }
            }
        }
    }
}

