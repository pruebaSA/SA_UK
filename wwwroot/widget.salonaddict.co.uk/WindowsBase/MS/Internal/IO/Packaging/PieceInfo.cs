namespace MS.Internal.IO.Packaging
{
    using MS.Internal.IO.Zip;
    using System;
    using System.IO.Packaging;

    internal class PieceInfo
    {
        private bool _isLastPiece;
        private string _normalizedPieceNamePrefix;
        private PackUriHelper.ValidatedPartUri _partUri;
        private int _pieceNumber;
        private string _prefixName;
        private MS.Internal.IO.Zip.ZipFileInfo _zipFileInfo;

        internal PieceInfo(MS.Internal.IO.Zip.ZipFileInfo zipFileInfo, PackUriHelper.ValidatedPartUri partUri, string prefixName, int pieceNumber, bool isLastPiece)
        {
            this._zipFileInfo = zipFileInfo;
            this._partUri = partUri;
            this._prefixName = prefixName;
            this._pieceNumber = pieceNumber;
            this._isLastPiece = isLastPiece;
            this._normalizedPieceNamePrefix = this._prefixName.ToUpperInvariant();
        }

        internal bool IsLastPiece =>
            this._isLastPiece;

        internal string NormalizedPrefixName =>
            this._normalizedPieceNamePrefix;

        internal PackUriHelper.ValidatedPartUri PartUri =>
            this._partUri;

        internal int PieceNumber =>
            this._pieceNumber;

        internal string PrefixName =>
            this._prefixName;

        internal MS.Internal.IO.Zip.ZipFileInfo ZipFileInfo =>
            this._zipFileInfo;
    }
}

