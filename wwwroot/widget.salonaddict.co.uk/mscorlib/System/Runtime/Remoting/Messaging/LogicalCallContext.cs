namespace System.Runtime.Remoting.Messaging
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Security.Principal;

    [Serializable, ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public sealed class LogicalCallContext : ISerializable, ICloneable
    {
        private Header[] _recvHeaders;
        private Header[] _sendHeaders;
        private Hashtable m_Datastore;
        private object m_HostContext;
        private bool m_IsCorrelationMgr;
        private CallContextRemotingData m_RemotingData;
        private CallContextSecurityData m_SecurityData;
        private static Type s_callContextType = typeof(LogicalCallContext);
        private const string s_CorrelationMgrSlotName = "System.Diagnostics.Trace.CorrelationManagerSlot";

        internal LogicalCallContext()
        {
        }

        internal LogicalCallContext(SerializationInfo info, StreamingContext context)
        {
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Name.Equals("__RemotingData"))
                {
                    this.m_RemotingData = (CallContextRemotingData) enumerator.Value;
                }
                else
                {
                    if (enumerator.Name.Equals("__SecurityData"))
                    {
                        if (context.State == StreamingContextStates.CrossAppDomain)
                        {
                            this.m_SecurityData = (CallContextSecurityData) enumerator.Value;
                        }
                        continue;
                    }
                    if (enumerator.Name.Equals("__HostContext"))
                    {
                        this.m_HostContext = enumerator.Value;
                        continue;
                    }
                    if (enumerator.Name.Equals("__CorrelationMgrSlotPresent"))
                    {
                        this.m_IsCorrelationMgr = (bool) enumerator.Value;
                        continue;
                    }
                    this.Datastore[enumerator.Name] = enumerator.Value;
                }
            }
        }

        public object Clone()
        {
            LogicalCallContext context = new LogicalCallContext();
            if (this.m_RemotingData != null)
            {
                context.m_RemotingData = (CallContextRemotingData) this.m_RemotingData.Clone();
            }
            if (this.m_SecurityData != null)
            {
                context.m_SecurityData = (CallContextSecurityData) this.m_SecurityData.Clone();
            }
            if (this.m_HostContext != null)
            {
                context.m_HostContext = this.m_HostContext;
            }
            context.m_IsCorrelationMgr = this.m_IsCorrelationMgr;
            if (this.HasUserData)
            {
                IDictionaryEnumerator enumerator = this.m_Datastore.GetEnumerator();
                if (this.m_IsCorrelationMgr)
                {
                    while (enumerator.MoveNext())
                    {
                        string key = (string) enumerator.Key;
                        if (key.Equals("System.Diagnostics.Trace.CorrelationManagerSlot"))
                        {
                            context.Datastore[key] = ((ICloneable) enumerator.Value).Clone();
                        }
                        else
                        {
                            context.Datastore[key] = enumerator.Value;
                        }
                    }
                    return context;
                }
                while (enumerator.MoveNext())
                {
                    context.Datastore[(string) enumerator.Key] = enumerator.Value;
                }
            }
            return context;
        }

        public void FreeNamedDataSlot(string name)
        {
            this.Datastore.Remove(name);
        }

        public object GetData(string name) => 
            this.Datastore[name];

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.SetType(s_callContextType);
            if (this.m_RemotingData != null)
            {
                info.AddValue("__RemotingData", this.m_RemotingData);
            }
            if ((this.m_SecurityData != null) && (context.State == StreamingContextStates.CrossAppDomain))
            {
                info.AddValue("__SecurityData", this.m_SecurityData);
            }
            if (this.m_HostContext != null)
            {
                info.AddValue("__HostContext", this.m_HostContext);
            }
            if (this.m_IsCorrelationMgr)
            {
                info.AddValue("__CorrelationMgrSlotPresent", this.m_IsCorrelationMgr);
            }
            if (this.HasUserData)
            {
                IDictionaryEnumerator enumerator = this.m_Datastore.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    info.AddValue((string) enumerator.Key, enumerator.Value);
                }
            }
        }

        internal static string GetPropertyKeyForHeader(Header header)
        {
            if (header == null)
            {
                return null;
            }
            if (header.HeaderNamespace != null)
            {
                return (header.Name + ", " + header.HeaderNamespace);
            }
            return header.Name;
        }

        internal Header[] InternalGetHeaders()
        {
            if (this._sendHeaders != null)
            {
                return this._sendHeaders;
            }
            return this._recvHeaders;
        }

        private Header[] InternalGetOutgoingHeaders()
        {
            Header[] headerArray = this._sendHeaders;
            this._sendHeaders = null;
            this._recvHeaders = null;
            return headerArray;
        }

        internal void InternalSetHeaders(Header[] headers)
        {
            this._sendHeaders = headers;
            this._recvHeaders = null;
        }

        internal void Merge(LogicalCallContext lc)
        {
            if (((lc != null) && (this != lc)) && lc.HasUserData)
            {
                IDictionaryEnumerator enumerator = lc.Datastore.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    this.Datastore[(string) enumerator.Key] = enumerator.Value;
                }
            }
        }

        internal void PropagateIncomingHeadersToCallContext(IMessage msg)
        {
            IInternalMessage message = msg as IInternalMessage;
            if ((message == null) || message.HasProperties())
            {
                IDictionaryEnumerator enumerator = msg.Properties.GetEnumerator();
                int num = 0;
                while (enumerator.MoveNext())
                {
                    string key = (string) enumerator.Key;
                    if (!key.StartsWith("__", StringComparison.Ordinal) && (enumerator.Value is Header))
                    {
                        num++;
                    }
                }
                Header[] headerArray = null;
                if (num > 0)
                {
                    headerArray = new Header[num];
                    num = 0;
                    enumerator.Reset();
                    while (enumerator.MoveNext())
                    {
                        string str2 = (string) enumerator.Key;
                        if (!str2.StartsWith("__", StringComparison.Ordinal))
                        {
                            Header header = enumerator.Value as Header;
                            if (header != null)
                            {
                                headerArray[num++] = header;
                            }
                        }
                    }
                }
                this._recvHeaders = headerArray;
                this._sendHeaders = null;
            }
        }

        internal void PropagateOutgoingHeadersToMessage(IMessage msg)
        {
            Header[] outgoingHeaders = this.InternalGetOutgoingHeaders();
            if (outgoingHeaders != null)
            {
                IDictionary properties = msg.Properties;
                foreach (Header header in outgoingHeaders)
                {
                    if (header != null)
                    {
                        string propertyKeyForHeader = GetPropertyKeyForHeader(header);
                        properties[propertyKeyForHeader] = header;
                    }
                }
            }
        }

        internal IPrincipal RemovePrincipalIfNotSerializable()
        {
            IPrincipal principal = this.Principal;
            if ((principal != null) && !principal.GetType().IsSerializable)
            {
                this.Principal = null;
            }
            return principal;
        }

        public void SetData(string name, object data)
        {
            this.Datastore[name] = data;
            if (name.Equals("System.Diagnostics.Trace.CorrelationManagerSlot"))
            {
                this.m_IsCorrelationMgr = true;
            }
        }

        private Hashtable Datastore
        {
            get
            {
                if (this.m_Datastore == null)
                {
                    this.m_Datastore = new Hashtable();
                }
                return this.m_Datastore;
            }
        }

        public bool HasInfo
        {
            get
            {
                bool flag = false;
                if ((((this.m_RemotingData == null) || !this.m_RemotingData.HasInfo) && ((this.m_SecurityData == null) || !this.m_SecurityData.HasInfo)) && ((this.m_HostContext == null) && !this.HasUserData))
                {
                    return flag;
                }
                return true;
            }
        }

        private bool HasUserData =>
            ((this.m_Datastore != null) && (this.m_Datastore.Count > 0));

        internal object HostContext
        {
            get => 
                this.m_HostContext;
            set
            {
                this.m_HostContext = value;
            }
        }

        internal IPrincipal Principal
        {
            get
            {
                if (this.m_SecurityData != null)
                {
                    return this.m_SecurityData.Principal;
                }
                return null;
            }
            set
            {
                this.SecurityData.Principal = value;
            }
        }

        internal CallContextRemotingData RemotingData
        {
            get
            {
                if (this.m_RemotingData == null)
                {
                    this.m_RemotingData = new CallContextRemotingData();
                }
                return this.m_RemotingData;
            }
        }

        internal CallContextSecurityData SecurityData
        {
            get
            {
                if (this.m_SecurityData == null)
                {
                    this.m_SecurityData = new CallContextSecurityData();
                }
                return this.m_SecurityData;
            }
        }
    }
}

