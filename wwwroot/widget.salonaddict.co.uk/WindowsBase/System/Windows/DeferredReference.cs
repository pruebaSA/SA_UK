namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;

    [FriendAccessAllowed]
    internal abstract class DeferredReference
    {
        protected DeferredReference()
        {
        }

        internal abstract object GetValue(BaseValueSourceInternal valueSource);
        internal abstract Type GetValueType();
    }
}

