namespace System.Runtime.Serialization.Formatters.Binary
{
    using System;
    using System.Diagnostics;

    internal sealed class MemberPrimitiveUnTyped : IStreamable
    {
        internal InternalPrimitiveTypeE typeInformation;
        internal object value;

        internal MemberPrimitiveUnTyped()
        {
        }

        public void Dump()
        {
        }

        [Conditional("_LOGGING")]
        private void DumpInternal()
        {
            if (BCLDebug.CheckEnabled("BINARY"))
            {
                Converter.ToComType(this.typeInformation);
            }
        }

        public void Read(__BinaryParser input)
        {
            this.value = input.ReadValue(this.typeInformation);
        }

        internal void Set(InternalPrimitiveTypeE typeInformation)
        {
            this.typeInformation = typeInformation;
        }

        internal void Set(InternalPrimitiveTypeE typeInformation, object value)
        {
            this.typeInformation = typeInformation;
            this.value = value;
        }

        public void Write(__BinaryWriter sout)
        {
            sout.WriteValue(this.typeInformation, this.value);
        }
    }
}

