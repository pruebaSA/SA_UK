namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.Remoting;

    internal class StackBasedReturnMessage : IMethodReturnMessage, IMethodMessage, IMessage, IInternalMessage
    {
        private ArgMapper _argMapper;
        private MRMDictionary _d;
        private Hashtable _h;
        private Message _m;

        internal StackBasedReturnMessage()
        {
        }

        public object GetArg(int argNum) => 
            this._m.GetArg(argNum);

        public string GetArgName(int index) => 
            this._m.GetArgName(index);

        internal System.Runtime.Remoting.Messaging.LogicalCallContext GetLogicalCallContext() => 
            this._m.GetLogicalCallContext();

        public object GetOutArg(int argNum) => 
            this._argMapper?.GetArg(argNum);

        public string GetOutArgName(int index) => 
            this._argMapper?.GetArgName(index);

        internal void InitFields(Message m)
        {
            this._m = m;
            if (this._h != null)
            {
                this._h.Clear();
            }
            if (this._d != null)
            {
                this._d.Clear();
            }
        }

        internal System.Runtime.Remoting.Messaging.LogicalCallContext SetLogicalCallContext(System.Runtime.Remoting.Messaging.LogicalCallContext callCtx) => 
            this._m.SetLogicalCallContext(callCtx);

        bool IInternalMessage.HasProperties() => 
            (this._h != null);

        void IInternalMessage.SetCallContext(System.Runtime.Remoting.Messaging.LogicalCallContext newCallContext)
        {
            this._m.SetLogicalCallContext(newCallContext);
        }

        void IInternalMessage.SetURI(string val)
        {
            this._m.Uri = val;
        }

        public int ArgCount =>
            this._m.ArgCount;

        public object[] Args =>
            this._m.Args;

        public System.Exception Exception =>
            null;

        public bool HasVarArgs =>
            this._m.HasVarArgs;

        public System.Runtime.Remoting.Messaging.LogicalCallContext LogicalCallContext =>
            this._m.GetLogicalCallContext();

        public System.Reflection.MethodBase MethodBase =>
            this._m.MethodBase;

        public string MethodName =>
            this._m.MethodName;

        public object MethodSignature =>
            this._m.MethodSignature;

        public int OutArgCount =>
            this._argMapper?.ArgCount;

        public object[] OutArgs =>
            this._argMapper?.Args;

        public IDictionary Properties
        {
            get
            {
                lock (this)
                {
                    if (this._h == null)
                    {
                        this._h = new Hashtable();
                    }
                    if (this._d == null)
                    {
                        this._d = new MRMDictionary(this, this._h);
                    }
                    return this._d;
                }
            }
        }

        public object ReturnValue =>
            this._m.GetReturnValue();

        Identity IInternalMessage.IdentityObject
        {
            get => 
                null;
            set
            {
            }
        }

        ServerIdentity IInternalMessage.ServerIdentityObject
        {
            get => 
                null;
            set
            {
            }
        }

        public string TypeName =>
            this._m.TypeName;

        public string Uri =>
            this._m.Uri;
    }
}

