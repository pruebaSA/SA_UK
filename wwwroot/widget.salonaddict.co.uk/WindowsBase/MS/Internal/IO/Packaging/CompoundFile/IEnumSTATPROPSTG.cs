namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal.Interop;
    using System;
    using System.Runtime.InteropServices;

    internal interface IEnumSTATPROPSTG
    {
        void Clone(out IEnumSTATPROPSTG ppenum);
        int Next(uint celt, STATPROPSTG rgelt, out uint pceltFetched);
        void Reset();
        void Skip(uint celt);
    }
}

