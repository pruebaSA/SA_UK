namespace PdfSharp.Pdf.Content.Objects
{
    using System;

    public sealed class OpCode
    {
        public readonly string Description;
        public readonly OpCodeFlags Flags;
        public readonly string Name;
        public readonly PdfSharp.Pdf.Content.Objects.OpCodeName OpCodeName;
        public readonly int Operands;
        public readonly string Postscript;

        private OpCode()
        {
        }

        internal OpCode(string name, PdfSharp.Pdf.Content.Objects.OpCodeName opcodeName, int operands, string postscript, OpCodeFlags flags, string description)
        {
            this.Name = name;
            this.OpCodeName = opcodeName;
            this.Operands = operands;
            this.Postscript = postscript;
            this.Flags = flags;
            this.Description = description;
        }
    }
}

