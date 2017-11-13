namespace MS.Internal.IO.Packaging
{
    using MS.Internal;
    using System;
    using System.Collections.Generic;

    internal sealed class PieceNameComparer : IComparer<PieceInfo>
    {
        int IComparer<PieceInfo>.Compare(PieceInfo pieceInfoA, PieceInfo pieceInfoB)
        {
            Invariant.Assert(pieceInfoA != null);
            Invariant.Assert(pieceInfoB != null);
            int num = string.Compare(pieceInfoA.NormalizedPrefixName, pieceInfoB.NormalizedPrefixName, StringComparison.Ordinal);
            if (num != 0)
            {
                return num;
            }
            return (pieceInfoA.PieceNumber - pieceInfoB.PieceNumber);
        }
    }
}

