namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security.Permissions;

    [ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class ReturnMessage : IMethodReturnMessage, IMethodMessage, IMessage
    {
        internal ArgMapper _argMapper;
        internal System.Runtime.Remoting.Messaging.LogicalCallContext _callContext;
        internal System.Exception _e;
        internal bool _hasVarArgs;
        internal System.Reflection.MethodBase _methodBase;
        internal string _methodName;
        internal Type[] _methodSignature;
        internal object[] _outArgs;
        internal int _outArgsCount;
        internal object _properties;
        internal object _ret;
        internal string _typeName;
        internal string _URI;

        public ReturnMessage(System.Exception e, IMethodCallMessage mcm)
        {
            this._e = IsCustomErrorEnabled() ? new RemotingException(Environment.GetResourceString("Remoting_InternalError")) : e;
            this._callContext = CallContext.GetLogicalCallContext();
            if (mcm != null)
            {
                this._URI = mcm.Uri;
                this._methodName = mcm.MethodName;
                this._methodSignature = null;
                this._typeName = mcm.TypeName;
                this._hasVarArgs = mcm.HasVarArgs;
                this._methodBase = mcm.MethodBase;
            }
        }

        public ReturnMessage(object ret, object[] outArgs, int outArgsCount, System.Runtime.Remoting.Messaging.LogicalCallContext callCtx, IMethodCallMessage mcm)
        {
            this._ret = ret;
            this._outArgs = outArgs;
            this._outArgsCount = outArgsCount;
            if (callCtx != null)
            {
                this._callContext = callCtx;
            }
            else
            {
                this._callContext = CallContext.GetLogicalCallContext();
            }
            if (mcm != null)
            {
                this._URI = mcm.Uri;
                this._methodName = mcm.MethodName;
                this._methodSignature = null;
                this._typeName = mcm.TypeName;
                this._hasVarArgs = mcm.HasVarArgs;
                this._methodBase = mcm.MethodBase;
            }
        }

        public object GetArg(int argNum)
        {
            if (this._outArgs == null)
            {
                if ((argNum < 0) || (argNum >= this._outArgsCount))
                {
                    throw new ArgumentOutOfRangeException("argNum");
                }
                return null;
            }
            if ((argNum < 0) || (argNum >= this._outArgs.Length))
            {
                throw new ArgumentOutOfRangeException("argNum");
            }
            return this._outArgs[argNum];
        }

        public string GetArgName(int index)
        {
            if (this._outArgs == null)
            {
                if ((index < 0) || (index >= this._outArgsCount))
                {
                    throw new ArgumentOutOfRangeException("index");
                }
            }
            else if ((index < 0) || (index >= this._outArgs.Length))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (this._methodBase != null)
            {
                return InternalRemotingServices.GetReflectionCachedData(this._methodBase).Parameters[index].Name;
            }
            return ("__param" + index);
        }

        internal System.Runtime.Remoting.Messaging.LogicalCallContext GetLogicalCallContext()
        {
            if (this._callContext == null)
            {
                this._callContext = new System.Runtime.Remoting.Messaging.LogicalCallContext();
            }
            return this._callContext;
        }

        public object GetOutArg(int argNum) => 
            this._argMapper?.GetArg(argNum);

        public string GetOutArgName(int index) => 
            this._argMapper?.GetArgName(index);

        internal bool HasProperties() => 
            (this._properties != null);

        internal static bool IsCustomErrorEnabled()
        {
            object data = CallContext.GetData("__CustomErrorsEnabled");
            return ((data != null) && ((bool) data));
        }

        internal System.Runtime.Remoting.Messaging.LogicalCallContext SetLogicalCallContext(System.Runtime.Remoting.Messaging.LogicalCallContext ctx)
        {
            System.Runtime.Remoting.Messaging.LogicalCallContext context = this._callContext;
            this._callContext = ctx;
            return context;
        }

        public int ArgCount =>
            this._outArgs?.Length;

        public object[] Args
        {
            get
            {
                if (this._outArgs == null)
                {
                    return new object[this._outArgsCount];
                }
                return this._outArgs;
            }
        }

        public System.Exception Exception =>
            this._e;

        public bool HasVarArgs =>
            this._hasVarArgs;

        public System.Runtime.Remoting.Messaging.LogicalCallContext LogicalCallContext =>
            this.GetLogicalCallContext();

        public System.Reflection.MethodBase MethodBase =>
            this._methodBase;

        public string MethodName =>
            this._methodName;

        public object MethodSignature
        {
            get
            {
                if ((this._methodSignature == null) && (this._methodBase != null))
                {
                    this._methodSignature = Message.GenerateMethodSignature(this._methodBase);
                }
                return this._methodSignature;
            }
        }

        public int OutArgCount =>
            this._argMapper?.ArgCount;

        public object[] OutArgs =>
            this._argMapper?.Args;

        public virtual IDictionary Properties
        {
            get
            {
                if (this._properties == null)
                {
                    this._properties = new MRMDictionary(this, null);
                }
                return (MRMDictionary) this._properties;
            }
        }

        public virtual object ReturnValue =>
            this._ret;

        public string TypeName =>
            this._typeName;

        public string Uri
        {
            get => 
                this._URI;
            set
            {
                this._URI = value;
            }
        }
    }
}

