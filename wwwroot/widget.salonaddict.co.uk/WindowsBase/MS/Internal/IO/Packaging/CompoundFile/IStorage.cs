namespace MS.Internal.IO.Packaging.CompoundFile
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    internal interface IStorage
    {
        void Commit(int grfCommitFlags);
        void CopyTo(int ciidExclude, Guid[] rgiidExclude, IntPtr snbExclude, IStorage ppstg);
        int CreateStorage(string pwcsName, int grfMode, int reserved1, int reserved2, out IStorage ppstg);
        int CreateStream(string pwcsName, int grfMode, int reserved1, int reserved2, out MS.Internal.IO.Packaging.CompoundFile.IStream ppstm);
        void DestroyElement(string pwcsName);
        void EnumElements(int reserved1, IntPtr reserved2, int reserved3, out IEnumSTATSTG ppEnum);
        void MoveElementTo(string pwcsName, IStorage pstgDest, string pwcsNewName, int grfFlags);
        int OpenStorage(string pwcsName, IStorage pstgPriority, int grfMode, IntPtr snbExclude, int reserved, out IStorage ppstg);
        int OpenStream(string pwcsName, int reserved1, int grfMode, int reserved2, out MS.Internal.IO.Packaging.CompoundFile.IStream ppstm);
        void RenameElement(string pwcsOldName, string pwcsNewName);
        void Revert();
        void SetClass(ref Guid clsid);
        void SetElementTimes(string pwcsName, System.Runtime.InteropServices.ComTypes.FILETIME pctime, System.Runtime.InteropServices.ComTypes.FILETIME patime, System.Runtime.InteropServices.ComTypes.FILETIME pmtime);
        void SetStateBits(int grfStateBits, int grfMask);
        void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag);
    }
}

