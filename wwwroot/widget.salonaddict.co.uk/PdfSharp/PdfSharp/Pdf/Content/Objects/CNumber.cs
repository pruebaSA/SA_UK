namespace PdfSharp.Pdf.Content.Objects
{
    using System;

    public abstract class CNumber : CObject
    {
        protected CNumber()
        {
        }

        public CNumber Clone() => 
            ((CNumber) this.Copy());

        protected override CObject Copy() => 
            base.Copy();
    }
}

