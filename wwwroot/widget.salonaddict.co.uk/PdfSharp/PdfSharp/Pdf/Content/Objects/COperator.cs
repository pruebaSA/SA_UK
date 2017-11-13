namespace PdfSharp.Pdf.Content.Objects
{
    using PdfSharp.Pdf.Content;
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("({Name}, operands={Operands.Count})")]
    public class COperator : CObject
    {
        private PdfSharp.Pdf.Content.Objects.OpCode opcode;
        private CSequence seqence;

        protected COperator()
        {
        }

        internal COperator(PdfSharp.Pdf.Content.Objects.OpCode opcode)
        {
            this.opcode = opcode;
        }

        public COperator Clone() => 
            ((COperator) this.Copy());

        protected override CObject Copy() => 
            base.Copy();

        public override string ToString() => 
            this.Name;

        internal override void WriteObject(ContentWriter writer)
        {
            int num = (this.seqence != null) ? this.seqence.Count : 0;
            for (int i = 0; i < num; i++)
            {
                this.seqence[i].WriteObject(writer);
            }
            writer.WriteLineRaw(this.ToString());
        }

        public virtual string Name =>
            this.opcode.Name;

        public PdfSharp.Pdf.Content.Objects.OpCode OpCode =>
            this.opcode;

        public CSequence Operands
        {
            get
            {
                if (this.seqence == null)
                {
                    this.seqence = new CSequence();
                }
                return this.seqence;
            }
        }
    }
}

