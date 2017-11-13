namespace System.Runtime.Serialization.Formatters.Binary
{
    using System;

    internal interface IStreamable
    {
        void Read(__BinaryParser input);
        void Write(__BinaryWriter sout);
    }
}

