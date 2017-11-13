namespace System.Runtime.Remoting.Lifetime
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security.Permissions;
    using System.Threading;

    [ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public sealed class LifetimeServices
    {
        private static bool isLeaseTime = false;
        private static bool isRenewOnCallTime = false;
        private static bool isSponsorshipTimeout = false;
        private static TimeSpan m_leaseTime = TimeSpan.FromMinutes(5.0);
        private static TimeSpan m_pollTime = TimeSpan.FromMilliseconds(10000.0);
        private static TimeSpan m_renewOnCallTime = TimeSpan.FromMinutes(2.0);
        private static TimeSpan m_sponsorshipTimeout = TimeSpan.FromMinutes(2.0);
        private static object s_LifetimeSyncObject = null;

        internal static ILease CreateLease(MarshalByRefObject obj) => 
            CreateLease(LeaseTime, RenewOnCallTime, SponsorshipTimeout, obj);

        internal static ILease CreateLease(TimeSpan leaseTime, TimeSpan renewOnCallTime, TimeSpan sponsorshipTimeout, MarshalByRefObject obj)
        {
            LeaseManager.GetLeaseManager(LeaseManagerPollTime);
            return new Lease(leaseTime, renewOnCallTime, sponsorshipTimeout, obj);
        }

        internal static ILease GetLease(MarshalByRefObject obj) => 
            LeaseManager.GetLeaseManager(LeaseManagerPollTime).GetLease(obj);

        internal static ILease GetLeaseInitial(MarshalByRefObject obj)
        {
            ILease lease = null;
            lease = LeaseManager.GetLeaseManager(LeaseManagerPollTime).GetLease(obj);
            if (lease == null)
            {
                lease = CreateLease(obj);
            }
            return lease;
        }

        public static TimeSpan LeaseManagerPollTime
        {
            get => 
                m_pollTime;
            [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
            set
            {
                lock (LifetimeSyncObject)
                {
                    m_pollTime = value;
                    if (LeaseManager.IsInitialized())
                    {
                        LeaseManager.GetLeaseManager().ChangePollTime(m_pollTime);
                    }
                }
            }
        }

        public static TimeSpan LeaseTime
        {
            get => 
                m_leaseTime;
            [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
            set
            {
                lock (LifetimeSyncObject)
                {
                    if (isLeaseTime)
                    {
                        throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Lifetime_SetOnce"), new object[] { "LeaseTime" }));
                    }
                    m_leaseTime = value;
                    isLeaseTime = true;
                }
            }
        }

        private static object LifetimeSyncObject
        {
            get
            {
                if (s_LifetimeSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_LifetimeSyncObject, obj2, null);
                }
                return s_LifetimeSyncObject;
            }
        }

        public static TimeSpan RenewOnCallTime
        {
            get => 
                m_renewOnCallTime;
            [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
            set
            {
                lock (LifetimeSyncObject)
                {
                    if (isRenewOnCallTime)
                    {
                        throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Lifetime_SetOnce"), new object[] { "RenewOnCallTime" }));
                    }
                    m_renewOnCallTime = value;
                    isRenewOnCallTime = true;
                }
            }
        }

        public static TimeSpan SponsorshipTimeout
        {
            get => 
                m_sponsorshipTimeout;
            [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.RemotingConfiguration)]
            set
            {
                lock (LifetimeSyncObject)
                {
                    if (isSponsorshipTimeout)
                    {
                        throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Lifetime_SetOnce"), new object[] { "SponsorshipTimeout" }));
                    }
                    m_sponsorshipTimeout = value;
                    isSponsorshipTimeout = true;
                }
            }
        }
    }
}

