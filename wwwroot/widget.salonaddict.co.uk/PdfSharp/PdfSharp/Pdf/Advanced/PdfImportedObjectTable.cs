namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal sealed class PdfImportedObjectTable
    {
        private PdfDocument.DocumentHandle externalDocumentHandle;
        private Dictionary<string, PdfReference> externalIDs = new Dictionary<string, PdfReference>();
        private PdfDocument owner;
        private PdfFormXObject[] xObjects;

        public PdfImportedObjectTable(PdfDocument owner, PdfDocument externalDocument)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            if (externalDocument == null)
            {
                throw new ArgumentNullException("externalDocument");
            }
            this.owner = owner;
            this.externalDocumentHandle = externalDocument.Handle;
            this.xObjects = new PdfFormXObject[externalDocument.PageCount];
        }

        public void Add(PdfObjectID externalID, PdfReference iref)
        {
            this.externalIDs[externalID.ToString()] = iref;
        }

        public bool Contains(PdfObjectID externalID) => 
            this.externalIDs.ContainsKey(externalID.ToString());

        public PdfFormXObject GetXObject(int pageNumber) => 
            this.xObjects[pageNumber - 1];

        public void SetXObject(int pageNumber, PdfFormXObject xObject)
        {
            this.xObjects[pageNumber - 1] = xObject;
        }

        public PdfDocument ExternalDocument
        {
            get
            {
                if (this.externalDocumentHandle.IsAlive)
                {
                    return this.externalDocumentHandle.Target;
                }
                return null;
            }
        }

        public PdfReference this[PdfObjectID externalID] =>
            this.externalIDs[externalID.ToString()];

        public PdfDocument Owner =>
            this.owner;
    }
}

