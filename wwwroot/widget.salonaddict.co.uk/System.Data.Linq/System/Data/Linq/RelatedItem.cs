namespace System.Data.Linq
{
    using System;
    using System.Data.Linq.Mapping;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct RelatedItem
    {
        internal MetaType Type;
        internal object Item;
        internal RelatedItem(MetaType type, object item)
        {
            this.Type = type;
            this.Item = item;
        }
    }
}

