namespace System.Data.Metadata.Edm
{
    using System;

    public abstract class GlobalItem : MetadataItem
    {
        internal GlobalItem()
        {
        }

        internal GlobalItem(MetadataItem.MetadataFlags flags) : base(flags)
        {
        }

        [MetadataProperty(typeof(System.Data.Metadata.Edm.DataSpace), false)]
        internal System.Data.Metadata.Edm.DataSpace DataSpace
        {
            get => 
                base.GetDataSpace();
            set
            {
                base.SetDataSpace(value);
            }
        }
    }
}

