namespace MS.Internal.IO.Packaging.CompoundFile
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    internal interface IStream
    {
        void Clone(out MS.Internal.IO.Packaging.CompoundFile.IStream ppstm);
        void Commit(int grfCommitFlags);
        void CopyTo(MS.Internal.IO.Packaging.CompoundFile.IStream pstm, long cb, out long pcbRead, out long pcbWritten);
        void LockRegion(long libOffset, long cb, int dwLockType);
        void Read(byte[] pv, int cb, out int pcbRead);
        void Revert();
        void Seek(long dlibMove, int dwOrigin, out long plibNewPosition);
        void SetSize(long libNewSize);
        void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag);
        void UnlockRegion(long libOffset, long cb, int dwLockType);
        void Write(byte[] pv, int cb, out int pcbWritten);
    }
}

