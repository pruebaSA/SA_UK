namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using System;

    public class PdfObjectInternals
    {
        private PdfObject obj;

        internal PdfObjectInternals(PdfObject obj)
        {
            this.obj = obj;
        }

        public int GenerationNumber =>
            this.obj.ObjectID.GenerationNumber;

        public PdfObjectID ObjectID =>
            this.obj.ObjectID;

        public int ObjectNumber =>
            this.obj.ObjectID.ObjectNumber;

        public string TypeID
        {
            get
            {
                if (this.obj is PdfArray)
                {
                    return "array";
                }
                if (this.obj is PdfDictionary)
                {
                    return "dictionary";
                }
                return this.obj.GetType().Name;
            }
        }
    }
}

