namespace System.Web.Hosting
{
    using System;
    using System.Collections;
    using System.Web;

    internal class MapPathBasedVirtualDirectory : VirtualDirectory
    {
        public MapPathBasedVirtualDirectory(string virtualPath) : base(virtualPath)
        {
        }

        public override IEnumerable Children =>
            new MapPathBasedVirtualPathCollection(VirtualPath.CreateNonRelative(base.VirtualPath), RequestedEntryType.All);

        public override IEnumerable Directories =>
            new MapPathBasedVirtualPathCollection(VirtualPath.CreateNonRelative(base.VirtualPath), RequestedEntryType.Directories);

        public override IEnumerable Files =>
            new MapPathBasedVirtualPathCollection(VirtualPath.CreateNonRelative(base.VirtualPath), RequestedEntryType.Files);
    }
}

