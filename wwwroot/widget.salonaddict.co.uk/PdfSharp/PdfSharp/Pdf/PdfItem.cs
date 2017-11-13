namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.IO;
    using System;

    public abstract class PdfItem : ICloneable
    {
        protected PdfItem()
        {
        }

        public PdfItem Clone() => 
            ((PdfItem) this.Copy());

        protected virtual object Copy() => 
            base.MemberwiseClone();

        object ICloneable.Clone() => 
            this.Copy();

        internal abstract void WriteObject(PdfWriter writer);
    }
}

