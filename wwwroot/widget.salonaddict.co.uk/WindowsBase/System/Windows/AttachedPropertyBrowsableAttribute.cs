namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;

    public abstract class AttachedPropertyBrowsableAttribute : Attribute
    {
        protected AttachedPropertyBrowsableAttribute()
        {
        }

        [FriendAccessAllowed]
        internal abstract bool IsBrowsable(DependencyObject d, DependencyProperty dp);

        internal virtual bool UnionResults =>
            false;
    }
}

