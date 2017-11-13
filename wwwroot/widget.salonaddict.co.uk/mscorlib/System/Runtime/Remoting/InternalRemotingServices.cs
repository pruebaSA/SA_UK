namespace System.Runtime.Remoting
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Cache;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Metadata;
    using System.Security.Permissions;

    [ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class InternalRemotingServices
    {
        [Conditional("_LOGGING")]
        public static void DebugOutChnl(string s)
        {
            Message.OutToUnmanagedDebugger("CHNL:" + s + "\n");
        }

        public static SoapAttribute GetCachedSoapAttribute(object reflectionObject)
        {
            MemberInfo mi = reflectionObject as MemberInfo;
            if (mi != null)
            {
                return GetReflectionCachedData(mi).GetSoapAttribute();
            }
            return GetReflectionCachedData((ParameterInfo) reflectionObject).GetSoapAttribute();
        }

        internal static RemotingCachedData GetReflectionCachedData(MemberInfo mi)
        {
            RemotingCachedData data = null;
            data = (RemotingCachedData) mi.Cache[CacheObjType.RemotingData];
            if (data == null)
            {
                if (mi is MethodBase)
                {
                    data = new RemotingMethodCachedData(mi);
                }
                else if (mi is Type)
                {
                    data = new RemotingTypeCachedData(mi);
                }
                else
                {
                    data = new RemotingCachedData(mi);
                }
                mi.Cache[CacheObjType.RemotingData] = data;
            }
            return data;
        }

        internal static RemotingMethodCachedData GetReflectionCachedData(MethodBase mi)
        {
            RemotingMethodCachedData data = null;
            data = (RemotingMethodCachedData) mi.Cache[CacheObjType.RemotingData];
            if (data == null)
            {
                data = new RemotingMethodCachedData(mi);
                mi.Cache[CacheObjType.RemotingData] = data;
            }
            return data;
        }

        internal static RemotingCachedData GetReflectionCachedData(ParameterInfo reflectionObject)
        {
            RemotingCachedData data = null;
            data = (RemotingCachedData) reflectionObject.Cache[CacheObjType.RemotingData];
            if (data == null)
            {
                data = new RemotingCachedData(reflectionObject);
                reflectionObject.Cache[CacheObjType.RemotingData] = data;
            }
            return data;
        }

        internal static RemotingTypeCachedData GetReflectionCachedData(Type mi)
        {
            RemotingTypeCachedData data = null;
            data = (RemotingTypeCachedData) mi.Cache[CacheObjType.RemotingData];
            if (data == null)
            {
                data = new RemotingTypeCachedData(mi);
                mi.Cache[CacheObjType.RemotingData] = data;
            }
            return data;
        }

        [Conditional("_DEBUG")]
        public static void RemotingAssert(bool condition, string message)
        {
        }

        [Conditional("_LOGGING")]
        public static void RemotingTrace(params object[] messages)
        {
        }

        [CLSCompliant(false)]
        public static void SetServerIdentity(MethodCall m, object srvID)
        {
            IInternalMessage message = m;
            message.ServerIdentityObject = (ServerIdentity) srvID;
        }
    }
}

