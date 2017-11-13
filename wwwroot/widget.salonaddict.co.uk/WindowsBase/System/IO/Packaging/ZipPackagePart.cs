namespace System.IO.Packaging
{
    using MS.Internal;
    using MS.Internal.IO.Packaging;
    using MS.Internal.IO.Zip;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public sealed class ZipPackagePart : PackagePart
    {
        private List<PieceInfo> _pieces;
        private ZipArchive _zipArchive;
        private MS.Internal.IO.Zip.ZipFileInfo _zipFileInfo;

        internal ZipPackagePart(ZipPackage container, ZipArchive zipArchive, PackUriHelper.ValidatedPartUri partUri, string contentType, CompressionOption compressionOption) : base(container, partUri, contentType, compressionOption)
        {
            this._zipArchive = zipArchive;
        }

        internal ZipPackagePart(ZipPackage container, MS.Internal.IO.Zip.ZipFileInfo zipFileInfo, PackUriHelper.ValidatedPartUri partUri, string contentType, CompressionOption compressionOption) : base(container, partUri, contentType, compressionOption)
        {
            this._zipArchive = zipFileInfo.ZipArchive;
            this._zipFileInfo = zipFileInfo;
        }

        internal ZipPackagePart(ZipPackage container, ZipArchive zipArchive, List<PieceInfo> pieces, PackUriHelper.ValidatedPartUri partUri, string contentType, CompressionOption compressionOption) : base(container, partUri, contentType, compressionOption)
        {
            this._zipArchive = zipArchive;
            this._pieces = pieces;
        }

        protected override Stream GetStreamCore(FileMode mode, FileAccess access)
        {
            if (base.Package.InStreamingCreation)
            {
                CompressionMethodEnum enum2;
                DeflateOptionEnum enum3;
                ZipPackage.GetZipCompressionMethodFromOpcCompressionOption(base.CompressionOption, out enum2, out enum3);
                return new StreamingZipPartStream(PackUriHelper.GetStringForPartUri(base.Uri), this._zipArchive, enum2, enum3, mode, access);
            }
            if (this._zipFileInfo != null)
            {
                return this._zipFileInfo.GetStream(mode, access);
            }
            Invariant.Assert(this._pieces != null);
            return new InterleavedZipPartStream(this, mode, access);
        }

        internal List<PieceInfo> PieceDescriptors =>
            this._pieces;

        internal MS.Internal.IO.Zip.ZipFileInfo ZipFileInfo =>
            this._zipFileInfo;
    }
}

