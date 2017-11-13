namespace MS.Internal.IO.Packaging.CompoundFile
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;

    internal interface IEnumSTATSTG
    {
        void Clone(out IEnumSTATSTG ppenum);
        void Next(uint celt, out System.Runtime.InteropServices.ComTypes.STATSTG rgelt, out uint pceltFetched);
        void Reset();
        void Skip(uint celt);
    }
}

