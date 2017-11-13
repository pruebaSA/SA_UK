namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal.Interop;
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    internal interface IPropertyStorage
    {
        void Commit(uint grfCommitFlags);
        void DeleteMultiple(uint cpspec, PROPSPEC[] rgpspec);
        void DeletePropertyNames(uint cpropid, uint[] rgpropid);
        void Enum(out IEnumSTATPROPSTG ppenum);
        int ReadMultiple(uint cpspec, PROPSPEC[] rgpspec, PROPVARIANT[] rgpropvar);
        void ReadPropertyNames(uint cpropid, uint[] rgpropid, string[] rglpwstrName);
        void Revert();
        void SetClass(ref Guid clsid);
        void SetTimes(ref System.Runtime.InteropServices.ComTypes.FILETIME pctime, ref System.Runtime.InteropServices.ComTypes.FILETIME patime, ref System.Runtime.InteropServices.ComTypes.FILETIME pmtime);
        void Stat(out STATPROPSETSTG pstatpsstg);
        void WriteMultiple(uint cpspec, PROPSPEC[] rgpspec, PROPVARIANT[] rgpropvar, uint propidNameFirst);
        void WritePropertyNames(uint cpropid, uint[] rgpropid, string[] rglpwstrName);
    }
}

