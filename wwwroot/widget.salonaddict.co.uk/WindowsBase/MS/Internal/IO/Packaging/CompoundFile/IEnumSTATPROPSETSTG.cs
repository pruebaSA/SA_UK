namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal.Interop;
    using System;
    using System.Runtime.InteropServices;

    internal interface IEnumSTATPROPSETSTG
    {
        void Clone(out IEnumSTATPROPSETSTG ppenum);
        int Next(uint celt, STATPROPSETSTG rgelt, out uint pceltFetched);
        void Reset();
        void Skip(uint celt);
    }
}

