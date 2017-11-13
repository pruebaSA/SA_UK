namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.IO;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("iref({ObjectNumber}, {GenerationNumber})")]
    public sealed class PdfReference : PdfItem
    {
        private PdfDocument document;
        private PdfObjectID objectID;
        private int position;
        private PdfObject value;

        public PdfReference(PdfObject pdfObject)
        {
            this.value = pdfObject;
        }

        public PdfReference(PdfObjectID objectID, int position)
        {
            this.objectID = objectID;
            this.position = position;
        }

        internal void SetObject(PdfObject value)
        {
            this.value = value;
        }

        public override string ToString() => 
            (this.objectID.ToString() + " R");

        internal override void WriteObject(PdfWriter writer)
        {
            writer.Write(this);
        }

        internal void WriteXRefEnty(PdfWriter writer)
        {
            string rawString = $"{this.position:0000000000} {this.objectID.GenerationNumber:00000} n
";
            writer.WriteRaw(rawString);
        }

        internal static PdfReferenceComparer Comparer =>
            new PdfReferenceComparer();

        public PdfDocument Document
        {
            get => 
                this.document;
            set
            {
                this.document = value;
            }
        }

        public int GenerationNumber =>
            this.objectID.GenerationNumber;

        public PdfObjectID ObjectID
        {
            get => 
                this.objectID;
            set
            {
                if (this.objectID != value)
                {
                    this.objectID = value;
                    PdfDocument document = this.Document;
                }
            }
        }

        public int ObjectNumber =>
            this.objectID.ObjectNumber;

        public int Position
        {
            get => 
                this.position;
            set
            {
                this.position = value;
            }
        }

        public PdfObject Value
        {
            get => 
                this.value;
            set
            {
                this.value = value;
                value.Reference = this;
            }
        }

        internal class PdfReferenceComparer : IComparer<PdfReference>
        {
            public int Compare(PdfReference x, PdfReference y)
            {
                PdfReference reference = x;
                PdfReference reference2 = y;
                if (reference != null)
                {
                    if (reference2 != null)
                    {
                        return reference.objectID.CompareTo(reference2.objectID);
                    }
                    return -1;
                }
                if (reference2 != null)
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}

