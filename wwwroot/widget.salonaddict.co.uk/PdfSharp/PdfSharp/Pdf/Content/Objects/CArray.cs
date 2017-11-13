namespace PdfSharp.Pdf.Content.Objects
{
    using PdfSharp.Pdf.Content;
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("(count={Count})")]
    public class CArray : CSequence
    {
        public CArray Clone() => 
            ((CArray) this.Copy());

        protected override CObject Copy() => 
            base.Copy();

        public override string ToString() => 
            ("[" + base.ToString() + "]");

        internal override void WriteObject(ContentWriter writer)
        {
            writer.WriteRaw(this.ToString());
        }
    }
}

