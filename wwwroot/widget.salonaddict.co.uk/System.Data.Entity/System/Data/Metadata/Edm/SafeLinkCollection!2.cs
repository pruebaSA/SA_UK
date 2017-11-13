namespace System.Data.Metadata.Edm
{
    using System;

    internal class SafeLinkCollection<TParent, TChild> : ReadOnlyMetadataCollection<TChild> where TParent: class where TChild: MetadataItem
    {
        public SafeLinkCollection(TParent parent, Func<TChild, SafeLink<TParent>> getLink, MetadataCollection<TChild> children) : base((IList<TChild>) SafeLink<TParent>.BindChildren<TChild>(parent, getLink, children))
        {
        }
    }
}

