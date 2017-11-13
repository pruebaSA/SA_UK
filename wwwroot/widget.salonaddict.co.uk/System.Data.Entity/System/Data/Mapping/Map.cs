namespace System.Data.Mapping
{
    using System;
    using System.Data.Metadata.Edm;

    internal abstract class Map : GlobalItem
    {
        protected Map() : base(MetadataItem.MetadataFlags.Readonly)
        {
        }

        internal abstract MetadataItem EdmItem { get; }
    }
}

