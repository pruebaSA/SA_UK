namespace System.Web.Security
{
    using System;
    using System.Configuration.Provider;
    using System.Reflection;
    using System.Web;

    public sealed class MembershipProviderCollection : ProviderCollection
    {
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }
            if (!(provider is MembershipProvider))
            {
                throw new ArgumentException(System.Web.SR.GetString("Provider_must_implement_type", new object[] { typeof(MembershipProvider).ToString() }), "provider");
            }
            base.Add(provider);
        }

        public void CopyTo(MembershipProvider[] array, int index)
        {
            base.CopyTo(array, index);
        }

        public MembershipProvider this[string name] =>
            ((MembershipProvider) base[name]);
    }
}

