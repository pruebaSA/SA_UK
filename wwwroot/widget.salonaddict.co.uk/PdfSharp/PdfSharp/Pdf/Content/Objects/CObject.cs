namespace PdfSharp.Pdf.Content.Objects
{
    using PdfSharp.Pdf.Content;
    using System;

    public abstract class CObject : ICloneable
    {
        protected CObject()
        {
        }

        public CObject Clone() => 
            this.Copy();

        protected virtual CObject Copy() => 
            ((CObject) base.MemberwiseClone());

        object ICloneable.Clone() => 
            this.Copy();

        internal abstract void WriteObject(ContentWriter writer);
    }
}

