namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Threading;

    [Serializable, ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public sealed class CallContext
    {
        private CallContext()
        {
        }

        public static void FreeNamedDataSlot(string name)
        {
            Thread.CurrentThread.GetLogicalCallContext().FreeNamedDataSlot(name);
            Thread.CurrentThread.GetIllogicalCallContext().FreeNamedDataSlot(name);
        }

        public static object GetData(string name)
        {
            object obj2 = LogicalGetData(name);
            if (obj2 == null)
            {
                return IllogicalGetData(name);
            }
            return obj2;
        }

        public static Header[] GetHeaders() => 
            Thread.CurrentThread.GetLogicalCallContext().InternalGetHeaders();

        internal static LogicalCallContext GetLogicalCallContext() => 
            Thread.CurrentThread.GetLogicalCallContext();

        private static object IllogicalGetData(string name) => 
            Thread.CurrentThread.GetIllogicalCallContext().GetData(name);

        public static object LogicalGetData(string name) => 
            Thread.CurrentThread.GetLogicalCallContext().GetData(name);

        public static void LogicalSetData(string name, object data)
        {
            Thread.CurrentThread.GetIllogicalCallContext().FreeNamedDataSlot(name);
            Thread.CurrentThread.GetLogicalCallContext().SetData(name, data);
        }

        public static void SetData(string name, object data)
        {
            if (data is ILogicalThreadAffinative)
            {
                LogicalSetData(name, data);
            }
            else
            {
                Thread.CurrentThread.GetLogicalCallContext().FreeNamedDataSlot(name);
                Thread.CurrentThread.GetIllogicalCallContext().SetData(name, data);
            }
        }

        public static void SetHeaders(Header[] headers)
        {
            Thread.CurrentThread.GetLogicalCallContext().InternalSetHeaders(headers);
        }

        internal static LogicalCallContext SetLogicalCallContext(LogicalCallContext callCtx) => 
            Thread.CurrentThread.SetLogicalCallContext(callCtx);

        internal static LogicalCallContext SetLogicalCallContext(Thread currThread, LogicalCallContext callCtx) => 
            currThread.SetLogicalCallContext(callCtx);

        public static object HostContext
        {
            get
            {
                object hostContext = Thread.CurrentThread.GetIllogicalCallContext().HostContext;
                if (hostContext == null)
                {
                    hostContext = GetLogicalCallContext().HostContext;
                }
                return hostContext;
            }
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
            set
            {
                if (value is ILogicalThreadAffinative)
                {
                    Thread.CurrentThread.GetIllogicalCallContext().HostContext = null;
                    GetLogicalCallContext().HostContext = value;
                }
                else
                {
                    GetLogicalCallContext().HostContext = null;
                    Thread.CurrentThread.GetIllogicalCallContext().HostContext = value;
                }
            }
        }

        internal static IPrincipal Principal
        {
            get => 
                GetLogicalCallContext().Principal;
            set
            {
                GetLogicalCallContext().Principal = value;
            }
        }

        internal static CallContextRemotingData RemotingData =>
            Thread.CurrentThread.GetLogicalCallContext().RemotingData;

        internal static CallContextSecurityData SecurityData =>
            Thread.CurrentThread.GetLogicalCallContext().SecurityData;
    }
}

