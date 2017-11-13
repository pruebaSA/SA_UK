namespace System.Runtime.Remoting.Lifetime
{
    using System;
    using System.Globalization;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Contexts;
    using System.Runtime.Remoting.Messaging;

    [Serializable]
    internal class LeaseLifeTimeServiceProperty : IContextProperty, IContributeObjectSink
    {
        public void Freeze(Context newContext)
        {
        }

        public IMessageSink GetObjectSink(MarshalByRefObject obj, IMessageSink nextSink)
        {
            bool flag;
            ServerIdentity identity = (ServerIdentity) MarshalByRefObject.GetIdentity(obj, out flag);
            if (!identity.IsSingleCall())
            {
                object obj2 = obj.InitializeLifetimeService();
                if (obj2 == null)
                {
                    return nextSink;
                }
                if (!(obj2 is ILease))
                {
                    throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_Lifetime_ILeaseReturn"), new object[] { obj2 }));
                }
                ILease lease = (ILease) obj2;
                if (lease.InitialLeaseTime.CompareTo(TimeSpan.Zero) <= 0)
                {
                    if (lease is Lease)
                    {
                        ((Lease) lease).Remove();
                    }
                    return nextSink;
                }
                Lease leaseInitial = null;
                lock (identity)
                {
                    if (identity.Lease != null)
                    {
                        leaseInitial = identity.Lease;
                        leaseInitial.Renew(leaseInitial.InitialLeaseTime);
                    }
                    else
                    {
                        if (lease is Lease)
                        {
                            leaseInitial = (Lease) lease;
                        }
                        else
                        {
                            leaseInitial = (Lease) LifetimeServices.GetLeaseInitial(obj);
                            if (leaseInitial.CurrentState == LeaseState.Initial)
                            {
                                leaseInitial.InitialLeaseTime = lease.InitialLeaseTime;
                                leaseInitial.RenewOnCallTime = lease.RenewOnCallTime;
                                leaseInitial.SponsorshipTimeout = lease.SponsorshipTimeout;
                            }
                        }
                        identity.Lease = leaseInitial;
                        if (identity.ObjectRef != null)
                        {
                            leaseInitial.ActivateLease();
                        }
                    }
                }
                if (leaseInitial.RenewOnCallTime > TimeSpan.Zero)
                {
                    return new LeaseSink(leaseInitial, nextSink);
                }
            }
            return nextSink;
        }

        public bool IsNewContextOK(Context newCtx) => 
            true;

        public string Name =>
            "LeaseLifeTimeServiceProperty";
    }
}

