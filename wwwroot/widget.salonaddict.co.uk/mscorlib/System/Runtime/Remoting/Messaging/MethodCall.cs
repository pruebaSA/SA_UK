﻿namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Metadata;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Permissions;

    [Serializable, CLSCompliant(false), ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class MethodCall : IMethodCallMessage, IMethodMessage, IMessage, ISerializable, IInternalMessage, ISerializationRootObject
    {
        private ArgMapper argMapper;
        private object[] args;
        private System.Runtime.Remoting.Messaging.LogicalCallContext callContext;
        protected IDictionary ExternalProperties;
        private bool fSoap;
        private bool fVarArgs;
        private Identity identity;
        private Type[] instArgs;
        protected IDictionary InternalProperties;
        private const BindingFlags LookupAll = (BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        private const BindingFlags LookupPublic = (BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        private string methodName;
        private Type[] methodSignature;
        private System.Reflection.MethodBase MI;
        private ServerIdentity srvID;
        private string typeName;
        private string uri;

        public MethodCall(Header[] h1)
        {
            this.Init();
            this.fSoap = true;
            this.FillHeaders(h1);
            this.ResolveMethod();
        }

        public MethodCall(IMessage msg)
        {
            if (msg == null)
            {
                throw new ArgumentNullException("msg");
            }
            this.Init();
            IDictionaryEnumerator enumerator = msg.Properties.GetEnumerator();
            while (enumerator.MoveNext())
            {
                this.FillHeader(enumerator.Key.ToString(), enumerator.Value);
            }
            IMethodCallMessage message = msg as IMethodCallMessage;
            if (message != null)
            {
                this.MI = message.MethodBase;
            }
            this.ResolveMethod();
        }

        internal MethodCall(object handlerObject, BinaryMethodCallMessage smuggledMsg)
        {
            if (handlerObject != null)
            {
                this.uri = handlerObject as string;
                if (this.uri == null)
                {
                    MarshalByRefObject obj2 = handlerObject as MarshalByRefObject;
                    if (obj2 != null)
                    {
                        bool flag;
                        this.srvID = MarshalByRefObject.GetIdentity(obj2, out flag) as ServerIdentity;
                        this.uri = this.srvID.URI;
                    }
                }
            }
            this.typeName = smuggledMsg.TypeName;
            this.methodName = smuggledMsg.MethodName;
            this.methodSignature = (Type[]) smuggledMsg.MethodSignature;
            this.args = smuggledMsg.Args;
            this.instArgs = smuggledMsg.InstantiationArgs;
            this.callContext = smuggledMsg.LogicalCallContext;
            this.ResolveMethod();
            if (smuggledMsg.HasProperties)
            {
                smuggledMsg.PopulateMessageProperties(this.Properties);
            }
        }

        internal MethodCall(SmuggledMethodCallMessage smuggledMsg, ArrayList deserializedArgs)
        {
            this.uri = smuggledMsg.Uri;
            this.typeName = smuggledMsg.TypeName;
            this.methodName = smuggledMsg.MethodName;
            this.methodSignature = (Type[]) smuggledMsg.GetMethodSignature(deserializedArgs);
            this.args = smuggledMsg.GetArgs(deserializedArgs);
            this.instArgs = smuggledMsg.GetInstantiation(deserializedArgs);
            this.callContext = smuggledMsg.GetCallContext(deserializedArgs);
            this.ResolveMethod();
            if (smuggledMsg.MessagePropertyCount > 0)
            {
                smuggledMsg.PopulateMessageProperties(this.Properties, deserializedArgs);
            }
        }

        internal MethodCall(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            this.Init();
            this.SetObjectData(info, context);
        }

        internal void FillHeader(string key, object value)
        {
            if (!this.FillSpecialHeader(key, value))
            {
                if (this.InternalProperties == null)
                {
                    this.InternalProperties = new Hashtable();
                }
                this.InternalProperties[key] = value;
            }
        }

        internal void FillHeaders(Header[] h)
        {
            this.FillHeaders(h, false);
        }

        private void FillHeaders(Header[] h, bool bFromHeaderHandler)
        {
            if (h != null)
            {
                if (bFromHeaderHandler && this.fSoap)
                {
                    for (int i = 0; i < h.Length; i++)
                    {
                        Header header = h[i];
                        if (header.HeaderNamespace == "http://schemas.microsoft.com/clr/soap/messageProperties")
                        {
                            this.FillHeader(header.Name, header.Value);
                        }
                        else
                        {
                            string propertyKeyForHeader = System.Runtime.Remoting.Messaging.LogicalCallContext.GetPropertyKeyForHeader(header);
                            this.FillHeader(propertyKeyForHeader, header);
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < h.Length; j++)
                    {
                        this.FillHeader(h[j].Name, h[j].Value);
                    }
                }
            }
        }

        internal virtual bool FillSpecialHeader(string key, object value)
        {
            if (key != null)
            {
                if (key.Equals("__Uri"))
                {
                    this.uri = (string) value;
                }
                else if (key.Equals("__MethodName"))
                {
                    this.methodName = (string) value;
                }
                else if (key.Equals("__MethodSignature"))
                {
                    this.methodSignature = (Type[]) value;
                }
                else if (key.Equals("__TypeName"))
                {
                    this.typeName = (string) value;
                }
                else if (key.Equals("__Args"))
                {
                    this.args = (object[]) value;
                }
                else
                {
                    if (!key.Equals("__CallContext"))
                    {
                        return false;
                    }
                    if (value is string)
                    {
                        this.callContext = new System.Runtime.Remoting.Messaging.LogicalCallContext();
                        this.callContext.RemotingData.LogicalCallID = (string) value;
                    }
                    else
                    {
                        this.callContext = (System.Runtime.Remoting.Messaging.LogicalCallContext) value;
                    }
                }
            }
            return true;
        }

        public object GetArg(int argNum) => 
            this.args[argNum];

        public string GetArgName(int index)
        {
            this.ResolveMethod();
            return InternalRemotingServices.GetReflectionCachedData(this.MI).Parameters[index].Name;
        }

        public object GetInArg(int argNum) => 
            this.argMapper?.GetArg(argNum);

        public string GetInArgName(int index) => 
            this.argMapper?.GetArgName(index);

        internal System.Runtime.Remoting.Messaging.LogicalCallContext GetLogicalCallContext()
        {
            if (this.callContext == null)
            {
                this.callContext = new System.Runtime.Remoting.Messaging.LogicalCallContext();
            }
            return this.callContext;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_Method"));
        }

        public virtual object HeaderHandler(Header[] h)
        {
            SerializationMonkey uninitializedObject = (SerializationMonkey) FormatterServices.GetUninitializedObject(typeof(SerializationMonkey));
            Header[] destinationArray = null;
            if (((h != null) && (h.Length > 0)) && (h[0].Name == "__methodName"))
            {
                this.methodName = (string) h[0].Value;
                if (h.Length > 1)
                {
                    destinationArray = new Header[h.Length - 1];
                    Array.Copy(h, 1, destinationArray, 0, h.Length - 1);
                }
                else
                {
                    destinationArray = null;
                }
            }
            else
            {
                destinationArray = h;
            }
            this.FillHeaders(destinationArray, true);
            this.ResolveMethod(false);
            uninitializedObject._obj = this;
            if (this.MI != null)
            {
                ArgMapper mapper = new ArgMapper(this.MI, false);
                uninitializedObject.fieldNames = mapper.ArgNames;
                uninitializedObject.fieldTypes = mapper.ArgTypes;
            }
            return uninitializedObject;
        }

        public virtual void Init()
        {
        }

        public void ResolveMethod()
        {
            this.ResolveMethod(true);
        }

        internal void ResolveMethod(bool bThrowIfNotResolved)
        {
            if ((this.MI == null) && (this.methodName != null))
            {
                RuntimeType t = this.ResolveType() as RuntimeType;
                if (!this.methodName.Equals(".ctor"))
                {
                    if (t == null)
                    {
                        throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_BadType"), new object[] { this.typeName }));
                    }
                    if (this.methodSignature != null)
                    {
                        try
                        {
                            this.MI = t.GetMethod(this.methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, CallingConventions.Any, this.methodSignature, null);
                        }
                        catch (AmbiguousMatchException)
                        {
                            MemberInfo[] infoArray = t.FindMembers(MemberTypes.Method, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, Type.FilterName, this.methodName);
                            int num = (this.instArgs == null) ? 0 : this.instArgs.Length;
                            int index = 0;
                            for (int i = 0; i < infoArray.Length; i++)
                            {
                                MethodInfo info = (MethodInfo) infoArray[i];
                                int num4 = info.IsGenericMethod ? info.GetGenericArguments().Length : 0;
                                if (num4 == num)
                                {
                                    if (i > index)
                                    {
                                        infoArray[index] = infoArray[i];
                                    }
                                    index++;
                                }
                            }
                            MethodInfo[] match = new MethodInfo[index];
                            for (int j = 0; j < index; j++)
                            {
                                match[j] = (MethodInfo) infoArray[j];
                            }
                            this.MI = Type.DefaultBinder.SelectMethod(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, match, this.methodSignature, null);
                        }
                        if ((this.instArgs != null) && (this.instArgs.Length > 0))
                        {
                            this.MI = ((MethodInfo) this.MI).MakeGenericMethod(this.instArgs);
                        }
                    }
                    else
                    {
                        RemotingTypeCachedData reflectionCachedData = null;
                        if (this.instArgs == null)
                        {
                            reflectionCachedData = InternalRemotingServices.GetReflectionCachedData((Type) t);
                            this.MI = reflectionCachedData.GetLastCalledMethod(this.methodName);
                            if (this.MI != null)
                            {
                                return;
                            }
                        }
                        bool flag = false;
                        try
                        {
                            this.MI = t.GetMethod(this.methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                            if ((this.instArgs != null) && (this.instArgs.Length > 0))
                            {
                                this.MI = ((MethodInfo) this.MI).MakeGenericMethod(this.instArgs);
                            }
                        }
                        catch (AmbiguousMatchException)
                        {
                            flag = true;
                            this.ResolveOverloadedMethod(t);
                        }
                        if (((this.MI != null) && !flag) && (reflectionCachedData != null))
                        {
                            reflectionCachedData.SetLastCalledMethod(this.methodName, this.MI);
                        }
                    }
                    if ((this.MI == null) && bThrowIfNotResolved)
                    {
                        throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Message_MethodMissing"), new object[] { this.methodName, this.typeName }));
                    }
                }
            }
        }

        private void ResolveOverloadedMethod(RuntimeType t)
        {
            if (this.args != null)
            {
                MemberInfo[] infoArray = t.GetMember(this.methodName, MemberTypes.Method, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                int length = infoArray.Length;
                switch (length)
                {
                    case 1:
                        this.MI = infoArray[0] as System.Reflection.MethodBase;
                        return;

                    case 0:
                        return;
                }
                int num2 = this.args.Length;
                System.Reflection.MethodBase base2 = null;
                for (int i = 0; i < length; i++)
                {
                    System.Reflection.MethodBase base3 = infoArray[i] as System.Reflection.MethodBase;
                    if (base3.GetParameters().Length == num2)
                    {
                        if (base2 != null)
                        {
                            throw new RemotingException(Environment.GetResourceString("Remoting_AmbiguousMethod"));
                        }
                        base2 = base3;
                    }
                }
                if (base2 != null)
                {
                    this.MI = base2;
                }
            }
        }

        private void ResolveOverloadedMethod(RuntimeType t, string methodName, ArrayList argNames, ArrayList argValues)
        {
            System.Reflection.MethodBase base2;
            MemberInfo[] infoArray = t.GetMember(methodName, MemberTypes.Method, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            int length = infoArray.Length;
            switch (length)
            {
                case 1:
                    this.MI = infoArray[0] as System.Reflection.MethodBase;
                    return;

                case 0:
                    return;

                default:
                    base2 = null;
                    for (int i = 0; i < length; i++)
                    {
                        System.Reflection.MethodBase base3 = infoArray[i] as System.Reflection.MethodBase;
                        ParameterInfo[] parameters = base3.GetParameters();
                        if (parameters.Length == argValues.Count)
                        {
                            bool flag = true;
                            for (int j = 0; j < parameters.Length; j++)
                            {
                                Type parameterType = parameters[j].ParameterType;
                                if (parameterType.IsByRef)
                                {
                                    parameterType = parameterType.GetElementType();
                                }
                                if (parameterType != argValues[j].GetType())
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                base2 = base3;
                                break;
                            }
                        }
                    }
                    break;
            }
            if (base2 == null)
            {
                throw new RemotingException(Environment.GetResourceString("Remoting_AmbiguousMethod"));
            }
            this.MI = base2;
        }

        internal Type ResolveType()
        {
            Type newType = null;
            if (this.srvID == null)
            {
                this.srvID = IdentityHolder.CasualResolveIdentity(this.uri) as ServerIdentity;
            }
            if (this.srvID != null)
            {
                Type lastCalledType = this.srvID.GetLastCalledType(this.typeName);
                if (lastCalledType != null)
                {
                    return lastCalledType;
                }
                int startIndex = 0;
                if (string.CompareOrdinal(this.typeName, 0, "clr:", 0, 4) == 0)
                {
                    startIndex = 4;
                }
                int index = this.typeName.IndexOf(',', startIndex);
                if (index == -1)
                {
                    index = this.typeName.Length;
                }
                lastCalledType = this.srvID.ServerType;
                newType = Type.ResolveTypeRelativeTo(this.typeName, startIndex, index - startIndex, lastCalledType);
            }
            if (newType == null)
            {
                newType = RemotingServices.InternalGetTypeFromQualifiedTypeName(this.typeName);
            }
            if (this.srvID != null)
            {
                this.srvID.SetLastCalledType(this.typeName, newType);
            }
            return newType;
        }

        public void RootSetObjectData(SerializationInfo info, StreamingContext ctx)
        {
            this.SetObjectData(info, ctx);
        }

        internal System.Runtime.Remoting.Messaging.LogicalCallContext SetLogicalCallContext(System.Runtime.Remoting.Messaging.LogicalCallContext ctx)
        {
            System.Runtime.Remoting.Messaging.LogicalCallContext callContext = this.callContext;
            this.callContext = ctx;
            return callContext;
        }

        internal void SetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            if (this.fSoap)
            {
                this.SetObjectFromSoapData(info);
            }
            else
            {
                SerializationInfoEnumerator enumerator = info.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    this.FillHeader(enumerator.Name, enumerator.Value);
                }
                if ((context.State == StreamingContextStates.Remoting) && (context.Context != null))
                {
                    Header[] headerArray = context.Context as Header[];
                    if (headerArray != null)
                    {
                        for (int i = 0; i < headerArray.Length; i++)
                        {
                            this.FillHeader(headerArray[i].Name, headerArray[i].Value);
                        }
                    }
                }
            }
        }

        internal void SetObjectFromSoapData(SerializationInfo info)
        {
            this.methodName = info.GetString("__methodName");
            ArrayList list = (ArrayList) info.GetValue("__paramNameList", typeof(ArrayList));
            Hashtable keyToNamespaceTable = (Hashtable) info.GetValue("__keyToNamespaceTable", typeof(Hashtable));
            if (this.MI == null)
            {
                ArrayList argValues = new ArrayList();
                ArrayList argNames = list;
                for (int i = 0; i < argNames.Count; i++)
                {
                    argValues.Add(info.GetValue((string) argNames[i], typeof(object)));
                }
                RuntimeType t = this.ResolveType() as RuntimeType;
                if (t == null)
                {
                    throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_BadType"), new object[] { this.typeName }));
                }
                this.ResolveOverloadedMethod(t, this.methodName, argNames, argValues);
                if (this.MI == null)
                {
                    throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Message_MethodMissing"), new object[] { this.methodName, this.typeName }));
                }
            }
            RemotingMethodCachedData reflectionCachedData = InternalRemotingServices.GetReflectionCachedData(this.MI);
            ParameterInfo[] parameters = reflectionCachedData.Parameters;
            int[] marshalRequestArgMap = reflectionCachedData.MarshalRequestArgMap;
            int[] outOnlyArgMap = reflectionCachedData.OutOnlyArgMap;
            object obj2 = this.InternalProperties?["__UnorderedParams"];
            this.args = new object[parameters.Length];
            if (((obj2 != null) && (obj2 is bool)) && ((bool) obj2))
            {
                for (int j = 0; j < list.Count; j++)
                {
                    string name = (string) list[j];
                    int index = -1;
                    for (int k = 0; k < parameters.Length; k++)
                    {
                        if (name.Equals(parameters[k].Name))
                        {
                            index = parameters[k].Position;
                            break;
                        }
                    }
                    if (index == -1)
                    {
                        if (!name.StartsWith("__param", StringComparison.Ordinal))
                        {
                            throw new RemotingException(Environment.GetResourceString("Remoting_Message_BadSerialization"));
                        }
                        index = int.Parse(name.Substring(7), CultureInfo.InvariantCulture);
                    }
                    if (index >= this.args.Length)
                    {
                        throw new RemotingException(Environment.GetResourceString("Remoting_Message_BadSerialization"));
                    }
                    this.args[index] = Message.SoapCoerceArg(info.GetValue(name, typeof(object)), parameters[index].ParameterType, keyToNamespaceTable);
                }
            }
            else
            {
                for (int m = 0; m < list.Count; m++)
                {
                    string str2 = (string) list[m];
                    this.args[marshalRequestArgMap[m]] = Message.SoapCoerceArg(info.GetValue(str2, typeof(object)), parameters[marshalRequestArgMap[m]].ParameterType, keyToNamespaceTable);
                }
                foreach (int num6 in outOnlyArgMap)
                {
                    Type elementType = parameters[num6].ParameterType.GetElementType();
                    if (elementType.IsValueType)
                    {
                        this.args[num6] = Activator.CreateInstance(elementType, true);
                    }
                }
            }
        }

        bool IInternalMessage.HasProperties()
        {
            if (this.ExternalProperties == null)
            {
                return (this.InternalProperties != null);
            }
            return true;
        }

        void IInternalMessage.SetCallContext(System.Runtime.Remoting.Messaging.LogicalCallContext newCallContext)
        {
            this.callContext = newCallContext;
        }

        void IInternalMessage.SetURI(string val)
        {
            this.uri = val;
        }

        public int ArgCount
        {
            get
            {
                if (this.args != null)
                {
                    return this.args.Length;
                }
                return 0;
            }
        }

        public object[] Args =>
            this.args;

        public bool HasVarArgs =>
            this.fVarArgs;

        public int InArgCount =>
            this.argMapper?.ArgCount;

        public object[] InArgs =>
            this.argMapper?.Args;

        public System.Runtime.Remoting.Messaging.LogicalCallContext LogicalCallContext =>
            this.GetLogicalCallContext();

        public System.Reflection.MethodBase MethodBase
        {
            get
            {
                if (this.MI == null)
                {
                    this.MI = RemotingServices.InternalGetMethodBaseFromMethodMessage(this);
                }
                return this.MI;
            }
        }

        public string MethodName =>
            this.methodName;

        public object MethodSignature
        {
            get
            {
                if (this.methodSignature != null)
                {
                    return this.methodSignature;
                }
                if (this.MI != null)
                {
                    this.methodSignature = Message.GenerateMethodSignature(this.MethodBase);
                }
                return null;
            }
        }

        public virtual IDictionary Properties
        {
            get
            {
                lock (this)
                {
                    if (this.InternalProperties == null)
                    {
                        this.InternalProperties = new Hashtable();
                    }
                    if (this.ExternalProperties == null)
                    {
                        this.ExternalProperties = new MCMDictionary(this, this.InternalProperties);
                    }
                    return this.ExternalProperties;
                }
            }
        }

        Identity IInternalMessage.IdentityObject
        {
            get => 
                this.identity;
            set
            {
                this.identity = value;
            }
        }

        ServerIdentity IInternalMessage.ServerIdentityObject
        {
            get => 
                this.srvID;
            set
            {
                this.srvID = value;
            }
        }

        public string TypeName =>
            this.typeName;

        public string Uri
        {
            get => 
                this.uri;
            set
            {
                this.uri = value;
            }
        }
    }
}

