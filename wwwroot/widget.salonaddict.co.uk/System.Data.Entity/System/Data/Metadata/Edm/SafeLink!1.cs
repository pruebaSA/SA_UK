namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;

    internal class SafeLink<TParent> where TParent: class
    {
        private TParent _value;

        internal static TChild BindChild<TChild>(TParent parent, Func<TChild, SafeLink<TParent>> getLink, TChild child)
        {
            getLink(child)._value = parent;
            return child;
        }

        internal static IEnumerable<TChild> BindChildren<TChild>(TParent parent, Func<TChild, SafeLink<TParent>> getLink, IEnumerable<TChild> children)
        {
            foreach (TChild local in children)
            {
                SafeLink<TParent>.BindChild<TChild>(parent, getLink, local);
            }
            return children;
        }

        public TParent Value =>
            this._value;
    }
}

