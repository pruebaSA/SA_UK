namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security.Permissions;

    [ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class MethodReturnMessageWrapper : InternalMessageWrapper, IMethodReturnMessage, IMethodMessage, IMessage
    {
        private ArgMapper _argMapper;
        private object[] _args;
        private System.Exception _exception;
        private IMethodReturnMessage _msg;
        private IDictionary _properties;
        private object _returnValue;

        public MethodReturnMessageWrapper(IMethodReturnMessage msg) : base(msg)
        {
            this._msg = msg;
            this._args = this._msg.Args;
            this._returnValue = this._msg.ReturnValue;
            this._exception = this._msg.Exception;
        }

        public virtual object GetArg(int argNum) => 
            this._args[argNum];

        public virtual string GetArgName(int index) => 
            this._msg.GetArgName(index);

        public virtual object GetOutArg(int argNum) => 
            this._argMapper?.GetArg(argNum);

        public virtual string GetOutArgName(int index) => 
            this._argMapper?.GetArgName(index);

        public virtual int ArgCount
        {
            get
            {
                if (this._args != null)
                {
                    return this._args.Length;
                }
                return 0;
            }
        }

        public virtual object[] Args
        {
            get => 
                this._args;
            set
            {
                this._args = value;
            }
        }

        public virtual System.Exception Exception
        {
            get => 
                this._exception;
            set
            {
                this._exception = value;
            }
        }

        public virtual bool HasVarArgs =>
            this._msg.HasVarArgs;

        public virtual System.Runtime.Remoting.Messaging.LogicalCallContext LogicalCallContext =>
            this._msg.LogicalCallContext;

        public virtual System.Reflection.MethodBase MethodBase =>
            this._msg.MethodBase;

        public virtual string MethodName =>
            this._msg.MethodName;

        public virtual object MethodSignature =>
            this._msg.MethodSignature;

        public virtual int OutArgCount =>
            this._argMapper?.ArgCount;

        public virtual object[] OutArgs =>
            this._argMapper?.Args;

        public virtual IDictionary Properties
        {
            get
            {
                if (this._properties == null)
                {
                    this._properties = new MRMWrapperDictionary(this, this._msg.Properties);
                }
                return this._properties;
            }
        }

        public virtual object ReturnValue
        {
            get => 
                this._returnValue;
            set
            {
                this._returnValue = value;
            }
        }

        public virtual string TypeName =>
            this._msg.TypeName;

        public string Uri
        {
            get => 
                this._msg.Uri;
            set
            {
                this._msg.Properties[Message.UriKey] = value;
            }
        }

        private class MRMWrapperDictionary : Hashtable
        {
            private IDictionary _idict;
            private IMethodReturnMessage _mrmsg;

            public MRMWrapperDictionary(IMethodReturnMessage msg, IDictionary idict)
            {
                this._mrmsg = msg;
                this._idict = idict;
            }

            public override object this[object key]
            {
                get
                {
                    string str2;
                    string str = key as string;
                    if ((str != null) && ((str2 = str) != null))
                    {
                        if (str2 == "__Uri")
                        {
                            return this._mrmsg.Uri;
                        }
                        if (str2 == "__MethodName")
                        {
                            return this._mrmsg.MethodName;
                        }
                        if (str2 == "__MethodSignature")
                        {
                            return this._mrmsg.MethodSignature;
                        }
                        if (str2 == "__TypeName")
                        {
                            return this._mrmsg.TypeName;
                        }
                        if (str2 == "__Return")
                        {
                            return this._mrmsg.ReturnValue;
                        }
                        if (str2 == "__OutArgs")
                        {
                            return this._mrmsg.OutArgs;
                        }
                    }
                    return this._idict[key];
                }
                set
                {
                    string str = key as string;
                    if (str != null)
                    {
                        string str2;
                        if (((str2 = str) != null) && (((str2 == "__MethodName") || (str2 == "__MethodSignature")) || (((str2 == "__TypeName") || (str2 == "__Return")) || (str2 == "__OutArgs"))))
                        {
                            throw new RemotingException(Environment.GetResourceString("Remoting_Default"));
                        }
                        this._idict[key] = value;
                    }
                }
            }
        }
    }
}

