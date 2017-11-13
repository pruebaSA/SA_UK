namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;

    internal static class Util
    {
        internal static void CheckItemHasIdentity(MetadataItem item, string argumentName)
        {
            EntityUtil.GenericCheckArgumentNull<MetadataItem>(item, argumentName);
            if (string.IsNullOrEmpty(item.Identity))
            {
                throw EntityUtil.EmptyIdentity(argumentName);
            }
        }

        internal static void ThrowIfReadOnly(MetadataItem item)
        {
            if (item.IsReadOnly)
            {
                throw EntityUtil.OperationOnReadOnlyItem();
            }
        }
    }
}

